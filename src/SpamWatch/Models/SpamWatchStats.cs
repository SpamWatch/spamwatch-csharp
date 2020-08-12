using Newtonsoft.Json;

namespace SpamWatch.Models
{
    public class SpamWatchStats
    {
        [JsonProperty("total_ban_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalBanCount { get; set; }
    }
}