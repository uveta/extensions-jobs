using Uveta.Extensions.Jobs.Abstractions.Serialization;
using Newtonsoft.Json;

namespace Uveta.Extensions.Serialization
{
    internal class JsonSerializer<T> : ISerializer<T>
    {
        private readonly JsonSerializerSettings _settings;

        public JsonSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
        }

        public T Deserialize(string value)
        {
            var instance = JsonConvert.DeserializeObject<T>(value, _settings);
            if (instance is null)
            {
                throw new JsonSerializationException(
                    $"Unable to deserialize input to {typeof(T).FullName}");
            }
            return instance;
        }

        public string Serialize(T instance)
        {
            return JsonConvert.SerializeObject(instance, _settings);
        }
    }
}