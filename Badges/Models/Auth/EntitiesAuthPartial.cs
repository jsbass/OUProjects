using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Badges.Models
{
    public partial class BadgeDbContext
    {
        #region Roles

        public async Task<int> ClearRolesFromUser(User user)
        {
            user.Roles.Clear();

            return await SaveChangesAsync();
        }

        public async Task<int> DeleteRoleFromUser(User user, int roleId)
        {
            var role = await Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
            user.Roles.Remove(role);
            return await SaveChangesAsync();
        }

        public async Task<int> DeleteRoleFromUser(User user, string roleName)
        {
            var role = await Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            user.Roles.Remove(role);
            return await SaveChangesAsync();
        }

        public async Task<int> InsertRoleForUser(User user, int roleId)
        {
            if (user.Roles.Any(r => r.RoleId == roleId)) return await Task.FromResult(-1);
            var role = await Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);

            user.Roles.Add(role);

            return await SaveChangesAsync();
        }

        public async Task<int> InsertRoleForUser(User user, string roleName)
        {
            if (user.Roles.Any(r => r.Name == roleName)) return await Task.FromResult(-1);
            var role = await Roles.FirstOrDefaultAsync(r => r.Name == roleName);

            user.Roles.Add(role);

            return await SaveChangesAsync();
        }

        public async Task<int> InsertNewRole(Role role)
        {
            if (Roles.Any(r => r.RoleId == role.Id)) return await Task.FromResult(-1);
            Roles.Add(role);

            return await SaveChangesAsync();
        }
        #endregion

        #region user
        public async Task<User> GetUserById(string userId)
        {
            return await Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<int> InsertUser(User user)
        {
            if (Users.Any(u => u.UserId == user.UserId)) return await Task.FromResult(-1);
            Users.Add(user);
            return await SaveChangesAsync();
        }

        public async Task<int> UpdateUser(User user)
        {
            Users.Attach(user);
            Entry(user).State = EntityState.Modified;
            return await SaveChangesAsync();
        } 
        #endregion

        #region password
        public async Task<int> SetPasswordHash(User user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return await SaveChangesAsync();
        } 
        #endregion

        #region email
        public async Task<int> SetUserEmail(User user, string email)
        {
            user.Email = email;

            return await SaveChangesAsync();
        }

        public async Task<int> SetUserEmailConfirmed(User user, bool isConfirmed)
        {
            user.EmailConfirmed = isConfirmed;

            return await SaveChangesAsync();
        }
        #endregion

        #region lockout
        public async Task<int> SetUserLockoutEndDate(User user, DateTime? endDate)
        {
            user.LockoutEndDateUtc = endDate;

            return await SaveChangesAsync();
        }

        public async Task<int> IncrementUserAccessFailedCount(User user)
        {
            return await SetUserAccessFailedCount(user, user.AccessFailedCount + 1);
        }

        public async Task<int> ResetUserAccessFailedCount(User user)
        {
            return await SetUserAccessFailedCount(user, 0);
        }

        public async Task<int> SetUserAccessFailedCount(User user, int i)
        {
            user.AccessFailedCount = i;
            return await SaveChangesAsync();
        }

        public async Task<bool> GetUserLockoutEnabled(User user)
        {
            return await Task.FromResult(true);
        }

        public async Task<int> SetUserLockoutEnabled(User user, bool lockoutEnabled)
        {
            return await Task.FromResult(1);
        }
        #endregion

        #region securityStamp

        public async Task<int> SetUserSecurityStamp(User user, string stamp)
        {
            user.SecurityStamp = stamp;
            return await SaveChangesAsync();
        }
        #endregion

        #region claims

        public async Task<int> AddUserClaim(User user, Claim claim)
        {
            user.UserClaims.Add(new UserClaim()
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });

            return await SaveChangesAsync();
        }

        public async Task<int> ClearUserClaims(User user)
        {
            user.UserClaims.Clear();

            return await SaveChangesAsync();
        }

        public async Task<int> RemoveUserClaim(User user, Claim claim)
        {
            var userClaim = user.UserClaims.FirstOrDefault(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type);
            user.UserClaims.Remove(userClaim);

            return await SaveChangesAsync();
        }

        public async Task<List<Claim>> GetUserClaims(User user)
        {
            return await Task.FromResult(user.UserClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList());
        }
        #endregion
    }
}