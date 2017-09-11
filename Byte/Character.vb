Public Class Character
    Inherits WorldObject

    Sub New(parent As Viewport, sprite As Image, location As Point, name As String, Optional hasDialog As Boolean = True, Optional dialog As String = "")
        MyBase.New(parent, sprite, location, True, True, 0, 0, 0)
        Me.Name = name
        Me.HasDialog = hasDialog
        Me.Dialog = dialog
    End Sub

    Public Name As String
    Public HasDialog As Boolean
    Public Dialog As String

    Public Function SpeakDialog() As String()
        If Dialog.Length = 0 Then
            Return {"Go away.", "Do I know you? I don't think I do.", "Sorry, I can't talk right now.", "...", "Um. Hi?", "You're weird. Why are you talking to me?", "Please leave me alone.", "What do you want? Oh wait, I don't care.", "!@#$"}.ElementAt(New Random().Next(0, 10)).Split(Environment.NewLine)
        Else
            Return Dialog.Split(Environment.NewLine)
        End If
    End Function
End Class
