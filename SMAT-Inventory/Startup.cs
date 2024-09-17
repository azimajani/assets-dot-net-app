using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SMAT_Inventory.Startup))]
namespace SMAT_Inventory
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
