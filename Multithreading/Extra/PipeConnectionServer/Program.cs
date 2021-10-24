using NamedPipeWrapper;
using PipeCoreInfrastructure;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace PipeConnectionServer
{
    public class PipeServer
    {
        public static ConcurrentBag<NamedPipeConnection<byte[], byte[]>> _connections = new ConcurrentBag<NamedPipeConnection<byte[], byte[]>>();
        public static ConcurrentQueue<(string, string)> _messageStorage = new ConcurrentQueue<(string, string)>();

        public static void Main()
        {
            var server = new NamedPipeServer<byte[]>("MyServerPipe");
            server.ClientConnected += HandleClientConnection;
            server.ClientMessage += HandleClientMessage;

            server.Start();

            Console.WriteLine("For ending session please press 'X' ");
            
            while(true)
            {
                var input = Console.ReadLine();

                if (string.Equals(input, "x", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            if (_connections.Any())
            {
                foreach (var connection in _connections)
                {
                    connection.PushMessage(ReadWrite.Encode("Server was stopped."));
                }
            }

            server.Stop();
        }

        public static void HandleClientConnection(NamedPipeConnection<byte[], byte[]> conn)
        {
            Console.WriteLine($"{conn.Name} is now connected!");

            if (!_connections.Any(item => item.Id == conn.Id))
            {
                _connections.Add(conn);
            }

            conn.PushMessage(ReadWrite.Encode("Hello, guys!"));

            if (_messageStorage.Any())
            {
                var messages = _messageStorage.ToArray();

                foreach (var item in messages)
                {
                    conn.PushMessage(ReadWrite.Encode($"{item.Item1} says {item.Item2}"));
                    Thread.Sleep(50);
                }
            }
        }

        public static void HandleClientMessage(NamedPipeConnection<byte[], byte[]> conn, byte[] message)
        {
            if (_connections.Any(item => item.Id != conn.Id))
            {
                var resendList = _connections.Where(item => item.Id != conn.Id).ToList();

                foreach (var resendConn in resendList)
                {
                    resendConn.PushMessage(message);
                }
            }

            var messageString = ReadWrite.Decode(message);

            _messageStorage.Enqueue((conn.Name, messageString));

            if (_messageStorage.Count > 20)
            {
                var __ = _messageStorage.TryDequeue(out _);
            }
        }
    }
}
