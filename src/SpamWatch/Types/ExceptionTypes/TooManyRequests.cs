using Newtonsoft.Json;

namespace SpamWatch.Types.ExceptionTypes
{
    public class TooManyRequests
    {
        [JsonProperty("code")] 
        public int Code;
        
        [JsonProperty("error")] 
        public string Error;

        [JsonProperty("until")] 
        public long Until;
    }
}