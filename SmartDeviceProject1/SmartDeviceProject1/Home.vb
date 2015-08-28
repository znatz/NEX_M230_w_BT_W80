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
'Imports System.Data.SqlServerCe

Imports System.Data.SQLite
Imports NEX_M2302B.POSCO.Printer




'Imports Excel
'Imports Excel.Core
'Imports Excel.ExcelBinaryReader
'Imports ICSharpCode


Public Class Home
    Public Shared posJAN As New ArrayList()


#Region "Database"
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
#End Region


    '-----------------------------------------------------------------------
    ' Register Image
    Private Sub MenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRegisterImage.Click
        Dim printer As NEX_M2302B.POSCO.Printer = New NEX_M2302B.POSCO.Printer(50960, "NEX-M230", "742B628FF070")
        printer.bufferRegisterImage("\logo.bmp", 0)
        printer.bufferSetAlignCenter()
        printer.bufferSetDoulbeStike()
        printer.bufferPrintString("画像登録成功しました。", 0)
        printer.startPrint()
    End Sub


    '-----------------------------------------------------------------------
    ' Exit Program
    Private Sub MenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItem1.Click
        Application.Exit()
    End Sub

    Private Sub Scanning_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Scanning.Click
        Dim scanner As frmScanner = New frmScanner
        scanner.Show()
        Me.Hide()
    End Sub

    Private Sub MenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItem2.Click
        Dim regItem As frmRegisterItem = New frmRegisterItem
        regItem.Show()
        Me.Hide()
    End Sub


    Private Sub Button2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim connection As New SQLiteConnection()
        Dim query As New SQLiteCommand
        Dim result As SQLiteDataReader
        Dim disp As String = ""
        connection.ConnectionString = "Data Source=Sales.db;"
        query = connection.CreateCommand()
        query.CommandText = "Select * From BTSMAS limit 10"
        connection.Open()
        result = query.ExecuteReader()
        While result.Read
            disp = disp & result.GetString(2)
        End While
        connection.Close()

        MessageBox.Show(disp, "SQLITE")
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim printer As NEX_M2302B.POSCO.Printer = New NEX_M2302B.POSCO.Printer(50960, "NEX-M230", "742B628FF070")
        'printer.bufferPrintRegisterImage(0)
        printer.bufferPrintString("テスト", 0)
        For Each item In posJAN
            printer.bufferPrintString(getGoodsName(item.ToString), 0)
        Next
        printer.startPrint()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
        Me.Dispose()
    End Sub


    Dim sock As Socket

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

        '解決したいホスト名
        Dim hostName As String = "posco-cloud.sakura.ne.jp"

        'IPHostEntryオブジェクトを取得
        Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(hostName)

        'IPアドレスのリストを取得
        Dim adList As System.Net.IPAddress() = iphe.AddressList

        ''IPアドレスを列挙
        'Dim i As Integer
        'For i = 0 To adList.Length - 1
        '    Console.WriteLine(adList(i).ToString())
        'Next



        Dim ftps As IPEndPoint          '送り先のエンドポイント（IPアドレス+ポート）
        'Dim Myip As IPAddress           '送信元のIPアドレス
        Dim Tcpl As TcpListener         '送信元でFTPサーバからのアクセスを待つオブジェクト

        'Dim Myport As Integer '送信元のポート

        sock = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        ftps = New IPEndPoint(IPAddress.Parse(adList(0).ToString()), 21)

        sock.Connect(ftps) 'リモート　ホストへの接続を確立します。

        SendCommand("USER posco-cloud") 'ユーザ送信
        SendCommand("PASS p8vx98hzru") 'パスワード送信
        SendCommand("TYPE I") 'バイナリモードに変換
        SendCommand("PASV")
        Dim localendpoint As IPEndPoint = SendCommandAndGetEndpoint("PASV")

        SendCommand("STOR Sales.db")

        'Tcpl = New TcpListener(localendpoint)


        'Dim Dsocket As Socket = Tcpl.AcceptSocket


        ''アップロードするファイルを開く
        Dim fs As New System.IO.FileStream("\Sales.db", System.IO.FileMode.Open, System.IO.FileAccess.Read)
        Dim buffer(fs.Length) As Byte
        Dim x As Integer = CInt(fs.Length)

        'Dim readSize As Integer = fs.Read(buffer, 0, x)

        'Tcpl.Start()
        'Dsocket.SendTo(buffer, Tcpl.LocalEndpoint)

        'Dsocket.Close()

        'Tcpl.Stop()

        MessageBox.Show(localendpoint.ToString)

        Dim sendSocket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        sendSocket.Connect(localendpoint) Then
        sendSocket.Send(buffer, buffer.Length, SocketFlags.DontRoute)

        sendSocket.Close()

        '//終わりましたよとコマンド送信
        SendCommand("QUIT")

    End Sub

   
    Private Sub SendCommand(ByVal sCommand As String)

        Dim bytes(255) As Byte
        Dim i As Integer

        sCommand = sCommand & ControlChars.CrLf
        Dim cmdbytes As Byte() = Encoding.ASCII.GetBytes(sCommand)
        sock.Send(cmdbytes, cmdbytes.Length, 0)

        i = sock.Receive(bytes)
        MessageBox.Show(Encoding.UTF8.GetString(bytes, 0, bytes.Length - 1))

    End Sub


    Private Function SendCommandAndGetEndpoint(ByVal sCommand As String) As IPEndPoint

        Dim bytes(255) As Byte
        Dim i As Integer

        sCommand = sCommand & ControlChars.CrLf
        Dim cmdbytes As Byte() = Encoding.ASCII.GetBytes(sCommand)
        sock.Send(cmdbytes, cmdbytes.Length, 0)

        '　Get　reply　from　the　server.
        i = sock.Receive(bytes)

        Dim response As String = Encoding.UTF8.GetString(bytes, 0, bytes.Length)

        MessageBox.Show("PASV get : " + response)

        Dim startpos = response.IndexOf("(") + 1
        Dim endpos = response.IndexOf(")")
        MessageBox.Show(startpos.ToString + " : " + endpos.ToString)

        Dim ips = response.Substring(startpos, endpos - startpos)



        Dim parts() As String = ips.Split(",")
        Dim ipAddr As String = parts(0) + "." + parts(1) + "." + parts(2) + "." + parts(3)
        Dim portNo As String = Convert.ToInt32(parts(4)) * 256 + Convert.ToInt32(5)
        Dim localendpoint As IPEndPoint = New IPEndPoint(IPAddress.Parse(ipAddr), portNo)
        Return localendpoint

    End Function

End Class