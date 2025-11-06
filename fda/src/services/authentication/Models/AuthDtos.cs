using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Authentication.Models
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public string? Organization { get; set; }
    }

    public class LoginRequest
    {
        // Either Email or PhoneNumber is required
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        // Either Password or OTP is required
        public string? Password { get; set; }

        public string? Otp { get; set; }

        // Login method indicator
        public LoginMethod LoginMethod { get; set; } = LoginMethod.EmailPassword;
    }

    public enum LoginMethod
    {
        EmailPassword,
        EmailOtp,
        PhonePassword,
        PhoneOtp
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public UserInfo User { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class UserInfo
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string Organization { get; set; }
        public List<string> Permissions { get; set; }
        public DateTime? LastLoginTime { get; set; }
    }

    public class ValidateTokenResponse
    {
        public bool IsValid { get; set; }
        public UserInfo? User { get; set; }
        public string? Error { get; set; }
    }

    public class UpdatePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }

    public class GenerateOtpRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        public OtpPurpose Purpose { get; set; }
    }

    public class VerifyOtpRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        public string Otp { get; set; }

        [Required]
        public OtpPurpose Purpose { get; set; }
    }

    public class GenerateOtpResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime ExpiresAt { get; set; }
        // In production, don't return OTP for security
        public string? OtpForTesting { get; set; }
    }

    public class VerifyOtpResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Error { get; set; }
    }

    public enum OtpPurpose
    {
        Login,
        PasswordReset,
        PhoneVerification,
        EmailVerification
    }
}