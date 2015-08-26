Imports System.Data
Public Class Commands

    'Command : 00 Ending
    Public Shared nullEnding As [Byte]() = New [Byte](0) {&H0}

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

    'Command : Register Image
    Public Shared startRegisterImage0 As [Byte]() = {&H1D, &H54, &H0}
    Public Shared startRegisterImage1 As [Byte]() = {&H1D, &H54, &H1}
    Public Shared startRegisterImage2 As [Byte]() = {&H1D, &H54, &H2}
    Public Shared finishRegisterImage As [Byte]() = {&H1D, &H54, &HFF}

    'Command : Print Register Image
    Public Shared printRegisterImage0 As [Byte]() = {&H1D, &H50, &H0}
    Public Shared printRegisterImage1 As [Byte]() = {&H1D, &H50, &H1}
    Public Shared printRegisterImage2 As [Byte]() = {&H1D, &H50, &H2}


    'Command : HRI Character
    Public Shared hriNon As [Byte]() = {&H1D, &H48, &H0}
    Public Shared hriAbove As [Byte]() = {&H1D, &H48, &H1}
    Public Shared hriBelow As [Byte]() = {&H1D, &H48, &H2}
    Public Shared hriBoth As [Byte]() = {&H1D, &H48, &H3}

    'Command : Print Barcode
    Public Shared printJAN13 As [Byte]() = {&H1D, &H6B, &H2}



End Class
