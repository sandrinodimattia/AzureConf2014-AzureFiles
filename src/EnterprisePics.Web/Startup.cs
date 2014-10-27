using Microsoft.Owin;

using Owin;

[assembly: OwinStartupAttribute(typeof(EnterprisePics.Web.Startup))]
namespace EnterprisePics.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
