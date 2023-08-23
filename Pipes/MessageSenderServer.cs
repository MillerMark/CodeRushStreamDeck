﻿using System;
using System.IO.Pipes;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;

public class MessageSenderServer
{
    private volatile bool _shouldStop;
    int numClientsConnected;
    ConcurrentDictionary<NamedPipeServerStream, ConcurrentQueue<string>> messagesToSend = new ();


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
        Task.Factory.StartNew(async () =>
        {
            while (!_shouldStop)
            {
                var server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances);

                await server.WaitForConnectionAsync();
                numClientsConnected++;
                Console.WriteLine($"{pipeName} client #{numClientsConnected} listener connected");
                Console.WriteLine("");
                if (server.IsConnected)
                    Task.Run(() => HandleClient(server));
            }
        });
    }

    public void StopServer()
    {
        _shouldStop = true;
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
                    if (lastMessageAttempted != null)
                    {
                        await writer.WriteLineAsync(lastMessageAttempted);
                        writer.Flush();
                        lastMessageAttempted = null;
                    }
                    while (messagesToSend.ContainsKey(server) && messagesToSend[server].TryDequeue(out var message))  // dequeued messages to be sent
                    {
                        lastMessageAttempted = message;
                        await writer.WriteLineAsync(message);
                        writer.Flush();
                        lastMessageAttempted = null;
                    }
                }
                messagesToSend.TryRemove(server, out _);
            }
        }
        catch (IOException ex) when(ex.Message.Contains("Pipe is broken"))
        {
            // This can happen when the client is closed (e.g., Visual Studio instance shut down), which is normal.
            messagesToSend.TryRemove(server, out _);
        }
        catch (Exception ex)
        {
            messagesToSend.TryRemove(server, out _);
            Console.WriteLine($"Exception: {ex.Message}");
        }

        Console.WriteLine("Client disconnected");
    }
}

