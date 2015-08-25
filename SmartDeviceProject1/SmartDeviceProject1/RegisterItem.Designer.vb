<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmRegisterItem
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
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
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnStartScan = New System.Windows.Forms.Button
        Me.ScanResult = New System.Windows.Forms.TextBox
        Me.lblJAN = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.tbxItemTitle = New System.Windows.Forms.TextBox
        Me.tbxItemPrice = New System.Windows.Forms.TextBox
        Me.tbxItemCount = New System.Windows.Forms.TextBox
        Me.btnRegisterItem = New System.Windows.Forms.Button
        Me.btnClearForm = New System.Windows.Forms.Button
        Me.MainMenu1 = New System.Windows.Forms.MainMenu
        Me.MenuItem1 = New System.Windows.Forms.MenuItem
        Me.MenuItem2 = New System.Windows.Forms.MenuItem
        Me.SuspendLayout()
        '
        'btnStartScan
        '
        Me.btnStartScan.Location = New System.Drawing.Point(155, 60)
        Me.btnStartScan.Name = "btnStartScan"
        Me.btnStartScan.Size = New System.Drawing.Size(76, 26)
        Me.btnStartScan.TabIndex = 1
        Me.btnStartScan.Text = "スキャン"
        '
        'ScanResult
        '
        Me.ScanResult.Location = New System.Drawing.Point(93, 33)
        Me.ScanResult.Name = "ScanResult"
        Me.ScanResult.Size = New System.Drawing.Size(138, 21)
        Me.ScanResult.TabIndex = 2
        '
        'lblJAN
        '
        Me.lblJAN.Location = New System.Drawing.Point(12, 33)
        Me.lblJAN.Name = "lblJAN"
        Me.lblJAN.Size = New System.Drawing.Size(76, 26)
        Me.lblJAN.Text = "JANコード"
        Me.lblJAN.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(12, 90)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(76, 26)
        Me.Label2.Text = "商品名"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(12, 147)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(76, 26)
        Me.Label3.Text = "売価"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(12, 204)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(76, 26)
        Me.Label4.Text = "個数"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'tbxItemTitle
        '
        Me.tbxItemTitle.Location = New System.Drawing.Point(93, 95)
        Me.tbxItemTitle.MaxLength = 50
        Me.tbxItemTitle.Name = "tbxItemTitle"
        Me.tbxItemTitle.Size = New System.Drawing.Size(138, 21)
        Me.tbxItemTitle.TabIndex = 10
        '
        'tbxItemPrice
        '
        Me.tbxItemPrice.Location = New System.Drawing.Point(93, 149)
        Me.tbxItemPrice.MaxLength = 50
        Me.tbxItemPrice.Name = "tbxItemPrice"
        Me.tbxItemPrice.Size = New System.Drawing.Size(138, 21)
        Me.tbxItemPrice.TabIndex = 11
        '
        'tbxItemCount
        '
        Me.tbxItemCount.Location = New System.Drawing.Point(93, 203)
        Me.tbxItemCount.MaxLength = 50
        Me.tbxItemCount.Name = "tbxItemCount"
        Me.tbxItemCount.Size = New System.Drawing.Size(138, 21)
        Me.tbxItemCount.TabIndex = 12
        '
        'btnRegisterItem
        '
        Me.btnRegisterItem.Location = New System.Drawing.Point(18, 240)
        Me.btnRegisterItem.Name = "btnRegisterItem"
        Me.btnRegisterItem.Size = New System.Drawing.Size(76, 39)
        Me.btnRegisterItem.TabIndex = 13
        Me.btnRegisterItem.Text = "登録"
        '
        'btnClearForm
        '
        Me.btnClearForm.Location = New System.Drawing.Point(161, 240)
        Me.btnClearForm.Name = "btnClearForm"
        Me.btnClearForm.Size = New System.Drawing.Size(76, 39)
        Me.btnClearForm.TabIndex = 14
        Me.btnClearForm.Text = "クリア"
        '
        'MainMenu1
        '
        Me.MainMenu1.MenuItems.Add(Me.MenuItem1)
        Me.MainMenu1.MenuItems.Add(Me.MenuItem2)
        '
        'MenuItem1
        '
        Me.MenuItem1.Text = "戻る"
        '
        'MenuItem2
        '
        Me.MenuItem2.Text = "終了"
        '
        'frmRegisterItem
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(257, 285)
        Me.Controls.Add(Me.btnClearForm)
        Me.Controls.Add(Me.btnRegisterItem)
        Me.Controls.Add(Me.tbxItemCount)
        Me.Controls.Add(Me.tbxItemPrice)
        Me.Controls.Add(Me.tbxItemTitle)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblJAN)
        Me.Controls.Add(Me.ScanResult)
        Me.Controls.Add(Me.btnStartScan)
        Me.Menu = Me.MainMenu1
        Me.Name = "frmRegisterItem"
        Me.Text = "RegisterItem"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnStartScan As System.Windows.Forms.Button
    Friend WithEvents ScanResult As System.Windows.Forms.TextBox
    Friend WithEvents lblJAN As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents tbxItemTitle As System.Windows.Forms.TextBox
    Friend WithEvents tbxItemPrice As System.Windows.Forms.TextBox
    Friend WithEvents tbxItemCount As System.Windows.Forms.TextBox
    Friend WithEvents btnRegisterItem As System.Windows.Forms.Button
    Friend WithEvents btnClearForm As System.Windows.Forms.Button
    Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
End Class
