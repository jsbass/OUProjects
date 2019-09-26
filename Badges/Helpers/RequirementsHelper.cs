using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.DynamicData;
using Badges.Models;
using Newtonsoft.Json;

namespace Badges.Helpers
{
    public class RequirementsHelper
    {
        private TimeSpan waitTime = new TimeSpan(0, 5, 0);
        private int startedStatusId = 2;

        public async Task UpdateRequirements(string userId)
        {
            using(var client = new HttpClient())
            using (var db = DbHelper.GetDb())
            {
                //get all of a users requirement values for started badges or make new ones if they don't already have a row
                var userRequirementsObjs = await db.Users
                    .Where(u => u.UserId == userId)
                    .Join(db.BadgeStatus, u => u.UserId, bs => bs.UserId, (u, bs) => new { u, bs})
                    .Where(x => x.bs.StatusId == startedStatusId)
                    .Join(db.Badges, x => x.bs.BadgeId, b => b.Id, (x, b) => new { x.u, b})
                    .Join(db.Requirements, x => x.b.Id, r => r.BadgeId, (x, r) => new { x.u, r })
                    .GroupJoin(db.UserRequirements, x => new { id = x.r.Id, userId = x.u.UserId},
                        (ur) => new { id = ur.RequirementId, userId = ur.UserId}, (x, ur) => new { x.u, x.r, ur = ur.DefaultIfEmpty() })
                        .SelectMany(x => x.ur.Select(ur => new { x.u, x.r, ur }))
                .Where(x => x.ur == null || (!x.ur.IsCompleted && DbFunctions.DiffSeconds(DateTime.Now, x.ur.LastUpdated) > waitTime.TotalSeconds)).ToListAsync();

                var userRequirements = userRequirementsObjs.Select(x =>
                {
                    if (x.ur == null)
                    {
                        return new UserRequirement()
                        {
                            UserId = userId,
                            User = x.u,
                            RequirementId = x.r.Id,
                            Requirement = x.r
                        };
                    }
                    return x.ur;
                }).ToList();

                var reqTasks = new Dictionary<int, Task<ApiAnswer>>();
                foreach (var userRequirement in userRequirements)
                {
                    var urlTemplate = userRequirement.Requirement.UrlForStatus;
                    var @params = userRequirement.Requirement.Parameters.ToDictionary(p => p.Name, p => p.Value);
                    @params.Add("soonerId", userRequirement.User.SoonerId ?? "");
                    @params.Add("ouNetId", userRequirement.User.OuNetId ?? "");
                    
                    var regex = new Regex(@"{([^}]+)}", RegexOptions.Compiled);

                    var url = regex.Replace(urlTemplate,
                        match => @params.ContainsKey(match.Groups[1].Value) ? @params[match.Groups[1].Value] : "");

                    reqTasks.Add(userRequirement.RequirementId, GetResponse(client, url));
                }

                await Task.WhenAll(reqTasks.Values);

                foreach (var kv in reqTasks)
                {
                    var result = kv.Value.Result;
                    if (result != null)
                    {
                        var userReq = userRequirements.FirstOrDefault(u => u.RequirementId == kv.Key);
                        userReq.IsCompleted = result.IsComplete;
                        userReq.LastUpdated = DateTime.Now;
                        userReq.PercentCompleted = (decimal?) result.PercentComplete;
                        userReq.StatusText = result.Status;
                    }
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task<ApiAnswer> GetResponse(HttpClient client, string url)
        {
            try
            {
                var response = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<ApiAnswer>(response);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}