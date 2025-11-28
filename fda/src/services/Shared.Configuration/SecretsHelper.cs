namespace Shared.Configuration;

/// <summary>
/// Helper class for reading Docker secrets and configuration values
/// </summary>
public static class SecretsHelper
{
    /// <summary>
    /// Reads a secret from Docker secrets file or falls back to environment variable
    /// </summary>
    /// <param name="configuration">The IConfiguration instance</param>
    /// <param name="secretName">Name of the secret (matches filename in /run/secrets/)</param>
    /// <param name="defaultValue">Optional default value if secret is not found</param>
    /// <returns>The secret value</returns>
    /// <exception cref="InvalidOperationException">Thrown when secret is not found and no default provided</exception>
    public static string GetSecret(IConfiguration configuration, string secretName, string? defaultValue = null)
    {
        // Try to read from Docker secret file first
        var secretPath = $"/run/secrets/{secretName}";
        if (File.Exists(secretPath))
        {
            return File.ReadAllText(secretPath).Trim();
        }
        
        // Fallback to environment variable (for local development)
        // Convert secret_name to ENVIRONMENT_VARIABLE format
        var envVarName = secretName.Replace("_", "__").ToUpper();
        var envValue = configuration[envVarName];
        if (!string.IsNullOrEmpty(envValue))
        {
            return envValue;
        }
        
        // Also try the original secret name as environment variable
        envValue = configuration[secretName];
        if (!string.IsNullOrEmpty(envValue))
        {
            return envValue;
        }
        
        // Use default if provided
        if (defaultValue != null)
        {
            return defaultValue;
        }
        
        throw new InvalidOperationException($"Secret '{secretName}' not found in Docker secrets or environment variables");
    }
    
    /// <summary>
    /// Checks if a secret exists
    /// </summary>
    public static bool SecretExists(IConfiguration configuration, string secretName)
    {
        var secretPath = $"/run/secrets/{secretName}";
        if (File.Exists(secretPath))
        {
            return true;
        }
        
        var envVarName = secretName.Replace("_", "__").ToUpper();
        return !string.IsNullOrEmpty(configuration[envVarName]) || 
               !string.IsNullOrEmpty(configuration[secretName]);
    }
}
