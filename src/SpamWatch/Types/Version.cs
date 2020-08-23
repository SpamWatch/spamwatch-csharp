using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SpamWatch.Types
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Version: TypeBase
    {
        [JsonProperty("major")]
        public long? Major { get; }

        [JsonProperty("minor")]
        public long? Minor { get; }

        [JsonProperty("patch")]
        public long? Patch { get; }

        [JsonProperty("version")]
        public string ApiVersion { get; }
        
        public Version DeserializeObject(string jsonBody)
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            return JsonConvert.DeserializeObject<Version>(jsonBody, jsonSerializerSettings);
        }
    }
}