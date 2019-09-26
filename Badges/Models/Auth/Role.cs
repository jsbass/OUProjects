using Microsoft.AspNet.Identity;

namespace Badges.Models
{
    public partial class Role : IRole<int>
    {
        public int Id => RoleId;
    }
}