#nullable enable
using DevExpress.CodeRush.Platform.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

public class MessageReceiverServer
{
    private volatile bool shouldStop;
    int numClientsConnected;
    readonly string pipeName;
    public event EventHandler<string>? MessageReceived;
    public bool LogToConsole { get; set; }

    public MessageReceiverServer(string pipeName)
    {
        this.pipeName = pipeName;
    }

    public void Start()
    {
        Task.Run(async () =>
        {
            while (!shouldStop)
            {
                var server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances);

                await server.WaitForConnectionAsync();
                numClientsConnected++;
                if (LogToConsole)
                {
                    Console.WriteLine($"{pipeName} client #{numClientsConnected} sender connected");
                    Console.WriteLine("");
                }
                if (server.IsConnected)
                    HandleClient(server);
            }
        });
    }

    public void StopServer()
    {
        shouldStop = true;
    }

    private async void HandleClient(NamedPipeServerStream server)
    {
        try
        {
            using (server)
            {
                var reader = new StreamReader(server);

                while (true)
                {
                    var line = await reader.ReadLineAsync();

                    if (line == null) // This indicates that the client has disconnected
                    {
                        break;
                    }
                    else
                    {
                        MessageReceived?.Invoke(this, line);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.SendException(ex);
        }

        if (LogToConsole)
            Console.WriteLine("Client disconnected");
    }
}

