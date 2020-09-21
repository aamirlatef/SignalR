using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalRInbox.Classes.Startup))]
namespace SignalRInbox.Classes
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}