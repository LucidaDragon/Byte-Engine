Public Class PlayerCharacter
    Inherits Character

    Public CharBounds As Rectangle
    Public IsLocal As Boolean = False
    Public IP As String = "localhost"

    Sub New(parent As Viewport)
        MyBase.New(parent, Window.Sprites.Images.Item("char.normal.bow.png"), parent.CameraPosition + New Point(parent.Screen.Width / 2, parent.Screen.Height / 2), "Player", False)
    End Sub

    Public Sub Update()
        loc = Parent.CameraPosition + New Point((Parent.Screen.Width / 2) - (Sprite.Width / 2), (Parent.Screen.Height / 2) - (Sprite.Height / 2))
        CharBounds = New Rectangle(loc, New Size(16, 16))
    End Sub

    Public Overrides Function ToString() As String
        Return "{" + Name.Replace(",", "").Replace("{", "").Replace("}", "").Replace(":", "") + ":" + CharBounds.X.ToString + "," + CharBounds.Y.ToString + ":" + IP + "}"
    End Function

    Public Shared Function FromString(str As String, parent As Viewport) As PlayerCharacter
        Dim player As New PlayerCharacter(parent)
        str = str.Replace("{", "").Replace("}", "")
        player.Name = str.Split(":").ElementAt(0)
        Dim location As New Point(Convert.ToInt32(str.Split(":").ElementAt(1).Split(",").First), Convert.ToInt32(str.Split(":").ElementAt(1).Split(",").Last))
        player.CharBounds = New Rectangle(location, New Size(16, 16))
        player.IP = str.Split(":").ElementAt(2)
        Return player
    End Function
End Class
