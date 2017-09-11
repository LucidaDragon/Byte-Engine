Public Class WorldObject
    Inherits Viewport.DrawCall

    Sub New(parent As Viewport, sprite As Image, location As Point, Optional hasCollision As Boolean = True, Optional hasPhyisics As Boolean = False, Optional bounce As Integer = 0, Optional gravity As Integer = 0, Optional friction As Decimal = 1)
        MyBase.New(parent, sprite, location)
        Collision = hasCollision
        Phyisics = hasPhyisics
        Bounciness = bounce
        Me.Friction = friction
        Me.Gravity = gravity
    End Sub

    Public Property Location As Point
        Set(value As Point)
            loc = value
        End Set
        Get
            Return loc
        End Get
    End Property

    Public Property Sprite As Image
        Set(value As Image)
            img = value
        End Set
        Get
            Return img
        End Get
    End Property

    Public Collision As Boolean
    Public Phyisics As Boolean
    Public Bounciness As Integer
    Public Friction As Decimal
    Public Gravity As Integer
    Public Velocity As New Point(0, 0)

    Private ClockSpeed As New System.Threading.Timer(New System.Threading.TimerCallback(AddressOf SimulatePhyisics), Nothing, 1, 100)
    Private Sub SimulatePhyisics()
        If Phyisics Then
            ClockSpeed.Change(100, 100)
            If Gravity Then
                Velocity += New Point(0, 980 * 16)
            End If
            If Collision And ((Not Velocity.X = 0) Or (Not Velocity.Y = 0)) Then
                Dim target As Point = loc + New Point(Velocity.X / 10, Velocity.Y / 10)
                For i As Integer = 0 To 100
                    loc = New Point(Functions.Lerp(loc.X, target.X, i \ 100), Functions.Lerp(loc.Y, target.Y, i \ 100))
                    If IsColliding() Then
                        loc = New Point(Functions.Lerp(loc.X, target.X, (i - 1) \ 100), Functions.Lerp(loc.Y, target.Y, (i - 1) \ 100))
                        Exit For
                    End If
                Next
            Else
                loc += New Point(Velocity.X / 10, Velocity.Y / 10)
            End If
        Else
            ClockSpeed.Change(100, New Random().Next(1000, 1100))
        End If
    End Sub

    Public Function IsColliding() As Boolean
        For Each obj In Parent.WorldObjects
            If IsOverlapping(obj) And obj.Collision Then
                Return True
            End If
        Next
        Return False
    End Function
End Class
