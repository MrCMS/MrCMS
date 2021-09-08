using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class MrCMSPasswordHasher : PasswordHasher<User>
    {
        private readonly IPasswordEncryptionManager _passwordEncryptionManager;

        public MrCMSPasswordHasher(IPasswordEncryptionManager passwordEncryptionManager)
        {
            _passwordEncryptionManager = passwordEncryptionManager;
        }

        public override PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword,
            string providedPassword)
        {
            if (user.CurrentEncryption == null ||
                user.CurrentEncryption == NopSHA1HashAlgorithm.Name ||
                user.CurrentEncryption == SHA512HashAlgorithm.Name)
            {
                var success = _passwordEncryptionManager.ValidateUser(user, providedPassword);
                if (success)
                {
                    return PasswordVerificationResult.SuccessRehashNeeded;
                }
                return PasswordVerificationResult.Failed;
            }

            return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }
    }
}