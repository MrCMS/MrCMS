using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserManager
    {
        void Dispose();
        Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType);
        Task<IdentityResult> CreateAsync(User user);
        Task<IdentityResult> UpdateAsync(User user);
        Task<IdentityResult> DeleteAsync(User user);
        Task<User> FindByIdAsync(int userId);
        Task<User> FindByNameAsync(string userName);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<User> FindAsync(string userName, string password);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> HasPasswordAsync(int userId);
        Task<IdentityResult> AddPasswordAsync(int userId, string password);
        Task<IdentityResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<IdentityResult> RemovePasswordAsync(int userId);
        Task<string> GetSecurityStampAsync(int userId);
        Task<IdentityResult> UpdateSecurityStampAsync(int userId);
        Task<string> GeneratePasswordResetTokenAsync(int userId);
        Task<IdentityResult> ResetPasswordAsync(int userId, string token, string newPassword);
        Task<User> FindAsync(UserLoginInfo login);
        Task<IdentityResult> RemoveLoginAsync(int userId, UserLoginInfo login);
        Task<IdentityResult> AddLoginAsync(int userId, UserLoginInfo login);
        Task<IList<UserLoginInfo>> GetLoginsAsync(int userId);
        Task<IdentityResult> AddClaimAsync(int userId, Claim claim);
        Task<IdentityResult> RemoveClaimAsync(int userId, Claim claim);
        Task<IList<Claim>> GetClaimsAsync(int userId);
        Task<IdentityResult> AddToRoleAsync(int userId, string role);
        Task<IdentityResult> AddToRolesAsync(int userId, params string[] roles);
        Task<IdentityResult> RemoveFromRolesAsync(int userId, params string[] roles);
        Task<IdentityResult> RemoveFromRoleAsync(int userId, string role);
        Task<IList<string>> GetRolesAsync(int userId);
        Task<bool> IsInRoleAsync(int userId, string role);
        Task<string> GetEmailAsync(int userId);
        Task<IdentityResult> SetEmailAsync(int userId, string email);
        Task<User> FindByEmailAsync(string email);
        Task<string> GenerateEmailConfirmationTokenAsync(int userId);
        Task<IdentityResult> ConfirmEmailAsync(int userId, string token);
        Task<bool> IsEmailConfirmedAsync(int userId);
        Task<string> GetPhoneNumberAsync(int userId);
        Task<IdentityResult> SetPhoneNumberAsync(int userId, string phoneNumber);
        Task<IdentityResult> ChangePhoneNumberAsync(int userId, string phoneNumber, string token);
        Task<bool> IsPhoneNumberConfirmedAsync(int userId);
        Task<string> GenerateChangePhoneNumberTokenAsync(int userId, string phoneNumber);
        Task<bool> VerifyChangePhoneNumberTokenAsync(int userId, string token, string phoneNumber);
        Task<bool> VerifyUserTokenAsync(int userId, string purpose, string token);
        Task<string> GenerateUserTokenAsync(string purpose, int userId);
        void RegisterTwoFactorProvider(string twoFactorProvider, IUserTokenProvider<User, int> provider);
        Task<IList<string>> GetValidTwoFactorProvidersAsync(int userId);
        Task<bool> VerifyTwoFactorTokenAsync(int userId, string twoFactorProvider, string token);
        Task<string> GenerateTwoFactorTokenAsync(int userId, string twoFactorProvider);
        Task<IdentityResult> NotifyTwoFactorTokenAsync(int userId, string twoFactorProvider, string token);
        Task<bool> GetTwoFactorEnabledAsync(int userId);
        Task<IdentityResult> SetTwoFactorEnabledAsync(int userId, bool enabled);
        Task SendEmailAsync(int userId, string subject, string body);
        Task SendSmsAsync(int userId, string message);
        Task<bool> IsLockedOutAsync(int userId);
        Task<IdentityResult> SetLockoutEnabledAsync(int userId, bool enabled);
        Task<bool> GetLockoutEnabledAsync(int userId);
        Task<DateTimeOffset> GetLockoutEndDateAsync(int userId);
        Task<IdentityResult> SetLockoutEndDateAsync(int userId, DateTimeOffset lockoutEnd);
        Task<IdentityResult> AccessFailedAsync(int userId);
        Task<IdentityResult> ResetAccessFailedCountAsync(int userId);
        Task<int> GetAccessFailedCountAsync(int userId);
        IPasswordHasher PasswordHasher { get; set; }
        IIdentityValidator<User> UserValidator { get; set; }
        IIdentityValidator<string> PasswordValidator { get; set; }
        IClaimsIdentityFactory<User, int> ClaimsIdentityFactory { get; set; }
        IIdentityMessageService EmailService { get; set; }
        IIdentityMessageService SmsService { get; set; }
        IUserTokenProvider<User, int> UserTokenProvider { get; set; }
        bool UserLockoutEnabledByDefault { get; set; }
        int MaxFailedAccessAttemptsBeforeLockout { get; set; }
        TimeSpan DefaultAccountLockoutTimeSpan { get; set; }
        bool SupportsUserTwoFactor { get; }
        bool SupportsUserPassword { get; }
        bool SupportsUserSecurityStamp { get; }
        bool SupportsUserRole { get; }
        bool SupportsUserLogin { get; }
        bool SupportsUserEmail { get; }
        bool SupportsUserPhoneNumber { get; }
        bool SupportsUserClaim { get; }
        bool SupportsUserLockout { get; }
        bool SupportsQueryableUsers { get; }
        IQueryable<User> Users { get; }
        IDictionary<string, IUserTokenProvider<User, int>> TwoFactorProviders { get; }
    }
}