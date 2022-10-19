Imports System
Imports System.IO
Imports System.Globalization
Imports System.Collections
Imports System.Text.RegularExpressions


Public Class Form1

    Dim degstep As Double                                        'Regelmäßiger Schritt in Grad zwischen Messungen
    Dim nMess As Integer                                         'Anzahl an importierten Messungen
    Dim R1 As Double                                             'Spiegelradius berechnet aus Software
    Dim R2 As Double = 0                                         'Spiegelradius durch Eingabe des Benutzers (wird für Zernikefit gebraucht. R1 kann nicht verwendet werden, da Piston, Tilt und Power nicht übereinstimmen dann
    Dim ArmLength As Double = 0                                  'Swingarmlänge
    Dim extraMessDeg As Object                                   'Gradanzahl einer extra Messung, die manuell eingefügt wird
    Dim PixelCount As Integer = 800                              'Pixelanzahl des Bildes. Immer gerade Anzahl an Pixel!!! Wegen NewData(0,0) Array wirds wieder ungerade und man kanns spiegeln
    Dim degFiles As New List(Of String)                          'Liste für Messungsnamen
    Dim xRef, yRef As New List(Of Double)                        'Listen für Referenz
    Dim xValExtra, yValExtra As New List(Of Double)              'Listen für extra Messungen
    Dim x_refresh, y_refresh, z_refresh As New List(Of Double)   'Listen für veränderte Auflösung
    Dim radList, phiList, zList As New List(Of Double)           'Listen für Zylinderkoordinaten
    Dim rSort, pSort, zSort As New List(Of Double)               'Listen für Zylinderkoordinaten (nach Radius sortiert)
    Dim xFinal, yFinal, zFinal As New List(Of Double)            'Listen für finale Kartesiche Koordinaten 
    Dim zFinal2 As New List(Of Double)                           'Extraliste, falls Spiegel kein Mittelloch hat und richtig dargestellt werden soll. Wird vermutlich noch überarbeitet, funktioniert aber so derzeit.
    Dim splitmes As Integer                                      'Sprung zwischen x-Werten der Einzelmessungen (falls Mittelloch existiert)
    Dim AvgStep = 4                                              'Datenglättungsparameter (je höher, desto glatter wird es. aber halt auch ungenauer)
    Dim RefPiston, RefTilt, RefPower As Object                   'Piston, Tilt & Power aus Referenz
    Dim splitref As Integer                                      'Sprung zwischen x-Werten der Referenz (falls Mittelloch existiert)
    Dim refPath As String                                        'Pfad der Referenz
    Dim DataPointsMess1 As Integer                               'Anzahl an Datenpunkte erster Messung. Wird benötigt, falls keine Referenz vorhanden ist und trotzdem damit gerechnet werden soll
    Dim splitmes1 As Integer                                     'Falls keine Referenz, ist das die Sprungstelle am Mittelloch der ersten Messung. Alle anderen Messungen werden an diese angepasst
    Dim ZScale As Integer = 1
    Dim SplineDensityCheckpoint As Integer = 0                   'Wird bei Datenglättung gebraucht. Bestimmt, ob geringe Datenpunktdichte oder hohe gewählt werden soll
    Dim DataSortingCheckpoint As Integer = 0                  'Entscheidet ob Sortierung aufsteigend (0) oder absteigend (1) erfolgen soll
    Dim NewData(PixelCount, PixelCount) As Double
    Dim Max, Min As Double
    Dim image As Image
    Dim ROC As Double = 0
    Dim k As Double = 0
    Dim gOptPist, gOptTilt, gOptPowe As Double

    Private Sub trckBarInt_Scroll(sender As Object, e As EventArgs) Handles trckBarInt.Scroll

    End Sub



    Private Sub Button1_Click_3(sender As Object, e As EventArgs) Handles btnRef.Click
        OpenFileDialog1.Filter = "TEXT Datei | *.txt"
        Dim refFile As New List(Of String)


        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            txtBoxReference.Text = Path.GetFileNameWithoutExtension(OpenFileDialog1.FileName)
            refPath = OpenFileDialog1.FileName


            'Lese Zeile für Zeile
            Dim Referenz = IO.File.ReadAllLines(refPath).ToList

            'Cleare Listen fürs mehrmalige Wiederholen
            xRef.Clear()
            yRef.Clear()

            'Lese Piston, Tilt & Power aus Referenzfile
            For i = 0 To Referenz.Count - 1                                             'Methode für alte Referenzfiles.. Falls das wieder mal gebraucht wird
                If Referenz(i).Contains("Pist") Then
                    RefPiston = Referenz(i).Split(";")(0)
                    RefTilt = Referenz(i).Split(";")(2)
                    RefPower = Referenz(i).Split(";")(1)

                    RefPiston = CDbl(RefPiston.split("=")(1))
                    RefTilt = CDbl(RefTilt.split("=")(1))
                    RefPower = CDbl(RefPower.split("=")(1))
                End If
            Next

            'RefPiston = CDbl(Referenz(1))
            'RefTilt = CDbl(Referenz(3))
            'RefPower = CDbl(Referenz(2))

            If RefPiston Is Nothing Then
                MsgBox("Referenzfile fehlerhaft. Kann Piston, Tilt & Power nicht finden.")
                Exit Sub
            End If


            'Lösche unnötige Einträge
            For i = Referenz.Count - 1 To 0 Step -1
                If Referenz(i).Contains("-999999") Then
                    Referenz.RemoveAt(i)
                ElseIf Not Referenz(i).Contains(";") Then
                    Referenz.RemoveAt(i)
                ElseIf Referenz(i).Contains("=") Then
                    Referenz.RemoveAt(i)
                End If
            Next



            'Teile x und y Werte aus der String-Liste einer neuen separaten Liste of Double zu
            For Each i In Referenz
                xRef.Add(i.Split(" ; ")(0))
                yRef.Add(i.Split(" ; ")(2))
            Next



            'Lösche xy Einträge wenn y-Koordinatenabtände > 50 ist für die ersten 30 Zeilen
            Dim index1 As Integer
            If Math.Abs(yRef(0) - yRef(1)) > 50 Then
                Do
                    'Lösche Einträge am jeweiligen Index
                    xRef.RemoveAt(index1)
                    yRef.RemoveAt(index1)
                    'solange unten stehende Bedingung erfüllt wird
                Loop Until Math.Abs(yRef(index1) - yRef(index1 + 1)) < 50 And index1 <= 30
            End If

            'Lösche xy Einträge wenn y-Koordinatenabtände > 50 ist für die letzten 30 Zeilen
            Dim index2 As Integer = yRef.Count - 1
            Do
                'definiere den vorstehenden Index
                Dim prev1 = index2 - 1

                If Not Math.Abs(yRef(prev1) - yRef(index2)) < 50 Then 'Lösche Einträge am jeweiligen Index, wenn Bedingung erfüllt wird
                    xRef.RemoveAt(index2)
                    yRef.RemoveAt(index2)
                Else
                    Exit Do
                End If
                index2 = index2 - 1 'Aktualisiere die Index-Nummerierung
            Loop While index2 >= yRef.Count - 30



            Dim split1 As Integer = 0
            Dim split2 As Integer = 0
            splitref = 0

            'Definiere Sprung zwischen x-Werte
            For z = 1 To xRef.Count - 2
                If Math.Abs(xRef(z) - xRef(z + 1)) > 40 Then
                    split1 = z
                    split2 = z + 1
                End If
            Next



            If split1 <> 0 Then                     'Für den Fall, dass Spiegel Mittelloch aufweist

                'Lösche Ausreißer an der Spiegelinnenkante in Richtung negative x-Werte (Schwellenwert: 50)
                Do
                    If Math.Abs(yRef(split2) - yRef(split2 + 1)) > 50 Then
                        xRef.RemoveAt(split2)
                        yRef.RemoveAt(split2)
                    Else
                        Exit Do
                    End If
                Loop


                'Lösche Ausreißer an der Spiegelinnenkante in Richtung positive x-Werte (Schwellenwert: 50)
                Do
                    If Math.Abs(yRef(split1) - yRef(split1 - 1)) > 50 Then
                        xRef.RemoveAt(split1)
                        yRef.RemoveAt(split1)
                    Else
                        Exit Do
                    End If
                    split1 = split1 - 1 'Aktualisiere die Index-Nummerierung
                Loop


                'Definiere neuen Sprung zwischen x-Werte
                For z = 1 To xRef.Count - 2
                    If Math.Abs(xRef(z) - xRef(z + 1)) > 40 Then
                        splitref = z
                    End If
                Next


                'Checke gerade Anzahl der Datenpunkte 
                If xRef.Count Mod 2 <> 0 Then
                    If splitref + 1 > xRef.Count - (splitref + 1) Then
                        xRef.RemoveAt(split1 - 5)
                        yRef.RemoveAt(split1 - 5)
                    ElseIf splitref + 1 < xRef.Count - (splitref + 1) Then
                        xRef.RemoveAt(splitref + 5)
                        yRef.RemoveAt(splitref + 5)
                    End If
                    'Definiere neuen Sprung zwischen x-Werte
                    For z = 1 To xRef.Count - 2
                        If Math.Abs(xRef(z) - xRef(z + 1)) > 40 Then
                            splitref = z
                        End If
                    Next
                End If


                'Checke Gleichheit der Daten auf jeder Seite und passe an
                If splitref + 1 > xRef.Count - (splitref + 1) Then
                    Dim splitdiff As Integer = (splitref + 1) - (xRef.Count - (splitref + 1))                'Anzahl des Datenpunktüberflusses auf unterer Seite
                    For i = 1 To splitdiff
                        xRef.RemoveAt(splitref / (i + 1))
                        yRef.RemoveAt(splitref / (i + 1))
                    Next
                ElseIf splitref + 1 < xRef.Count - (splitref + 1) Then
                    Dim splitdiff As Integer = (xRef.Count - (splitref + 1)) - (splitref + 1)                 'Anzahl des Datenpunktüberflusses auf oberer Seite
                    For i = 1 To splitdiff
                        xRef.RemoveAt(splitref + splitref / (i + 1))
                        yRef.RemoveAt(splitref + splitref / (i + 1))
                    Next
                End If

            Else                                                                                             'Für den Fall, dass Spiegel kein Mittelloch aufweist
                'Checke gerade Anzahl der Datenpunkte 
                If xRef.Count Mod 2 <> 0 Then
                    xRef.RemoveAt(xRef.Count / 2 - 0.5)
                End If
            End If



        End If

    End Sub

    Private Sub cbOptimized_CheckedChanged(sender As Object, e As EventArgs) Handles cbOptimized.CheckedChanged

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnMess.Click
        'Dialogfilter
        OpenFileDialog1.Multiselect = True
        OpenFileDialog1.Filter = "TEXT Datei | *.txt"

        'Lösche Liste für wiederholtes Ausführen
        degFiles.Clear()

        'Frage Benutzer nach Datei
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            For i = 0 To OpenFileDialog1.FileNames.Count - 1
                degFiles.Add(OpenFileDialog1.FileNames(i))
            Next
        Else Exit Sub
        End If

        nMess = OpenFileDialog1.FileNames.Count
        Dim degFilesNotSorted(nMess - 1) As String

        For i = 0 To nMess - 1
            degFilesNotSorted(i) = degFiles(i)
        Next

        degFiles.Clear()

        'Sortierung der Messungen nach aufsteigendem Tischdrehwinkel mithilfe des AlphanumComparator (alphanumerisch)
        Array.Sort(degFilesNotSorted, New AlphanumComparator())

        For i = 0 To nMess - 1
            degFiles.Add(degFilesNotSorted(i))
        Next

        Dim textboxFileNames As String = ""
        For i = 0 To degFiles.Count - 1
            textboxFileNames = textboxFileNames & Path.GetFileNameWithoutExtension(degFiles(i)) & vbNewLine
        Next

        txtBoxMessungen.Text = textboxFileNames
        degstep = 360 / nMess
        Winkelanzeige.Text = "Angle step [deg] = " & degstep
        nMessungen.Text = "Number of Measurements: " & nMess

    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles btnCalc.Click

        If txtBoxReference.Text = "" And txtBoxMessungen.Text = "" Then
            MsgBox("Keine Referenz sowie Einzelmessungen ausgewählt.")
            Exit Sub
        ElseIf txtBoxMessungen.Text = "" Then
            MsgBox("Keine Einzelmessungen ausgewählt.")
            Exit Sub
        ElseIf ArmLength = 0 Then
            MsgBox("Keine Swingarmlänge ausgewählt.")
            Exit Sub
        ElseIf k = 0 Then
            MsgBox("Keine Conical ausgewählt.")
            Exit Sub
        ElseIf ROC = 0 Then
            MsgBox("Keinen ROC ausgewählt.")
            Exit Sub
        ElseIf R2 = 0 Then
            MsgBox("Keinen Radius ausgewählt.")
            Exit Sub
        Else
            If txtBoxReference.Text = "" Then
                Dim answer As Integer = MsgBox("Keine Referenz ausgewählt. Trotzdem fortfahren?", vbQuestion + vbYesNo + vbDefaultButton2, "Keine Referenz")
                If answer = vbNo Then
                    Exit Sub
                End If
            End If


            Dim xValSpline, yValSpline As New List(Of Double)           'Listen für Splinedaten

            For q = 0 To nMess - 1                  'Für jedes Messfile ein Durchgang


                Dim fileName As String = degFiles(q)
                Dim xVal As New List(Of Double)
                Dim yVal As New List(Of Double)

                'Cleare Listen für wiederholte Anwendung
                xVal.Clear()
                yVal.Clear()
                rSort.Clear()
                pSort.Clear()
                zSort.Clear()


                'Lese Zeile für Zeile
                Dim Messung = IO.File.ReadAllLines(fileName).ToList


                'Lösche unnötige Einträge
                If Not Messung(0).Contains(" ; ") Then
                    Messung.RemoveAt(0)
                End If

                For i = Messung.Count - 1 To 0 Step -1
                    If i = 0 Then
                        Double.TryParse(Messung(i), gOptPist)
                    End If

                    If i = 1 Then
                        Double.TryParse(Messung(i), gOptPowe)
                    End If

                    If i = 2 Then
                        Double.TryParse(Messung(i), gOptTilt)
                    End If

                Next

                For i = Messung.Count - 1 To 0 Step -1

                    If Messung(i).Contains("-999999") Then
                        Messung.RemoveAt(i)
                    ElseIf Messung(i).Contains("Empty") Then
                        Messung.RemoveAt(i)
                    ElseIf Messung(i).Contains(" ; 0") And Not Messung(i).Contains(" ; 0,") Then
                        Messung.RemoveAt(i)
                    ElseIf Not Messung(i).Contains(";") Then
                        Messung.RemoveAt(i)
                    End If
                Next


                'Teile x und y Werte aus der String-Liste einer neuen separaten Liste of Double zu
                For Each i In Messung
                    xVal.Add(i.Split(" ; ")(0))
                    yVal.Add(i.Split(" ; ")(2))
                Next




                'Lösche xy Einträge wenn y-Koordinatenabtände > 50 ist für die ersten 10% der Einträge
                Dim Anfangsprung As Integer
                For i = Math.Round(xVal.Count * 0.1) To 1 Step -1
                    If Math.Abs(yVal(i) - yVal(i - 1)) > 50 Then
                        Anfangsprung = i - 1
                        xVal.RemoveRange(0, Anfangsprung + 1)
                        yVal.RemoveRange(0, Anfangsprung + 1)
                        Exit For
                    End If
                Next



                'Lösche xy Einträge wenn y-Koordinatenabtände > 50 ist für die letzten 10% der Einträge
                Dim Endsprung As Integer
                For i = Math.Round(xVal.Count * 0.9) To xVal.Count - 2
                    If Math.Abs(yVal(i) - yVal(i + 1)) > 50 Then
                        Endsprung = i + 1
                        xVal.RemoveRange(Endsprung, xVal.Count - Endsprung)
                        yVal.RemoveRange(Endsprung, yVal.Count - Endsprung)
                        Exit For
                    End If
                Next





                'Definiere Sprung zwischen x-Werte bei vorhandenem Mittelloch
                Dim split1 As Integer = 0                                                   'Sprungstelle linke Seite
                Dim split2 As Integer = 0                                                   'Sprungstelle rechte Seite
                For i = 1 To xVal.Count - 2
                    If Math.Abs(xVal(i) - xVal(i + 1)) > 40 Then
                        split1 = i
                        split2 = i + 1
                    End If
                Next


                'Lösche Außreißer bei vorhandenem Spiegelloch
                '---------------------
                If split1 <> 0 Then

                    'Lösche Ausreißer an der Spiegelinnenkante in Richtung negative x-Werte (Schwellenwert: 50)
                    Do
                        If split2 <> 0 And Math.Abs(yVal(split2) - yVal(split2 + 1)) > 50 Then
                            xVal.RemoveAt(split2)
                            yVal.RemoveAt(split2)
                        Else
                            Exit Do
                        End If
                    Loop

                    'Lösche Ausreißer an der Spiegelinnenkante in Richtung positive x-Werte (Schwellenwert: 50)
                    Do
                        If split1 <> 0 And Math.Abs(yVal(split1) - yVal(split1 - 1)) > 50 Then
                            xVal.RemoveAt(split1)
                            yVal.RemoveAt(split1)
                        Else
                            Exit Do
                        End If
                        split1 = split1 - 1 'Aktualisiere die Index-Nummerierung
                    Loop

                End If
                '------------------


                'Definiere neuen Sprung zwischen x-Werte
                For i = 1 To xVal.Count - 2
                    If Math.Abs(xVal(i) - xVal(i + 1)) > 40 Then
                        splitmes = i
                    End If
                Next

                Dim midrad As Double     'Bogenlänge des Spiegellochs

                'Zeige Infos in UI
                If splitmes <> 0 Then
                    midrad = (Math.Abs(xVal(splitmes)) + Math.Abs(xVal(splitmes + 1))) / 2

                    lblMirrorhole.Text = "Mirrorhole radius [mm]: " & Math.Round(2 * ArmLength * Math.Sin(midrad / ArmLength * 0.5), 2)
                Else lblMirrorhole.Text = "Mirrorhole radius [mm]: No hole detected!"
                End If


                Dim iteration As Integer = q + 1




                '------------------------------- passe Einzelmessungen an Referenz an, falls nötig
                If xVal.Count <> xRef.Count And xRef.Count <> 0 Then

                    Dim diff As Integer = xRef.Count - xVal.Count                                                       'Differenz der Datenpunktanzahlen zwischen Referenz und Messung
                    Dim splitjump As Integer = splitref - splitmes                                                      'Definiere Differenz zwischen Mittelindex von Referenz und Messung. Vorzeichen entscheidet, wo bei ungerader Datendifferenz der mittlere Punkt platziert wird

                    If splitjump < 0 Then
                        splitjump = 1
                    Else
                        splitjump = -1
                    End If


                    If diff > 0 And diff Mod 2 <> 0 Then                                                                      'Wenn Referenz mehr Punkte als Messung hat und das in ungerader Anzahl
                        Dim intervall As Integer = xVal.Count / (1 + Math.Abs(diff)) - 10 * splitjump                         'Definiere Intervall, in dem Punkte der Liste hinzugefügt werden sollen. Bei Spiegeln mit Mittelloch könnte es allerdings zu Problemen kommen, wenn eine ungerade Anzahl an fehlenden Datenpunkte vorhanden ist. Deshalb um -10 verschoben (offset, frei wählbarer Paramater).
                        For z = 1 To Math.Abs(diff)
                            xVal.Insert(intervall * z, (xVal(intervall * z) + xVal(intervall * z + 1)) / 2)
                            yVal.Insert(intervall * z, (yVal(intervall * z) + yVal(intervall * z + 1)) / 2)
                        Next


                    ElseIf diff > 0 And diff Mod 2 = 0 Then                                                                   'Wenn Referenz mehr Punkte als Messung hat und das in gerader Anzahl
                        Dim intervall As Integer = xVal.Count / (1 + Math.Abs(diff))                                          'Definiere Intervall, in dem Punkte der Liste hinzugefügt werden sollen. Diesmal braucht es keinen Offset, da bei gerader Anzahl das Mittellochproblem keine Rolle spielt.
                        For z = 1 To Math.Abs(diff)
                            xVal.Insert(intervall * z, (xVal(intervall * z) + xVal(intervall * z + 1)) / 2)
                            yVal.Insert(intervall * z, (yVal(intervall * z) + yVal(intervall * z + 1)) / 2)
                        Next


                    ElseIf diff < 0 And diff Mod 2 <> 0 Then                                                                   'Wenn Referenz weniger Punkte als Messung hat und das in ungerader Anzahl
                        Dim intervall As Integer = xVal.Count / (1 + Math.Abs(diff)) - 10 * splitjump                          'Definiere Intervall, in dem Punkte der Liste hinzugefügt werden sollen. Bei Spiegeln mit Mittelloch könnte es allerdings zu Problemen kommen, wenn eine ungerade Anzahl an fehlenden Datenpunkte vorhanden ist. Deshalb um -10 verschoben (offset, frei wählbarer Paramater).
                        For z = 1 To Math.Abs(diff)
                            xVal.RemoveAt(intervall * z)
                            yVal.RemoveAt(intervall * z)
                        Next


                    ElseIf diff < 0 And diff Mod 2 = 0 Then                                                                    'Wenn Referenz weniger Punkte als Messung hat und das in gerader Anzahl
                        Dim intervall As Integer = xVal.Count / (1 + Math.Abs(diff))                                           'Definiere Intervall, in dem Punkte der Liste hinzugefügt werden sollen. Diesmal braucht es keinen Offset, da bei gerader Anzahl das Mittellochproblem keine Rolle spielt.
                        For z = 1 To Math.Abs(diff)
                            xVal.RemoveAt(intervall * z)
                            yVal.RemoveAt(intervall * z)
                        Next

                    End If


                End If
                '----------------------------------------



                '---------------Definiere neuen Sprung zwischen x-Werte
                For i = 1 To xVal.Count - 2
                    If Math.Abs(xVal(i) - xVal(i + 1)) > 40 Then
                        splitmes = i
                    End If
                Next
                '---------------


                '------------------ Passe Sprung an Mittelloch an, sodass negative und positive Seite gleiche Anzahl an Punkten aufweisen
                If splitmes <> splitref And xRef.Count <> 0 Then                                                          'Wenn negative und positive Hälfte unterschiedliche Anzahl an Datenpunkte enthält

                    Dim splitjump As Integer = splitref - splitmes                                                      'Unterschied zwischen Sprünge

                    If splitjump < 0 Then                                                                               'Wenn negative Hälfte mehr Punkte hat
                        For i = 1 To Math.Abs(splitjump)
                            xVal.RemoveAt(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                            xVal.Insert(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (xVal(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + xVal(1 + splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                            yVal.RemoveAt(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                            yVal.Insert(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (yVal(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + yVal(1 + splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                        Next
                    Else                                                                                                'Wenn positive Hälfte mehr Punkte hat
                        For i = 1 To Math.Abs(splitjump)
                            xVal.RemoveAt(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                            xVal.Insert(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (xVal(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + xVal(1 + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                            yVal.RemoveAt(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                            yVal.Insert(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (yVal(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + yVal(1 + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                        Next
                    End If
                ElseIf q = 0 And xRef.Count = 0 And splitmes <> 0 Then                                                                      'falls keine Referenz: checke Datenpunkanzahl erster Messung um andere Messungen daran zu orientieren
                    DataPointsMess1 = xVal.Count                                                                                            'Datenpunktanzahl erster Messung
                    If DataPointsMess1 Mod 2 <> 0 Then                                                                                      'checke ob Anzahl gerade ist. Wenn nicht, wirds gemacht
                        xVal.RemoveAt(Math.Round(1.5 * splitmes))
                        yVal.RemoveAt(Math.Round(1.5 * splitmes))
                    End If
                    DataPointsMess1 = xVal.Count
                    Dim diff As Double = DataPointsMess1 / 2 - (splitmes + 1)                                                               'Differenz an Messpunkten oberer und unterer Hälfte
                    If diff < 0 Then                                                                                                        'Falls untere Hälfte mehr Punkte hat
                        Dim intervall As Integer = Math.Round(splitmes / (1 + Math.Abs(diff)))                                              'Intervall, in dem Punkte gesetzt/gelöscht werden
                        For z = 1 To Math.Abs(diff)
                            xVal.RemoveAt(intervall * z)
                            yVal.RemoveAt(intervall * z)
                            xVal.Insert(splitmes + intervall * z, (xVal(splitmes + intervall * z) + xVal(splitmes + intervall * z + 1)) / 2)
                            yVal.Insert(splitmes + intervall * z, (yVal(splitmes + intervall * z) + yVal(splitmes + intervall * z + 1)) / 2)
                        Next
                    ElseIf diff > 0 Then                                                                                                    'Falls obere Hälfte mehr Punkte hat
                        Dim intervall As Integer = Math.Round(splitmes / (1 + Math.Abs(diff)))                                              'Intervall, in dem Punkte gesetzt/gelöscht werden
                        For z = 1 To Math.Abs(diff)
                            xVal.RemoveAt(splitmes + intervall * z)
                            yVal.RemoveAt(splitmes + intervall * z)
                            xVal.Insert(intervall * z, (xVal(intervall * z) + xVal(intervall * z + 1)) / 2)
                            yVal.Insert(intervall * z, (yVal(intervall * z) + yVal(intervall * z + 1)) / 2)
                        Next
                    End If
                    '---------------Definiere neuen Sprung zwischen x-Werte der 1. Messung
                    For i = 1 To xVal.Count - 2
                        If Math.Abs(xVal(i) - xVal(i + 1)) > 40 Then
                            splitmes1 = i
                        End If
                    Next
                    '---------------
                ElseIf q > 0 And xRef.Count = 0 And splitmes <> 0 Then                                                      'Falls keine Referenz: passe Messungen an 1. Messung an, um Punktegleichheit zu erreichen
                    If xVal.Count <> DataPointsMess1 Then                                                                   'Passe Datenpunktanzahl von folgenden Messungen an 1. Messung an
                        Dim diff As Integer = DataPointsMess1 - xVal.Count
                        If diff > 0 Then                                                                                    'Wenn Messung weniger hat als 1. Messung
                            Dim intervall As Integer = xVal.Count / (1 + Math.Abs(diff))
                            For z = 1 To Math.Abs(diff)
                                xVal.Insert(intervall * z - 10, (xVal(intervall * z - 10) + xVal(intervall * z + 1 - 10)) / 2)          '-10 wegen Mittellochproblem
                                yVal.Insert(intervall * z - 10, (yVal(intervall * z - 10) + yVal(intervall * z + 1 - 10)) / 2)
                            Next
                        ElseIf diff < 0 Then                                                                                'Wenn Messung mehr hat als 1. Messung
                            Dim intervall As Integer = xVal.Count / (1 + Math.Abs(diff))
                            For z = 1 To Math.Abs(diff)
                                xVal.RemoveAt(intervall * z - 10)
                                yVal.RemoveAt(intervall * z - 10)
                            Next
                        End If

                    End If

                    '---------------Definiere neuen Sprung zwischen x-Werte
                    For i = 1 To xVal.Count - 2
                        If Math.Abs(xVal(i) - xVal(i + 1)) > 40 Then
                            splitmes = i
                        End If
                    Next
                    '---------------

                    Dim splitjump As Integer = splitmes1 - splitmes                                                         'Unterschied zwischen Sprünge

                    If splitjump < 0 Then                                                                                   'Wenn negative Hälfte mehr Punkte hat
                        For i = 1 To Math.Abs(splitjump)
                            xVal.RemoveAt(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                            xVal.Insert(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (xVal(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + xVal(1 + splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                            yVal.RemoveAt(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                            yVal.Insert(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (yVal(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + yVal(1 + splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                        Next
                    ElseIf splitjump > 0 Then                                                                                                     'Wenn positive Hälfte mehr Punkte hat
                        For i = 1 To Math.Abs(splitjump)
                            xVal.RemoveAt(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                            xVal.Insert(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (xVal(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + xVal(1 + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                            yVal.RemoveAt(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                            yVal.Insert(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (yVal(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + yVal(1 + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                        Next
                    End If
                End If



                '------------------------ Ziehe Referenzmessung von Einzelmessungen ab und anschließende Reduzierung durch Zernike Polynome. Formel: z(x) = a - b * x / R + c * ((x / R) ^ 2)
                ''Dim R2 As Double = 294  'Math.Abs(2 * ArmLength * Math.Sin(xVal(0) / (2 * ArmLength)))          'INFO an ASA: Philip hat für den Spiegelradius (311 war vom 625 Nr20 Spiegel) einen leicht anderen Wert genommen, als ich hier berechenet habe. Würde vermutlich keinen großen Unterschied machen.
                If xRef.Count <> 0 Then
                    For i = 0 To yVal.Count - 1
                        Dim r As Double = Math.Abs(2 * ArmLength * Math.Sin(xVal(i) / (2 * ArmLength)))                 'INFO an ASA: Philip hat für den Abstand zur Achse r (oder x in der Zernike Nomenklatur) die Bogenlänge gewählt, was genau genommen nicht korrekt ist. Möglicherweise sind die Auswirkungen marginal sodass es vernachlässigbar ist. Der richtige Wert wäre r, den ich in dieser Zeile berechne. Er wird aber nicht verwendet, da vermutlich Piston, Tilt und Power dann nicht übereinstimmen.
                        yVal(i) = yVal(i) - yRef(i) - (RefPiston - RefTilt * xVal(i) / R2 + RefPower * (xVal(i) / R2) ^ 2)
                    Next
                End If
                '--------------------------



                Dim Asphere, Sphere As New List(Of Double)

                'Asphäre & Sphäre
                For i = 0 To xVal.Count - 1
                    Dim r As Double = Math.Abs(2 * ArmLength * Math.Sin(xVal(i) / (2 * ArmLength)))    'Auch hier gilt das gleiche wie in Zeile 628
                    Asphere.Add(xVal(i) ^ 2 / (ROC * (1 + (1 - (1 + k) * xVal(i) ^ 2 / ROC ^ 2) ^ 0.5)))
                    Sphere.Add(xVal(i) ^ 2 / (ROC * (1 + (1 - (1 + 0) * xVal(i) ^ 2 / ROC ^ 2) ^ 0.5)))
                Next



                'Jetzt kommen die Zernike Terme für jedes Messfile.
                If cbOptimized.Checked Then
                    For i = 0 To yVal.Count - 1
                        Dim r As Double = Math.Abs(2 * ArmLength * Math.Sin(xVal(i) / (2 * ArmLength)))
                        yVal(i) = yVal(i) - (Sphere(i) - Asphere(i)) * 10000 - (gOptPist - gOptTilt * xVal(i) / R2 + gOptPowe * (xVal(i) / R2) ^ 2)
                    Next
                End If

                '-----------------




                '---------------Definiere neuen Sprung zwischen x-Werte (sollte nun sowieso gleich splitref sein, falls keine Referenz wirds nochmal definiert)
                For i = 1 To xVal.Count - 2
                    If Math.Abs(xVal(i) - xVal(i + 1)) > 40 Then
                        splitmes = i
                    End If
                Next
                '---------------


                'Datenglättung. Parameter dafür wurde in Zeile 26 definiert und kann verändert werden
                If splitmes <> 0 Then                   'Falls Spiegel Mittelloch aufweist
                    For t = 0 To 1

                        'Glätte Daten
                        '--------------------   

                        Dim xAvg, yAvg As New List(Of Double)
                        xAvg.Clear()
                        yAvg.Clear()

                        For u = 1 To (xVal.Count / 2) / AvgStep
                            Dim xmid As Double = 0
                            Dim ymid As Double = 0
                            For v = 0 To AvgStep - 1
                                xmid += xVal(v + AvgStep * (u - 1) + t * (xVal.Count / 2))
                                ymid += yVal(v + AvgStep * (u - 1) + t * (xVal.Count / 2))
                            Next
                            xAvg.Add(xmid / AvgStep)
                            yAvg.Add(ymid / AvgStep)
                        Next

                        If t = 0 Then
                            xAvg.Add(xVal(0))                                            'Füge äußere Randpunkte hinzu (negative Werte)
                            yAvg.Add(yVal(0))
                            xAvg.Add(xVal(splitmes))                                     'Füge innere Randpunkte hinzu (negative Werte)
                            yAvg.Add(yVal(splitmes))
                        ElseIf t = 1 Then
                            xAvg.Add(xVal(xVal.Count - 1))                               'Füge äußere Randpunkte hinzu (positive Werte)
                            yAvg.Add(yVal(yVal.Count - 1))
                            xAvg.Add(xVal(splitmes + 1))                                 'Füge innere Randpunkte hinzu (positive Werte)
                            yAvg.Add(yVal(splitmes + 1))
                        End If
                        '--------------------



                        'Data Sorting: Ordne Datenpunkte nach aufsteigendem x; verlinkt mit y-Wert
                        '-----------------
                        yAvg = DataSorting(xAvg, yAvg)
                        For i = yAvg.Count - 1 To 0 Step -1
                            If i Mod 2 = 0 Then
                                xAvg.Add(yAvg(i))
                                yAvg.RemoveAt(i)
                            End If
                        Next
                        xAvg.Sort()
                        '-----------------

                        'Spline Interpolation
                        '-----------------
                        SplineDensityCheckpoint = 1
                        If t = 0 Then
                            yValSpline = SplineInterpolation(xAvg, yAvg)
                            For i = yValSpline.Count - 1 To 0 Step -1
                                If i Mod 2 = 0 Then
                                    xValSpline.Add(yValSpline(i))
                                    yValSpline.RemoveAt(i)
                                End If
                            Next
                            xValSpline.Sort()
                        ElseIf t = 1 Then
                            yAvg = SplineInterpolation(xAvg, yAvg)
                            For i = yAvg.Count - 1 To 0 Step -1
                                If i Mod 2 = 0 Then
                                    xAvg.Add(yAvg(i))
                                    yAvg.RemoveAt(i)
                                End If
                            Next
                            xAvg.Sort()
                            For i = 0 To xAvg.Count - 1
                                xValSpline.Add(xAvg(i))
                                yValSpline.Add(yAvg(i))
                            Next
                        End If
                        SplineDensityCheckpoint = 0
                        '-----------------
                    Next


                    'Data Sorting: Ordne Datenpunkte nach sinkendem x; verlinkt mit y-Wert
                    '-----------------
                    DataSortingCheckpoint = 1
                    yValSpline = DataSorting(xValSpline, yValSpline)
                    For i = 0 To yValSpline.Count - 1
                        If i Mod 2 = 0 Then
                            xValSpline.Add(yValSpline(i))
                        End If
                    Next
                    For i = yValSpline.Count - 1 To 0 Step -1
                        If i Mod 2 = 0 Then
                            yValSpline.RemoveAt(i)
                        End If
                    Next
                    DataSortingCheckpoint = 0
                    '-----------------

                Else                                   'Falls Spiegel kein Mittelloch aufweist
                    'Glätte Daten 
                    '--------------------   

                    Dim xAvg, yAvg As New List(Of Double)
                    xAvg.Clear()
                    yAvg.Clear()

                    For u = 1 To xVal.Count / AvgStep
                        Dim xmid As Double = 0
                        Dim ymid As Double = 0
                        For v = 0 To AvgStep - 1
                            xmid += xVal(v + AvgStep * (u - 1))
                            ymid += yVal(v + AvgStep * (u - 1))
                        Next
                        xAvg.Add(xmid / AvgStep)
                        yAvg.Add(ymid / AvgStep)
                    Next

                    xAvg.Add(xVal(0))                                            'Füge äußere Randpunkte hinzu (negative Werte)
                    yAvg.Add(yVal(0))
                    xAvg.Add(xVal(xVal.Count - 1))                               'Füge äußere Randpunkte hinzu (positive Werte)
                    yAvg.Add(yVal(yVal.Count - 1))






                    'Data Sorting: Ordne Datenpunkte nach aufsteigendem x; verlinkt mit y-Wert
                    '-----------------
                    yAvg = DataSorting(xAvg, yAvg)
                    For i = yAvg.Count - 1 To 0 Step -1
                        If i Mod 2 = 0 Then
                            xAvg.Add(yAvg(i))
                            yAvg.RemoveAt(i)
                        End If
                    Next
                    xAvg.Sort()
                    '-----------------


                    'Spline Interpolation
                    '-----------------
                    SplineDensityCheckpoint = 1
                    yValSpline = SplineInterpolation(xAvg, yAvg)
                    For i = yValSpline.Count - 1 To 0 Step -1
                        If i Mod 2 = 0 Then
                            xValSpline.Add(yValSpline(i))
                            yValSpline.RemoveAt(i)
                        End If
                    Next
                    xValSpline.Sort()
                    SplineDensityCheckpoint = 0
                    '-----------------
                End If


                For i = 0 To xValSpline.Count - 1
                    Dim rad As Double
                    Dim phi As Double
                    phi = 180 / Math.PI * Math.Atan(1 / Math.Tan(xValSpline(i) / (2 * ArmLength))) - degstep * q                 'Zylinderkoordinaten Winkel + Drehwinkel der Messung
                    rad = Math.Abs(2 * ArmLength * Math.Sin(xValSpline(i) / (2 * ArmLength)))                                    'Zylinderkoordinaten Radius

                    phiList.Add(phi)
                    radList.Add(rad)
                    zList.Add(yValSpline(i))                                                                                     'Zylinderkoordinaten Z-Wert
                Next

                xValSpline.Clear()
                yValSpline.Clear()
            Next




            '------------------ Sortiere Daten nach aufsteigendem Radius
            Dim Mid As Integer = radList.Count / (2 * nMess)                  'Mittlerer Wert der Einzelmessungen (mit Nullpunkt um 1 verschoben, wie normal)
            For i = 0 To Mid - 1
                For j = 0 To nMess - 1
                    rSort.Add(radList(Mid + 2 * Mid * j - i - 1))
                    rSort.Add(radList(Mid + 2 * Mid * j + i))
                    pSort.Add(phiList(Mid + 2 * Mid * j - i - 1))
                    pSort.Add(phiList(Mid + 2 * Mid * j + i))
                    zSort.Add(zList(Mid + 2 * Mid * j - i - 1))
                    zSort.Add(zList(Mid + 2 * Mid * j + i))
                Next
            Next
            '--------------------


            phiList.Clear()
            radList.Clear()
            zList.Clear()
            xValSpline.Clear()
            yValSpline.Clear()



        End If



    End Sub

    Private Sub btnSpline_Click(sender As Object, e As EventArgs) Handles btnInt.Click
        Dim phiListSpline, zListSpline As New List(Of Double)                  'Splinelisten
        Dim phiFinal, radFinal As New List(Of Double)                          'fertige phi und z Listen
        Dim RadDat As Integer                                                  'Anzahl an Datenpunkte entlang des Radius 
        Dim radStep As Double                                                  'Radius für jeden Datenpunkt 

        'Zur Sicherheit (bei mehrmaligem Durchführen) werden die Listen geleert
        '-----------------
        xFinal.Clear()
        yFinal.Clear()
        zFinal.Clear()
        zFinal2.Clear()
        phiFinal.Clear()
        radFinal.Clear()
        phiListSpline.Clear()
        zListSpline.Clear()
        x_refresh.Clear()
        y_refresh.Clear()
        z_refresh.Clear()
        '----------------


        'Initialisiere NewData noch mit 'keine Daten' (5555555)
        For i = 0 To NewData.GetLength(0) - 1
            For j = 0 To NewData.GetLength(1) - 1
                NewData(i, j) = 5555555
            Next
        Next
        '-----------------



        'Aus den sortierten Listen für Rad, Phi und Z wird nun der Zwischenraum interpoliert. Der Radius wird festgehalten und eine Funktion z(phi) erstellt. 
        'Diese Funktion wird mit der Spline Interpolation bearbeitet. Allerdings sind bekanntlich die Endpunkte problematisch, daher wurden die Phi-Daten mehrmals um Vielfache von 360 erweitert.
        'Nachdem darüber eine Spline Interpolation gelegt wurde, konnte ein mittlerer Bereich gewählt werden. Dabei wurde ein fixer Punkt als Start gewählt und dieser um 360 Grad erweitert, welcher den Endpunkt darstellt. Ein Shift um 360 Grad wurde anschließend durchgeführt.
        'Dies wurde für jeden Radius Datenpunkt durchgeführt. Am Ende erfolgt eine Transformation zu kartesischen Koordinaten.
        For RadDat = 1 To rSort.Count / (nMess * 2)                            'Shift wegen Bruch
            phiListSpline.Clear()
            zListSpline.Clear()
            For i = nMess * 2 * (RadDat - 1) To 2 * RadDat * nMess - 1         'Sieht kompliziert aus. Was machts? Wählt alle bekannten Datenpunkte aus, die auf gleichem Radius liegen
                radStep += rSort(i)

                phiListSpline.Add(pSort(i))
                zListSpline.Add(zSort(i))
            Next





            'Vervielfache Datenpunkte um smoothe Endpunkt zu bekommen
            For v = 1 To 2
                For w = 0 To phiListSpline.Count - 1
                    phiListSpline.Add(phiListSpline(w) + 360 * v)
                    zListSpline.Add(zListSpline(w) + 0 * v)
                Next
            Next




            'Data Sorting: Ordne Datenpunkte nach aufsteigendem phi; verlinkt mit z-Wert
            '-----------------
            zListSpline = DataSorting(phiListSpline, zListSpline)
            For i = zListSpline.Count - 1 To 0 Step -1
                If i Mod 2 = 0 Then
                    phiListSpline.Add(zListSpline(i))
                    zListSpline.RemoveAt(i)
                End If
            Next
            phiListSpline.Sort()
            '-----------------



            Dim pt As Double = phiListSpline(0)             'erster Datenpunkt der geordneten philiste. Wird benötigt, um iterierte Daten auszumisten und smoothen Übergang zu erzeugen



            'Glätte Daten
            '--------------------   
            Dim xSplineAvg, ySplineAvg As New List(Of Double)
            Dim AvgStep2 As Integer = 2
            xSplineAvg.Clear()
            ySplineAvg.Clear()

            For u = 1 To phiListSpline.Count / AvgStep2
                Dim xmid As Double = 0
                Dim ymid As Double = 0
                For v = 0 To AvgStep2 - 1
                    xmid += phiListSpline(v + AvgStep2 * (u - 1))
                    ymid += zListSpline(v + AvgStep2 * (u - 1))
                Next
                xSplineAvg.Add(xmid / AvgStep2)
                ySplineAvg.Add(ymid / AvgStep2)
            Next
            '-----------------

            xSplineAvg.Add(phiListSpline(0))                                            'Füge äußere Randpunkte hinzu (negative Werte)
            ySplineAvg.Add(zListSpline(0))
            xSplineAvg.Add(phiListSpline(phiListSpline.Count - 1))                               'Füge äußere Randpunkte hinzu (positive Werte)
            ySplineAvg.Add(zListSpline(zListSpline.Count - 1))


            'Data Sorting: Ordne Datenpunkte nach aufsteigendem x; verlinkt mit y-Wert
            '-----------------
            ySplineAvg = DataSorting(xSplineAvg, ySplineAvg)
            For i = ySplineAvg.Count - 1 To 0 Step -1
                If i Mod 2 = 0 Then
                    xSplineAvg.Add(ySplineAvg(i))
                    ySplineAvg.RemoveAt(i)
                End If
            Next
            xSplineAvg.Sort()
            '-----------------

            phiListSpline.Clear()
            zListSpline.Clear()

            For i = 0 To xSplineAvg.Count - 1
                phiListSpline.Add(xSplineAvg(i))
                zListSpline.Add(ySplineAvg(i))
            Next



            'Spline Interpolation
            '-----------------
            zListSpline = SplineInterpolation(phiListSpline, zListSpline)
            For i = zListSpline.Count - 1 To 0 Step -1
                If i Mod 2 = 0 Then
                    phiListSpline.Add(zListSpline(i))
                    zListSpline.RemoveAt(i)
                End If
            Next
            phiListSpline.Sort()
            '-----------------



            'iterierte Spline Daten ausmisten; Nur Werte zwischen ersten Datenpunk +540° und ersten Datenpunkt +900° akzeptiert. Damit ist eine gesamte Drehung mit 360° abgedeckt.
            '----------------------------
            Dim index3 As Integer

            Do
                phiListSpline.RemoveAt(index3)
                zListSpline.RemoveAt(index3)
            Loop Until phiListSpline(index3) > pt + 540


            For i = phiListSpline.Count - 1 To 0 Step -1
                If phiListSpline(i) > pt + 900 Then
                    phiListSpline.RemoveAt(i)
                    zListSpline.RemoveAt(i)
                End If
            Next

            For i = 0 To phiListSpline.Count - 1
                phiFinal.Add(phiListSpline(i))
                zFinal.Add(zListSpline(i))
            Next
            '-------------------------------------



            radFinal.Add(radStep / (2 * nMess))
            radStep = 0

        Next



        'Addiere für jeden phi und z Wert den zugehörigen Radius
        '-----------------------------------------------
        Dim multiple As Integer = phiFinal.Count / radFinal.Count
        Dim RadCount As Integer = radFinal.Count
        For i = 1 To multiple - 1
            For j = 0 To RadCount - 1
                radFinal.Add(radFinal(j + 0 * i))
            Next
        Next
        '-----------------------------------------------

        radFinal.Sort()
        R1 = radFinal(radFinal.Count - 1)

        If splitref = 0 Then
            For i = CInt((radFinal.Count - 1) * 0.00) To 1 * (radFinal.Count - 1)                  'Kofaktoren 0 und 1 können prozentuell den Spiegel beschneiden
                xFinal.Add(R1 + radFinal(i) * Math.Cos(phiFinal(i) * Math.PI / 180))               'Kartesische x Koordinate mit rein positiven Werten (Ursprung verschoben)
                yFinal.Add(R1 + radFinal(i) * Math.Sin(phiFinal(i) * Math.PI / 180))               'Kartesische y Koordinate mit rein positiven Werten (Ursprung verschoben)
                zFinal2.Add((zFinal(i) / 10) * 1000 / 632.8 * 2)                                   'Division mit 10 bringt Werte zuerst auf Mikrometer; anschließende Umwandlung von Mikrometer zu Wave
            Next
            zFinal.Clear()
            For i = 0 To zFinal2.Count - 1                                                         'Folgender Schritt wird gemacht, um einheitlich zu bleiben (im Bezug auf den Listennamen mit anderen Spiegeln)
                zFinal.Add(zFinal2(i))
            Next
        Else
            For i = 0 To radFinal.Count - 1
                xFinal.Add(R1 + radFinal(i) * Math.Cos(phiFinal(i) * Math.PI / 180))                'Kartesische x Koordinate mit rein positiven Werten (Ursprung verschoben)
                yFinal.Add(R1 + radFinal(i) * Math.Sin(phiFinal(i) * Math.PI / 180))                'Kartesische y Koordinate mit rein positiven Werten (Ursprung verschoben)
                zFinal(i) = (zFinal(i) / 10) * 1000 / 632.8 * 2                                     'Division mit 10 bringt Werte zuerst auf Mikrometer; anschließende Umwandlung von Mikrometer zu Wave
            Next
        End If



        For i = 0 To xFinal.Count - 1                                                              'Erstelle separate Liste für das Ändern der Auflösung
            x_refresh.Add(xFinal(i))
            y_refresh.Add(yFinal(i))
            z_refresh.Add(zFinal(i))
        Next

        Dim R1R2diff As Double = R2 - R1                                                          'Differenz zwischen R2 und R1. Wird gebraucht, um Skalierung mit Interferometer zu erreichen
        Dim XScale As Double = R2 * 2 / PixelCount

        For i = 0 To xFinal.Count - 1                                                               'Wandle in Pixel um.
            xFinal(i) = Math.Round((xFinal(i) + R1R2diff) / XScale)
            yFinal(i) = Math.Round((yFinal(i) + R1R2diff) / XScale)
        Next


        '----------------- Spiegle Topo an horizontaler Achse
        Dim MidPix As Integer = NewData.GetLength(1) / 2 - 0.5
        For i = 0 To yFinal.Count - 1
            yFinal(i) = yFinal(i) - (yFinal(i) - MidPix) * 2
        Next
        '-----------------




        For i = 0 To xFinal.Count - 1
            NewData(xFinal(i), yFinal(i)) = zFinal(i)
        Next

        image = CreateBitMap(NewData, NewData.GetLength(0), NewData.GetLength(1), 0, NewData.GetLength(0) / 2, Max, Min)

        'Bild wurde erzeugt. Zeige es
        PictureBox1.Image = image
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox1.Refresh()





    End Sub

    Private Sub btnSvDAT_Click(sender As Object, e As EventArgs) Handles btnSvDAT.Click
        Dim XScale As Double = R1 * 2 / PixelCount
        Dim MyName As String
        Dim MyFile As New SaveFileDialog
        MyFile.Filter = "XYZ 4D Files (*.dat)|*.dat"
        If MyFile.ShowDialog <> DialogResult.OK Then Exit Sub
        MyName = MyFile.FileName

        SaveMetroProFile(MyName, NewData, XScale, ZScale)


    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click

        Dim XScale As Double = R1 * 2 / (trckBarRes.Value * 2)
        ReDim NewData(2 * trckBarRes.Value, 2 * trckBarRes.Value)                        'doppelte, um zu gewährleisten, dass Newdata Array ungerade Anzahl an Einträgen vorweist (nicht gerade, wegen NewData(0,0)). Wird für Spiegelung des image benötigt
        Dim x2_refresh, y2_refresh As New List(Of Double)                                'Wird benötigt, um wiederholtes refreshen zu ermöglichen. 

        x2_refresh.Clear()
        y2_refresh.Clear()

        For i = 0 To x_refresh.Count - 1                                                  'Wandle in Pixel um.
            x2_refresh.Add(Math.Round(x_refresh(i) / XScale))
            y2_refresh.Add(Math.Round(y_refresh(i) / XScale))
        Next


        '----------------- Spiegle Topo an horizontaler Achse
        Dim MidPix As Integer = NewData.GetLength(1) / 2 - 0.5
        For i = 0 To y2_refresh.Count - 1
            y2_refresh(i) = y2_refresh(i) - (y2_refresh(i) - MidPix) * 2
        Next
        '-----------------


        'Initialisiere NewData noch mit 'keine Daten' (5555555)
        For i = 0 To NewData.GetLength(0) - 1
            For j = 0 To NewData.GetLength(1) - 1
                NewData(i, j) = 5555555
            Next
        Next


        For i = 0 To x2_refresh.Count - 1
            NewData(x2_refresh(i), y2_refresh(i)) = z_refresh(i)
        Next

        image = CreateBitMap(NewData, NewData.GetLength(0), NewData.GetLength(1), 0, NewData.GetLength(0) / 2, Max, Min)

        'Bild wurde erzeugt. Zeige es
        PictureBox1.Image = image
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox1.Refresh()

    End Sub

    Private Sub Button1_Click_2(sender As Object, e As EventArgs)
        'Anzeige für Winkelabstände
        lblAngle.Text = (degstep)
    End Sub

    Private Sub Button1_Click_4(sender As Object, e As EventArgs) Handles btnClear1.Click
        degFiles.Clear()
        txtBoxMessungen.Text = ""
        Winkelanzeige.Text = "Angle step [deg] = "
        nMessungen.Text = "Number of Measurements: "
        lblMirrorhole.Text = "Mirrorhole radius [mm]:"
    End Sub

    Private Sub btnClear2_Click(sender As Object, e As EventArgs) Handles btnClear2.Click
        xRef.Clear()
        yRef.Clear()
        txtBoxReference.Text = ""
    End Sub

    Private Sub Button1_Click_5(sender As Object, e As EventArgs) Handles btnWolke.Click

        Dim xList, yList, zList As New List(Of Double)


        rSort.Sort()
        R1 = rSort(rSort.Count - 1)


        For i = 0 To rSort.Count - 1
            xList.Add(R1 + rSort(i) * Math.Cos(pSort(i) * Math.PI / 180))
            yList.Add(R1 + rSort(i) * Math.Sin(pSort(i) * Math.PI / 180))
            zList.Add((zSort(i) / 10) * 1000 / 632.8 * 2)
        Next


        'Initialisiere NewData noch mit 'keine Daten' (5555555)
        For i = 0 To NewData.GetLength(0) - 1
            For j = 0 To NewData.GetLength(1) - 1
                NewData(i, j) = 5555555
            Next
        Next
        '-----------------



        Dim XScale As Double = R1 * 2 / PixelCount

        For i = 0 To xList.Count - 1                                                              'Wandle in Pixel um.
            xList(i) = Math.Round(xList(i) / XScale)
            yList(i) = Math.Round(yList(i) / XScale)
        Next


        '----------------- Spiegle Topo an horizontaler Achse
        Dim MidPix As Integer = NewData.GetLength(1) / 2 - 0.5
        For i = 0 To yList.Count - 1
            yList(i) = yList(i) - (yList(i) - MidPix) * 2
        Next
        '-----------------




        For i = 0 To xList.Count - 1
            NewData(xList(i), yList(i)) = zList(i)
        Next

        image = CreateBitMap(NewData, NewData.GetLength(0), NewData.GetLength(1), 0, NewData.GetLength(0) / 2, Max, Min)

        'Bild wurde erzeugt. Zeige es
        PictureBox1.Image = image
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox1.Refresh()





    End Sub

    Private Sub btnSetArmlength_Click(sender As Object, e As EventArgs) Handles btnSetArmlength.Click
        If Not Double.TryParse(txtBoxArmLength.Text, ArmLength) Or txtBoxArmLength.Text.Contains(".") Then
            MsgBox("Bitte nur Zahlen im Format 1234,56 (Komma statt Punkt verwenden)")
        End If
    End Sub

    Private Sub btnSetROC_Click(sender As Object, e As EventArgs) Handles btnSetROC.Click
        If Not Double.TryParse(txtBoxROC.Text, ROC) Or txtBoxROC.Text.Contains(".") Then
            MsgBox("Bitte nur Zahlen im Format 1234,56 (Komma statt Punkt verwenden)")
        End If
    End Sub

    Private Sub btnSetConical_Click(sender As Object, e As EventArgs) Handles btnSetConical.Click
        If Not Double.TryParse(txtBoxConical.Text, k) Or txtBoxConical.Text.Contains(".") Then
            MsgBox("Bitte nur Zahlen im Format 1234,56 (Komma statt Punkt verwenden)")
        End If
    End Sub

    Private Sub btnSetRadius_Click(sender As Object, e As EventArgs) Handles btnSetRadius.Click
        If Not Double.TryParse(txtBoxRadius.Text, R2) Or txtBoxRadius.Text.Contains(".") Then
            MsgBox("Bitte nur Zahlen im Format 1234,56 (Komma statt Punkt verwenden)")
        End If
    End Sub

    Private Sub btnSetDeg_Click(sender As Object, e As EventArgs) Handles btnSetDeg.Click
        If Not Double.TryParse(txtBoxDeg.Text, extraMessDeg) Then
            MsgBox("Bitte nur Zahlen im Format 1234,56")
            Exit Sub
        End If
        extraMessDeg = txtBoxDeg.Text


        If extraMessDeg = "" Then
            MsgBox("Keine Gradanzahl eingegeben.")
            Exit Sub
        ElseIf xRef.Count = 0 Then
            Dim answer As Integer = MsgBox("Keine Referenz ausgewählt. Trotzdem fortfahren?", vbQuestion + vbYesNo + vbDefaultButton2, "Keine Referenz")
            If answer = vbNo Then
                Exit Sub
            End If
        ElseIf rSort.Count = 0 Then
            MsgBox("Zuerst auf 'Calculate Data' drücken und dann die extra Messung hinzufügen")
            Exit Sub
        ElseIf extraMessDeg.Contains(".") Then                              'Ersetze Punkt mit Komma, falls nötig
            extraMessDeg = extraMessDeg.Replace(".", ",")
        End If

        extraMessDeg = CDbl(extraMessDeg)

        OpenFileDialog1.Filter = "TEXT Datei | *.txt"

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim extraMessPath As String = OpenFileDialog1.FileName


            'Lese Zeile für Zeile
            Dim extraMessFile = IO.File.ReadAllLines(extraMessPath).ToList
            Dim xValSpline, yValSpline As New List(Of Double)


            'Cleare Listen fürs mehrmaliges Wiederholen
            xValExtra.Clear()
            yValExtra.Clear()
            xValSpline.Clear()
            yValSpline.Clear()

            'Lösche unnötige Einträge
            If Not extraMessFile(0).Contains(" ; ") Then
                extraMessFile.RemoveAt(0)
            End If


            For i = extraMessFile.Count - 1 To 0 Step -1
                If i = 0 Then
                    Double.TryParse(extraMessFile(i), gOptPist)
                End If

                If i = 1 Then
                    Double.TryParse(extraMessFile(i), gOptPowe)
                End If

                If i = 2 Then
                    Double.TryParse(extraMessFile(i), gOptTilt)
                End If

            Next



            'Lösche unnötige Einträge
            For i = extraMessFile.Count - 1 To 0 Step -1
                If extraMessFile(i).Contains("-999999") Then
                    extraMessFile.RemoveAt(i)
                ElseIf extraMessFile(i).Contains("Empty") Then
                    extraMessFile.RemoveAt(i)
                ElseIf extraMessFile(i).Contains(" ; 0") And Not extraMessFile(i).Contains(" ; 0,") Then
                    extraMessFile.RemoveAt(i)
                ElseIf Not extraMessFile(i).Contains(";") Then
                    extraMessFile.RemoveAt(i)
                End If
            Next


            'Teile x und y Werte aus der String-Liste einer neuen separaten Liste of Double zu
            For Each i In extraMessFile
                xValExtra.Add(i.Split(" ; ")(0))
                yValExtra.Add(i.Split(" ; ")(2))
            Next



            'Lösche xy Einträge wenn y-Koordinatenabtände > 50 ist für die ersten 10% der Einträge
            Dim Anfangsprung As Integer
            For i = Math.Round(xValExtra.Count * 0.1) To 1 Step -1
                If Math.Abs(yValExtra(i) - yValExtra(i - 1)) > 50 Then
                    Anfangsprung = i - 1
                    xValExtra.RemoveRange(0, Anfangsprung + 1)
                    yValExtra.RemoveRange(0, Anfangsprung + 1)
                    Exit For
                End If
            Next



            'Lösche xy Einträge wenn y-Koordinatenabtände > 50 ist für die letzten 10% der Einträge
            Dim Endsprung As Integer
            For i = Math.Round(xValExtra.Count * 0.9) To xValExtra.Count - 2
                If Math.Abs(yValExtra(i) - yValExtra(i + 1)) > 50 Then
                    Endsprung = i + 1
                    xValExtra.RemoveRange(Endsprung, xValExtra.Count - Endsprung)
                    yValExtra.RemoveRange(Endsprung, yValExtra.Count - Endsprung)
                    Exit For
                End If
            Next





            'Definiere Sprung zwischen x-Werte bei vorhandenem Mittelloch
            Dim split1 As Integer = 0
            Dim split2 As Integer = 0
            For i = 1 To xValExtra.Count - 2
                If Math.Abs(xValExtra(i) - xValExtra(i + 1)) > 40 Then
                    split1 = i
                    split2 = i + 1
                End If
            Next



            If split1 <> 0 Then

                'Lösche Ausreißer an der Spiegelinnenkante in Richtung negative x-Werte (Schwellenwert: 50)
                Do
                    If split2 <> 0 And Math.Abs(yValExtra(split2) - yValExtra(split2 + 1)) > 50 Then
                        xValExtra.RemoveAt(split2)
                        yValExtra.RemoveAt(split2)
                    Else
                        Exit Do
                    End If
                Loop

                'Lösche Ausreißer an der Spiegelinnenkante in Richtung positive x-Werte (Schwellenwert: 50)
                Do
                    If split1 <> 0 And Math.Abs(yValExtra(split1) - yValExtra(split1 - 1)) > 50 Then
                        xValExtra.RemoveAt(split1)
                        yValExtra.RemoveAt(split1)
                    Else
                        Exit Do
                    End If
                    split1 = split1 - 1 'Aktualisiere die Index-Nummerierung
                Loop

            End If



            'Definiere neuen Sprung zwischen x-Werte
            For i = 1 To xValExtra.Count - 2
                If Math.Abs(xValExtra(i) - xValExtra(i + 1)) > 40 Then
                    splitmes = i
                End If
            Next



            '------------------------------- passe Messung an Referenz an
            If xValExtra.Count <> xRef.Count Then

                Dim diff As Integer = xRef.Count - xValExtra.Count                                                              'Differenz der Datenpunktanzahlen zwischen Referenz und Messung
                Dim splitjump As Integer = splitref - splitmes                                                                  'Definiere Differenz zwischen Mittelindex von Referenz und Messung. Vorzeichen entscheidet, wo bei ungerader Datendifferenz der mittlere Punkt platziert wird

                If splitjump < 0 Then
                    splitjump = 1
                Else
                    splitjump = -1
                End If


                If diff > 0 And diff Mod 2 <> 0 Then                                                                            'Wenn Referenz mehr Punkte als Messung hat und das in ungerader Anzahl
                    Dim intervall As Integer = xValExtra.Count / (1 + diff) - 10 * splitjump                                    'Definiere Intervall, in dem Punkte der Liste hinzugefügt werden sollen. Bei Spiegeln mit Mittelloch könnte es allerdings zu Problemen kommen, wenn eine ungerade Anzahl an fehlenden Datenpunkte vorhanden ist. Deshalb um -10 verschoben (offset, frei wählbarer Paramater).
                    For z = 1 To diff
                        xValExtra.Insert(intervall * z, (xValExtra(intervall * z) + xValExtra(intervall * z + 1)) / 2)
                        yValExtra.Insert(intervall * z, (yValExtra(intervall * z) + yValExtra(intervall * z + 1)) / 2)
                    Next


                ElseIf diff > 0 And diff Mod 2 = 0 Then                                                                         'Wenn Referenz mehr Punkte als Messung hat und das in gerader Anzahl
                    Dim intervall As Integer = xValExtra.Count / (1 + diff)                                                     'Definiere Intervall, in dem Punkte der Liste hinzugefügt werden sollen. Diesmal braucht es keinen Offset, da bei gerader Anzahl das Mittellochproblem keine Rolle spielt.
                    For z = 1 To diff
                        xValExtra.Insert(intervall * z, (xValExtra(intervall * z) + xValExtra(intervall * z + 1)) / 2)
                        yValExtra.Insert(intervall * z, (yValExtra(intervall * z) + yValExtra(intervall * z + 1)) / 2)
                    Next


                ElseIf diff < 0 And diff Mod 2 <> 0 Then                                                                       'Wenn Referenz weniger Punkte als Messung hat und das in ungerader Anzahl
                    Dim intervall As Integer = xValExtra.Count / (1 + diff) - 10 * splitjump                                   'Definiere Intervall, in dem Punkte der Liste hinzugefügt werden sollen. Bei Spiegeln mit Mittelloch könnte es allerdings zu Problemen kommen, wenn eine ungerade Anzahl an fehlenden Datenpunkte vorhanden ist. Deshalb um -10 verschoben (offset, frei wählbarer Paramater).
                    For z = 1 To diff
                        xValExtra.RemoveAt(intervall * z)
                        yValExtra.RemoveAt(intervall * z)
                    Next


                ElseIf diff < 0 And diff Mod 2 = 0 Then                                                                       'Wenn Referenz weniger Punkte als Messung hat und das in gerader Anzahl
                    Dim intervall As Integer = xValExtra.Count / (1 + diff)                                                   'Definiere Intervall, in dem Punkte der Liste hinzugefügt werden sollen. Diesmal braucht es keinen Offset, da bei gerader Anzahl das Mittellochproblem keine Rolle spielt.
                    For z = 1 To diff
                        xValExtra.RemoveAt(intervall * z)
                        yValExtra.RemoveAt(intervall * z)
                    Next

                End If

            End If
            '----------------------------------------


            '---------------Definiere neuen Sprung zwischen x-Werte
            For i = 1 To xValExtra.Count - 2
                If Math.Abs(xValExtra(i) - xValExtra(i + 1)) > 40 Then
                    splitmes = i
                End If
            Next
            '---------------



            '------------------ Passe Sprung an Mittelloch an, sodass negative und positive Seite gleiche Anzahl an Punkten aufweisen
            If splitmes <> splitref And xRef.Count <> 0 Then                                                        'Wenn negative und positive Hälfte unterschiedliche Anzahl an Datenpunkte enthält und es keine Referenz gibt

                Dim splitjump As Integer = splitref - splitmes                                                      'Unterschied zwischen Sprünge

                If splitjump < 0 Then                                                                               'Wenn negative Hälfte mehr Punkte hat
                    For i = 1 To Math.Abs(splitjump)
                        xValExtra.RemoveAt(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                        xValExtra.Insert(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (xValExtra(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + xValExtra(1 + splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                        yValExtra.RemoveAt(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                        yValExtra.Insert(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (yValExtra(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + yValExtra(1 + splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                    Next
                Else                                                                                                'Wenn positive Hälfte mehr Punkte hat
                    For i = 1 To Math.Abs(splitjump)
                        xValExtra.RemoveAt(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                        xValExtra.Insert(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (xValExtra(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + xValExtra(1 + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                        yValExtra.RemoveAt(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                        yValExtra.Insert(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (yValExtra(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + yValExtra(1 + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                    Next
                End If
            ElseIf xRef.Count = 0 And splitmes <> 0 Then
                If xValExtra.Count <> DataPointsMess1 Then                                                                   'Passe Datenpunktanzahl von folgenden Messungen an 1. Messung an
                    Dim diff As Integer = DataPointsMess1 - xValExtra.Count
                    If diff > 0 Then                                                                                    'Wenn Messung weniger hat als 1. Messung
                        Dim intervall As Integer = xValExtra.Count / (1 + Math.Abs(diff))
                        For z = 1 To Math.Abs(diff)
                            xValExtra.Insert(intervall * z - 10, (xValExtra(intervall * z - 10) + xValExtra(intervall * z + 1 - 10)) / 2)          '-10 wegen Mittellochproblem
                            yValExtra.Insert(intervall * z - 10, (yValExtra(intervall * z - 10) + yValExtra(intervall * z + 1 - 10)) / 2)
                        Next
                    ElseIf diff < 0 Then                                                                                'Wenn Messung mehr hat als 1. Messung
                        Dim intervall As Integer = xValExtra.Count / (1 + Math.Abs(diff))
                        For z = 1 To Math.Abs(diff)
                            xValExtra.RemoveAt(intervall * z - 10)
                            yValExtra.RemoveAt(intervall * z - 10)
                        Next
                    End If
                End If
                '------------------

                '---------------Definiere neuen Sprung zwischen x-Werte
                For i = 1 To xValExtra.Count - 2
                    If Math.Abs(xValExtra(i) - xValExtra(i + 1)) > 40 Then
                        splitmes = i
                    End If
                Next
                '---------------

                Dim splitjump As Integer = splitmes1 - splitmes                                                         'Unterschied zwischen Sprünge

                If splitjump < 0 Then                                                                                   'Wenn negative Hälfte mehr Punkte hat
                    For i = 1 To Math.Abs(splitjump)
                        xValExtra.RemoveAt(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                        xValExtra.Insert(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (xValExtra(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + xValExtra(1 + splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                        yValExtra.RemoveAt(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                        yValExtra.Insert(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (yValExtra(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + yValExtra(1 + splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                    Next
                ElseIf splitjump > 0 Then                                                                                                     'Wenn positive Hälfte mehr Punkte hat
                    For i = 1 To Math.Abs(splitjump)
                        xValExtra.RemoveAt(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                        xValExtra.Insert(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (xValExtra(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + xValExtra(1 + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                        yValExtra.RemoveAt(splitmes + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))
                        yValExtra.Insert(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)), 0.5 * (yValExtra(i * Math.Round(splitmes / (Math.Abs(splitjump) + 1))) + yValExtra(1 + i * Math.Round(splitmes / (Math.Abs(splitjump) + 1)))))
                    Next
                End If

            End If




            '------------------------ Ziehe Referenzmessung von Einzelmessungen ab und anschließende Reduzierung durch Zernike Polynome. Formel: z(x) = a - b * x / R + c * ((x / R) ^ 2)
            'Dim R2 As Double = 311 'Math.Abs(2 * ArmLength * Math.Sin(xVal(0) / (2 * ArmLength)))
            If xRef.Count <> 0 Then
                For i = 0 To yValExtra.Count - 1
                    Dim r As Double = Math.Abs(2 * ArmLength * Math.Sin(xValExtra(i) / (2 * ArmLength)))                            'Auch hier wieder... siehe Zeile 501
                    yValExtra(i) = yValExtra(i) - yRef(i) - (RefPiston - RefTilt * xValExtra(i) / R2 + RefPower * (xValExtra(i) / R2) ^ 2)
                Next
                '--------------------------
            End If



            'Asphäre & Sphäre
            Dim Asphere, Sphere As New List(Of Double)
            For i = 0 To xValExtra.Count - 1
                Dim r As Double = Math.Abs(2 * ArmLength * Math.Sin(xValExtra(i) / (2 * ArmLength)))                          'Auch hier wieder... siehe Zeile 501
                Asphere.Add(xValExtra(i) ^ 2 / (ROC * (1 + (1 - (1 + k) * xValExtra(i) ^ 2 / ROC ^ 2) ^ 0.5)))
                Sphere.Add(xValExtra(i) ^ 2 / (ROC * (1 + (1 - (1 + 0) * xValExtra(i) ^ 2 / ROC ^ 2) ^ 0.5)))
            Next





            For i = 0 To yValExtra.Count - 1
                Dim r As Double = Math.Abs(2 * ArmLength * Math.Sin(xValExtra(i) / (2 * ArmLength)))
                yValExtra(i) = yValExtra(i) - (Sphere(i) - Asphere(i)) * 10000 - (gOptPist - gOptTilt * xValExtra(i) / R2 + gOptPowe * (xValExtra(i) / R2) ^ 2)
            Next




            '---------------Definiere neuen Sprung zwischen x-Werte (sollte nun sowieso gleich splitref sein)
            For i = 1 To xValExtra.Count - 2
                If Math.Abs(xValExtra(i) - xValExtra(i + 1)) > 40 Then
                    splitmes = i
                End If
            Next
            '---------------

            'Datenglättung
            If splitmes <> 0 Then                   'Falls Spiegel Mittelloch aufweist
                For t = 0 To 1

                    'Glätte Daten
                    '--------------------   

                    Dim xAvg, yAvg As New List(Of Double)
                    xAvg.Clear()
                    yAvg.Clear()

                    For u = 1 To (xValExtra.Count / 2) / AvgStep
                        Dim xmid As Double = 0
                        Dim ymid As Double = 0
                        For v = 0 To AvgStep - 1
                            xmid += xValExtra(v + AvgStep * (u - 1) + t * (xValExtra.Count / 2))
                            ymid += yValExtra(v + AvgStep * (u - 1) + t * (xValExtra.Count / 2))
                        Next
                        xAvg.Add(xmid / AvgStep)
                        yAvg.Add(ymid / AvgStep)
                    Next

                    If t = 0 Then
                        xAvg.Add(xValExtra(0))                                            'Füge äußere Randpunkte hinzu (negative Werte)
                        yAvg.Add(yValExtra(0))
                        xAvg.Add(xValExtra(splitmes))                                     'Füge innere Randpunkte hinzu (negative Werte)
                        yAvg.Add(yValExtra(splitmes))
                    ElseIf t = 1 Then
                        xAvg.Add(xValExtra(xValExtra.Count - 1))                          'Füge äußere Randpunkte hinzu (positive Werte)
                        yAvg.Add(yValExtra(yValExtra.Count - 1))
                        xAvg.Add(xValExtra(splitmes + 1))                                 'Füge innere Randpunkte hinzu (positive Werte)
                        yAvg.Add(yValExtra(splitmes + 1))
                    End If
                    '--------------------


                    'Data Sorting: Ordne Datenpunkte nach aufsteigendem x; verlinkt mit y-Wert
                    '-----------------
                    yAvg = DataSorting(xAvg, yAvg)
                    For i = yAvg.Count - 1 To 0 Step -1
                        If i Mod 2 = 0 Then
                            xAvg.Add(yAvg(i))
                            yAvg.RemoveAt(i)
                        End If
                    Next
                    xAvg.Sort()
                    '-----------------


                    'Spline Interpolation
                    '-----------------
                    SplineDensityCheckpoint = 1
                    If t = 0 Then
                        yValSpline = SplineInterpolation(xAvg, yAvg)
                        For i = yValSpline.Count - 1 To 0 Step -1
                            If i Mod 2 = 0 Then
                                xValSpline.Add(yValSpline(i))
                                yValSpline.RemoveAt(i)
                            End If
                        Next
                        xValSpline.Sort()
                    ElseIf t = 1 Then
                        yAvg = SplineInterpolation(xAvg, yAvg)
                        For i = yAvg.Count - 1 To 0 Step -1
                            If i Mod 2 = 0 Then
                                xAvg.Add(yAvg(i))
                                yAvg.RemoveAt(i)
                            End If
                        Next
                        xAvg.Sort()
                        For i = 0 To xAvg.Count - 1
                            xValSpline.Add(xAvg(i))
                            yValSpline.Add(yAvg(i))
                        Next
                    End If
                    SplineDensityCheckpoint = 0
                    '-----------------

                Next

                'Data Sorting: Ordne Datenpunkte nach sinkendem x; verlinkt mit y-Wert
                '-----------------
                DataSortingCheckpoint = 1
                yValSpline = DataSorting(xValSpline, yValSpline)
                For i = 0 To yValSpline.Count - 1
                    If i Mod 2 = 0 Then
                        xValSpline.Add(yValSpline(i))
                    End If
                Next
                For i = yValSpline.Count - 1 To 0 Step -1
                    If i Mod 2 = 0 Then
                        yValSpline.RemoveAt(i)
                    End If
                Next
                DataSortingCheckpoint = 0
                '-----------------

            Else                                   'Falls Spiegel kein Mittelloch aufweist
                'Glätte Daten
                '--------------------   

                Dim xAvg, yAvg As New List(Of Double)
                xAvg.Clear()
                yAvg.Clear()

                For u = 1 To xValExtra.Count / AvgStep
                    Dim xmid As Double = 0
                    Dim ymid As Double = 0
                    For v = 0 To AvgStep - 1
                        xmid += xValExtra(v + AvgStep * (u - 1))
                        ymid += yValExtra(v + AvgStep * (u - 1))
                    Next
                    xAvg.Add(xmid / AvgStep)
                    yAvg.Add(ymid / AvgStep)
                Next

                xAvg.Add(xValExtra(0))                                                 'Füge äußere Randpunkte hinzu (negative Werte)
                yAvg.Add(yValExtra(0))
                xAvg.Add(xValExtra(xValExtra.Count - 1))                               'Füge äußere Randpunkte hinzu (positive Werte)
                yAvg.Add(yValExtra(yValExtra.Count - 1))


                'Data Sorting: Ordne Datenpunkte nach aufsteigendem x; verlinkt mit y-Wert
                '-----------------
                yAvg = DataSorting(xAvg, yAvg)
                For i = yAvg.Count - 1 To 0 Step -1
                    If i Mod 2 = 0 Then
                        xAvg.Add(yAvg(i))
                        yAvg.RemoveAt(i)
                    End If
                Next
                xAvg.Sort()
                '-----------------



                'Spline Interpolation
                '-----------------
                SplineDensityCheckpoint = 1
                yValSpline = SplineInterpolation(xAvg, yAvg)
                For i = yValSpline.Count - 1 To 0 Step -1
                    If i Mod 2 = 0 Then
                        xValSpline.Add(yValSpline(i))
                        yValSpline.RemoveAt(i)
                    End If
                Next
                xValSpline.Sort()
                SplineDensityCheckpoint = 0
                '-----------------

            End If


            For i = 0 To xValSpline.Count - 1
                Dim rad As Double
                Dim phi As Double
                phi = 180 / Math.PI * Math.Atan(1 / Math.Tan(xValSpline(i) / (2 * ArmLength))) - extraMessDeg                  'Zylinderkoordinaten Winkel + Drehwinkel der Messung
                rad = Math.Abs(2 * ArmLength * Math.Sin(xValSpline(i) / (2 * ArmLength)))                                      'Zylinderkoordinaten Radius

                phiList.Add(phi)
                radList.Add(rad)
                zList.Add(yValSpline(i))                                                                                       'Zylinderkoordinaten Z-Wert
            Next


            'Füge Daten der sortierten Liste hinzu
            Dim Mid As Integer = radList.Count / 2                   'Mittlerer Wert der Einzelmessung (mit Nullpunkt um 1 verschoben, wie normal)
            For i = radList.Count - 1 To 0 Step -1
                rSort.Insert(i * nMess, radList(Math.Round(0.1 + (Mid - 1) - i / 2 * (-1) ^ i)))        'Warum +0.1? Eine Zahl, deren Komma-Ziffer 5 ist, wird in VB.net aufgerundet, wenn die vorhergehende Ziffer ungerade ist. Ist die vorhergehende Ziffer jedoch gerade, so wird abgerundet! Deswegen +0.1 um dem entgegenzuwirken. Siehe: http://vb-tec.de/runden.htm
                pSort.Insert(i * nMess, phiList(Math.Round(0.1 + (Mid - 1) - i / 2 * (-1) ^ i)))
                zSort.Insert(i * nMess, zList(Math.Round(0.1 + (Mid - 1) - i / 2 * (-1) ^ i)))
            Next


            phiList.Clear()
            radList.Clear()
            zList.Clear()
            xValSpline.Clear()
            yValSpline.Clear()

            nMess = nMess + 1               'erweitere die Anzahl der Messungen um 1, da eine weitere nun hinzugefügt wurde

        Else
            Exit Sub

        End If

    End Sub

    Public Class AlphanumComparator
        Implements IComparer

        Public Function Compare(ByVal x As Object,
                            ByVal y As Object) As Integer Implements IComparer.Compare

            ' [1] Validate the arguments.
            Dim s1 As String = x
            If s1 = Nothing Then
                Return 0
            End If

            Dim s2 As String = y
            If s2 = Nothing Then
                Return 0
            End If

            Dim len1 As Integer = s1.Length
            Dim len2 As Integer = s2.Length
            Dim marker1 As Integer = 0
            Dim marker2 As Integer = 0

            ' [2] Loop over both Strings.
            While marker1 < len1 And marker2 < len2

                ' [3] Get Chars.
                Dim ch1 As Char = s1(marker1)
                Dim ch2 As Char = s2(marker2)

                Dim space1(len1) As Char
                Dim loc1 As Integer = 0
                Dim space2(len2) As Char
                Dim loc2 As Integer = 0

                ' [4] Collect digits for String one.
                Do
                    space1(loc1) = ch1
                    loc1 += 1
                    marker1 += 1

                    If marker1 < len1 Then
                        ch1 = s1(marker1)
                    Else
                        Exit Do
                    End If
                Loop While Char.IsDigit(ch1) = Char.IsDigit(space1(0))

                ' [5] Collect digits for String two.
                Do
                    space2(loc2) = ch2
                    loc2 += 1
                    marker2 += 1

                    If marker2 < len2 Then
                        ch2 = s2(marker2)
                    Else
                        Exit Do
                    End If
                Loop While Char.IsDigit(ch2) = Char.IsDigit(space2(0))

                ' [6] Convert to Strings.
                Dim str1 = New String(space1)
                Dim str2 = New String(space2)

                ' [7] Parse Strings into Integers.
                Dim result As Integer
                If Char.IsDigit(space1(0)) And Char.IsDigit(space2(0)) Then
                    Dim thisNumericChunk = Integer.Parse(str1)
                    Dim thatNumericChunk = Integer.Parse(str2)
                    result = thisNumericChunk.CompareTo(thatNumericChunk)
                Else
                    result = str1.CompareTo(str2)
                End If

                ' [8] Return result if not equal.
                If Not result = 0 Then
                    Return result
                End If
            End While

            ' [9] Compare lengths.
            Return len1 - len2
        End Function
    End Class

    Function SplineInterpolation(ByVal xList As List(Of Double), ByVal yList As List(Of Double)) As List(Of Double)

        Dim result As New List(Of Double)
        Dim N As Integer = xList.Count - 1
        Dim A(N), B(N), C(N), D(N), H(N), X(N), Y(N) As Double
        Dim S(N + 1) As Double
        Dim x_p As Double, y_p As Double
        Dim density As Integer = trckBarInt.Value

        If SplineDensityCheckpoint = 1 Then
            density = 10
        End If

        For i = 0 To N
            X(i) = xList(i)
            Y(i) = yList(i)
        Next

        xList.Clear()
        yList.Clear()

        For i = 0 To N - 1
            H(i) = X(i + 1) - X(i)
        Next

        For i = 1 To N - 1
            B(i) = 2 * (H(i - 1) + H(i))
            D(i) = 6 * ((Y(i + 1) - Y(i)) / H(i) - (Y(i) - Y(i - 1)) / H(i - 1))
        Next

        For i = 2 To N - 1
            A(i) = H(i - 1)
        Next

        C(1) = H(1) / B(1)
        For i = 2 To N - 2
            C(i) = H(i) / (B(i) - C(i - 1) * A(i))
        Next

        D(1) = D(1) / B(1)
        For i = 2 To N - 1
            D(i) = (D(i) - D(i - 1) * A(i)) / (B(i) - C(i - 1) * A(i))
        Next

        S(N - 1) = D(N - 1)
        For i = (N - 2) To 1 Step -1
            S(i) = D(i) - C(i) * S(i + 1)
        Next

        For i = 0 To N - 1
            A(i) = (S(i + 1) - S(i)) / (6 * H(i))
            B(i) = S(i) / 2
            C(i) = (Y(i + 1) - Y(i)) / H(i) - (2 * H(i) * S(i) + H(i) * S(i + 1)) / 6
            D(i) = Y(i)
        Next i

        For i = 0 To N - 1
            For j = 1 To density
                x_p = X(i) + (H(i) / density) * (j - 1)
                y_p = A(i) * (x_p - X(i)) ^ 3 + B(i) * (x_p - X(i)) ^ 2 + C(i) * (x_p - X(i)) + D(i)

                result.Add(x_p)
                result.Add(y_p)
            Next j
        Next i

        SplineInterpolation = result
    End Function

    Function DataSorting(ByVal xList As List(Of Double), ByVal yList As List(Of Double)) As List(Of Double)

        Dim result As New List(Of Double)

        Dim dt As New DataTable
        dt.Columns.Add("ColumnX", GetType(Double))
        dt.Columns.Add("ColumnY", GetType(Double))

        For i = 0 To xList.Count - 1
            dt.Rows.Add(xList(i), yList(i))
        Next

        xList.Clear()
        yList.Clear()

        Dim dfv As DataView = dt.DefaultView

        If DataSortingCheckpoint = 0 Then
            dfv.Sort = "ColumnX ASC"                      'let the magic happen
        ElseIf DataSortingCheckpoint = 1 Then
            dfv.Sort = "ColumnX DESC"
        End If

        Dim dtnew As DataTable = dfv.ToTable()

        For i = 0 To dt.Rows.Count - 1
            result.Add(dtnew.Rows(i)(0))
            result.Add(dtnew.Rows(i)(1))
        Next

        dt.Clear()
        dtnew.Clear()

        Do While dfv.Count > 0
            dfv.Delete(0)
        Loop



        DataSorting = result
    End Function

End Class