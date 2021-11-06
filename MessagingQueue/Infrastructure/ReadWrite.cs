using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Infrastructure
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

        public static byte[] Encode<T>(T objectToSend)
        {
            var jsonObj = JsonConvert.SerializeObject(objectToSend);
            return Encoding.ASCII.GetBytes(jsonObj);
        }

        public static T Decode<T>(byte[] message)
        {
            var jsonString = Encoding.ASCII.GetString(message);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static T Decode<T>(string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }

        public static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput))
            {
                return false;
            }

            strInput = strInput.Trim();

            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}