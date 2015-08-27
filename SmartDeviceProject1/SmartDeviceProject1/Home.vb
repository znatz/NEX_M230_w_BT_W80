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

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
    End Sub
End Class