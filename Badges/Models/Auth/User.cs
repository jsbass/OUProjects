using System.Linq;
using Microsoft.AspNet.Identity;

namespace Badges.Models
{
    public partial class User : IUser<string>
    {
        public string Id => UserId;

        public string UserName
        {
            get { return Id; }
            set { UserId = value; }
        }

        //public virtual bool EmailConfirmed { get; set; }

        //public virtual DateTime? LockoutEndDateUtc { get; set; }

        //public virtual int AccessFailedCount { get; set; }

        public bool IsAdmin()
        {
            if (Roles.Any(x => x.Id == 0))
            {
                return true;
            }

            return false;
        }
    }
}