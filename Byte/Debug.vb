Public Class Debug
    Public Shared DebugOutput As New List(Of String)
    Public Shared LogSize As Integer = 10
    Public Shared Enabled As Boolean = True

    Public Shared Sub Print(obj As Object)
        DebugOutput.Add(obj.ToString)
        If DebugOutput.Count > LogSize Then
            DebugOutput.RemoveAt(0)
        End If
    End Sub
End Class
