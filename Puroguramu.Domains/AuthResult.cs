namespace Puroguramu.Domains;

public enum AuthResult
{
    Success,
    Failed,
    Lockout,
    RequiresTwoFactor,
}
