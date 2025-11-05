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
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public string? Organization { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Address? Address { get; set; }

        // Customer-specific fields
        public List<string>? DietaryPreferences { get; set; }
        public List<string>? FavoriteRestaurants { get; set; }

        // Employee-specific fields
        public EmployeeInfo? EmployeeInfo { get; set; }

        // Business-specific fields (for Biller role)
        public BusinessInfo? BusinessInfo { get; set; }

        // Delivery agent-specific fields
        public DeliveryInfo? DeliveryInfo { get; set; }

        // IT technician-specific fields
        public TechInfo? TechInfo { get; set; }
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public string Organization { get; set; }
        public List<string> Permissions { get; set; }
        public DateTime? LastLoginTime { get; set; }

        // Role-specific info (only populated if relevant to the user's role)
        public EmployeeInfo? EmployeeInfo { get; set; }
        public BusinessInfo? BusinessInfo { get; set; }
        public DeliveryInfo? DeliveryInfo { get; set; }
        public TechInfo? TechInfo { get; set; }
    }

    public class ValidateTokenResponse
    {
        public bool IsValid { get; set; }
        public UserInfo? User { get; set; }
        public string? Error { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public Address? Address { get; set; }
        public List<string>? DietaryPreferences { get; set; }
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