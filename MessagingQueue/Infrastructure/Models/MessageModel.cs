using System;

namespace Infrastructure.Models
{
    public class MessageModel
    {
        public Guid FileId { get; set; }

        public string FileName { get; set; }

        public int MessageOrderNumber { get; set; }

        public byte[] Message { get; set; }

        public bool IsLastPart { get; set; }
    }
}
