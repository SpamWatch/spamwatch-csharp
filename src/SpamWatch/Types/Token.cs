using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpamWatch.Enums;
using SpamWatch.Types.Utils;

namespace SpamWatch.Types
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Token: TypeBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("permission")]
        [JsonConverter(typeof(PermissionConverter))]
        public Permissions Permission { get; set; }
        
        [JsonProperty("retired")]
        public bool Retired { get; set; }
        
        [JsonProperty("token")]
        public string ApiToken { get; set; }
        
        [JsonProperty("userid")]
        public long UserId { get; set; }
        
        public string SerializeObject()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}