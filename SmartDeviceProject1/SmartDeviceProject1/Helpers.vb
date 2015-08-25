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


Module Helpers

    Public Function Bluetooth_Connect(ByVal stInfoSet As LibDef.BT_BLUETOOTH_TARGET, ByVal pin As StringBuilder, ByVal pinlen As UInt32) As [Boolean]
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
    Public Function SppSend(ByVal bBuf As [Byte](), ByRef ssize As UInt32) As [Boolean]
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

    '*******************************************************************************
    '* Bluetooth プリンタ通信（SPP受信）
    '*******************************************************************************
    Public Function SppRecv(ByRef bBuf As [Byte](), ByRef rsize As UInt32) As [Boolean]
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


    Public Function SplitString(ByVal TheString As String, ByVal StringLen As Integer) As String()
        Dim ArrCount As Integer  'as it is declared locally, it will automatically reset to 0 when this is called again
        Dim I As Long  'we are going to use it.. so declare it (with local scope to avoid breaking other code)
        Dim TempArray() As String
        ReDim TempArray((Len(TheString) - 1) \ StringLen)
        For I = 1 To Len(TheString) Step StringLen
            TempArray(ArrCount) = Mid$(TheString, I, StringLen)
            ArrCount = ArrCount + 1
        Next
        SplitString = TempArray   'actually return the value
    End Function


    Public Function padLineTo8Bit(ByVal line As String) As String
        'pad to 8 bit
        Dim padding As Integer = 8 - (line.Length Mod 8)
        'Dim paddedline(bmp.Width + padding - 1) As Char
        Dim paddedline(line.Length + padding - 1) As Char
        paddedline = line
        For i As Integer = 0 To padding - 1
            paddedline = paddedline + "0"
        Next
        Return paddedline
    End Function


    Public Sub printImage(ByRef bBuf() As Byte, ByRef len As Int32)

        Dim bBufWork As [Byte]() = New [Byte]() {}
        Dim bmp As Bitmap = New Bitmap("\logo.bmp")

        Dim n1 As Byte = (bmp.Width + (8 - (bmp.Width Mod 8))) / 8
        Dim n2 As Byte = bmp.Height

        bBufWork = New Byte() {&H1B, &H62, n1, n2, &H0}
        bBufWork.CopyTo(bBuf, len)
        len = len + bBufWork.Length

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
                bBufWork.CopyTo(bBuf, len)
                len = len + bBufWork.Length
            Next
        Next

        bBufWork = New Byte() {&H1B, &H4A, &H0}
        bBufWork.CopyTo(bBuf, len)
        len = len + bBufWork.Length
    End Sub


    Public Sub registerImage(ByRef bBuf() As Byte, ByRef len As Int32, ByRef filename As String, ByVal number As Int32)

        Dim bBufWork As [Byte]() = New [Byte]() {}
        Dim bmp As Bitmap = New Bitmap(filename)

        Dim n1 As Byte = (bmp.Width + (8 - (bmp.Width Mod 8))) / 8
        Dim n2 As Byte = bmp.Height

        Select Case number
            Case 0
                Commands.startRegisterImage0.CopyTo(bBuf, len)
                len = len + Commands.startRegisterImage0.Length
            Case 1
                Commands.startRegisterImage1.CopyTo(bBuf, len)
                len = len + Commands.startRegisterImage0.Length
            Case 2
                Commands.startRegisterImage2.CopyTo(bBuf, len)
                len = len + Commands.startRegisterImage0.Length
        End Select

        bBufWork = New Byte() {&H1B, &H62, n1, n2, &H0}
        bBufWork.CopyTo(bBuf, len)
        len = len + bBufWork.Length

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
                bBufWork.CopyTo(bBuf, len)
                len = len + bBufWork.Length
            Next
        Next

        bBufWork = New Byte() {&H1B, &H4A, &H0}
        bBufWork.CopyTo(bBuf, len)
        len = len + bBufWork.Length

        Commands.finishRegisterImage.CopyTo(bBuf, len)
        len = len + Commands.finishRegisterImage.Length
    End Sub



    Public Sub printRegisterImage(ByRef bBuf() As Byte, ByRef len As Int32, ByVal number As Int32)
        Select Case number
            Case 0
                Commands.printRegisterImage0.CopyTo(bBuf, len)
                len = len + Commands.printRegisterImage0.Length
            Case 1
                Commands.printRegisterImage1.CopyTo(bBuf, len)
                len = len + Commands.printRegisterImage1.Length
            Case 2
                Commands.printRegisterImage2.CopyTo(bBuf, len)
                len = len + Commands.printRegisterImage2.Length
        End Select
    End Sub



    Public Sub printReceiptContents(ByRef bBuf() As Byte, ByRef len As Int32)
        setAlignLeft(bBuf, len)
        printString(bBuf, len, DateTime.Now.ToString, 0)
        printString(bBuf, len, "商品名　　　　　数量　　　　金額", 2)

        Dim items() As Items = New Items() {New Items("0000", "宮崎牛　盛り合わせセット", 19999, 3), _
                                            New Items("1111", "生ビール　キリン", 1990, 2), _
                                            New Items("2222", "生薬", 500, 7)}
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
    End Sub


    ' -----------------------------------------------------------------------
    ' Alignment
    Public Sub setAlignLeft(ByRef bBuf() As Byte, ByRef len As Int32)
        Commands.bAlignLeft.CopyTo(bBuf, len)
        len = len + Commands.bAlignLeft.Length
    End Sub
    Public Sub setAlignCenter(ByRef bBuf() As Byte, ByRef len As Int32)
        Commands.bAlignCenter.CopyTo(bBuf, len)
        len = len + Commands.bAlignCenter.Length
    End Sub
    Public Sub setAlignRight(ByRef bBuf() As Byte, ByRef len As Int32)
        Commands.bAlignRight.CopyTo(bBuf, len)
        len = len + Commands.bAlignRight.Length
    End Sub

    ' -----------------------------------------------------------------------
    ' Size
    Public Sub setSizeNormal(ByRef bBuf() As Byte, ByRef len As Int32)
        Commands.bSizeNor.CopyTo(bBuf, len)
        len = len + Commands.bSizeNor.Length
    End Sub
    Public Sub setSizeDouble(ByRef bBuf() As Byte, ByRef len As Int32)
        Commands.bSizeDbl.CopyTo(bBuf, len)
        len = len + Commands.bSizeDbl.Length
    End Sub
    Public Sub setSizeTriple(ByRef bBuf() As Byte, ByRef len As Int32)
        Commands.bSizeTri.CopyTo(bBuf, len)
        len = len + Commands.bSizeTri.Length
    End Sub
    Public Sub setDoulbeStike(ByRef bBuf() As Byte, ByRef len As Int32)
        Commands.bDoulbeStrik.CopyTo(bBuf, len)
        len = len + Commands.bDoulbeStrik.Length
    End Sub
    Public Sub setNonDoubleStrike(ByRef bBuf() As Byte, ByRef len As Int32)
        Commands.bNonDoulbeStrik.CopyTo(bBuf, len)
        len = len + Commands.bNonDoulbeStrik.Length
    End Sub


    Public Sub printString(ByRef bBuf() As Byte, ByRef len As Int32, ByVal contents As String, ByVal underline As Integer)
        Dim bBufWork As [Byte]() = New [Byte]() {}
        Dim LF As [Byte] = &HA
        Dim bLF As [Byte]() = New [Byte](0) {LF}
        Dim CR As [Byte] = &HD
        Dim bCR As [Byte]() = New [Byte](0) {CR}



        Select Case underline
            Case 0
                Commands.bUnderline0.CopyTo(bBuf, len)
                len = len + Commands.bUnderline0.Length
            Case 1
                Commands.bUnderline1.CopyTo(bBuf, len)
                len = len + Commands.bUnderline1.Length
            Case 2
                Commands.bUnderline2.CopyTo(bBuf, len)
                len = len + Commands.bUnderline2.Length
        End Select


        Commands.bJp.CopyTo(bBuf, len)
        len = len + Commands.bJp.Length



        bBufWork = System.Text.Encoding.GetEncoding(932).GetBytes(contents)
        bBufWork.CopyTo(bBuf, len)
        len = len + bBufWork.Length

        bCR.CopyTo(bBuf, len)
        len = len + bCR.Length

        bLF.CopyTo(bBuf, len)
        len = len + bLF.Length
    End Sub

End Module
