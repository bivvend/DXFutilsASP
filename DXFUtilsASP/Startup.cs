using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DXFUtilsASP.Startup))]
namespace DXFUtilsASP
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
