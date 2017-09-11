Public Class Functions
    Public Shared Function Lerp(X As Double, Y As Double, A As Decimal) As Double
        Return X + A * (Y - X)
    End Function

    Public Shared Function SnapPointToGrid(loc As Point, gridSize As Integer) As Point
        Return New Point((loc.X \ gridSize) * gridSize, (loc.Y \ gridSize) * gridSize)
    End Function
End Class
