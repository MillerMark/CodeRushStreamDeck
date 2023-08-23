using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

public class MessageReceiverServer
{
    private volatile bool _shouldStop;
    int numClientsConnected;
    readonly string pipeName;

    public MessageReceiverServer(string pipeName)
    {
        this.pipeName = pipeName;
    }

    public void Start()
    {
        Task.Factory.StartNew(async () =>
        {
            while (!_shouldStop)
            {
                var server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances);

                await server.WaitForConnectionAsync();
                numClientsConnected++;
                Console.WriteLine($"{pipeName} client #{numClientsConnected} sender connected");
                Console.WriteLine("");
                if (server.IsConnected)
                    HandleClient(server);
            }
        });
    }

    public void StopServer()
    {
        _shouldStop = true;
    }

    private async void HandleClient(NamedPipeServerStream server)
    {
        try
        {
            using (server)
            {
                var reader = new StreamReader(server);
                //var writer = new StreamWriter(server);

                //await writer.WriteLineAsync("Server says hello!");
                //writer.Flush();

                while (true)
                {
                    var line = await reader.ReadLineAsync();

                    if (line == null) // This indicates that the client has disconnected
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Received from client: " + line);
                    }

                    //while (_messagesToSend.TryDequeue(out var message))  // dequeued messages to be sent
                    //{
                    //    await writer.WriteLineAsync(message);
                    //    writer.Flush();
                    //}
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        Console.WriteLine("Client disconnected");
    }
}

