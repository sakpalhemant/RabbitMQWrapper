using Newtonsoft.Json;

namespace RabbitMQWrapper.Serializer
{
    public class NewtonSoftSerializerAdapter : AbstractSerializerAdapter
    {
        private JsonSerializerSettings settings;

        public NewtonSoftSerializerAdapter(JsonSerializerSettings settings)
        {
            this.settings = settings;
        }

        public override T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public override string Serialize<T>(T o)
        {
            return JsonConvert.SerializeObject(o, settings);
        }
    }
}