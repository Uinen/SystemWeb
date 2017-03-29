using Glimpse.AspNet.Extensions;
using Glimpse.Core.Extensibility;

namespace SystemWeb
{
    public class GlimpseSecurityPolicy:IRuntimePolicy
    {
        public RuntimePolicy Execute(IRuntimePolicyContext policyContext)
        {
            
			 var httpContext = policyContext.GetHttpContext();
             if (!httpContext.User.IsInRole("Administrator"))
			 {
                 return RuntimePolicy.Off;
			 }

            return RuntimePolicy.On;
        }

        public RuntimeEvent ExecuteOn
        {
			// The RuntimeEvent.ExecuteResource is only needed in case you create a security policy
			// Have a look at http://blog.getglimpse.com/2013/12/09/protect-glimpse-axd-with-your-custom-runtime-policy/ for more details
            get { return RuntimeEvent.EndRequest | RuntimeEvent.ExecuteResource; }
        }
    }
}