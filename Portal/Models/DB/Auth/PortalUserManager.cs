using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Portal.Helpers;
using Portal.Models.Auth;

namespace Portal.Models.DB.Auth
{
    public class PortalUserManager : UserManager<User>
    {
        public PortalUserManager() : base(new PortalUserStore())
        {
        }

        public static PortalUserManager Create(IdentityFactoryOptions<PortalUserManager> options, IOwinContext context)
        {
            //TODO Verify validator requirements
            var manager = new PortalUserManager();

            // Configure custom password hashing
            manager.PasswordHasher = new PortalPasswordHasher();

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is {0}"
            //});
            manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public class PortalPasswordHasher : PasswordHasher
        {
            public override string HashPassword(string password)
            {
                return Hashing.GenerateHash(password);
            }

            public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
            {
                var isSame = Hashing.CheckPassword(providedPassword, hashedPassword);
                return isSame ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
            }
        }
    }
}