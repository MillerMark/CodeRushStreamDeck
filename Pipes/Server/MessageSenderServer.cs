#nullable enable
using System;
using System.IO.Pipes;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using DevExpress.CodeRush.Platform.Diagnostics;
using System.Threading;

namespace Pipes.Server
{
    public class MessageSenderServer
    {
        private volatile bool shouldStop;
        int numClientsConnected;
        ConcurrentDictionary<NamedPipeServerStream, ConcurrentQueue<string>> messagesToSend = new();

        public bool LogToConsole { get; set; }


        readonly string pipeName;

        public void EnqueueMessage(string message)
        {
            foreach (NamedPipeServerStream server in messagesToSend.Keys)
                messagesToSend[server].Enqueue(message);
        }

        public MessageSenderServer(string pipeName)
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
                        Console.WriteLine($"{pipeName} client #{numClientsConnected} listener connected");
                        Console.WriteLine("");
                    }
                    if (server.IsConnected)
                        _ = Task.Run(() => HandleClient(server)).ConfigureAwait(false);
                }
            });
        }

        public void StopServer()
        {
            shouldStop = true;
        }

        private async Task HandleClient(NamedPipeServerStream server)
        {
            if (!messagesToSend.ContainsKey(server))
                messagesToSend.TryAdd(server, new ConcurrentQueue<string>());

            string? lastMessageAttempted = null;
            try
            {
                using (server)
                {
                    var writer = new StreamWriter(server);

                    while (server.IsConnected)
                    {
                        bool sentMessage = false;
                        if (lastMessageAttempted != null)
                        {
                            await writer.WriteLineAsync(lastMessageAttempted);
                            writer.Flush();
                            lastMessageAttempted = null;
                            sentMessage = true;
                        }
                        while (messagesToSend.ContainsKey(server) && messagesToSend[server].Count > 0 && messagesToSend[server].TryDequeue(out var message))  // dequeued messages to be sent
                        {
                            lastMessageAttempted = message;
                            await writer.WriteLineAsync(message);
                            writer.Flush();
                            lastMessageAttempted = null;
                            sentMessage = true;
                        }
                        if (!sentMessage)
                            await Task.Delay(30);
                    }
                    messagesToSend.TryRemove(server, out _);
                }
            }
            catch (IOException ex) when (ex.Message.Contains("Pipe is broken"))
            {
                // This can happen when the client is closed (e.g., Visual Studio instance shut down), which is normal.
                messagesToSend.TryRemove(server, out _);
            }
            catch (Exception ex)
            {
                messagesToSend.TryRemove(server, out _);
                Log.SendException(ex);
            }

            if (LogToConsole)
                Console.WriteLine("Client disconnected");
        }
    }
}



