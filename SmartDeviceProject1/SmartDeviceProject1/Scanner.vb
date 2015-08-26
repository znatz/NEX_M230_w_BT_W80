Imports System.Runtime.InteropServices

Public Class frmScanner
    <DllImport("coredll.dll", EntryPoint:="DeleteObject")> _
    Public Shared Function DeleteObject(ByVal hObject As IntPtr) As Boolean
    End Function
    Public Shared ScanMode As Int32 = 0
    Private MsgWin As MsgWindow
    Public Shared scannedJAN As New ArrayList()



    Public Sub New()
        InitializeComponent()

        ' メッセージウインドウインスタンス作成
        Me.MsgWin = New MsgWindow(ScanResult)
    End Sub


    Private Sub MenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItem2.Click
        Application.Exit()
    End Sub

    Private Sub MenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItem1.Click
        Dim home As Home = New Home
        home.Show()
        Me.Hide()
    End Sub

    Private Sub btnStartScan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartScan.Click
        Dim ret As Int32 = 0
        Dim disp As [String] = ""

        Try
            ' スキャンモード＝「個別」にセット
            ScanMode = 1

            ret = Bt.ScanLib.Control.btScanEnable()
            If ret <> LibDef.BT_OK Then
                disp = "btScanEnable error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return
            End If

            ret = Bt.ScanLib.Control.btScanSoftTrigger(1)
            If ret <> LibDef.BT_OK Then
                disp = "btScanSoftTrigger error ret[" & ret & "]"
                MessageBox.Show(disp, "エラー")
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        End Try
    End Sub


    Public Class MsgWindow
        Inherits Microsoft.WindowsCE.Forms.MessageWindow
        Private listbox As ListBox
        Public Sub New(ByRef listbox As ListBox)
            Me.listbox = listbox
        End Sub

        Protected Overrides Sub WndProc(ByRef msg As Microsoft.WindowsCE.Forms.Message)
            Select Case msg.Msg
                Case CType(LibDef.WM_BT_SCAN, Int32)
                    ' 読み取り成功の場合
                    If msg.WParam.ToInt32() = CType(LibDef.BTMSG_WPARAM.WP_SCN_SUCCESS, Int32) Then
                        If ScanMode = 1 Then
                            ' 読み取り(個別)
                            ScanData_kobetu()
                        ElseIf ScanMode = 2 Then
                            ' 読み取り(一括)
                            ScanData_ikkatu()
                        End If
                    End If
                    Exit Select
            End Select
            MyBase.WndProc(msg)
        End Sub

        '*******************************************************************************
        '             * 機能 ：読み取りコードを個別に取得します。
        '             * API  ：btScanGetResultCount, btScanGetDataSize, btScanGetData, btScanGetOCRData
        '*******************************************************************************

        Public Sub ScanData_kobetu()

            Dim ret As Int32 = 0
            Dim disp As [String] = ""

            Dim resultCount As Int32 = 0
            Dim codedataGet As [Byte]()
            Dim codeLen As Int32 = 0
            Dim stReportGet As New LibDef.BT_SCAN_REPORT()
            Dim stQrReportGet As New LibDef.BT_SCAN_QR_REPORT()

            Try
                resultCount = Bt.ScanLib.Control.btScanGetResultCount()
                If resultCount <= 0 Then
                    disp = "btScanGetResultCount error ret[" & resultCount & "]"
                    MessageBox.Show(disp, "エラー")
                    GoTo L_END
                End If

                ' コード読み取りされた場合
                If resultCount > 0 Then
                    '-----------------------------------------------------------
                    ' 読み取り(個別)
                    '-----------------------------------------------------------
                    For i As UInt32 = 0 To resultCount - 1
                        codeLen = 0
                        codeLen = Bt.ScanLib.Control.btScanGetDataSize(i)
                        If codeLen <= 0 Then
                            disp = "btScanGetDataSize error ret[" & codeLen & "]"
                            MessageBox.Show(disp, "エラー")
                            GoTo L_END
                        End If
                        codedataGet = New [Byte](codeLen - 1) {}

                        ' btScanGetData
                        stReportGet = New LibDef.BT_SCAN_REPORT()
                        stQrReportGet = New LibDef.BT_SCAN_QR_REPORT()

                        ret = Bt.ScanLib.Control.btScanGetData(i, codedataGet, stReportGet, stQrReportGet)
                        If ret <> LibDef.BT_OK Then
                            disp = "btScanGetData error ret[" & ret & "]"
                            MessageBox.Show(disp, "エラー")
                            GoTo L_END
                        End If



                        If stReportGet.codetype = LibDef.BT_SCAN_CODE_JAN Then
                            Dim result() As Byte = New [Byte](Bt.ScanLib.Control.btScanGetDataSize(0)) {}
                            Dim objJAN As LibDef.BT_SCAN_REPORT = New LibDef.BT_SCAN_REPORT()
                            ret = Bt.ScanLib.Control.btScanGetData(0, result, objJAN, Nothing)


                            Dim jan As String = System.Text.Encoding.ASCII.GetString(result, 0, result.Length)
                            Me.listbox.Items.Add(getGoodsName(jan))
                            scannedJAN.Add(jan)
                        End If


                        If stReportGet.codetype = LibDef.BT_SCAN_CODE_OCR Then
                            Dim objOcr As LibDef.BT_SCAN_OCR_REPORT = New LibDef.BT_SCAN_OCR_REPORT()
                            Dim objOcrImg As LibDef.BT_SCAN_OCR_REPORT_IMAGE = New LibDef.BT_SCAN_OCR_REPORT_IMAGE()
                            ret = Bt.ScanLib.Control.btScanGetOCRData(objOcr, objOcrImg)
                            If ret <> LibDef.BT_OK Then
                                disp = "btScanGetOCRData error"
                                MessageBox.Show(disp, "エラー")
                                DeleteObject(objOcrImg.bitmap)
                                GoTo L_END
                            End If
                            Dim strAtt As String = ""
                            For j As UInt32 = 0 To objOcr.SourceDataLen - 1
                                strAtt = strAtt & objOcr.CharAttention(j)
                            Next
                            disp = _
                                "OCR認識パターン：" & objOcr.DataFormat & vbCr & vbLf & _
                                "誤認識アラート情報：" & objOcr.AlertType & vbCr & vbLf & _
                                "フォーマット登録番号：" & objOcr.FormatNumber & vbCr & vbLf & _
                                "認識文字列の長さ：" & objOcr.SourceDataLen & vbCr & vbLf & _
                                "認識文字列：" & System.Text.Encoding.ASCII.GetString(objOcr.SourceData, 0, objOcr.SourceDataLen) & vbCr & vbLf & _
                                "文字信頼情報:" & strAtt & vbCr & vbLf

                            MessageBox.Show(disp, "読み取り(個別)")
                            DeleteObject(objOcrImg.bitmap)
                            'Else
                            '    disp = "データサイズ         :" & codeLen & vbCr & vbLf & "[Report]" & vbCr & vbLf & "桁数                  :" & stReportGet.keta & vbCr & vbLf & "コード種別           :" & stReportGet.codetype & vbCr & vbLf & "コンポジットであるか :" & stReportGet.composite & vbCr & vbLf & "品質                  :" & stReportGet.quality & vbCr & vbLf & "詳細情報            :" & stReportGet.extraType & vbCr & vbLf & "コード合成           :" & stReportGet.codelink & vbCr & vbLf & "[QR]" & vbCr & vbLf & "読み取り結果位置 :" & stQrReportGet.pos & vbCr & vbLf & "パリティ                :" & stQrReportGet.parity & vbCr & vbLf & "トータル連結数      :" & stQrReportGet.count & vbCr & vbLf
                            '    MessageBox.Show(disp, "読み取り(個別)")
                        End If

                    Next
                End If
