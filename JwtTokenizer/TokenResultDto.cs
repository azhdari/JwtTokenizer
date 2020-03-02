using Newtonsoft.Json;

namespace JwtTokenizer
{
    public class TokenResultDto
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
