<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class MainForm1
    Inherits System.Windows.Forms.Form

    'Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタでこのプロシージャを変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnStart = New System.Windows.Forms.Button
        Me.cmbPRINTER = New System.Windows.Forms.ComboBox
        Me.label1 = New System.Windows.Forms.Label
        Me.cmbCOMM = New System.Windows.Forms.ComboBox
        Me.label3 = New System.Windows.Forms.Label
        Me.SuspendLayout()
		' 
		' label3
		' 
		Me.label3.Font = New System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular)
		Me.label3.Location = New System.Drawing.Point(26, 14)
		Me.label3.Name = "label3"
		Me.label3.Size = New System.Drawing.Size(174, 29)
		Me.label3.Text = "通信経路"
		' 
		' cmbCOMM
		' 
		Me.cmbCOMM.Font = New System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular)
		Me.cmbCOMM.Location = New System.Drawing.Point(26, 47)
		Me.cmbCOMM.Name = "cmbCOMM"
		Me.cmbCOMM.Size = New System.Drawing.Size(257, 32)
		Me.cmbCOMM.TabIndex = 2
		' 
		' cmbPRINTER
		' 
		Me.cmbPRINTER.Font = New System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular)
		Me.cmbPRINTER.Location = New System.Drawing.Point(26, 123)
		Me.cmbPRINTER.Name = "cmbPRINTER"
		Me.cmbPRINTER.Size = New System.Drawing.Size(257, 32)
		Me.cmbPRINTER.TabIndex = 4
		' 
		' label1
		' 
		Me.label1.Font = New System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular)
		Me.label1.Location = New System.Drawing.Point(26, 94)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(174, 25)
		Me.label1.Text = "プリンタ選択"
		' 
		' btnStart
		' 
		Me.btnStart.Font = New System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular)
		Me.btnStart.Location = New System.Drawing.Point(99, 186)
		Me.btnStart.Name = "btnStart"
		Me.btnStart.Size = New System.Drawing.Size(110, 42)
		Me.btnStart.TabIndex = 6
		Me.btnStart.Text = "開始"
		' 
        'MainForm1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(478, 615)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.cmbPRINTER)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.cmbCOMM)
        Me.Controls.Add(Me.label3)
        Me.Name = "MainForm1"
        Me.Text = "プリンタ印字サンプル(VB)"
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents btnStart As System.Windows.Forms.Button
    Private WithEvents cmbPRINTER As System.Windows.Forms.ComboBox
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents cmbCOMM As System.Windows.Forms.ComboBox
    Private WithEvents label3 As System.Windows.Forms.Label

End Class
