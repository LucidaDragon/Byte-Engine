Public Class CenterPanel
    Inherits Panel

    Sub CenterPanel_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
        DoubleBuffered = True
        BackColor = Color.Transparent
        For Each Control In Controls
            Control.Size = New Size(Math.Min(Width, Height), Math.Min(Width, Height))
            Control.Location = New Point((Width / 2) - (Control.Width / 2), (Height / 2) - (Control.Height / 2))
        Next
    End Sub
End Class
