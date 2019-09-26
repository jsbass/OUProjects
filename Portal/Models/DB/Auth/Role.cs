using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Portal.Models.DB;

namespace Portal.Models.DB
{
    public partial class Role : IRole<int>
    {
        public int Id => RoleId;
    }
}