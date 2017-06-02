using System.Security.Principal;
using System.Web.Security;

namespace GestioniDirette.Models
{
    public class WebIdentity : IIdentity
    {
        private readonly FormsAuthenticationTicket _ticket;

        public WebIdentity(FormsAuthenticationTicket ticket)
        {
            _ticket = ticket;
        }

        public string AuthenticationType => "WebUser";

        public bool IsAuthenticated => true;

        public string Name => _ticket.Name;

        public string FriendlyName => _ticket.UserData;
    }
}