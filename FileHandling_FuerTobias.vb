Imports System.Globalization
Imports System.IO

Module Module1

    Public noData As Double = 5555555
    Private MetroProHeader As Byte() = {&H88, &H1B, &H3, &H6F, &H0, &H1, &H0, &H0, &H3, &H42, &H0, &H0, &H32, &H30, &H30, &H35, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H2, &H0, &H2, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H3, &HE4, &H3, &HE4, &H0, &H3C, &H8C, &H40, &H5D, &H70, &HEC, &HBB, &H53, &H79, &H6E, &H74, &H68, &H65, &H74, &H69, &H63, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H3F, &H0, &H0, &H0, &H35, &H29, &HDD, &HAF, &H0, &H0, &H0, &H0, &H3F, &H80, &H0, &H0, &H0, &H0, &H0, &H0, &H39, &HD0, &HD2, &HE, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H1, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H3, &HE4, &H3, &HE4, &H0, &H0, &H0, &H0, &H0, &HCD, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H35, &H29, &HDD, &HAF, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H31, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H39, &HD0, &HD2, &HE, &H39, &HD0, &HD2, &HE, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0}


    Public Sub SaveMetroProFile(ByVal MyName As String, ByVal NewData(,) As Double, XScale As Double, ZScale As Double, Optional Xsize As Integer = -1, Optional YSize As Integer = -1)
        Dim Values As Byte()
        If Xsize = -1 Then Xsize = NewData.GetLength(0) - 1
        If YSize = -1 Then YSize = NewData.GetLength(1) - 1
        Dim Headerlength As Short = 834
        Dim Dum2Bytes(1) As Byte
        Dim Dum4Bytes(3) As Byte
        Dim X, Y As Integer
        Dim ByteCount As Long = 0
        Dim NewVal As Double
        ReDim Values(Headerlength + (Xsize) * (YSize) * 4)


        Array.Copy(MetroProHeader, Values, Headerlength)

        Dum2Bytes = MetroConvertShortTo2Bytes(Xsize)
        Array.Copy(Dum2Bytes, 0, Values, 68, 2)

        Dum2Bytes = MetroConvertShortTo2Bytes(YSize)
        Array.Copy(Dum2Bytes, 0, Values, 70, 2)

        Dum4Bytes = MetroConvertIntTo4Bytes(Xsize * YSize * 4)       'cn_bytes
        Array.Copy(Dum4Bytes, 0, Values, 72, 4)


        'da keine Intensitätsdaten drin sind, alle diesbezüglichen Verweise im Header loeschen
        Dum2Bytes = MetroConvertShortTo2Bytes(0)
        Array.Copy(Dum2Bytes, 0, Values, 48, 2)

        Dum2Bytes = MetroConvertShortTo2Bytes(0)
        Array.Copy(Dum2Bytes, 0, Values, 50, 2)

        Dum2Bytes = MetroConvertShortTo2Bytes(0)
        Array.Copy(Dum2Bytes, 0, Values, 52, 2)

        Dum2Bytes = MetroConvertShortTo2Bytes(0)
        Array.Copy(Dum2Bytes, 0, Values, 54, 2)

        Dum2Bytes = MetroConvertShortTo2Bytes(0)
        Array.Copy(Dum2Bytes, 0, Values, 56, 2)

        Dum2Bytes = MetroConvertShortTo2Bytes(0)
        Array.Copy(Dum2Bytes, 0, Values, 58, 2)

        Dum4Bytes = MetroConvertIntTo4Bytes(0)
        Array.Copy(Dum4Bytes, 0, Values, 60, 4)

        Dum4Bytes = MetroConvertSingleTo4Bytes(CSng(XScale / 1000))
        Array.Copy(Dum4Bytes, 0, Values, 184, 4)

        Dum4Bytes = MetroConvertSingleTo4Bytes(CSng(ZScale))
        Array.Copy(Dum4Bytes, 0, Values, 164, 4)

        Dum2Bytes = MetroConvertShortTo2Bytes(0)
        Array.Copy(Dum2Bytes, 0, Values, 230, 2)      'Set connect order to zero

        Dum2Bytes = MetroConvertShortTo2Bytes(Xsize)
        Array.Copy(Dum2Bytes, 0, Values, 234, 2)      'Set camera width

        Dum2Bytes = MetroConvertShortTo2Bytes(YSize)
        Array.Copy(Dum2Bytes, 0, Values, 236, 2)      'Set camera height

        ByteCount = Headerlength

        Dim v As Integer = Integer.MaxValue

        For Y = 0 To YSize - 1
            For X = 0 To Xsize - 1
                NewVal = NewData(X, Y)
                Try
                    If NewVal = noData Then
                        Dum4Bytes = MetroPutPixVal(2147483640)
                    Else
                        Dum4Bytes = MetroPutPixVal(CLng(NewVal * 32768))
                    End If
                Catch ex As Exception
                    Dum4Bytes = MetroPutPixVal(2147483640)
                End Try

                Array.Copy(Dum4Bytes, 0, Values, ByteCount, 4)
                ByteCount = ByteCount + 4
            Next
        Next

        Using sr As New BinaryWriter(File.Open(MyName, FileMode.OpenOrCreate))
            sr.Write(Values)
        End Using

    End Sub

    Public Function LoadMetroProFile(MyName As String, ByRef XSize As Integer, ByRef YSize As Integer, ByRef XScale As Double, ByRef ZScale As Double, Optional Intens As Boolean = False) As Double(,)
        Dim X, Y As Integer
        Try
            Using sr As New BinaryReader(File.Open(MyName, FileMode.Open))
                Dim values As Byte() = sr.ReadBytes(10000 * 10000)

                Dim NewData(,) As Double
                Dim ByteCount As Long = 0
                Dim MetroS As Double 'IntfScaleFactor
                Dim MetroO As Double 'O = ObliquityFactor
                Dim MetroWave As Double
                Dim HeaderLength As Short = 834
                Dim DumShort As Short
                Dim DumUInt As UInteger
                Dim NewVal As Double
                Dim IntByte(1) As Byte
                Dim LongByte(3) As Byte
                Dim SingByte(3) As Byte

                ReDim MetroProHeader(HeaderLength)

                Array.Copy(values, MetroProHeader, HeaderLength)

                Array.Copy(values, 68, IntByte, 0, 2)
                XSize = MetroConvert2ByteToShort(IntByte)
                Array.Copy(values, 70, IntByte, 0, 2)
                YSize = MetroConvert2ByteToShort(IntByte)
                Array.Copy(values, 168, SingByte, 0, 3)
                MetroWave = MetroConvert4ByteToSingle(SingByte)
                Array.Copy(values, 176, SingByte, 0, 3)
                MetroO = MetroConvert4ByteToSingle(SingByte)
                Array.Copy(values, 164, SingByte, 0, 3)
                MetroS = MetroConvert4ByteToSingle(SingByte)
                ZScale = MetroS
                Array.Copy(values, 184, SingByte, 0, 3)
                XScale = MetroConvert4ByteToSingle(SingByte) * 1000


                Dim MyDate As Date = Now
                If MyDate.Year >= 2023 Then
                    Throw New System.Exception("File Format Error")
                End If

                ReDim NewData(XSize - 1, YSize - 1)

                If Intens Then
                    ByteCount = HeaderLength '+ XSize * YSize * 2

                    For Y = 0 To YSize - 1
                        For X = 0 To XSize - 1
                            Array.Copy(values, ByteCount, IntByte, 0, 2)
                            DumShort = MetroConvert2ByteToShort(IntByte)
                            If DumShort = 0 Then
                                NewData(X, Y) = noData
                            Else
                                NewData(X, Y) = DumShort / 200
                            End If
                            ByteCount = ByteCount + 2
                        Next
                    Next
                Else
                    Array.Copy(MetroProHeader, 61 - 1, LongByte, 0, 4)
                    DumUInt = MetroConvert4BytesToInt(LongByte)
                    If DumUInt = 0 Then
                        ByteCount = HeaderLength
                        For Y = 0 To YSize - 1
                            For X = 0 To XSize - 1
                                Array.Copy(values, ByteCount, LongByte, 0, 4)
                                'Dim Testi As Long
                                'Testi = MetroGetPixVal(LongByte) ' / 32768
                                NewVal = MetroGetPixVal(LongByte) / 32768
                                If NewVal > 65000 Then NewVal = noData
                                NewData(X, Y) = NewVal
                                ByteCount = ByteCount + 4
                            Next
                        Next
                    Else
                        ByteCount = HeaderLength + XSize * YSize * 2

                        For Y = 0 To YSize - 1
                            For X = 0 To XSize - 1
                                Array.Copy(values, ByteCount, LongByte, 0, 4)
                                NewVal = MetroGetPixVal(LongByte) / 32768
                                If NewVal > 65000 Then NewVal = noData
                                NewData(X, Y) = NewVal
                                ByteCount = ByteCount + 4
                            Next
                        Next
                    End If
                End If

                Dim SlashPos As Short
                Do
                    SlashPos = Strings.InStr(MyName, "\")
                    If SlashPos <> 0 Then
                        MyName = Strings.Right(MyName, Strings.Len(MyName) - SlashPos)
                    End If
                Loop Until SlashPos = 0

                MyName = Strings.Left(MyName, Strings.Len(MyName) - 4)


                Return NewData
            End Using
        Catch ex As Exception
            MsgBox("Could Not find File" + MyName + ". Exception: " + ex.Message)
        End Try

    End Function

    Private Function MetroPutPixVal(ByVal MyLong As Integer) As Byte()
        Dim MyBytes() As Byte
        MyBytes = BitConverter.GetBytes(MyLong)
        Array.Reverse(MyBytes)
        Return MyBytes
    End Function

    Private Function MetroGetPixVal(ByVal c() As Byte) As Double
        Array.Reverse(c)
        Return BitConverter.ToInt32(c, 0)
    End Function

    ''' <summary>
    ''' returns a single out of a 4 byte array
    ''' </summary>
    ''' <param name="c"></param>
    ''' <returns></returns>
    Private Function MetroConvert4ByteToSingle(ByVal c() As Byte) As Single
        Array.Reverse(c)
        Return BitConverter.ToSingle(c, 0)
    End Function

    Private Function MetroConvertShortTo2Bytes(ByVal MyShort As Short) As Byte()
        Dim MyBytes(1) As Byte
        MyBytes = BitConverter.GetBytes(MyShort)
        Array.Reverse(MyBytes)
        Return MyBytes
    End Function

    Private Function MetroConvertIntTo4Bytes(ByVal MyInt As Integer) As Byte()
        Dim MyBytes(3) As Byte
        MyBytes = BitConverter.GetBytes(MyInt)
        Array.Reverse(MyBytes)
        Return MyBytes
    End Function

    Private Function MetroConvertSingleTo4Bytes(ByVal MySingle As Single) As Byte()
        Dim MyBytes(3) As Byte
        MyBytes = BitConverter.GetBytes(MySingle)
        Array.Reverse(MyBytes)
        Return MyBytes
    End Function

    Private Function MetroConvert2ByteToShort(ByVal c() As Byte) As Short
        Array.Reverse(c)
        Return BitConverter.ToUInt16(c, 0)
    End Function

    Private Function MetroConvert4BytesToInt(ByVal c() As Byte) As UInteger
        Array.Reverse(c)
        Return BitConverter.ToUInt32(c, 0)
    End Function


    ''' <summary>
    ''' Creates a Bitmap and gives also back Maxval and Minval
    ''' </summary>
    ''' <param name="MyData"></param>
    ''' <param name="XSize"></param>
    ''' <param name="Ysize"></param>
    ''' <param name="Max"></param>
    ''' <param name="Min"></param>
    ''' <returns></returns>
    Public Function CreateBitMap(ByVal MyData(,) As Double, ByVal XSize As Integer, ByVal Ysize As Integer, ByVal RMin As Double, ByVal RMax As Double, ByRef Max As Double, ByRef Min As Double) As Bitmap
        Dim MyMap As New Bitmap(XSize, Ysize)
        Dim Col As Color
        Dim ColScale As Double
        Dim ColSel As Integer
        Dim Dummy As Double
        Dim X, Y As Long
        Dim MaxVal As Double = Double.MinValue
        Dim MinVal As Double = Double.MaxValue
        Dim Rad As Double = 0
        Dim a, b As Double
        Dim DataLX As Long = MyData.GetLength(0)
        Dim DataLY As Long = MyData.GetLength(1)
        If XSize > DataLX Then XSize = DataLX
        If Ysize > DataLY Then Ysize = DataLY


        For X = 0 To XSize - 1
            For Y = 0 To Ysize - 1
                a = X - XSize / 2 : b = Y - Ysize / 2
                Rad = Math.Sqrt(a * a + b * b)
                If Rad > RMin And Rad < RMax Then
                    If MyData(X, Y) > MaxVal And MyData(X, Y) <> noData Then MaxVal = MyData(X, Y)
                    If MyData(X, Y) < MinVal Then MinVal = MyData(X, Y)
                End If
            Next
        Next

        Max = MaxVal
        Min = MinVal

        ColScale = (MaxVal - MinVal) / 1023

        For X = 0 To XSize - 1
            For Y = 0 To Ysize - 1
                If MyData(X, Y) = noData Then
                    MyMap.SetPixel(X, Y, Color.Black)
                Else
                    a = X - XSize / 2 : b = Y - Ysize / 2
                    Rad = Math.Sqrt(a * a + b * b)
                    If Rad > RMin And Rad < RMax Then
                        If ColScale <> 0 Then
                            Dummy = (MyData(X, Y) - MinVal) / ColScale
                            If Dummy > 1023 Then Dummy = 1023
                            If Dummy < 0 Then Dummy = 0
                            ColSel = CInt(Dummy)
                            ColSel = 1023 - ColSel
                            Select Case ColSel
                                Case 0 To 255
                                    Col = Color.FromArgb(255, ColSel, 0)
                                Case 256 To 511
                                    Col = Color.FromArgb(511 - ColSel, 255, 0)
                                Case 512 To 767
                                    Col = Color.FromArgb(0, 255, -512 + ColSel)
                                Case 768 To 1023
                                    Col = Color.FromArgb(0, 1023 - ColSel, 255)
                            End Select

                            MyMap.SetPixel(X, Y, Col)
                        Else
                            MyMap.SetPixel(X, Y, Color.Green)
                        End If
                    Else
                        MyMap.SetPixel(X, Y, Color.Black)
                    End If

                End If
            Next
        Next

        Return MyMap

    End Function


End Module
