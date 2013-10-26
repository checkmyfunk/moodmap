using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Mvc.Facebook;
using Logic;
using System.Threading;

namespace WorldMoodMap
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FacebookConfig.Register(GlobalFacebookConfiguration.Configuration);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start()
        {
            AppStart.Start(HttpContext.Current);
        }

        /// <summary>
        /// Class is called only on the first request
        /// </summary>
        private class AppStart
        {
            static bool _init = false;
            private static Object _lock = new Object();

            /// <summary>
            /// Does nothing after first request
            /// </summary>
            /// <param name="context"></param>
            public static void Start(HttpContext context)
            {
                if (_init)
                {
                    return;
                }
                //create class level lock in case multiple sessions start simultaneously
                lock (_lock)
                {
                    if (!_init)
                    {
                        DatabaseInitializer.Initialize();
                        _init = true;
                    }
                }
            }
        }
    }
}