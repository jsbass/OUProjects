using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Badges.Startup))]
namespace Badges
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
