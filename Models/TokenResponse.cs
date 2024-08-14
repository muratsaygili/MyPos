using System.Text.Json.Serialization;

namespace MyPosTest.Models;

public class TokenResponse
{
    [JsonPropertyName("expires_in")]
    public string ExpiresIn { get; set; }
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}


