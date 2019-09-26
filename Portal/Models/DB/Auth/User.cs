using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Portal.Models.DB;

namespace Portal.Models.DB
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
            if (this.Roles.Where(x => x.Id == 0) != null)
            {
                return true;
            }

            return false;
        }
    }
}