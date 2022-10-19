<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.btnMess = New System.Windows.Forms.Button()
        Me.btnCalc = New System.Windows.Forms.Button()
        Me.FilesInfoBox = New System.Windows.Forms.GroupBox()
        Me.lblinfo = New System.Windows.Forms.Label()
        Me.btnClear2 = New System.Windows.Forms.Button()
        Me.btnClear1 = New System.Windows.Forms.Button()
        Me.lblRef = New System.Windows.Forms.Label()
        Me.txtBoxReference = New System.Windows.Forms.TextBox()
        Me.lblMirrorhole = New System.Windows.Forms.Label()
        Me.txtBoxMessungen = New System.Windows.Forms.TextBox()
        Me.Winkelanzeige = New System.Windows.Forms.Label()
        Me.lblAngle = New System.Windows.Forms.Label()
        Me.nMessungen = New System.Windows.Forms.Label()
        Me.btnRef = New System.Windows.Forms.Button()
        Me.btnInt = New System.Windows.Forms.Button()
        Me.btnSvDAT = New System.Windows.Forms.Button()
        Me.lblSwingarm = New System.Windows.Forms.Label()
        Me.btnSetArmlength = New System.Windows.Forms.Button()
        Me.txtBoxArmLength = New System.Windows.Forms.TextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lblRes = New System.Windows.Forms.Label()
        Me.trckBarRes = New System.Windows.Forms.TrackBar()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.trckBarInt = New System.Windows.Forms.TrackBar()
        Me.txtBoxDeg = New System.Windows.Forms.TextBox()
        Me.btnSetDeg = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnWolke = New System.Windows.Forms.Button()
        Me.cbOptimized = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtBoxROC = New System.Windows.Forms.TextBox()
        Me.txtBoxConical = New System.Windows.Forms.TextBox()
        Me.btnSetROC = New System.Windows.Forms.Button()
        Me.btnSetConical = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtBoxRadius = New System.Windows.Forms.TextBox()
        Me.btnSetRadius = New System.Windows.Forms.Button()
        Me.FilesInfoBox.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trckBarRes, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trckBarInt, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'btnMess
        '
        Me.btnMess.Location = New System.Drawing.Point(406, 85)
        Me.btnMess.Name = "btnMess"
        Me.btnMess.Size = New System.Drawing.Size(103, 44)
        Me.btnMess.TabIndex = 0
        Me.btnMess.Text = "Select Measurments"
        Me.btnMess.UseVisualStyleBackColor = True
        '
        'btnCalc
        '
        Me.btnCalc.Location = New System.Drawing.Point(407, 241)
        Me.btnCalc.Name = "btnCalc"
        Me.btnCalc.Size = New System.Drawing.Size(103, 44)
        Me.btnCalc.TabIndex = 2
        Me.btnCalc.Text = "Calculate Data"
        Me.btnCalc.UseVisualStyleBackColor = True
        '
        'FilesInfoBox
        '
        Me.FilesInfoBox.Controls.Add(Me.lblinfo)
        Me.FilesInfoBox.Controls.Add(Me.btnClear2)
        Me.FilesInfoBox.Controls.Add(Me.btnClear1)
        Me.FilesInfoBox.Controls.Add(Me.lblRef)
        Me.FilesInfoBox.Controls.Add(Me.txtBoxReference)
        Me.FilesInfoBox.Controls.Add(Me.lblMirrorhole)
        Me.FilesInfoBox.Controls.Add(Me.txtBoxMessungen)
        Me.FilesInfoBox.Controls.Add(Me.Winkelanzeige)
        Me.FilesInfoBox.Controls.Add(Me.lblAngle)
        Me.FilesInfoBox.Controls.Add(Me.nMessungen)
        Me.FilesInfoBox.Location = New System.Drawing.Point(47, 233)
        Me.FilesInfoBox.Name = "FilesInfoBox"
        Me.FilesInfoBox.Size = New System.Drawing.Size(342, 268)
        Me.FilesInfoBox.TabIndex = 4
        Me.FilesInfoBox.TabStop = False
        Me.FilesInfoBox.Text = "Files Info"
        '
        'lblinfo
        '
        Me.lblinfo.AutoSize = True
        Me.lblinfo.Location = New System.Drawing.Point(10, 142)
        Me.lblinfo.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblinfo.Name = "lblinfo"
        Me.lblinfo.Size = New System.Drawing.Size(271, 13)
        Me.lblinfo.TabIndex = 13
        Me.lblinfo.Text = "Caution! Files need to be sorted according to the angle!!"
        '
        'btnClear2
        '
        Me.btnClear2.Location = New System.Drawing.Point(179, 32)
        Me.btnClear2.Name = "btnClear2"
        Me.btnClear2.Size = New System.Drawing.Size(49, 20)
        Me.btnClear2.TabIndex = 12
        Me.btnClear2.Text = "Clear"
        Me.btnClear2.UseVisualStyleBackColor = True
        '
        'btnClear1
        '
        Me.btnClear1.Location = New System.Drawing.Point(179, 96)
        Me.btnClear1.Name = "btnClear1"
        Me.btnClear1.Size = New System.Drawing.Size(49, 23)
        Me.btnClear1.TabIndex = 11
        Me.btnClear1.Text = "Clear"
        Me.btnClear1.UseVisualStyleBackColor = True
        '
        'lblRef
        '
        Me.lblRef.AutoSize = True
        Me.lblRef.Location = New System.Drawing.Point(10, 15)
        Me.lblRef.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblRef.Name = "lblRef"
        Me.lblRef.Size = New System.Drawing.Size(76, 13)
        Me.lblRef.TabIndex = 8
        Me.lblRef.Text = "Reference File"
        '
        'txtBoxReference
        '
        Me.txtBoxReference.Location = New System.Drawing.Point(12, 32)
        Me.txtBoxReference.Margin = New System.Windows.Forms.Padding(2)
        Me.txtBoxReference.Name = "txtBoxReference"
        Me.txtBoxReference.Size = New System.Drawing.Size(161, 20)
        Me.txtBoxReference.TabIndex = 7
        '
        'lblMirrorhole
        '
        Me.lblMirrorhole.AutoSize = True
        Me.lblMirrorhole.Location = New System.Drawing.Point(10, 226)
        Me.lblMirrorhole.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblMirrorhole.Name = "lblMirrorhole"
        Me.lblMirrorhole.Size = New System.Drawing.Size(112, 13)
        Me.lblMirrorhole.TabIndex = 10
        Me.lblMirrorhole.Text = "Mirrorhole radius [mm]:"
        '
        'txtBoxMessungen
        '
        Me.txtBoxMessungen.Location = New System.Drawing.Point(12, 74)
        Me.txtBoxMessungen.Margin = New System.Windows.Forms.Padding(2)
        Me.txtBoxMessungen.Multiline = True
        Me.txtBoxMessungen.Name = "txtBoxMessungen"
        Me.txtBoxMessungen.Size = New System.Drawing.Size(161, 67)
        Me.txtBoxMessungen.TabIndex = 8
        '
        'Winkelanzeige
        '
        Me.Winkelanzeige.AutoSize = True
        Me.Winkelanzeige.Location = New System.Drawing.Point(10, 203)
        Me.Winkelanzeige.Name = "Winkelanzeige"
        Me.Winkelanzeige.Size = New System.Drawing.Size(96, 13)
        Me.Winkelanzeige.TabIndex = 6
        Me.Winkelanzeige.Text = "Angle step [deg] = "
        '
        'lblAngle
        '
        Me.lblAngle.AutoSize = True
        Me.lblAngle.Location = New System.Drawing.Point(80, 49)
        Me.lblAngle.Name = "lblAngle"
        Me.lblAngle.Size = New System.Drawing.Size(0, 13)
        Me.lblAngle.TabIndex = 4
        '
        'nMessungen
        '
        Me.nMessungen.AutoSize = True
        Me.nMessungen.Location = New System.Drawing.Point(10, 54)
        Me.nMessungen.Name = "nMessungen"
        Me.nMessungen.Size = New System.Drawing.Size(131, 13)
        Me.nMessungen.TabIndex = 0
        Me.nMessungen.Text = "Number of Measurements:"
        '
        'btnRef
        '
        Me.btnRef.Location = New System.Drawing.Point(407, 27)
        Me.btnRef.Margin = New System.Windows.Forms.Padding(2)
        Me.btnRef.Name = "btnRef"
        Me.btnRef.Size = New System.Drawing.Size(103, 44)
        Me.btnRef.TabIndex = 5
        Me.btnRef.Text = "Select Reference"
        Me.btnRef.UseVisualStyleBackColor = True
        '
        'btnInt
        '
        Me.btnInt.Location = New System.Drawing.Point(407, 290)
        Me.btnInt.Margin = New System.Windows.Forms.Padding(2)
        Me.btnInt.Name = "btnInt"
        Me.btnInt.Size = New System.Drawing.Size(103, 101)
        Me.btnInt.TabIndex = 7
        Me.btnInt.Text = "Interpolate!"
        Me.btnInt.UseVisualStyleBackColor = True
        '
        'btnSvDAT
        '
        Me.btnSvDAT.Location = New System.Drawing.Point(407, 469)
        Me.btnSvDAT.Name = "btnSvDAT"
        Me.btnSvDAT.Size = New System.Drawing.Size(102, 32)
        Me.btnSvDAT.TabIndex = 8
        Me.btnSvDAT.Text = "Save as .DAT"
        Me.btnSvDAT.UseVisualStyleBackColor = True
        '
        'lblSwingarm
        '
        Me.lblSwingarm.AutoSize = True
        Me.lblSwingarm.Location = New System.Drawing.Point(44, 30)
        Me.lblSwingarm.Name = "lblSwingarm"
        Me.lblSwingarm.Size = New System.Drawing.Size(85, 13)
        Me.lblSwingarm.TabIndex = 10
        Me.lblSwingarm.Text = "Swingarm length"
        '
        'btnSetArmlength
        '
        Me.btnSetArmlength.Location = New System.Drawing.Point(268, 25)
        Me.btnSetArmlength.Name = "btnSetArmlength"
        Me.btnSetArmlength.Size = New System.Drawing.Size(74, 28)
        Me.btnSetArmlength.TabIndex = 12
        Me.btnSetArmlength.Text = "Set"
        Me.btnSetArmlength.UseVisualStyleBackColor = True
        '
        'txtBoxArmLength
        '
        Me.txtBoxArmLength.Location = New System.Drawing.Point(162, 27)
        Me.txtBoxArmLength.Name = "txtBoxArmLength"
        Me.txtBoxArmLength.Size = New System.Drawing.Size(100, 20)
        Me.txtBoxArmLength.TabIndex = 13
        Me.txtBoxArmLength.Text = "855,2"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.PictureBox1.Location = New System.Drawing.Point(582, 18)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(437, 437)
        Me.PictureBox1.TabIndex = 14
        Me.PictureBox1.TabStop = False
        '
        'lblRes
        '
        Me.lblRes.AutoSize = True
        Me.lblRes.Location = New System.Drawing.Point(642, 468)
        Me.lblRes.Name = "lblRes"
        Me.lblRes.Size = New System.Drawing.Size(79, 13)
        Me.lblRes.TabIndex = 15
        Me.lblRes.Text = "Set Resolution:"
        '
        'trckBarRes
        '
        Me.trckBarRes.Location = New System.Drawing.Point(738, 461)
        Me.trckBarRes.Maximum = 600
        Me.trckBarRes.Minimum = 200
        Me.trckBarRes.Name = "trckBarRes"
        Me.trckBarRes.Size = New System.Drawing.Size(104, 45)
        Me.trckBarRes.TabIndex = 18
        Me.trckBarRes.Value = 400
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(864, 466)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(75, 23)
        Me.btnRefresh.TabIndex = 19
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'trckBarInt
        '
        Me.trckBarInt.Location = New System.Drawing.Point(407, 404)
        Me.trckBarInt.Maximum = 700
        Me.trckBarInt.Minimum = 100
        Me.trckBarInt.Name = "trckBarInt"
        Me.trckBarInt.Size = New System.Drawing.Size(104, 45)
        Me.trckBarInt.TabIndex = 20
        Me.trckBarInt.Value = 400
        '
        'txtBoxDeg
        '
        Me.txtBoxDeg.Location = New System.Drawing.Point(162, 64)
        Me.txtBoxDeg.Name = "txtBoxDeg"
        Me.txtBoxDeg.Size = New System.Drawing.Size(100, 20)
        Me.txtBoxDeg.TabIndex = 22
        '
        'btnSetDeg
        '
        Me.btnSetDeg.Location = New System.Drawing.Point(268, 59)
        Me.btnSetDeg.Name = "btnSetDeg"
        Me.btnSetDeg.Size = New System.Drawing.Size(74, 28)
        Me.btnSetDeg.TabIndex = 23
        Me.btnSetDeg.Text = "Set + Open"
        Me.btnSetDeg.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(44, 67)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(114, 13)
        Me.Label1.TabIndex = 24
        Me.Label1.Text = "Degrees additional File"
        '
        'btnWolke
        '
        Me.btnWolke.Location = New System.Drawing.Point(130, 529)
        Me.btnWolke.Name = "btnWolke"
        Me.btnWolke.Size = New System.Drawing.Size(75, 23)
        Me.btnWolke.TabIndex = 25
        Me.btnWolke.Text = "Point Cloud"
        Me.btnWolke.UseVisualStyleBackColor = True
        '
        'cbOptimized
        '
        Me.cbOptimized.AutoSize = True
        Me.cbOptimized.Checked = True
        Me.cbOptimized.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbOptimized.Location = New System.Drawing.Point(47, 203)
        Me.cbOptimized.Name = "cbOptimized"
        Me.cbOptimized.Size = New System.Drawing.Size(165, 17)
        Me.cbOptimized.TabIndex = 26
        Me.cbOptimized.Text = "Use Zernike optimized values"
        Me.cbOptimized.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(44, 101)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(30, 13)
        Me.Label2.TabIndex = 27
        Me.Label2.Text = "ROC"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(44, 129)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(42, 13)
        Me.Label3.TabIndex = 28
        Me.Label3.Text = "Conical"
        '
        'txtBoxROC
        '
        Me.txtBoxROC.Location = New System.Drawing.Point(162, 98)
        Me.txtBoxROC.Name = "txtBoxROC"
        Me.txtBoxROC.Size = New System.Drawing.Size(100, 20)
        Me.txtBoxROC.TabIndex = 29
        Me.txtBoxROC.Text = "-4000"
        '
        'txtBoxConical
        '
        Me.txtBoxConical.Location = New System.Drawing.Point(162, 129)
        Me.txtBoxConical.Name = "txtBoxConical"
        Me.txtBoxConical.Size = New System.Drawing.Size(100, 20)
        Me.txtBoxConical.TabIndex = 30
        Me.txtBoxConical.Text = "-1,1382"
        '
        'btnSetROC
        '
        Me.btnSetROC.Location = New System.Drawing.Point(268, 93)
        Me.btnSetROC.Name = "btnSetROC"
        Me.btnSetROC.Size = New System.Drawing.Size(74, 28)
        Me.btnSetROC.TabIndex = 31
        Me.btnSetROC.Text = "Set"
        Me.btnSetROC.UseVisualStyleBackColor = True
        '
        'btnSetConical
        '
        Me.btnSetConical.Location = New System.Drawing.Point(268, 125)
        Me.btnSetConical.Name = "btnSetConical"
        Me.btnSetConical.Size = New System.Drawing.Size(74, 28)
        Me.btnSetConical.TabIndex = 32
        Me.btnSetConical.Text = "Set"
        Me.btnSetConical.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(44, 167)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(40, 13)
        Me.Label4.TabIndex = 33
        Me.Label4.Text = "Radius"
        '
        'txtBoxRadius
        '
        Me.txtBoxRadius.Location = New System.Drawing.Point(162, 164)
        Me.txtBoxRadius.Name = "txtBoxRadius"
        Me.txtBoxRadius.Size = New System.Drawing.Size(100, 20)
        Me.txtBoxRadius.TabIndex = 34
        Me.txtBoxRadius.Text = "417,75"
        '
        'btnSetRadius
        '
        Me.btnSetRadius.Location = New System.Drawing.Point(268, 159)
        Me.btnSetRadius.Name = "btnSetRadius"
        Me.btnSetRadius.Size = New System.Drawing.Size(74, 28)
        Me.btnSetRadius.TabIndex = 35
        Me.btnSetRadius.Text = "Set"
        Me.btnSetRadius.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1059, 576)
        Me.Controls.Add(Me.btnSetRadius)
        Me.Controls.Add(Me.txtBoxRadius)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btnSetConical)
        Me.Controls.Add(Me.btnSetROC)
        Me.Controls.Add(Me.txtBoxConical)
        Me.Controls.Add(Me.txtBoxROC)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cbOptimized)
        Me.Controls.Add(Me.btnWolke)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnSetDeg)
        Me.Controls.Add(Me.txtBoxDeg)
        Me.Controls.Add(Me.trckBarInt)
        Me.Controls.Add(Me.btnRefresh)
        Me.Controls.Add(Me.trckBarRes)
        Me.Controls.Add(Me.lblRes)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.txtBoxArmLength)
        Me.Controls.Add(Me.btnSetArmlength)
        Me.Controls.Add(Me.lblSwingarm)
        Me.Controls.Add(Me.btnSvDAT)
        Me.Controls.Add(Me.btnInt)
        Me.Controls.Add(Me.btnRef)
        Me.Controls.Add(Me.FilesInfoBox)
        Me.Controls.Add(Me.btnCalc)
        Me.Controls.Add(Me.btnMess)
        Me.Name = "Form1"
        Me.Text = "Swingarm Profilometer 3D Interpolation"
        Me.FilesInfoBox.ResumeLayout(False)
        Me.FilesInfoBox.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trckBarRes, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trckBarInt, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents btnMess As Button
    Friend WithEvents btnCalc As Button
    Friend WithEvents FilesInfoBox As GroupBox
    Friend WithEvents nMessungen As Label
    Friend WithEvents lblAngle As Label
    Friend WithEvents Winkelanzeige As Label
    Friend WithEvents btnRef As Button
    Friend WithEvents txtBoxReference As TextBox
    Friend WithEvents txtBoxMessungen As TextBox
    Friend WithEvents lblRef As Label
    Friend WithEvents lblMirrorhole As Label
    Friend WithEvents btnClear1 As Button
    Friend WithEvents btnClear2 As Button
    Friend WithEvents btnInt As Button
    Friend WithEvents lblinfo As Label
    Friend WithEvents btnSvDAT As Button
    Friend WithEvents lblSwingarm As Label
    Friend WithEvents btnSetArmlength As Button
    Friend WithEvents txtBoxArmLength As TextBox
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents lblRes As Label
    Friend WithEvents trckBarRes As TrackBar
    Friend WithEvents btnRefresh As Button
    Friend WithEvents trckBarInt As TrackBar
    Friend WithEvents txtBoxDeg As TextBox
    Friend WithEvents btnSetDeg As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnWolke As Button
    Friend WithEvents cbOptimized As CheckBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtBoxROC As TextBox
    Friend WithEvents txtBoxConical As TextBox
    Friend WithEvents btnSetROC As Button
    Friend WithEvents btnSetConical As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents txtBoxRadius As TextBox
    Friend WithEvents btnSetRadius As Button
End Class
