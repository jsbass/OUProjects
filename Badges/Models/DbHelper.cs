using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Badges.Models;
using Badges.Models.Auth;
using Newtonsoft.Json;

namespace Badges.Models
{
    public static class DbHelper
    {
        public static BadgeDbContext GetDb()
        {
            return new BadgeDbContext();
        }
    }
}