using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Portal.Models.DB;

namespace Portal.Models.Auth
{
    public class PortalUserStore :
        IUserRoleStore<User>,
        IUserPasswordStore<User>,
        IQueryableUserStore<User>,
        IUserEmailStore<User>,
        IUserLockoutStore<User, string>,
        IUserClaimStore<User>,
        IUserLoginStore<User>,
        IUserPhoneNumberStore<User>,
        IUserSecurityStampStore<User>,
        IUserTwoFactorStore<User, string>,
        IUserStore<User>
    {
        private readonly Entities _db;
        
        public PortalUserStore() : this(DbHelper.GetDb())
        {
        }

        public PortalUserStore(Entities database)
        {
            _db = database;
        }

        public Entities GetDb()
        {
            return _db;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task CreateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            await _db.InsertUser(user);
        }

        public async Task UpdateAsync(User user)
        {
            await _db.UpdateUser(user);
        }

        public async Task DeleteAsync(User user)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task<User> FindByIdAsync(string userId)
        {
            return await _db.GetUserById(userId);
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserId == userName);
        }

        public async Task AddToRoleAsync(User user, string roleName)
        {
            await _db.InsertRoleForUser(user, roleName);
        }

        public async Task RemoveFromRoleAsync(User user, string roleName)
        {
            await _db.DeleteRoleFromUser(user, roleName);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await Task.FromResult<IList<string>>(user.Roles.Select(r => r.Name).ToList());
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName)
        {
            return await Task.FromResult(user.Roles.Any(r => r.Name == roleName));
        }

        public async Task SetPasswordHashAsync(User user, string passwordHash)
        {
            await _db.SetPasswordHash(user, passwordHash);
        }

        public async Task<string> GetPasswordHashAsync(User user)
        {
            return await Task.FromResult(user.PasswordHash);
        }

        public async Task<bool> HasPasswordAsync(User user)
        {
            return await Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public IQueryable<User> Users => _db.Users;

        public async Task SetEmailAsync(User user, string email)
        {
            await _db.SetUserEmail(user, email);
        }

        public async Task<string> GetEmailAsync(User user)
        {
            return await Task.FromResult(user.Email);
        }

        public async Task<bool> GetEmailConfirmedAsync(User user)
        {
            return await Task.FromResult(user.EmailConfirmed);
        }

        public async Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            await _db.SetUserEmailConfirmed(user, confirmed);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            return await Task.FromResult(user.LockoutEndDateUtc.HasValue ?
                new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc)) :
                new DateTimeOffset());
        }

        public async Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            await _db.SetUserLockoutEndDate(user, lockoutEnd.UtcDateTime);
        }

        public async Task<int> IncrementAccessFailedCountAsync(User user)
        {
            return await _db.IncrementUserAccessFailedCount(user);
        }

        public async Task ResetAccessFailedCountAsync(User user)
        {
            await _db.ResetUserAccessFailedCount(user);
        }

        public async Task<int> GetAccessFailedCountAsync(User user)
        {
            return await Task.FromResult(user.AccessFailedCount);
        }

        public async Task<bool> GetLockoutEnabledAsync(User user)
        {
            return await IsInRoleAsync(user, "Admin");
        }

        public async Task SetLockoutEnabledAsync(User user, bool enabled)
        {
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            return await _db.GetUserClaims(user);
        }

        public async Task AddClaimAsync(User user, Claim claim)
        {
            await _db.AddUserClaim(user, claim);
        }

        public async Task RemoveClaimAsync(User user, Claim claim)
        {
            await _db.RemoveUserClaim(user, claim);
        }

        public async Task SetPhoneNumberAsync(User user, string phoneNumber)
        {
            //TODO
            return;
        }

        public async Task<string> GetPhoneNumberAsync(User user)
        {
            //TODO
            return await Task.FromResult("");
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(User user)
        {
            //TODO
            return await Task.FromResult(true);
        }

        public async Task SetPhoneNumberConfirmedAsync(User user, bool confirmed)
        {
            //TODO
            return;
            throw new NotImplementedException();
        }

        public async Task AddLoginAsync(User user, UserLoginInfo login)
        {
            //TODO
            return;
        }

        public async Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            //TODO
            return;
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            //TODO
            return await Task.FromResult<IList<UserLoginInfo>>(null);
        }

        public async Task<User> FindAsync(UserLoginInfo login)
        {
            //TODO
            return await Task.FromResult<User>(null);
        }

        public async Task SetSecurityStampAsync(User user, string stamp)
        {
            //TODO
            await _db.SetUserSecurityStamp(user, stamp);
        }

        public async Task<string> GetSecurityStampAsync(User user)
        {
            //TODO
            return await Task.FromResult(user.SecurityStamp);
        }

        public async Task SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            //TODO
            return;
        }

        public async Task<bool> GetTwoFactorEnabledAsync(User user)
        {
            //TODO
            return await Task.FromResult(false);
        }
    }
}