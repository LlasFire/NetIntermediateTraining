using Infrastructure;
using Infrastructure.Models;
using Microsoft.Extensions.Caching.Memory;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QueueServer
{
    internal class Program
    {
        private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private static readonly IModel _fileSendChannel = MessageClient.GenerateFileChannel();
        private static readonly IModel _healthCheckChannel = MessageClient.GenerateHeathCheckChannel();
        private static string _pathToFolder = @$"{Directory.GetCurrentDirectory()}\ReceivedMaterials\";

        static void Main()
        {
            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(_fileSendChannel);
            consumer.Received += HandleMessageFromFileChannel;

            _fileSendChannel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

            var healthCheckConsumer = new EventingBasicConsumer(_healthCheckChannel);
            healthCheckConsumer.Received += HandleHealthCheckMessage;

            _healthCheckChannel.BasicConsume(queue: "healthcheck", autoAck: true, consumer: healthCheckConsumer);

            SetupSettingsForClients();

            TearDownAllChannels();
            Console.WriteLine("The server was closed.");
        }

        private static void TearDownAllChannels()
        {
            _fileSendChannel.Close();
            _fileSendChannel.Dispose();

            _healthCheckChannel.Close();
            _healthCheckChannel.Dispose();
        }

        private static void SetupSettingsForClients()
        {
            Console.WriteLine("Please setup the maximum message size (Kbyte) or press Q to exit.");
            var input = Console.ReadLine();

            (var settingsChannel, _) = MessageClient.GenerateSettingsChannel();

            while (input.Trim().ToUpper() != "Q")
            {
                if (int.TryParse(input, out int value) &&
                    value > 0 &&
                    (long)value * 1024 < int.MaxValue)
                {
                    var settings = new SettingsModel
                    {
                        MaxMessageSize = value * 1024,
                    };

                    settingsChannel.BasicPublish(exchange: "settings",
                                                routingKey: "",
                                                basicProperties: null,
                                                body: ReadWrite.Encode(settings));
                }
                else
                {
                    if (value < 0)
                    {
                        Console.WriteLine("Value cannot be less than 0. Please try again.");
                    }
                    else
                    {
                        Console.WriteLine("We cannot match entered value as integer, or value is very big. Please try again.");
                    }
                }

                input = Console.ReadLine();
            }

            settingsChannel.Close();
            settingsChannel.Dispose();
        }

        private static void HandleHealthCheckMessage(object sender, BasicDeliverEventArgs e)
        {
            var byteArray = e.Body.ToArray();
            var message = ReadWrite.Decode(byteArray);

            if (ReadWrite.IsValidJson(message))
            {
                var healthNotification = ReadWrite.Decode<HealthCheckModel>(message);

                HandleHealthCheckModel(healthNotification);
            }
        }

        private static void HandleHealthCheckModel(HealthCheckModel healthNotification)
        {
            Console.WriteLine($"{healthNotification.ClientName} in status {healthNotification.Status}.\r\nMaximum message size for it - {healthNotification.MaxMessageSize / 1024} Kbyte");
        }

        private static void HandleMessageFromFileChannel(object model, BasicDeliverEventArgs ea)
        {
            var byteArray = ea.Body.ToArray();
            var message = ReadWrite.Decode(byteArray);

            if (ReadWrite.IsValidJson(message))
            {
                var fileModel = ReadWrite.Decode<MessageModel>(message);

                HandleFileModel(fileModel);
            }

            Console.WriteLine($" [x] Received {message}");
        }

        private static void HandleFileModel(MessageModel model)
        {
            var fileStoreBody = new SortedDictionary<int, List<byte>>();
            var fileKey = model.FileId.ToString();

            if (!_cache.TryGetValue(fileKey, out fileStoreBody))
            {
                fileStoreBody = new SortedDictionary<int, List<byte>>
                {
                    { model.MessageOrderNumber, model.Message.ToList()}
                };

                _cache.CreateEntry(fileKey);
                _cache.Set(fileKey, fileStoreBody);
            }

            if (!model.IsLastPart)
            {
                fileStoreBody.TryAdd(model.MessageOrderNumber, model.Message.ToList());
                _cache.Set(fileKey, fileStoreBody);
            }
            else
            {
                var resultContent = fileStoreBody.SelectMany(item => item.Value).ToList();

                using var stream = File.Create(_pathToFolder + model.FileName, resultContent.Count);

                if (stream.CanWrite)
                {
                    stream.Write(resultContent.ToArray(), 0, resultContent.Count - 1);
                    stream.Flush();
                }

                _cache.Remove(fileKey);
            }

        }
    }
}
