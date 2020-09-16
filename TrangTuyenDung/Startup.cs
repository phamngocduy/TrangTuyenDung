using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TrangTuyenDung.Startup))]
namespace TrangTuyenDung {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }

    }
}
