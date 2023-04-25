using System;

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class JwtSettings
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int TokenExpirationInMinutes { get; set; }
}


public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}


