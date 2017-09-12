Module Main
    Public Active As Boolean = True

    Public Instances As New List(Of ServiceInstance)
    Public TotalBytesRecv As Integer = 0
    Public TotalBytesSent As Integer = 0
    Public Const Port As Integer = 65452

    Sub Main()
        Console.ForegroundColor = ConsoleColor.White
        For i As Integer = 0 To 9
            Instances.Add(New ServiceInstance(Port + i))
        Next
        For Each inst In Instances
            inst.RunWorkerAsync()
        Next
        While Active
            If GC.GetTotalMemory(False) > 1000000 Then
                GC.Collect()
            End If

            Console.Clear()
            Console.WriteLine("[" & DateTime.Now & "]: (Memory Usage: " & GC.GetTotalMemory(False) / 1000 & "KB, Total Network Data: " & (TotalBytesRecv + TotalBytesSent) / 1000 & "KB)")
            For Each Service In Instances
                Console.Write(vbTab & "Server: " & Service.ServerInst.PortID & vbTab & "Active Connections: " & Service.ServerInst.Connections & vbTab & "Pings per Tick: " & Service.ServerInst.Pings & vbTab & "Lag Ratio: " & Service.ServerInst.Lag & Environment.NewLine)
            Next
            Console.Write(Environment.NewLine)
            Dim input As String = ReadLineWithTimeOut(1000)
            If Not input = "" Then
                Try
                    RunCommand(input.ToLower)
                Catch ex As Exception
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine(ex.Message)
                    Console.ForegroundColor = ConsoleColor.White
                    Threading.Thread.Sleep(2000)
                End Try
            End If
        End While
        For Each inst In Instances
            If inst.Equals(Instances.Last) Then
                Console.WriteLine(" └ " & "Stopping Server: " & inst.ServerInst.PortID)
            Else
                Console.WriteLine(" ├ " & "Stopping Server: " & inst.ServerInst.PortID)
            End If
            inst.ServerInst.Server.Stop()
            Threading.Thread.Sleep(1000)
            inst.Dispose()
            GC.Collect()
        Next
        Console.WriteLine("Done." + Environment.NewLine + Environment.NewLine + "Press any key to continue...")
        Console.ReadKey()
    End Sub

    Sub RunCommand(command As String)
        If command = "stop" Then
            Console.WriteLine("Stopping Services...")
            Active = False
        ElseIf command.Split(" ").First = "recv" Then
            Dim client As New Net.Sockets.TcpClient()
            client.Connect(New Net.IPEndPoint(Net.IPAddress.Loopback, Convert.ToInt32(command.Split(" ").ElementAt(1))))
            SendString(client, command.Split(" ").Last)
            Threading.Thread.Sleep(100)
            While client.Connected
                Console.WriteLine("Client Output: '" & ReadString(client) & "'")
            End While
            client.Dispose()
            Console.WriteLine("Done." + Environment.NewLine + Environment.NewLine + "Press any key to continue...")
            Console.ReadKey()
        End If
    End Sub

    Public Function ReadLineWithTimeOut(timeOutMS As Integer) As String
        Dim timeoutvalue As DateTime = DateTime.Now.AddMilliseconds(timeOutMS)
        Dim input As String = ""
        While DateTime.Now < timeoutvalue
            If Console.KeyAvailable Then
                Dim key As ConsoleKeyInfo = Console.ReadKey()
                If key.Key = ConsoleKey.Enter Then
                    Return input
                ElseIf key.Key = ConsoleKey.Backspace Then
                    input.Remove(input.Length - 1, 1)
                Else
                    input += key.KeyChar
                End If
                timeoutvalue = DateTime.Now.AddMilliseconds(60000)
            Else
                System.Threading.Thread.Sleep(10)
            End If
        End While
        Return ""
    End Function

    Public Sub SendString(Client As Net.Sockets.TcpClient, str As String)
        TotalBytesSent += System.Text.Encoding.ASCII.GetBytes(str).Length
        Client.GetStream.Write(System.Text.Encoding.ASCII.GetBytes(str), 0, System.Text.Encoding.ASCII.GetBytes(str).Length)
    End Sub

    Public Function ReadString(Client As Net.Sockets.TcpClient) As String
        Dim buffer As New List(Of Byte)
        Dim pos As Integer = 0
        While Client.GetStream.DataAvailable And Client.Available > 0
            Try
                Client.GetStream.Read(buffer.ToArray, pos, 1)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            End Try
            pos += 1
        End While
        TotalBytesRecv += buffer.Count
        Return System.Text.Encoding.ASCII.GetChars(buffer.ToArray)
    End Function

    Public Function GetPlayerNameFromString(str As String) As Integer
        str = str.Replace("{", "").Replace("}", "")
        Return str.Split(":").ElementAt(0)
    End Function

    Public Function GetPlayerIPFromString(str As String) As String
        Return str.Split(":").ElementAt(2)
    End Function
End Module

Public Class ServerInstance
    Public Server As Net.Sockets.TcpListener

    Public Lag As Integer = 0
    Public Connections As Integer = 0
    Public Pings As Integer = 0

    Public Shared Players As New List(Of String)

    Public Shared Function Contains(ip As String) As String
        For Each line In Players
            If line.Contains(ip) Then
                Return line
            End If
        Next
        Return ""
    End Function

    Public PortID As Integer = 404

    Sub New(port As Integer)
        Server = New Net.Sockets.TcpListener(Net.IPAddress.Any, port)
        PortID = port
        Server.Start()
    End Sub

    Sub ServerLoop()
        Dim BusyCount As Integer = 0
        While Server.Pending
            Dim Client As Net.Sockets.TcpClient = Server.AcceptTcpClient
            Do Until Client.Available > 0
                Threading.Thread.Sleep(50)
            Loop

            Dim CharData As String = ReadString(Client)
            Dim OldData As String = ServerInstance.Contains(GetPlayerIPFromString(CharData))
            If Not CharData = "" Then
                Players.Remove(OldData)
                Players.Add(CharData)
            Else
                Players.Add(CharData)
            End If

            For Each player In ServerInstance.Players
                SendString(Client, player)
            Next
            Client.Close()
            BusyCount += 1
        End While

        Connections = Players.Count
        Pings = BusyCount
        If Players.Count > 0 Then
            Lag = (BusyCount / Players.Count)
        Else
            Lag = 0
        End If
    End Sub
End Class

Public Class ServiceInstance
    Inherits System.ComponentModel.BackgroundWorker

    Public ServerInst As ServerInstance

    Sub New(port)
        ServerInst = New ServerInstance(port)
    End Sub

    Sub ServiceLoop() Handles MyBase.DoWork
        While Active
            ServerInst.ServerLoop()
            GC.Collect()
        End While
    End Sub
End Class