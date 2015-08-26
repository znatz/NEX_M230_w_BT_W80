<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmScanner
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
    Private mainMenu1 As System.Windows.Forms.MainMenu

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.mainMenu1 = New System.Windows.Forms.MainMenu
        Me.MenuItem1 = New System.Windows.Forms.MenuItem
        Me.MenuItem2 = New System.Windows.Forms.MenuItem
        Me.btnStartScan = New System.Windows.Forms.Button
        Me.ScanResult = New System.Windows.Forms.ListBox
        Me.deleteOneLine = New System.Windows.Forms.Button
        Me.submit = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'mainMenu1
        '
        Me.mainMenu1.MenuItems.Add(Me.MenuItem1)
        Me.mainMenu1.MenuItems.Add(Me.MenuItem2)
        '
        'MenuItem1
        '
        Me.MenuItem1.Text = "トップへ"
        '
        'MenuItem2
        '
        Me.MenuItem2.Text = "終了"
        '
        'btnStartScan
        '
        Me.btnStartScan.Location = New System.Drawing.Point(12, 27)
        Me.btnStartScan.Name = "btnStartScan"
        Me.btnStartScan.Size = New System.Drawing.Size(72, 20)
        Me.btnStartScan.TabIndex = 0
        Me.btnStartScan.Text = "スキャン"
        '
        'ScanResult
        '
        Me.ScanResult.Location = New System.Drawing.Point(12, 54)
        Me.ScanResult.Name = "ScanResult"
        Me.ScanResult.Size = New System.Drawing.Size(215, 212)
        Me.ScanResult.TabIndex = 1
        '
        'deleteOneLine
        '
        Me.deleteOneLine.Location = New System.Drawing.Point(75, 27)
        Me.deleteOneLine.Name = "deleteOneLine"
        Me.deleteOneLine.Size = New System.Drawing.Size(72, 20)
        Me.deleteOneLine.TabIndex = 2
        Me.deleteOneLine.Text = "一列削除"
        '
        'submit
        '
        Me.submit.Location = New System.Drawing.Point(147, 28)
        Me.submit.Name = "submit"
        Me.submit.Size = New System.Drawing.Size(72, 20)
        Me.submit.TabIndex = 3
        Me.submit.Text = "清算"
        '
        'frmScanner
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 268)
        Me.Controls.Add(Me.submit)
        Me.Controls.Add(Me.deleteOneLine)
        Me.Controls.Add(Me.ScanResult)
        Me.Controls.Add(Me.btnStartScan)
        Me.Menu = Me.mainMenu1
        Me.Name = "frmScanner"
        Me.Text = "売上げ登録(バーコードスキャン)"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
    Friend WithEvents btnStartScan As System.Windows.Forms.Button
    Friend WithEvents ScanResult As System.Windows.Forms.ListBox
    Friend WithEvents deleteOneLine As System.Windows.Forms.Button
    Friend WithEvents submit As System.Windows.Forms.Button
End Class
