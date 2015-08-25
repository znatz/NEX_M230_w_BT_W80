Public Class Items
    Private _JAN As String
    Private _title As String
    Private _price As Integer
    Private _count As Integer
    Public Property title() As String
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
        End Set
    End Property
    Public Property price() As Integer
        Get
            Return _price
        End Get
        Set(ByVal value As Integer)
            _price = value
        End Set
    End Property
    Public Property count() As Integer
        Get
            Return _count
        End Get
        Set(ByVal value As Integer)
            _count = value
        End Set
    End Property
    Public Property jan() As String
        Get
            Return _JAN
        End Get
        Set(ByVal value As String)
            _JAN = value
        End Set
    End Property
    Public Sub New(ByVal jan As String, ByVal title As String, ByVal price As Integer, ByVal count As Integer)
        _JAN = jan
        _title = title
        _price = price
        _count = count
    End Sub
End Class
