using Newtonsoft.Json;

namespace SpamWatch.Types.ExceptionTypes
{
    public class BadRequest
    {
        [JsonProperty("code")] 
        public int Code;
        
        [JsonProperty("error")] 
        public string Error;
        
        [JsonProperty("reason")] 
        public string Reason;
    }
}