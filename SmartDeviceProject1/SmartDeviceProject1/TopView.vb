Imports System.Linq
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Net
Imports System.Net.Sockets
Imports System.IO
Imports Bt.CommLib
Imports Bt




Public Class TopView

    ' coredll.dllで使用する定数
    Public Const WAIT_OBJECT_0 As Int32 = &H0
    ' 印字データで使用する定数
    Public Const STX As [Byte] = &H2
    Public Const ETX As [Byte] = &H3
    Public Const DLE As [Byte] = &H10
    Public Const SYN As [Byte] = &H16
    Public Const ENQ As [Byte] = &H5
    Public Const ACK As [Byte] = &H6
    Public Const NAK As [Byte] = &H15
    Public Const ESC As [Byte] = &H1B
    Public Const LF As [Byte] = &HA

    '##### -------  Exit Button Handler     ------#######
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    '##### -------  Print Button Handle     ------#######
    Private Sub testBT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles testBT.Click
        Dim stInfoSet As New LibDef.BT_BLUETOOTH_TARGET()   ' Bluetooth機器情報
        Dim pin As New StringBuilder("")                    ' PINコード
        Dim pinlen As UInt32 = 0                            ' PINコード長

        stInfoSet.name = "NEX-M230"
        stInfoSet.addr = "742B628FF070"
        pin = New StringBuilder("0000000000000000")
        pinlen = CType(pin.Length, UInt32)

        Bluetooth_Print_PR2(stInfoSet, pin, pinlen)
    End Sub


    '##### -------  Print Process           ------#######
    Private Sub Bluetooth_Print_PR2(ByVal stInfoSet As LibDef.BT_BLUETOOTH_TARGET, ByVal pin As StringBuilder, ByVal pinlen As UInt32)

        Dim logo As Bitmap = New Bitmap("Logo.bmp")

        Dim ret As Int32 = 0
        Dim disp As [String] = ""

        Dim sbBuf As New StringBuilder("")

        Dim ssizeGet As UInt32 = 0
        Dim rsizeGet As UInt32 = 0

        Dim bBufGet As [Byte]() = New [Byte](4094) {}



        '4094
        'Running line
        Dim bBuf = New [Byte](18188) {}
        Dim bBufWork As [Byte]() = New [Byte]() {}
        Dim len As Int32 = 0


        Try
            ' Bluetooth接続
            If Bluetooth_Connect(stInfoSet, pin, pinlen) = False Then
                GoTo L_END2
            End If

            Label1.Text = "接続成功プリント開始"


            '-----------------------------------------------------------------------
            'image(start)

            printImage(bBuf, len)

            'image end
            '-----------------------------------------------------------------------

            setAlignLeft(bBuf, len)
            printString(bBuf, len, DateTime.Now.ToString, 0)
            printString(bBuf, len, "商品名　　　　　数量　　　　金額", 2)

            Dim items() As Items = New Items() {New Items("宮崎牛　盛り合わせセット", 19999, 3), _
                                                New Items("生ビール　キリン", 1990, 2), _
                                                New Items("生薬", 500, 7)}
            Dim totalCount As Integer = 0
            Dim totalPrice As Integer = 0
            For Each item In items
                setAlignLeft(bBuf, len)
                printString(bBuf, len, item.title, 0)
                setAlignRight(bBuf, len)
                printString(bBuf, len, item.count.ToString + " X " + item.price.ToString("#,0") + "    　" + (item.count * item.price).ToString("C"), 0)

                totalCount += item.count
                totalPrice += item.count * item.price
            Next
            printString(bBuf, len, "　　　　　　　　　　　　　　　　", 2)
            setAlignRight(bBuf, len)
            setDoulbeStike(bBuf, len)
            printString(bBuf, len, "合計　　　　　　　　　　" + totalPrice.ToString("C"), 0)
            setNonDoubleStrike(bBuf, len)
            printString(bBuf, len, "(内消費税　5%　　　  　" + Math.Round(totalPrice * 0.05).ToString("C") + ")", 0)
            printString(bBuf, len, "預かり金              　" + 80000.ToString("C"), 0)
            printString(bBuf, len, "お釣り              　　" + (80000 - totalPrice).ToString("C"), 0)
            '-----------------------------------------------------------------------
            ' Footer Start

            Commands.nFeedLine.CopyTo(bBuf, len)
            len = len + Commands.nFeedLine.Length

            Commands.bCut.CopyTo(bBuf, len)
            len = len + Commands.bCut.Length

            ' Footer End
            '-----------------------------------------------------------------------




            '-----------------------------------------------------------------------
            ' Load to printer

            If SppSend(bBuf, ssizeGet) = False Then
                'If SppSend(bBuf, ssizeGet) = False Then
                Label1.Text = "接続成功がプリント失敗だ"
            Else
                Label1.Text = "OK"
            End If

            ' Load to printer End
            '-----------------------------------------------------------------------



            '-----------------------------------------------------------------------
            ' Print
            Dim printflg As [Boolean] = False
            While True
                Dim recvFlg As [Boolean] = False
                For i As Int32 = 0 To 9
                    ' データ受信
                    bBufGet = New [Byte](0) {}
                    If SppRecv(bBufGet, rsizeGet) = False Then
                        Continue For
                    End If
                    recvFlg = True
                    Exit For
                Next
                If recvFlg = False Then
                    Exit While
                End If

                If bBufGet(0) = ACK Then
                    bBuf = New [Byte]() {ENQ}
                    If SppSend(bBuf, ssizeGet) = False Then
                        GoTo L_END1
                    End If
                ElseIf bBufGet(0) = NAK Then
                    GoTo L_END1
                ElseIf bBufGet(0) = STX Then
                    bBufGet = New [Byte](4094) {}
                    If SppRecv(bBufGet, rsizeGet) = False Then
                        GoTo L_END1
                    End If
                    If bBufGet(9) <> ETX Then
                        GoTo L_END1
                    End If
                    If bBufGet(2) = &H47 OrElse bBufGet(2) = &H48 OrElse bBufGet(2) = &H53 OrElse bBufGet(2) = &H54 Then
                        ' 印刷中なので少し待機
                        Thread.Sleep(200)
                        bBuf = New [Byte]() {ENQ}
                        If SppSend(bBuf, ssizeGet) = False Then
                            GoTo L_END1
                        End If
                        Continue While
                    ElseIf (bBufGet(2) <> &H0) AndAlso (bBufGet(2) <> &H1) AndAlso (bBufGet(2) <> &H41) AndAlso (bBufGet(2) <> &H42) AndAlso (bBufGet(2) <> &H4E) AndAlso (bBufGet(2) <> &H4D) Then
                        Exit While
                    End If
                    ' 印刷成功
                    printflg = True
                    Exit While
                End If
            End While
            If printflg = True Then
                disp = "印刷成功しました。"
                MessageBox.Show(disp, "印刷完了")
            End If
L_END1:
            ret = Bluetooth.btBluetoothSPPDisconnect()
            If ret <> LibDef.BT_OK Then
                disp = "btBluetoothSPPDisconnect error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                GoTo L_END2
            End If
L_END2:
            ret = Bluetooth.btBluetoothClose()
            If ret <> LibDef.BT_OK Then
                disp = "btBluetoothClose error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return
            End If

            ' Print End
            '-----------------------------------------------------------------------

        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        Finally
        End Try
    End Sub
End Class


