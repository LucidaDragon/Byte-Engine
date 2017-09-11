Public Class Viewport
    Inherits Panel

    Public WorldObjects As New List(Of WorldObject)
    Public PixelSize As Integer = 128
    Public Screen As Bitmap
    Public CameraPosition As New Point(64, 64)

    Sub New()
        Screen = New Bitmap(PixelSize, PixelSize)
        DoubleBuffered = True
    End Sub

    Dim DrawCalls As New List(Of DrawCall)
    Sub DrawSprite(sprite As Image, location As Point)
        DrawCalls.Add(New DrawCall(Me, sprite, location))
    End Sub

    Public Class DrawCall
        Public IsVisible As Boolean = False
        Public Parent As Viewport
        Public SnapGrid As Integer = 1

        Protected Property img As Image
            Set(value As Image)
                i = value
                b = New Rectangle(l, i.Size)
            End Set
            Get
                Return i
            End Get
        End Property
        Private i As Image

        Protected Property loc As Point
            Set(value As Point)
                l = value
                b = New Rectangle(l, i.Size)
            End Set
            Get
                Return l
            End Get
        End Property
        Private l As Point

        ReadOnly Property Bounds As Rectangle
            Get
                Return b
            End Get
        End Property
        Private b As Rectangle

        Sub New(parent As Viewport, sprite As Image, location As Point)
            img = sprite
            loc = location
            Me.Parent = parent
        End Sub

        Sub Draw(graphics As Graphics)
            graphics.DrawImage(img, Functions.SnapPointToGrid(loc, SnapGrid) - Parent.CameraPosition)
        End Sub

        Public Function IsOverlapping(obj As DrawCall) As Boolean
            Return Bounds.IntersectsWith(obj.Bounds)
        End Function

        Public Sub BlockingUpdate(otherCalls As List(Of DrawCall)) 'To work on
            IsVisible = True
        End Sub
    End Class

    Sub GenerateDrawCalls()
        Dim BlockingObjs As New List(Of DrawCall)
        BlockingObjs.AddRange(WorldObjects)
        BlockingObjs.Reverse()
        For Each obj In WorldObjects
            obj.BlockingUpdate(BlockingObjs)
            If obj.IsVisible Then
                DrawCalls.Add(obj)
            End If
        Next
        GC.Collect()
        Debug.Print(DrawCalls.Count)
    End Sub

    Sub MoveCamera(direction As Point)
        CameraPosition += direction
    End Sub

    Public MsLagDetect As New Stopwatch
    Private IsDrawing As Boolean = False
    Sub Viewport_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
        If Not IsDrawing Then
            If MsLagDetect.IsRunning Then
                Debug.Print(MsLagDetect.ElapsedMilliseconds.ToString + " ms")
                MsLagDetect.Restart()
            Else
                MsLagDetect.Start()
            End If
            IsDrawing = True
            GenerateDrawCalls()
            Dim FxBuffer As BufferedGraphics = New BufferedGraphicsContext().Allocate(Graphics.FromImage(Screen), e.ClipRectangle)
            FxBuffer.Graphics.Clear(Color.Black)
            For Each DrawingCall In DrawCalls
                DrawingCall.Draw(FxBuffer.Graphics)
            Next
            If DrawCalls.Count > 0 Then
                FxBuffer.Render()
            End If
            e.Graphics.DrawImage(Screen, 0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height)
            DrawCalls.Clear()
            IsDrawing = False
        End If
    End Sub
End Class
