using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(theblogcore.Startup))]
namespace theblogcore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
