Imports System.Data.SQLite


Module DBHelper
    Public Structure SQLitePair
        Public dataReader As SQLiteDataReader
        Public connection As SQLiteConnection
    End Structure

    Public Function getDBReader(ByVal dbfilename As String, ByVal query As String) As SQLitePair
        Dim connection As New SQLiteConnection()
        Dim qry As New SQLiteCommand
        Dim reader As SQLiteDataReader
        Dim jan As String = ""
        connection.ConnectionString = "Data Source=" & dbfilename & ";"
        qry = connection.CreateCommand()
        qry.CommandText = query
        connection.Open()
        reader = qry.ExecuteReader()
        Dim result As SQLitePair = New SQLitePair
        result.dataReader = reader
        result.connection = connection
        Return result
    End Function

    'Public Function getGoodsName(ByVal jan As String)
    '    Dim name As String = ""
    '    Dim query As String = "Select * From BTSMAS Where ｺｰﾄﾞ='" & jan & "'"

    '    Dim dbpair As SQLitePair = getDBReader("Sales.db", query)
    '    While dbpair.dataReader.Read
    '        name = dbpair.dataReader.GetString(2)
    '    End While
    '    dbpair.connection.Close()
    '    MessageBox.Show(name, "SQLITE")
    '    Return name
    'End Function

    Public Function getGoodsName(ByRef jan As String)

        Dim connection As New SQLiteConnection()
        Dim qry As New SQLiteCommand
        Dim reader As SQLiteDataReader

        Dim query As String = "Select * From BTSMAS Where ｺｰﾄﾞ= '" + jan.Substring(0, 13) + "'"

        connection.ConnectionString = "Data Source=Sales.db;"
        qry = connection.CreateCommand()
        qry.CommandText = query

        connection.Open()
        reader = qry.ExecuteReader()

        Dim name As String = ""

        While reader.Read
            name = reader.GetString(2).Trim()
        End While

        connection.Close()
        Return name

    End Function


End Module
