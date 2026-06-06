using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using System.Threading;

namespace Vanrise_Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // THIS IS THE CRITICAL METHOD FOR ROLES
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                // If the ticket is valid and has a Role saved in the UserData property
                if (authTicket != null && !string.IsNullOrEmpty(authTicket.UserData))
                {
                    string[] roles = { authTicket.UserData }; // Extract "Editor" or "ReadOnly"

                    // Attach the role to the current user's request
                    var userPrincipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), roles);
                    HttpContext.Current.User = userPrincipal;
                    Thread.CurrentPrincipal = userPrincipal;
                }
            }
        }
    }
}