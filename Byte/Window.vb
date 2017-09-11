Public Class Window

    Public LocalPlayer As PlayerCharacter
    Public NetworkPlayers As New List(Of PlayerCharacter)

    Private Sub Window_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        DoubleBuffered = True
        LocalPlayer = PlayerCharacter.FromString("{LocalPlayer:0,0:localhost}", Viewport1)
        LocalPlayer.isLocal = True
        RandomizeBackground()
        Draw.Start()
    End Sub

    Private Sub Window_Shown(sender As System.Object, e As System.EventArgs) Handles MyBase.Shown
        Dim rand As New Random
        For x As Integer = 0 To Width Step 16
            For y As Integer = 0 To Height Step 16
                Viewport1.WorldObjects.Add(New WorldObject(Me.Viewport1, Sprites.Images.Item("obj.floor.woodFancy.png"), New Point(x, y)))
                Debug.Print(Convert.ToInt32((Convert.ToDecimal(x) / Width) * 100).ToString + "%")
            Next
        Next
        Viewport1.WorldObjects.Add(LocalPlayer)
    End Sub

    Private Sub UpdateNetwork()
        'TODO sync info
    End Sub

    Private Sub RandomizeBackground()
        Dim val As Integer = New Random().Next(0, 3)
        If val = 0 Then
            BackgroundImage = My.Resources.background_large_A
        ElseIf val = 1 Then
            BackgroundImage = My.Resources.background_large_B
        ElseIf val = 2 Then
            BackgroundImage = My.Resources.background_large_C
        End If
    End Sub

    Public Property BackgroundSpeed As Integer
        Set(value As Integer)
            BckSpeed = Math.Max(Math.Min(value, 99), 0)
            BckTick = 0
        End Set
        Get
            Return BckSpeed
        End Get
    End Property
    Private BckSpeed As Integer = 8
    Private BckTick As Decimal = 0
    Private StillDrawing As Boolean = False
    Private SkipRate As Integer = 0

    Public MsLagDetect As New Stopwatch

    Private Sub Window_Paint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
        BckTick += BackgroundSpeed
        If BckTick >= 99 Then
            BckTick = 0
        End If
        Dim pos As New Point(((BckTick / 100) * 16) - 16, ((BckTick / 100) * 16) - 16)

        StillDrawing = True
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        e.Graphics.DrawImage(BackgroundImage, pos)

        If Debug.Enabled Then
            Dim rect As Rectangle = New Rectangle(New Point(8, 8), New Size(Viewport1.Location.X - 16, Viewport1.Size.Height - 16))
            e.Graphics.FillRectangle(Brushes.Black, rect)
            Dim textPos As Integer = 10
            For Each msg In Debug.DebugOutput
                e.Graphics.DrawString(msg, New Font(FontFamily.Families.First, 12), Brushes.Lime, New Point(10, textPos))
                textPos += 15
            Next
        End If

        If MsLagDetect.IsRunning Then
            Debug.Print("W: " + MsLagDetect.ElapsedMilliseconds.ToString)
            MsLagDetect.Restart()
        Else
            MsLagDetect.Start()
        End If
        StillDrawing = False
    End Sub

    Private Sub ProcessInputs()
        If InputList.Contains(Keys.Up) Then
            Viewport1.MoveCamera(New Point(0, -1))
        ElseIf InputList.Contains(Keys.Down) Then
            Viewport1.MoveCamera(New Point(0, 1))
        ElseIf InputList.Contains(Keys.Left) Then
            Viewport1.MoveCamera(New Point(-1, 0))
        ElseIf InputList.Contains(Keys.Right) Then
            Viewport1.MoveCamera(New Point(1, 0))
        End If

        LocalPlayer.Update()
    End Sub

    Private Sub Draw_Tick(sender As System.Object, e As System.EventArgs) Handles Draw.Tick
        ProcessInputs()
        If Not StillDrawing Then
            If MsLagDetect.ElapsedMilliseconds > 30 Or Not MsLagDetect.IsRunning Then
                Invalidate(False)
            End If
            If Viewport1.MsLagDetect.ElapsedMilliseconds > 30 Or Not Viewport1.MsLagDetect.IsRunning Then
                Viewport1.Invalidate(True)
            End If
        End If
    End Sub

    Public InputList As New List(Of Keys)
    Private Sub Window_KeyDown(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        InputList.Add(e.KeyCode)
    End Sub

    Private Sub Window_KeyUp(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp
        While InputList.Contains(e.KeyCode)
            InputList.Remove(e.KeyCode)
        End While
    End Sub

    Private Sub Window_FormClosed(sender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        End
    End Sub
End Class
