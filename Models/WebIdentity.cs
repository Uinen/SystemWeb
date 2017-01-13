﻿using System.Security.Principal;
using System.Web.Security;

namespace SystemWeb.Models
{
    public class WebIdentity : IIdentity
    {
        private FormsAuthenticationTicket ticket;

        public WebIdentity(FormsAuthenticationTicket ticket)
        {
            this.ticket = ticket;
        }

        public string AuthenticationType
        {
            get { return "WebUser"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return ticket.Name; }
        }

        public string FriendlyName
        {
            get { return ticket.UserData; }
        }
    }
}