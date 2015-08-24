'/********************************************************************************
'	動作説明	：	プリンタ印字のサンプルコードです。
'
'	注意		：	本プログラムはサンプルプログラムになりますので
'					弊社サポートの対象外となります
'
'	Copyright(c) 2012-2013 KEYENCE CORPORATION. All rights reserved.
'********************************************************************************/
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

Partial Public Class MainForm1
    Inherits Form
    Public Shared MainForm1Instance As MainForm1
    ' フォーム
    '--------------------------------------------------------------
    ' DLLImport
    '--------------------------------------------------------------
    <DllImport("wininet.dll", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Private Shared Function InternetOpen(ByVal lpszAgent As [String], ByVal dwAccessType As Int32, ByVal lpszProxyName As [String], ByVal lpszProxyBypass As [String], ByVal dwFlags As Int32) As IntPtr
    End Function

    <DllImport("wininet.dll", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Private Shared Function InternetConnect(ByVal hInternet As IntPtr, ByVal lpszServerName As [String], ByVal nServerPort As Int16, ByVal lpszUsername As [String], ByVal lpszPassword As [String], ByVal dwService As Int32, _
     ByVal dwFlags As Int32, ByVal dwContext As IntPtr) As IntPtr
    End Function

    <DllImport("wininet.dll", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Private Shared Function InternetCloseHandle(ByVal hInternet As IntPtr) As <MarshalAs(UnmanagedType.Bool)> [Boolean]
    End Function

    <DllImport("wininet.dll", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Private Shared Function FtpPutFile(ByVal hConnect As IntPtr, ByVal lpszLocalFile As [String], ByVal lpszNewRemoteFile As [String], ByVal dwFlags As Int64, ByVal dwContext As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("coredll.dll", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Public Shared Function WaitForSingleObject(ByVal Handle As IntPtr, ByVal Wait As Int32) As Int32
    End Function

    <DllImport("coredll.dll", SetLastError:=True, CallingConvention:=CallingConvention.Winapi, CharSet:=CharSet.Unicode)> _
    Public Shared Function CreateEvent(ByVal lpEventAttributes As IntPtr, <[In](), MarshalAs(UnmanagedType.Bool)> ByVal bManualReset As [Boolean], <[In](), MarshalAs(UnmanagedType.Bool)> ByVal bIntialState As Boolean, <[In](), MarshalAs(UnmanagedType.BStr)> ByVal lpName As [String]) As IntPtr
    End Function

    <DllImport("coredll.dll", SetLastError:=True, CallingConvention:=CallingConvention.Winapi, CharSet:=CharSet.Auto)> _
    Public Shared Function CloseHandle(ByVal hObject As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    '--------------------------------------------------------------
    ' 定数定義
    '--------------------------------------------------------------
    ' wininetで使用する定数
    Public Const INTERNET_DEFAULT_FTP_PORT As Int32 = 21
    Public Const INTERNET_OPEN_TYPE_PRECONFIG As Int32 = 0
    Public Const INTERNET_OPEN_TYPE_DIRECT As Int32 = 1
    Public Const INTERNET_OPEN_TYPE_PROXY As Int32 = 3
    Public Const INTERNET_INVALID_PORT_NUMBER As Int32 = 0
    Public Const INTERNET_SERVICE_FTP As Int32 = 1
    Public Const INTERNET_SERVICE_GOPHER As Int32 = 2
    Public Const INTERNET_SERVICE_HTTP As Int32 = 3
    Public Const FTP_TRANSFER_TYPE_BINARY As Int64 = &H2
    Public Const FTP_TRANSFER_TYPE_ASCII As Int64 = &H1
    Public Const INTERNET_FLAG_NO_CACHE_WRITE As Int64 = &H4000000
    Public Const INTERNET_FLAG_RELOAD As Int64 = &H80000000UI
    Public Const INTERNET_FLAG_KEEP_CONNECTION As Int64 = &H400000
    Public Const INTERNET_FLAG_MULTIPART As Int64 = &H200000
    Public Const INTERNET_FLAG_PASSIVE As Int64 = &H8000000
    Public Const FILE_ATTRIBUTE_READONLY As Int64 = &H1
    Public Const FILE_ATTRIBUTE_HIDDEN As Int64 = &H2
    Public Const FILE_ATTRIBUTE_SYSTEM As Int64 = &H4
    Public Const FILE_ATTRIBUTE_DIRECTORY As Int64 = &H10
    Public Const FILE_ATTRIBUTE_ARCHIVE As Int64 = &H20
    Public Const FILE_ATTRIBUTE_NORMAL As Int64 = &H80
    Public Const FILE_ATTRIBUTE_TEMPORARY As Int64 = &H100
    Public Const FILE_ATTRIBUTE_COMPRESSED As Int64 = &H800
    Public Const FILE_ATTRIBUTE_OFFLINE As Int64 = &H1000
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

    Public Sub New()
        InitializeComponent()
    End Sub

    '*******************************************************************************
    '	フォームロード
    '*******************************************************************************
    Private Sub MainForm1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' VGAの画面をQVGAに縮小する
        If System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width = 240 Then
            Call resolution.VGAtoQVGA(Me)
        End If
        ' フォームの最大化・最小化ボタン非表示
        Me.MaximizeBox = Not Me.MaximizeBox
        Me.MinimizeBox = Not Me.MinimizeBox
        MainForm1Instance = Me

        ' 通信経路コンボボックスへ値設定
        cmbCOMM.Items.Clear()
        cmbCOMM.Items.Add("無線LAN")
        cmbCOMM.Items.Add("Bluetooth")

        ' プリンタ選択コンボボックスへ値設定
        cmbPRINTER.Items.Clear()
        cmbPRINTER.Items.Add("KEYENCE BT-PR2")
        cmbPRINTER.Items.Add("SATO プチラパン")
        cmbPRINTER.Items.Add("TEC ポケプリ")
    End Sub

    '*******************************************************************************
    '* [フォーム終了時のリリース処理]
    '*******************************************************************************
    Private Sub MainForm1_Closed(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Closed
        If MainForm1.MainForm1Instance IsNot Nothing Then
            MainForm1.MainForm1Instance.Dispose()
        End If
    End Sub

    '*******************************************************************************
    '* 印字開始
    '* ★Bluetooth/無線LANともに、接続する携帯プリンタの通信設定に合わせてください。
    '*******************************************************************************
    Private Sub btnStart_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStart.Click
        Dim disp As [String] = ""

        Dim SelComm As Int32 = 0
        Dim SelPrinter As Int32 = 0

        Dim ssid As [String] = ""
        Dim wepkey As [String] = ""
        Dim server As [String] = ""

        Dim stInfoSet As New LibDef.BT_BLUETOOTH_TARGET()   ' Bluetooth機器情報
        Dim pin As New StringBuilder("")    ' PINコード

        Dim pinlen As UInt32 = 0    ' PINコード長
        Try
            ' 選択済みコンボボックス値を取得
            SelComm = cmbCOMM.SelectedIndex
            If SelComm = -1 Then
                disp = "通信経路を選択してください。"
                MessageBox.Show(disp, "エラー")
                Return
            End If
            SelPrinter = cmbPRINTER.SelectedIndex
            If SelPrinter = -1 Then
                disp = "プリンタを選択してください。"
                MessageBox.Show(disp, "エラー")
                Return
            End If

            If SelComm = 0 AndAlso SelPrinter = 0 Then
                ' 無線LAN & KEYENCE BT_PR2
                disp = "該当しません。"
                MessageBox.Show(disp, "プリンタ印字サンプル")
                Return
            ElseIf SelComm = 0 AndAlso SelPrinter = 1 Then
                ' 無線LAN & SATO プチラパン
                ssid = "SATO"
                wepkey = "keyenceautoid"
                server = "192.168.162.1"
                Wlan_Print(ssid, wepkey, server, 1)
            ElseIf SelComm = 0 AndAlso SelPrinter = 2 Then
                ' 無線LAN & TEC ポケプリ
                ssid = "TECTEST"
                wepkey = "keyenceautoid"
                server = "192.168.162.2"
                Wlan_Print(ssid, wepkey, server, 2)
            ElseIf SelComm = 1 AndAlso SelPrinter = 0 Then
                ' Bluetooth & KEYENCE BT_PR2
                stInfoSet.name = "BT-PR2"
                stInfoSet.addr = "000190e38496"
                pin = New StringBuilder("0000000000003500")
                pinlen = CType(pin.Length, UInt32)
                Bluetooth_Print_PR2(stInfoSet, pin, pinlen)
            ElseIf SelComm = 1 AndAlso SelPrinter = 1 Then
                ' Bluetooth & SATO プチラパン
                stInfoSet.name = "SATO MOBILE PRINTER"
                stInfoSet.addr = "000b5d3d94eb"
                pin = New StringBuilder("3500")
                pinlen = CType(pin.Length, UInt32)
                ' SBPLモード or PT200/PT400互換モード
                Bluetooth_Print_Lapin_SBPL(stInfoSet, pin, pinlen)          ' SBPLモード
                ' Bluetooth_Print_Lapin_Compatible(stInfoSet, pin, pinlen)    ' PT200/PT400互換モード
            ElseIf SelComm = 1 AndAlso SelPrinter = 2 Then
                ' Bluetooth & TEC ポケプリ
                stInfoSet.name = "TOSHIBA TEC BT"
                stInfoSet.addr = "0015b510279d"
                pin = New StringBuilder("0000000000000000")
                pinlen = CType(pin.Length, UInt32)
                Bluetooth_Print_Tec(stInfoSet, pin, pinlen)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        Finally
        End Try
    End Sub

    '*******************************************************************************
    '* KEYENCE BT-PR2用の印字処理（Bluetooth）
    '*******************************************************************************
    Private Sub Bluetooth_Print_PR2(ByVal stInfoSet As LibDef.BT_BLUETOOTH_TARGET, ByVal pin As StringBuilder, ByVal pinlen As UInt32)
        Dim ret As Int32 = 0
        Dim disp As [String] = ""

        Dim sbBuf As New StringBuilder("")
        Dim ssizeGet As UInt32 = 0
        Dim bBuf As [Byte]() = New Byte() {}

        Dim rsizeGet As UInt32 = 0
        Dim bBufGet As [Byte]() = New [Byte](4094) {}

        Try
            ' Bluetooth接続
            If Bluetooth_Connect(stInfoSet, pin, pinlen) = False Then
                GoTo L_END2
            End If

            ' データ送信
            bBuf = New [Byte](4094) {}
            Dim bBufWork As [Byte]() = New [Byte]() {}
            Dim bESC As [Byte]() = New [Byte](0) {ESC}
            Dim bSTX As [Byte]() = New [Byte](0) {STX}
            Dim bETX As [Byte]() = New [Byte](0) {ETX}
            Dim bLF As [Byte]() = New [Byte](0) {LF}
            Dim b00 As [Byte]() = New [Byte](0) {&H0}
            Dim b30 As [Byte]() = New [Byte](0) {&H30}
            Dim len As Int32 = 0

            ' BTPR2：レシートモード
            bSTX.CopyTo(bBuf, len)
            len = len + bSTX.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("A")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("PG33A1010112800384+000+000+00+00+00005100")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("V0020")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("H0010")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("P02")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("L0202")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("K9B")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("Test Print")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("V0071")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("H0010")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("K9B")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("RECEIPT mode")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("Q0001")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("Z")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bETX.CopyTo(bBuf, len)
            len = len + bETX.Length

            If SppSend(bBuf, ssizeGet) = False Then
                GoTo L_END1
            End If

            ' 応答待機
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
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        Finally
        End Try
    End Sub

    '*******************************************************************************
    '* SATO プチラパン用の印字処理（Bluetooth）
    '*******************************************************************************
    Private Sub Bluetooth_Print_Lapin_SBPL(ByVal stInfoSet As LibDef.BT_BLUETOOTH_TARGET, ByVal pin As StringBuilder, ByVal pinlen As UInt32)
        Dim ret As Int32 = 0
        Dim disp As [String] = ""

        Dim sbBuf As New StringBuilder("")
        Dim ssizeGet As UInt32 = 0
        Dim bBuf As [Byte]() = New Byte() {}

        Dim rsizeGet As UInt32 = 0
        Dim bBufGet As [Byte]() = New [Byte](4094) {}

        Try
            ' Bluetooth接続
            If Bluetooth_Connect(stInfoSet, pin, pinlen) = False Then
                GoTo L_END2
            End If

            ' データ送信
            bBuf = New [Byte](4094) {}
            Dim bBufWork As [Byte]() = New [Byte]() {}
            Dim bESC As [Byte]() = New [Byte](0) {ESC}
            Dim bSTX As [Byte]() = New [Byte](0) {STX}
            Dim bETX As [Byte]() = New [Byte](0) {ETX}
            Dim bLF As [Byte]() = New [Byte](0) {LF}
            Dim b00 As [Byte]() = New [Byte](0) {&H0}
            Dim b30 As [Byte]() = New [Byte](0) {&H30}
            Dim len As Int32 = 0

            ' BTPR2：レシートモード
            bSTX.CopyTo(bBuf, len)
            len = len + bSTX.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("A")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("PG33A1010112800384+000+000+00+00+00005100")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("V0020")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("H0010")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("P02")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("L0202")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("K9B")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("Test Print")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("V0071")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("H0010")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("K9B")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("RECEIPT mode")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("Q0001")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("Z")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bETX.CopyTo(bBuf, len)
            len = len + bETX.Length

            If SppSend(bBuf, ssizeGet) = False Then
                GoTo L_END1
            End If

            ' 応答待機
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
                    ' PW208シリーズでは、スリープモードで印字コマンドを受信するとNAKを返す場合がある。
                    ' この場合、ENQでリトライすることで回避できる。
                    bBuf = New [Byte]() {ENQ}
                    If SppSend(bBuf, ssizeGet) = False Then
                        GoTo L_END1
                    End If
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
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        Finally
        End Try
    End Sub

    Private Sub Bluetooth_Print_Lapin_Compatible(ByVal stInfoSet As LibDef.BT_BLUETOOTH_TARGET, ByVal pin As StringBuilder, ByVal pinlen As UInt32)
        Dim ret As Int32 = 0
        Dim disp As [String] = ""

        Dim sbBuf As New StringBuilder("")
        Dim ssizeGet As UInt32 = 0
        Dim bBuf As [Byte]() = New Byte() {}

        Dim rsizeGet As UInt32 = 0
        Dim bBufGet As [Byte]() = New [Byte](4094) {}

        Try
            ' Bluetooth接続
            If Bluetooth_Connect(stInfoSet, pin, pinlen) = False Then
                GoTo L_END2
            End If

            ' データ送信
            bBuf = New [Byte](4094) {}
            Dim bBufWork As [Byte]() = New [Byte]() {}
            Dim bESC As [Byte]() = New [Byte](0) {ESC}
            Dim bSTX As [Byte]() = New [Byte](0) {STX}
            Dim bETX As [Byte]() = New [Byte](0) {ETX}
            Dim bLF As [Byte]() = New [Byte](0) {LF}
            Dim b00 As [Byte]() = New [Byte](0) {&H0}
            Dim b30 As [Byte]() = New [Byte](0) {&H30}
            Dim len As Int32 = 0

            ' ラパン・プチラパン：レシートモード
            bSTX.CopyTo(bBuf, len)
            len = len + bSTX.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("A")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("#1")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("L0020001008222000")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("DTest Print")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("L0071001008222000")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("DRECEIPT mode")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("Q0001")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("Z")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bETX.CopyTo(bBuf, len)
            len = len + bETX.Length

            If SppSend(bBuf, ssizeGet) = False Then
                GoTo L_END1
            End If

            ' 応答待機
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
                    If bBufGet(4) <> ETX Then
                        GoTo L_END1
                    End If
                    If (bBufGet(2) <> &H0) AndAlso (bBufGet(2) <> &H1) Then
                        ret = (0 - bBufGet(2) - 2)
                        Exit While
                    ElseIf bBufGet(2) = &H1 Then
                        Thread.Sleep(200)
                    End If
                    ' 印刷中なので少し待機

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
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        Finally
        End Try
    End Sub

    '*******************************************************************************
    '* TEC ポケプリ用の印字処理（Bluetooth）
    '*******************************************************************************
    Private Sub Bluetooth_Print_Tec(ByVal stInfoSet As LibDef.BT_BLUETOOTH_TARGET, ByVal pin As StringBuilder, ByVal pinlen As UInt32)
        Dim ret As Int32 = 0
        Dim disp As [String] = ""

        Dim sbBuf As New StringBuilder("")
        Dim dsizeSet As UInt32 = 0
        Dim ssizeGet As UInt32 = 0
        Dim pBufSet As IntPtr
        Dim bBuf As [Byte]() = New Byte() {}

        Try
            ' Bluetooth接続
            If Bluetooth_Connect_Tec(stInfoSet, pin, pinlen) = False Then
                GoTo L_END2
            End If

            ' プリンタモード変更
            Dim bPrintMode As [Byte](,) = New [Byte](2, 5) {{&H1B, &H4D, &H3B, &H30, &HA, &H0}, {&H1B, &H4D, &H3B, &H31, &HA, &H0}, {&H1B, &H4D, &H3B, &H41, &HA, &H0}}
            bBuf = New [Byte](5) {}
            For i As Int32 = 0 To 5
                bBuf(i) = bPrintMode(1, i)
            Next
            dsizeSet = CType(bBuf.Length, UInt32)
            pBufSet = Marshal.AllocCoTaskMem(CType(dsizeSet, Int32))
            Marshal.Copy(bBuf, 0, pBufSet, CType(dsizeSet, Int32))
            ret = Bluetooth.btBluetoothSPPSend(pBufSet, dsizeSet, ssizeGet)
            Marshal.FreeCoTaskMem(pBufSet)
            If ret <> LibDef.BT_OK Then
                disp = "btBluetoothSPPSend error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                GoTo L_END1
            End If

            ' データ送信
            bBuf = New [Byte](4094) {}
            Dim bBufWork As [Byte]() = New [Byte]() {}
            Dim bESC As [Byte]() = New [Byte](0) {ESC}
            Dim bSTX As [Byte]() = New [Byte](0) {STX}
            Dim bETX As [Byte]() = New [Byte](0) {ETX}
            Dim bLF As [Byte]() = New [Byte](0) {LF}
            Dim b00 As [Byte]() = New [Byte](0) {&H0}
            Dim b30 As [Byte]() = New [Byte](0) {&H30}
            Dim len As Int32 = 0

            ' TECポケプリ：レシートモード
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("M;1")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bLF.CopyTo(bBuf, len)
            len = len + bLF.Length
            b00.CopyTo(bBuf, len)
            len = len + b00.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("!")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            b30.CopyTo(bBuf, len)
            len = len + b30.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("Test Print")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bLF.CopyTo(bBuf, len)
            len = len + bLF.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("!")
            b30.CopyTo(bBuf, len)
            len = len + b30.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("RECEIPT mode")
            bBufWork.CopyTo(bBuf, len)
            len = len + bBufWork.Length
            bLF.CopyTo(bBuf, len)
            len = len + bLF.Length
            bESC.CopyTo(bBuf, len)
            len = len + bESC.Length
            bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes("!")
            b00.CopyTo(bBuf, len)
            len = len + b00.Length
            bLF.CopyTo(bBuf, len)
            len = len + bLF.Length
            bLF.CopyTo(bBuf, len)
            len = len + bLF.Length
            bLF.CopyTo(bBuf, len)
            len = len + bLF.Length

            dsizeSet = CType(bBuf.Length, UInt32)
            pBufSet = Marshal.AllocCoTaskMem(CType(dsizeSet, Int32))
            Marshal.Copy(bBuf, 0, pBufSet, CType(dsizeSet, Int32))
            ret = Bluetooth.btBluetoothSPPSend(pBufSet, dsizeSet, ssizeGet)
            Marshal.FreeCoTaskMem(pBufSet)
            If ret <> LibDef.BT_OK Then
                disp = "btBluetoothSPPSend error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                GoTo L_END1
            End If
            ' 印刷成功
            disp = "印刷成功しました。"
            MessageBox.Show(disp, "印刷完了")
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
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        Finally
        End Try
    End Sub

    '*******************************************************************************
    '* Bluetooth接続(プチラパン、BT-PR2用)
    '*******************************************************************************
    Private Function Bluetooth_Connect(ByVal stInfoSet As LibDef.BT_BLUETOOTH_TARGET, ByVal pin As StringBuilder, ByVal pinlen As UInt32) As [Boolean]
        Dim bRet As [Boolean] = False
        Dim ret As Int32 = 0
        Dim disp As [String] = ""

        Try
            ret = Bluetooth.btBluetoothOpen()
            If ret <> LibDef.BT_OK Then
                disp = "btBluetoothOpen error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return bRet
            End If

            ret = Bluetooth.btBluetoothPairing(stInfoSet, pinlen, pin)
            If ret <> LibDef.BT_OK Then
                disp = "btBluetoothPairing error ret[" & ret & "]"
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

    '*******************************************************************************
    '* Bluetooth接続(TEC用)
    '*******************************************************************************
    Private Function Bluetooth_Connect_Tec(ByVal stInfoSet As LibDef.BT_BLUETOOTH_TARGET, ByVal pin As StringBuilder, ByVal pinlen As UInt32) As [Boolean]
        Dim bRet As [Boolean] = False
        Dim ret As Int32 = 0
        Dim disp As [String] = ""

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

    '*******************************************************************************
    '* Bluetooth プリンタ通信（SPP送信）
    '*******************************************************************************
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
                disp = "btBluetoothSPPSend error ret[" & ret & "]"
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

    '*******************************************************************************
    '* Bluetooth プリンタ通信（SPP受信）
    '*******************************************************************************
    Private Function SppRecv(ByRef bBuf As [Byte](), ByRef rsize As UInt32) As [Boolean]
        Dim bRet As [Boolean] = False
        Dim ret As Int32 = 0

        Dim dsizeSet As UInt32 = 0
        Dim rsizeGet As UInt32 = 0
        Dim pBufGet As IntPtr
        Dim bBufGet As [Byte]() = New [Byte]() {}

        Try
            Thread.Sleep(1000)
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

    '*******************************************************************************
    '* 無線LANプリント
    '		 * @param	[in]	String	ssid		：SSID
    '		 * @param	[in]	String	wepkey		：WERキー
    '		 * @param	[in]	String	ipaddr		：プリンタのIPアドレス
    '		 * @param	[in]	Int32	mode		：プリンタ種類(1:SATO 2:TEC)
    '*******************************************************************************
    Private Sub Wlan_Print(ByVal ssid As [String], ByVal wepkey As [String], ByVal ipaddr As [String], ByVal mode As Int32)
        Dim ret As Int32 = 0
        Dim disp As [String] = ""

        Try
            ' 無線LAN接続
            If Wlan_Connect(ssid, wepkey) = False Then
                Return
            End If

            If mode = 1 Then
                ' FTP送信
                If FTP_Put(ipaddr, mode) = False Then
                    GoTo L_END
                End If
            Else
                ' ソケット(TCP)転送
                If TCP_Xmit(ipaddr, mode) = False Then
                    GoTo L_END
                End If
            End If

            disp = "印刷成功しました。"
            MessageBox.Show(disp, "印刷完了")

L_END:
            ret = Wlan.btWLANClose()
            If ret <> LibDef.BT_OK Then
                disp = "btWLANClose error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        Finally
        End Try
    End Sub

    '*******************************************************************************
    '* FTP送信処理（SATO プチラパン）
    '		 * @param	[in]	String	ipaddr		：プリンタのIPアドレス
    '		 * @param	[in]	Int32	mode		：プリンタ種類(1:SATO 2:TEC)
    '		 * @return	Boolean(true:成功 false:失敗)
    '*******************************************************************************
    Private Function FTP_Put(ByVal ipaddr As [String], ByVal mode As Int32) As [Boolean]
        Dim bRet As [Boolean] = True
        Dim disp As [String] = ""

        Dim hInternetOpen As IntPtr
        Dim hInternetConnect As IntPtr
        Dim strUser As [String] = ""
        Dim strPswd As [String] = ""
        Dim strLocalPath As [String] = ""
        Dim strRemotePath As [String] = ""

        Try
            ' インターネットオープン(FTPサーバ接続準備)
            hInternetOpen = InternetOpen("FtpClient", INTERNET_OPEN_TYPE_DIRECT, Nothing, Nothing, 0)
            If hInternetOpen = 0 Then
                disp = "InternetOpen error ret[" & hInternetOpen.ToString() & "]"
                MessageBox.Show(disp, "エラー")
                bRet = False
                GoTo L_END3
            End If
            Thread.Sleep(3000)

            ' FTPサーバ接続
            strUser = "admin"
            strPswd = "admin"
            hInternetConnect = InternetConnect(hInternetOpen, ipaddr, INTERNET_DEFAULT_FTP_PORT, strUser, strPswd, INTERNET_SERVICE_FTP, _
             0, IntPtr.Zero)
            If hInternetConnect = 0 Then
                disp = "InternetConnect error ret[" & hInternetConnect.ToString() & "]"
                MessageBox.Show(disp, "エラー")
                bRet = False
                GoTo L_END2
            End If

            Thread.Sleep(3000)

            ' FTP送信
            strLocalPath = "RamDisk/sato_wlanprn.pm"
            strRemotePath = "sato_wlanprn.pm"
            bRet = FtpPutFile(hInternetConnect, strLocalPath, strRemotePath, FTP_TRANSFER_TYPE_BINARY, IntPtr.Zero)
            If bRet = False Then
                disp = "FtpPutFile error"
                MessageBox.Show(disp, "エラー")
                bRet = False
                GoTo L_END1
            End If

L_END1:
            InternetCloseHandle(hInternetConnect)
L_END2:
            InternetCloseHandle(hInternetOpen)
L_END3:
            Return bRet
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
            bRet = False
            Return bRet
        Finally
        End Try
    End Function

    '*******************************************************************************
    '* ソケット(TCP)送信処理（TEC ポケプリ）
    '		 * @param	[in]	String	hostName	：プリンタのIPアドレス(ホスト名)
    '		 * @param	[in]	Int32	mode		：プリンタ種類(1:SATO 2:TEC)
    '		 * @return	Boolean(true:成功 false:失敗)
    '*******************************************************************************
    Private Function TCP_Xmit(ByVal hostName As [String], ByVal mode As Int32) As [Boolean]
        Try
            Using reader As New BinaryReader(File.Open("\RamDisk\tec_wlanprn.pm", FileMode.Open))
                '
                '* TEC ポケプリの仕様
                '* ・制御コマンドをTCP/IPにて、送受信します。
                '* ・相手側が、TCPセッションを切断した場合、次のデータから継続する必要があります
                '* 　（再送ではありません）
                '
                For i As Integer = 0 To 9
                    Using client As New TcpClient()
                        ' プリンタへconnect(セッションが切断される場合があるので、TCPClientを破棄->再connectします)
                        Try
                            Dim ipaddr As IPAddress = IPAddress.Parse(hostName)
                            client.Connect(ipaddr, 1024)
                        Catch generatedExceptionName As FormatException
                            client.Connect(hostName, 1024)
                        End Try
                        Try
                            Dim writer As New BinaryWriter(client.GetStream())
                            While True
                                writer.Write(reader.ReadByte())
                                ' 1byteずつ転送(理由は、次のデータから継続する必要があるため)
                                ' 成功したら、0
                                i = 0
                            End While
                        Catch generatedExceptionName As IOException
                            ' 相手から、シャットダウンされた場合、再送で引き続き送る転送する必要がある
                            Continue For
                        End Try
                    End Using
                Next
            End Using
        Catch generatedExceptionName As EndOfStreamException
            ' ここは転送終了(読み取り側の)
            Return True
        Catch generatedExceptionName As SocketException
            Return False
        Catch generatedExceptionName As ObjectDisposedException
            Return False
        Finally
        End Try
        Return True
    End Function

    '*******************************************************************************
    '* 無線LAN接続（アドホック）
    '*******************************************************************************
    Private Function Wlan_Connect(ByVal ssid As [String], ByVal wepkey As [String]) As [Boolean]
        Dim bRet As [Boolean] = False
        Dim ret As Int32 = 0
        Dim disp As [String] = ""

        Dim strValueSet As [String] = ""
        Dim ulValueSet As UInt32 = 0
        Dim pstrValueSet As IntPtr
        Dim plValueSet As IntPtr
        Dim ulOpenMode As UInt32 = LibDef.BT_WLAN_OPEN_ASYNC
        Dim plValueGet As IntPtr

        plValueSet = Marshal.AllocCoTaskMem(Marshal.SizeOf(ulValueSet))
        plValueGet = Marshal.AllocCoTaskMem(Marshal.SizeOf(ulOpenMode))

        Try
            ' 必要なプロパティをセット
            '   SSID（アドホック）
            strValueSet = ssid
            pstrValueSet = Marshal.StringToBSTR(strValueSet)
            ret = Wlan.btWLANSetProperty(CType(LibDef.BTWLAN_PROPID.BT_WLAN_PROP_SSID_ADHOC, UInt32), pstrValueSet)
            Marshal.FreeBSTR(pstrValueSet)
            If ret <> LibDef.BT_OK Then
                disp = "btWLANSetProperty error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return bRet
            End If
            '   ネットワークモード（インフラ／アドホック）
            ulValueSet = LibDef.BT_WLAN_BSS_ADHOC
            Marshal.WriteInt32(plValueSet, CType(ulValueSet, Int32))
            ret = Wlan.btWLANSetProperty(CType(LibDef.BTWLAN_PROPID.BT_WLAN_PROP_NETMODE, UInt32), plValueSet)
            If ret <> LibDef.BT_OK Then
                disp = "btWLANSetProperty error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return bRet
            End If
            '   暗号化モード
            ulValueSet = LibDef.BT_WLAN_ENC_WEP
            Marshal.WriteInt32(plValueSet, CType(ulValueSet, Int32))
            ret = Wlan.btWLANSetProperty(CType(LibDef.BTWLAN_PROPID.BT_WLAN_PROP_ENCRYPTION_ADHOC, UInt32), plValueSet)
            If ret <> LibDef.BT_OK Then
                disp = "btWLANSetProperty error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return bRet
            End If
            '   認証モード
            ulValueSet = LibDef.BT_WLAN_AUTH_OPEN
            Marshal.WriteInt32(plValueSet, CType(ulValueSet, Int32))
            ret = Wlan.btWLANSetProperty(CType(LibDef.BTWLAN_PROPID.BT_WLAN_PROP_AUTHENTICATION_ADHOC, UInt32), plValueSet)
            If ret <> LibDef.BT_OK Then
                disp = "btWLANSetProperty error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return bRet
            End If
            '   WEPキー文字列（アドホック）
            strValueSet = wepkey
            pstrValueSet = Marshal.StringToBSTR(strValueSet)
            ret = Wlan.btWLANSetProperty(CType(LibDef.BTWLAN_PROPID.BT_WLAN_PROP_WEP_KEY_ADHOC, UInt32), pstrValueSet)
            Marshal.FreeBSTR(pstrValueSet)
            If ret <> LibDef.BT_OK Then
                disp = "btWLANSetProperty error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return bRet
            End If
            '   WEPキーインデックス（アドホック）
            ulValueSet = 1
            Marshal.WriteInt32(plValueSet, CType(ulValueSet, Int32))
            ret = Wlan.btWLANSetProperty(CType(LibDef.BTWLAN_PROPID.BT_WLAN_PROP_WEP_KEYINDEX_ADHOC, UInt32), plValueSet)
            If ret <> LibDef.BT_OK Then
                disp = "btWLANSetProperty error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return bRet
            End If

            ret = Wlan.btWLANOpen()
            If ret <> LibDef.BT_OK Then
                If ret = LibDef.BT_ERR_COMM_ALREADY_OPEN Then
                Else
                    disp = "btWLANOpen error ret[" & ret & "]"
                    MessageBox.Show(disp, "エラー")
                    Return bRet
                End If
            End If

            '   無線LANオープンモード(取得)
            ret = Wlan.btWLANGetProperty(CType(LibDef.BTWLAN_PROPID.BT_WLAN_PROP_OPENMODE, UInt32), plValueGet)
            If ret <> LibDef.BT_OK Then
                disp = "btWLANGetProperty error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return bRet
            End If
            ulOpenMode = Marshal.ReadInt32(plValueGet)

            ' 無線LANのオープンモードが非同期の場合、ここでメッセージ/イベント取得を行うことで
            ' 接続完了を待つことができます。
            If ulOpenMode = LibDef.BT_WLAN_OPEN_ASYNC Then
                Dim sbEvent As New StringBuilder(LibDef.EVT_NAME_WLAN_CONNECT)
                Dim hEvent As New IntPtr()
                hEvent = CreateEvent(IntPtr.Zero, False, False, sbEvent.ToString())
                If hEvent <> IntPtr.Zero Then
                    WaitForSingleObject(hEvent, 30000)
                    CloseHandle(hEvent)
                End If
            End If

            bRet = True
            Return bRet
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
            Return bRet
        Finally
            Marshal.FreeCoTaskMem(plValueSet)
            Marshal.FreeCoTaskMem(plValueGet)
        End Try
    End Function

End Class
