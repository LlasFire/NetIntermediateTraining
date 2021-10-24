using NamedPipeWrapper;
using PipeCoreInfrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public class PipeClient
{
    private static readonly List<string> _preparedMesages = new List<string>
    {
        "Lorem ipsum dolor sit amet",
        "Platea dictumst quisque sagittis purus sit amet volutpat consequat.",
        "Metus vulputate eu scelerisque felis imperdiet proin fermentum leo vel.",
        "Arcu non sodales neque sodales ut etiam sit. Adipiscing elit ut aliquam purus sit amet luctus venenatis.",
        "Mi eget mauris pharetra et ultrices neque. Ornare quam viverra orci sagittis eu.",
        "Sed turpis tincidunt id aliquet."
    };

    private static readonly NamedPipeClient<byte[]> _client = GenerateNamedPipeClient();

    public static void Main()
    {

        var rand = new Random();
        var messageCount = rand.Next(3, 7);

        for (var index = 1; index < messageCount; index++)
        {
            var message = _preparedMesages[rand.Next(0, _preparedMesages.Count - 1)];

            _client.PushMessage(ReadWrite.Encode(message));
            Thread.Sleep(rand.Next(100, 2500));
        }

        Console.ReadKey();
        _client.Stop();
    }

    public static NamedPipeClient<byte[]> GenerateNamedPipeClient()
    {
        var client = new NamedPipeClient<byte[]>("MyServerPipe");
        client.ServerMessage += HandleServerMessage;
        client.Disconnected += Client_Disconnected;
        client.Start();

        return client;
    }

    private static void Client_Disconnected(NamedPipeConnection<byte[], byte[]> connection)
    {
        _client.Stop();
    }

    public static void HandleServerMessage(NamedPipeConnection<byte[], byte[]> conn, byte[] output)
    {
        var message = ReadWrite.Decode(output);
        var name = conn.Name;

        if (message.StartsWith("Client"))
        {
            Console.WriteLine(message);
        }
        else
        {
            Console.WriteLine($"{name} says: {message}");
        }
    }
}