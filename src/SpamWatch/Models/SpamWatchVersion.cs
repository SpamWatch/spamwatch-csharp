using Newtonsoft.Json;

namespace SpamWatch.Models
{
    public class SpamWatchVersion
    {
        [JsonProperty("major", NullValueHandling = NullValueHandling.Ignore)]
        public long? Major { get; set; }

        [JsonProperty("minor", NullValueHandling = NullValueHandling.Ignore)]
        public long? Minor { get; set; }

        [JsonProperty("patch", NullValueHandling = NullValueHandling.Ignore)]
        public long? Patch { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }
    }
}