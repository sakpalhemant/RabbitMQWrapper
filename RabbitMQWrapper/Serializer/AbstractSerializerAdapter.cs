using System.Text;

namespace RabbitMQWrapper.Serializer
{
    public abstract class AbstractSerializerAdapter
    {
        public abstract string Serialize<T>(T o);

        public abstract T Deserialize<T>(string json);

        public byte[] SerializeBytes(object value)
        {
            return Encoding.UTF8.GetBytes(this.Serialize(value));
        }

        public T DeserializeBytes<T>(byte[] bytes)
        {
            return this.Deserialize<T>(Encoding.UTF8.GetString(bytes));
        }
    }
}