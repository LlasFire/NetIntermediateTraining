using Infrastructure;
using Infrastructure.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace QueueClient
{
    internal class Program
    {
        private static readonly IModel _fileSendChannel = MessageClient.GenerateFileChannel();
        private static readonly IModel _healthCheckChannel = MessageClient.GenerateHeathCheckChannel();
        private static readonly string _pathToFolder = @$"{Directory.GetCurrentDirectory()}/ListeningMaterials";

        static void Main()
        {
            (var fileSender, var checker) = GenerateInitialModels();
            var cancellationSource = new CancellationTokenSource();
            var token = cancellationSource.Token;
            HandleHealthChecking(checker, token);

            var settingsChannel = SetupSettingsListening(fileSender, checker);

            checker.Status = StatusEnum.Busy;
            _fileSendChannel.BasicPublish(exchange: "",
                                    routingKey: "hello",
                                    basicProperties: null,
                                    body: ReadWrite.Encode("Hi everyone!"));

            checker.Status = StatusEnum.Free;

            using var watcher = FileWatcher.GenerateWatcher(_pathToFolder, fileSender, checker);

            Console.WriteLine("Please setup the file filter or press Q to exit.");
            var input = Console.ReadLine();

            while (input.Trim().ToUpper() != "Q")
            {
                watcher.Filter = input;
                CheckAndSendFilesByFilter(input, fileSender, checker);

                input = Console.ReadLine();
            }

            cancellationSource.Cancel();
            TearDownAllChannels(settingsChannel);
            Console.WriteLine("The client was closed.");
        }

        private static IModel SetupSettingsListening(FileSender fileSender, HealthCheckModel checker)
        {
            (var settingsChannel, var settingsChannelName) = MessageClient.GenerateSettingsChannel();

            var consumer = new EventingBasicConsumer(settingsChannel);

            consumer.Received += (obj, args) =>
            {
                var byteArray = args.Body.ToArray();
                var message = ReadWrite.Decode(byteArray);

                if (ReadWrite.IsValidJson(message))
                {
                    var newSettings = ReadWrite.Decode<SettingsModel>(message);

                    fileSender.MaxMessageSize = newSettings.MaxMessageSize;
                    checker.MaxMessageSize = newSettings.MaxMessageSize;

                    SendHealthCheckMessage(checker);
                }
            };

            settingsChannel.BasicConsume(queue: settingsChannelName, autoAck: true, consumer: consumer);

            return settingsChannel;
        }

        private static void HandleHealthChecking(HealthCheckModel checker, CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    SendHealthCheckMessage(checker);
                    Thread.Sleep(5000);
                }
            }, token);
        }


        private static void SendHealthCheckMessage(HealthCheckModel checker)
        {
            _healthCheckChannel.BasicPublish(exchange: "",
                                                routingKey: "healthcheck",
                                                basicProperties: null,
                                                body: ReadWrite.Encode(checker));
        }

        private static (FileSender fileSender, HealthCheckModel checker) GenerateInitialModels()
        {
            var fileSender = new FileSender
            {
                Channel = _fileSendChannel,
                MaxMessageSize = new SettingsModel().MaxMessageSize,
            };

            var rand = new Random();

            var checker = new HealthCheckModel
            {
                MaxMessageSize = fileSender.MaxMessageSize,
                ClientName = $"Client_{rand.Next(1, int.MaxValue)}",
                Status = StatusEnum.Free
            };

            return (fileSender, checker);
        }

        private static void TearDownAllChannels(IModel settingsChannel)
        {
            _fileSendChannel.Close();
            _fileSendChannel.Dispose();

            _healthCheckChannel.Close();
            _healthCheckChannel.Dispose();

            settingsChannel.Close();
            settingsChannel.Dispose();
        }

        private static void CheckAndSendFilesByFilter(string input, FileSender fileSender, HealthCheckModel checker)
        {
            var files = Directory.GetFiles(_pathToFolder, input);

            foreach (var fileName in files)
            {
                fileSender.SendFileInQueue(fileName, checker);
            }
        }
    }
}
