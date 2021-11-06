using Infrastructure;
using Infrastructure.Models;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Threading;

namespace QueueClient
{
    internal class FileSender
    {
        public IModel Channel { get; set; }

        public int MaxMessageSize { get; set; }

        public void SendFileInQueue(string fullPath, HealthCheckModel checker)
        {
            var numberOfChecking = 0;

            while (!IsFileReadyToRead(fullPath))
            {
                numberOfChecking++;
                Thread.Sleep(200);

                if (numberOfChecking > 10)
                {
                    checker.Status = StatusEnum.Error;
                    throw new IOException("Cannot open the file.");
                }
            }

            checker.Status = StatusEnum.Busy;
            var info = new FileInfo(fullPath);
            var file = File.ReadAllBytes(fullPath);

            var fileName = info.Name;
            var uniqueId = Guid.NewGuid();

            var messageOrder = 0;

            foreach (var message in file.Split(MaxMessageSize))
            {
                var rabbitMessage = new MessageModel
                {
                    FileId = uniqueId,
                    MessageOrderNumber = messageOrder++,
                    Message = message,
                };

                Channel.BasicPublish(exchange: "",
                                    routingKey: "hello",
                                    basicProperties: null,
                                    body: ReadWrite.Encode(rabbitMessage));

                Console.WriteLine($" [x] Sent message for file {fileName} #{messageOrder}");
            }

            var lastMessage = new MessageModel
            {
                FileId = uniqueId,
                MessageOrderNumber = 0,
                FileName = fileName,
                IsLastPart = true
            };

            Channel.BasicPublish(exchange: "",
                                routingKey: "hello",
                                basicProperties: null,
                                body: ReadWrite.Encode(lastMessage));

            Console.WriteLine($" [x] Sent last message for file {fileName}");

            checker.Status = StatusEnum.Free;
        }

        private static bool IsFileReadyToRead(string path)
        {
            try
            {
                File.Open(path, FileMode.Open, FileAccess.Read).Dispose();
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
