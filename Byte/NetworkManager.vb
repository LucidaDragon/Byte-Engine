Public Class NetworkManager
    Private Client As Net.Sockets.TcpClient

    Private Sub SendString(str As String)
        Client.GetStream.Write(System.Text.Encoding.ASCII.GetBytes(str), 0, System.Text.Encoding.ASCII.GetBytes(str).Length)
    End Sub

    Private Function ReadString() As String
        Dim buffer(1000000000) As Byte
        Dim pos As Integer = 0
        While Client.GetStream.DataAvailable
            Client.GetStream.Read(buffer, pos, 1)
            pos += 1
        End While
        Return System.Text.Encoding.ASCII.GetChars(buffer)
    End Function

    Public Sub SyncCharacterData(server As Net.IPAddress)
        Client.Connect(New Net.IPEndPoint(server, 65452))
        SendString("Fetch data.")
    End Sub
End Class
