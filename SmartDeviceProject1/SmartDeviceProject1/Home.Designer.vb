<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class Home
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
        Me.Button1 = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.testBT = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.MainMenu1 = New System.Windows.Forms.MainMenu
        Me.MenuItem4 = New System.Windows.Forms.MenuItem
        Me.btnRegisterImage = New System.Windows.Forms.MenuItem
        Me.RegisterSales = New System.Windows.Forms.MenuItem
        Me.Scanning = New System.Windows.Forms.MenuItem
        Me.MenuItem1 = New System.Windows.Forms.MenuItem
        Me.MenuItem2 = New System.Windows.Forms.MenuItem
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(41, 231)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(158, 20)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "終了"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(76, 57)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(77, 20)
        Me.Label1.Text = "レシートテスト"
        '
        'testBT
        '
        Me.testBT.Location = New System.Drawing.Point(41, 96)
        Me.testBT.Name = "testBT"
        Me.testBT.Size = New System.Drawing.Size(158, 20)
        Me.testBT.TabIndex = 1
        Me.testBT.Text = "Bluetoothテスト"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(41, 145)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(158, 20)
        Me.Button2.TabIndex = 3
        Me.Button2.Text = "データベーステスト"
        '
        'MainMenu1
        '
        Me.MainMenu1.MenuItems.Add(Me.MenuItem4)
        Me.MainMenu1.MenuItems.Add(Me.MenuItem2)
        Me.MainMenu1.MenuItems.Add(Me.RegisterSales)
        Me.MainMenu1.MenuItems.Add(Me.MenuItem1)
        '
        'MenuItem4
        '
        Me.MenuItem4.MenuItems.Add(Me.btnRegisterImage)
        Me.MenuItem4.Text = "設定"
        '
        'btnRegisterImage
        '
        Me.btnRegisterImage.Text = "レシートイメージ登録"
        '
        'RegisterSales
        '
        Me.RegisterSales.MenuItems.Add(Me.Scanning)
        Me.RegisterSales.Text = "売上げ登録"
        '
        'Scanning
        '
        Me.Scanning.Text = "スキャン登録"
        '
        'MenuItem1
        '
        Me.MenuItem1.Text = "終了"
        '
        'MenuItem2
        '
        Me.MenuItem2.Text = "商品登録"
        '
        'Home
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 268)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.testBT)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Button1)
        Me.Menu = Me.MainMenu1
        Me.Name = "Home"
        Me.Text = "POSCO_ZN"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents testBT As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents MenuItem4 As System.Windows.Forms.MenuItem
    Friend WithEvents btnRegisterImage As System.Windows.Forms.MenuItem
    Friend WithEvents RegisterSales As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents Scanning As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem

End Class
