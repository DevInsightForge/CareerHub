namespace CareerHub.Application.Configurations;

public class JwtSettings
{
    public string SecretKey { get; set; } = "Default_Super_Secret_256_Bits_Signing_Key";
    public string ValidIssuer { get; set; } = "DefaultIssuer";
    public string ValidAudience { get; set; } = "DefaultAudience";
    public bool ValidateIssuer { get; set; } = false;
    public bool ValidateAudience { get; set; } = false;
    public double AccessTokenExpirationInMinutes { get; set; } = 60;
}
