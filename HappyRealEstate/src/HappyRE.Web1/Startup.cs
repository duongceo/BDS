using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HappyRE.Web1.Startup))]
namespace HappyRE.Web1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
