using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SpamWatch.Types
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Stats : TypeBase
    {
        [JsonProperty("total_ban_count")]
        public long TotalBanCount { get; }
        
        public Stats DeserializeObject(string jsonBody)
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            return JsonConvert.DeserializeObject<Stats>(jsonBody, jsonSerializerSettings);
        }
    }
}