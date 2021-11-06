using NamedPipeWrapper;
using PipeCoreInfrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

    public static void Main()
    {
        for (var i = 0; i < 5; i++)
        {
            HandleSeparateClientInTask();
            Thread.Sleep(3000);
        }

        Console.ReadKey();
    }

    private static void HandleSeparateClientInTask()
    {
        static void GenerateClient()
        {
            var client = GenerateNamedPipeClient();
            var rand = new Random();
            var messageCount = rand.Next(3, 7);

            for (var index = 1; index < messageCount; index++)
            {
                var message = _preparedMesages[rand.Next(0, _preparedMesages.Count - 1)];

                client.PushMessage(ReadWrite.Encode(message));
                Thread.Sleep(rand.Next(100, 2500));
            }
        }

        Task.Factory.StartNew(GenerateClient);
    }

    public static NamedPipeClient<byte[]> GenerateNamedPipeClient()
    {
        var client = new NamedPipeClient<byte[]>("MyServerPipe");
        client.ServerMessage += HandleServerMessage;
        client.Disconnected += (connection) => { client.Stop(); };
        client.Start();

        return client;
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