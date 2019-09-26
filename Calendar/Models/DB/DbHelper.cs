using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Calendar.Models.DB
{
    public static class DbHelper
    {
        public static DB_109670_portalEntities GetDb()
        {
            return new DB_109670_portalEntities();
        }

        public static async Task<CalendarGroup> GetGroup(int groupIdFromSource, int sourceId)
        {
            using (var db = GetDb())
            {
                return await
                    db.CalendarGroups
                        .FirstOrDefaultAsync(g => g.fkSourceId == sourceId && g.GroupIdFromSource == groupIdFromSource);
            }
        }
    }
}