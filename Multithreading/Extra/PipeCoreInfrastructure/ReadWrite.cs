using System.Text;

namespace PipeCoreInfrastructure
{
    public class ReadWrite
    {
        public static byte[] Encode(string message)
        {
            return Encoding.ASCII.GetBytes(message);
        }

        public static string Decode(byte[] message)
        {
            return Encoding.ASCII.GetString(message);
        }
    }
}