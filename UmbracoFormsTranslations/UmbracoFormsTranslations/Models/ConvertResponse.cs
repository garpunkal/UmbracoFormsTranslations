using Newtonsoft.Json;

namespace UmbracoFormsTranslations.Models
{
    public class ConvertResponse
    {
        public ConvertResponse(string message)
        {
            Message = message;
        }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; private set; }

        [JsonProperty(PropertyName = "resultType")]
        public Enums.ResultType ResultType { get; set; }
    }
}