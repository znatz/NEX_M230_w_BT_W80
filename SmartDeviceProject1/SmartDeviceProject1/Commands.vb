Imports System.Data
Public Class Commands

    'Command : Japanese Encoding => Shift-JIS
    Public Shared bJp As [Byte]() = {&H1C, &H26, &H1C, &H43, &H1}

    'Command : Double Strike
    Public Shared bDoulbeStrik As [Byte]() = {&H1B, &H47, &H1}
    Public Shared bNonDoulbeStrik As [Byte]() = {&H1B, &H47, &H0}

    'Command : Alignment
    Public Shared bAlignLeft As [Byte]() = {&H1B, &H61, &H0}
    Public Shared bAlignCenter As [Byte]() = {&H1B, &H61, &H1}
    Public Shared bAlignRight As [Byte]() = {&H1B, &H61, &H2}

    'Command : Size
    Public Shared bSizeNor As [Byte]() = {&H1D, &H21, Convert.ToInt32("00000000", 2)}
    Public Shared bSizeDbl As [Byte]() = {&H1D, &H21, Convert.ToInt32("00010001", 2)}
    Public Shared bSizeTri As [Byte]() = {&H1D, &H21, Convert.ToInt32("00001010", 2)}

    'Command : Feed 4 lines
    Public Shared nFeedLine As [Byte]() = {&H1B, &H64, &H4}

    'Command : Cut Paper
    Public Shared bCut As [Byte]() = {&H1B, &H69}

    'Command : Underline
    Public Shared bUnderline0 As [Byte]() = {&H1C, &H2D, &H0}
    Public Shared bUnderline1 As [Byte]() = {&H1C, &H2D, &H1}
    Public Shared bUnderline2 As [Byte]() = {&H1C, &H2D, &H2}



End Class