L_END:

                ret = Bt.ScanLib.Control.btScanDisable()
                If ret <> LibDef.BT_OK Then
                    disp = "btScanDisable error ret[" & ret & "]"
                    MessageBox.Show(disp, "エラー")
                End If
            Catch mmex As MissingMethodException
                MessageBox.Show("本端末では使用できないAPIを含んでいます。")
            Catch e As Exception
                MessageBox.Show(e.ToString())
            End Try
        End Sub

        '*******************************************************************************
        '             * 機能 ：読み取りコードを一括で取得します。
        '             * API  ：btScanGetStringSize, btScanGetString
        '*******************************************************************************

        Public Sub ScanData_ikkatu()

            Dim ret As Int32 = 0
            Dim disp As [String] = ""

            Dim codedataGet As [Byte]()
            Dim strCodedata As [String] = ""
            Dim codeLen As Int32 = 0
            Dim symbolGet As UInt16 = 0

            Try
                '-----------------------------------------------------------
                ' 読み取り(一括)
                '-----------------------------------------------------------
                codeLen = Bt.ScanLib.Control.btScanGetStringSize()
                If codeLen <= 0 Then
                    disp = "btScanGetStringSize error ret[" & codeLen & "]"
                    MessageBox.Show(disp, "エラー")
                    GoTo L_END
                End If
                codedataGet = New [Byte](codeLen - 1) {}

                ret = Bt.ScanLib.Control.btScanGetString(codedataGet, symbolGet)
                If ret <> LibDef.BT_OK Then
                    disp = "btScanGetString error ret[" & ret & "]"
                    MessageBox.Show(disp, "エラー")
                    GoTo L_END
                End If
                strCodedata = System.Text.Encoding.GetEncoding(932).GetString(codedataGet, 0, codeLen)
                disp = "データサイズ    :" & codeLen & vbCr & vbLf & "コード種別     :" & symbolGet & vbCr & vbLf & "コード文字列  :" & strCodedata & vbCr & vbLf
                MessageBox.Show(disp, "読み取り(一括)")
L_END:

                ret = Bt.ScanLib.Control.btScanDisable()
                If ret <> LibDef.BT_OK Then
                    disp = "btScanDisable error ret[" & ret & "]"
                    MessageBox.Show(disp, "エラー")
                End If
            Catch e As Exception
                MessageBox.Show(e.ToString())
            End Try
        End Sub
    End Class



    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles deleteOneLine.Click
        ScanResult.Items.RemoveAt(ScanResult.SelectedIndex)
        scannedJAN.RemoveAt(ScanResult.SelectedIndex)
    End Sub

    Private Sub submit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles submit.Click
        Dim frmHome As Home = New Home
        Home.posJAN = scannedJAN
        frmHome.Show()
        Me.Hide()
    End Sub
End Class



