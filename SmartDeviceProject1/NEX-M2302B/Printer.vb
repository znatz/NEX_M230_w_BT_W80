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


Namespace POSCO
    Public Class Printer
        Public Shared bufferToPrint() As Byte
        Public Shared currentLength As Int32
        Public Shared devName As String
        Public Shared devAddr As String

        Public Const STX As [Byte] = &H2
        Public Const ETX As [Byte] = &H3
        Public Const DLE As [Byte] = &H10
        Public Const SYN As [Byte] = &H16
        Public Const ENQ As [Byte] = &H5
        Public Const ACK As [Byte] = &H6
        Public Const NAK As [Byte] = &H15
        Public Const ESC As [Byte] = &H1B
        Public Const LF As [Byte] = &HA
        Public Const CR As [Byte] = &HD




        '-------------------------------------------------------------------
        ' Printer Settings
        '===================================================================
        ' strInfoSet    : name and bluetooth address of NEX-M230
        ' pin           : pin           (NEVER USED)
        ' pinlen        : length of pin (NEVER USED)
        '-------------------------------------------------------------------
        'Dim stInfoSet As New LibDef.BT_BLUETOOTH_TARGET() _
        '            With { _
        '                    .name = devName, _
        '                    .addr = devAddr _
        '                 }
        Public Shared stInfoSet As New LibDef.BT_BLUETOOTH_TARGET()
        Dim pin As StringBuilder = New StringBuilder("0000000000000000")
        Dim pinlen As UInt32 = CType(pin.Length, UInt32)
 
        '-------------------------------------------------------------------
        ' Constructor       : Initialize printing buffer 
        '===================================================================
        ' # bufferSize      : size of buffer
        ' bBuf              : buffer of all
        ' deviceName        : device name
        ' deviceBDAddress   : device bluetooth address
        '-------------------------------------------------------------------
        Public Sub New(ByVal bufferSize As Integer, ByVal deviceName As String, ByVal deviceBDAddress As String)
            bufferToPrint = New [Byte](bufferSize) {}
            currentLength = 0

            'devName = deviceName
            'devAddr = deviceBDAddress
            stInfoSet.name = deviceName.Trim
            stInfoSet.addr = deviceBDAddress.Trim

        End Sub




        Public Sub startPrint()

            Dim ret As Int32 = 0
            Dim disp As [String] = ""

            Dim sbBuf As New StringBuilder("")

            Dim ssizeGet As UInt32 = 0
            Dim rsizeGet As UInt32 = 0

            Dim bBufGet As [Byte]() = New [Byte](4094) {}


            Try
                If Bluetooth_Connect(stInfoSet, pin, pinlen) = False Then
                    GoTo L_END2
                End If


                'printRegisterImage(bBuf, len, 0)
                'printReceiptContents(bBuf, len)



                '-----------------------------------------------------------------------
                ' Footer Start

                Commands.nFeedLine.CopyTo(bufferToPrint, currentLength)
                currentLength = currentLength + Commands.nFeedLine.Length

                Commands.bCut.CopyTo(bufferToPrint, currentLength)
                currentLength = currentLength + Commands.bCut.Length

                ' Footer End
                '-----------------------------------------------------------------------

                '-----------------------------------------------------------------------
                ' Load to printer

                If SppSend(bufferToPrint, ssizeGet) = False Then _
                    MessageBox.Show("接続成功がプリント失敗だ")


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
                        bufferToPrint = New [Byte]() {ENQ}
                        If SppSend(bufferToPrint, ssizeGet) = False Then
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
                            Thread.Sleep(100)
                            bufferToPrint = New [Byte]() {ENQ}
                            If SppSend(bufferToPrint, ssizeGet) = False Then
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


        '-------------------------------------------------------------------
        ' Bluetooth_Connect       : Connect via bluetooth
        '===================================================================
        ' # sInfoSet      : printer information structure
        ' # pin           : pin             (NEVER USED without pairing)
        ' # pinlen        : length of pin   (NEVER USED without pairing)
        '===================================================================
        ' * Return        : sucess or not
        '-------------------------------------------------------------------
        Private Function Bluetooth_Connect(ByVal stInfoSet As LibDef.BT_BLUETOOTH_TARGET, _
                                           ByVal pin As StringBuilder, _
                                           ByVal pinlen As UInt32 _
                                           ) As [Boolean]

            Dim ret As Int32 = 0
            Dim disp As [String] = ""
            Dim bRet As [Boolean] = False

            Try
                ret = Bluetooth.btBluetoothOpen()
                If ret <> LibDef.BT_OK Then
                    disp = "btBluetoothOpen error ret[" & ret & "]"
                    MessageBox.Show(disp, "エラー")
                    Return bRet
                End If

                ret = Bluetooth.btBluetoothSPPConnect(stInfoSet, 30000)
                If ret <> LibDef.BT_OK Then
                    disp = "btBluetoothSPPConnect error ret[" & ret & "]"
                    MessageBox.Show(disp, "エラー")
                    Return bRet
                End If

                bRet = True
                Return bRet
            Catch ex As Exception
                MessageBox.Show(ex.ToString())
                Return bRet
            Finally
            End Try
        End Function

        '-------------------------------------------------------------------
        ' SppSend     : Send via bluetooth
        '===================================================================
        ' # bBuf      : contents to print
        ' # ssize     : size of contents
        '===================================================================
        ' * Return    : sucess or not
        '-------------------------------------------------------------------
        Private Function SppSend(ByVal bBuf As [Byte](), ByRef ssize As UInt32) As [Boolean]
            Dim bRet As [Boolean] = False
            Dim ret As Int32 = 0
            Dim disp As [String] = ""

            Dim dsizeSet As UInt32 = 0
            Dim ssizeGet As UInt32 = 0
            Dim pBufSet As IntPtr

            Try
                dsizeSet = CType(bBuf.Length, UInt32)
                pBufSet = Marshal.AllocCoTaskMem(CType(dsizeSet, Int32))
                Marshal.Copy(bBuf, 0, pBufSet, CType(dsizeSet, Int32))
                ret = Bluetooth.btBluetoothSPPSend(pBufSet, dsizeSet, ssizeGet)
                Marshal.FreeCoTaskMem(pBufSet)
                If ret <> LibDef.BT_OK Then
                    disp = "When printing, btBluetoothSPPSend error ret[" & ret & "]"
                    MessageBox.Show(disp, "エラー")
                    Return bRet
                End If

                ssize = ssizeGet
                bRet = True
                Return bRet
            Catch ex As Exception
                MessageBox.Show(ex.ToString())
                Return bRet
            Finally
            End Try
        End Function

        '-------------------------------------------------------------------
        ' SppRecv     : Receive via bluetooth
        '===================================================================
        ' # bBuf      : contents get
        ' # rsize     : size of contents 
        '===================================================================
        ' * Return    : sucess or not
        '-------------------------------------------------------------------
        Private Function SppRecv(ByRef bBuf As [Byte](), ByRef rsize As UInt32) As [Boolean]
            Dim bRet As [Boolean] = False
            Dim ret As Int32 = 0

            Dim dsizeSet As UInt32 = 0
            Dim rsizeGet As UInt32 = 0
            Dim pBufGet As IntPtr
            Dim bBufGet As [Byte]() = New [Byte]() {}

            Try
                Thread.Sleep(100)
                Dim buflen As Int32 = bBuf.Length
                bBufGet = New [Byte](buflen - 1) {}
                pBufGet = Marshal.AllocCoTaskMem(Marshal.SizeOf(GetType(Byte)) * bBufGet.Length)
                dsizeSet = CType(buflen, UInt32)
                ret = Bluetooth.btBluetoothSPPRecv(pBufGet, dsizeSet, rsizeGet)
                Marshal.Copy(pBufGet, bBufGet, 0, CType(rsizeGet, Int32))
                Marshal.FreeCoTaskMem(pBufGet)
                If ret <> LibDef.BT_OK Then
                    Return bRet
                End If

                bBuf = bBufGet
                rsize = rsizeGet
                bRet = True
                Return bRet
            Catch ex As Exception
                MessageBox.Show(ex.ToString())
                Return bRet
            Finally
            End Try
        End Function

        '-------------------------------------------------------------------
        ' SplitString     : Slice string by length
        '===================================================================
        ' # TheString     : original string
        ' # StringLen     : length of pieces of string
        '===================================================================
        ' * Return        : array contains every slices
        '-------------------------------------------------------------------
        Private Function SplitString(ByVal TheString As String, ByVal StringLen As Integer) As String()
            Dim ArrCount As Integer
            Dim I As Long
            Dim TempArray() As String
            ReDim TempArray((Len(TheString) - 1) \ StringLen)
            For I = 1 To Len(TheString) Step StringLen
                TempArray(ArrCount) = Mid$(TheString, I, StringLen)
                ArrCount = ArrCount + 1
            Next
            SplitString = TempArray
        End Function

        '-------------------------------------------------------------------
        ' padLineTo8Bit   : Pad string of '0's and '1's to eight characters
        '===================================================================
        ' # line          : original string
        '===================================================================
        ' * Return        : padded string
        '-------------------------------------------------------------------
        Private Function padLineTo8Bit(ByVal line As String) As String

            Dim padding As Integer = 8 - (line.Length Mod 8)
            Dim paddedline(line.Length + padding - 1) As Char
            paddedline = line
            For i As Integer = 0 To padding - 1
                paddedline = paddedline + "0"
            Next
            Return paddedline
        End Function

        '----------------------------------------------------------------------------
        ' bufferRegisterImage   : [register image process] to buffer
        '============================================================================
        ' # filename      : image file name
        ' # number        : number of register image
        '----------------------------------------------------------------------------
        Public Sub bufferRegisterImage(ByRef filename As String, ByVal number As Int32)

            Dim bBufWork As [Byte]() = New [Byte]() {}
            Dim bmp As Bitmap = New Bitmap(filename)

            Dim n1 As Byte = (bmp.Width + (8 - (bmp.Width Mod 8))) / 8
            Dim n2 As Byte = bmp.Height

            Select Case number
                Case 0
                    Commands.startRegisterImage0.CopyTo(bufferToPrint, currentLength)
                    currentLength = currentLength + Commands.startRegisterImage0.Length
                Case 1
                    Commands.startRegisterImage1.CopyTo(bufferToPrint, currentLength)
                    currentLength = currentLength + Commands.startRegisterImage0.Length
                Case 2
                    Commands.startRegisterImage2.CopyTo(bufferToPrint, currentLength)
                    currentLength = currentLength + Commands.startRegisterImage0.Length
            End Select

            bBufWork = New Byte() {&H1B, &H62, n1, n2, &H0}
            bBufWork.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + bBufWork.Length

            For y As Integer = 0 To bmp.Height - 1
                Dim line(bmp.Width - 1) As Char
                For x As Integer = 0 To bmp.Width - 1
                    If bmp.GetPixel(x, y).R <> &H0 Then
                        line(x) = "0"
                    Else
                        line(x) = "1"
                    End If
                Next

                Dim every8bit() As String = SplitString(padLineTo8Bit(line), 8)

                For Each eightBit In every8bit
                    bBufWork = New Byte() {Convert.ToInt32(eightBit, 2)}
                    bBufWork.CopyTo(bufferToPrint, currentLength)
                    currentLength = currentLength + bBufWork.Length
                Next
            Next

            bBufWork = New Byte() {&H1B, &H4A, &H0}
            bBufWork.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + bBufWork.Length

            Commands.finishRegisterImage.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.finishRegisterImage.Length
        End Sub

        '----------------------------------------------------------------------------
        ' bufferPrintRegisterImage   : [print registered image process] to buffer
        '============================================================================
        ' # number        : number of register image
        '----------------------------------------------------------------------------
        Public Sub bufferPrintRegisterImage(ByVal number As Int32)
            Select Case number
                Case 0
                    Commands.printRegisterImage0.CopyTo(bufferToPrint, currentLength)
                    currentLength = currentLength + Commands.printRegisterImage0.Length
                Case 1
                    Commands.printRegisterImage1.CopyTo(bufferToPrint, currentLength)
                    currentLength = currentLength + Commands.printRegisterImage1.Length
                Case 2
                    Commands.printRegisterImage2.CopyTo(bufferToPrint, currentLength)
                    currentLength = currentLength + Commands.printRegisterImage2.Length
            End Select
        End Sub



        'Public Sub printReceiptContents(ByRef bBuf() As Byte, ByRef len As Int32)
        '    setAlignLeft(bBuf, len)
        '    printString(bBuf, len, DateTime.Now.ToString, 0)
        '    printString(bBuf, len, "商品名　　　　　数量　　　　金額", 2)

        '    Dim items() As Items = New Items() {New Items("0000", "宮崎牛　盛り合わせセット", 19999, 3), _
        '                                        New Items("1111", "生ビール　キリン", 1990, 2), _
        '                                        New Items("2222", "生薬", 500, 7)}
        '    Dim totalCount As Integer = 0
        '    Dim totalPrice As Integer = 0
        '    For Each item In items
        '        setAlignLeft(bBuf, len)
        '        printString(bBuf, len, item.title, 0)
        '        setAlignRight(bBuf, len)
        '        printString(bBuf, len, item.count.ToString + " X " + item.price.ToString("#,0") + "    　" + (item.count * item.price).ToString("C"), 0)

        '        totalCount += item.count
        '        totalPrice += item.count * item.price
        '    Next
        '    printString(bBuf, len, "　　　　　　　　　　　　　　　　", 2)
        '    setAlignRight(bBuf, len)
        '    setDoulbeStike(bBuf, len)
        '    printString(bBuf, len, "合計　　　　　　　　　　" + totalPrice.ToString("C"), 0)
        '    setNonDoubleStrike(bBuf, len)
        '    printString(bBuf, len, "(内消費税　5%　　　  　" + Math.Round(totalPrice * 0.05).ToString("C") + ")", 0)
        '    printString(bBuf, len, "預かり金              　" + 80000.ToString("C"), 0)
        '    printString(bBuf, len, "お釣り              　　" + (80000 - totalPrice).ToString("C"), 0)
        'End Sub


        '----------------------------------------------------------------------------
        ' bufferSetAlignLeft     : [align left process] to buffer
        '============================================================================
        '----------------------------------------------------------------------------
        Public Sub bufferSetAlignLeft()
            Commands.bAlignLeft.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.bAlignLeft.Length
        End Sub

        '----------------------------------------------------------------------------
        ' bufferSetAlignCenter   : [align center process] to buffer
        '============================================================================
        '----------------------------------------------------------------------------
        Public Sub bufferSetAlignCenter()
            Commands.bAlignCenter.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.bAlignCenter.Length
        End Sub

        '----------------------------------------------------------------------------
        ' bufferSetAlignRight   : [align right process] to buffer
        '============================================================================
        '----------------------------------------------------------------------------
        Public Sub bufferSetAlignRight()
            Commands.bAlignRight.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.bAlignRight.Length
        End Sub

        '----------------------------------------------------------------------------
        ' bufferSetSizeNormal   : [nromal size process] to buffer
        '============================================================================
        '----------------------------------------------------------------------------
        Public Sub bufferSetSizeNormal()
            Commands.bSizeNor.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.bSizeNor.Length
        End Sub

        '----------------------------------------------------------------------------
        ' bufferSetSizeDouble   : [double size process] to buffer
        '============================================================================
        '----------------------------------------------------------------------------
        Public Sub bufferSetSizeDouble()
            Commands.bSizeDbl.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.bSizeDbl.Length
        End Sub

        '----------------------------------------------------------------------------
        ' bufferSetSizeTriple   : [triple size process] to buffer
        '============================================================================
        '----------------------------------------------------------------------------
        Public Sub bufferSetSizeTriple()
            Commands.bSizeTri.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.bSizeTri.Length
        End Sub

        '----------------------------------------------------------------------------
        ' bufferSetDoulbeStike       : [double strike size process] to buffer
        '============================================================================
        '----------------------------------------------------------------------------
        Public Sub bufferSetDoulbeStike()
            Commands.bDoulbeStrik.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.bDoulbeStrik.Length
        End Sub

        '----------------------------------------------------------------------------
        ' bufferSetNonDoubleStrike   : [release double strike process] to buffer
        '============================================================================
        '----------------------------------------------------------------------------
        Public Sub bufferSetNonDoubleStrike()
            Commands.bNonDoulbeStrik.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.bNonDoulbeStrik.Length
        End Sub

        '-------------------------------------------------------------------
        ' bufferPrintString     : [print string process] to buffer
        '===================================================================
        ' # contents            : string to print
        ' # underline           : underline size    [0,1,2]
        '-------------------------------------------------------------------
        Public Sub bufferPrintString(ByVal contents As String, ByVal underline As Integer)

            Dim bBufWork As [Byte]() = New [Byte]() {}
            Dim bLF As [Byte]() = New [Byte](0) {LF}
            Dim bCR As [Byte]() = New [Byte](0) {CR}

            Select Case underline
                Case 0
                    Commands.bUnderline0.CopyTo(bufferToPrint, currentLength)
                    currentLength = currentLength + Commands.bUnderline0.Length
                Case currentLength
                    Commands.bUnderline1.CopyTo(bufferToPrint, currentLength)
                    currentLength = currentLength + Commands.bUnderline1.Length
                Case 2
                    Commands.bUnderline2.CopyTo(bufferToPrint, currentLength)
                    currentLength = currentLength + Commands.bUnderline2.Length
            End Select

            ' Japanese setting
            Commands.bJp.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.bJp.Length

            ' Contents to Shift-JIS
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes(contents)
            bBufWork.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + bBufWork.Length

            ' Carry Return
            bCR.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + bCR.Length
            bLF.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + bLF.Length

        End Sub

        '-------------------------------------------------------------------
        ' bufferPrintJAN13_00Ending     : [print JAN13 process] to buffer
        '===================================================================
        ' # jan                         : JAN13 string
        '-------------------------------------------------------------------
        Public Sub bufferPrintJAN13_00Ending(ByVal jan As String)

            Dim bBufWork As [Byte]() = New [Byte]() {}

            Commands.hriBelow.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.hriBelow.Length

            Commands.printJAN13.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.printJAN13.Length

            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes(jan)
            bBufWork.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + bBufWork.Length

            Commands.nullEnding.CopyTo(bufferToPrint, currentLength)
            currentLength = currentLength + Commands.nullEnding.Length
        End Sub



    End Class
End Namespace
