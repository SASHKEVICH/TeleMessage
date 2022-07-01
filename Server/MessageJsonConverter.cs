using Newtonsoft.Json;

namespace Server
{
    public static class MessageJsonConverter<T>
    {
        public static string SerializeMessage(T message)
        {
            var messageString = JsonConvert.SerializeObject(message, Formatting.Indented);

            return messageString;
        }
    }
}

