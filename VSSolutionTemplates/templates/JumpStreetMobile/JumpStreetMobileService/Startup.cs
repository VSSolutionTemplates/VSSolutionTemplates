using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(JumpStreetMobileService.Startup))]

namespace JumpStreetMobileService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}