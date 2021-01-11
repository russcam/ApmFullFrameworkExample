using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Elastic.Apm;
using Elastic.Apm.AspNetFullFramework;
using Elastic.Apm.Logging;

namespace ApmFullFrameworkExample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            
            
            ElasticUtilities.AddAPMGlobalFilter();
        }
    }
    
    public class ElasticUtilities
    {
        public static void AddAPMGlobalFilter()
        {
            AgentDependencies.Logger = new ApmLoggerBridge();

            try
            {
                bool filterAdded = Agent.AddFilter((Elastic.Apm.Api.ITransaction transaction) =>
                {
                    try
                    {
                        LogMessage("APM Transaction filter" + transaction.Id);
                        transaction.SetLabel("UserID", "My custom code user id");
                        transaction.SetLabel("EmailAddress","My custom code email address");
                    }
                    catch (Exception exc)
                    {
                        transaction.CaptureException(exc);
                        LogException(exc);
                    }
                    return transaction;
                });
                LogMessage("Filter added:" + filterAdded);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private static void LogMessage(string message)
        {
            Console.WriteLine(message);
        }

        private static void LogException(Exception exception)
        {
            Console.WriteLine(exception);
        }

        internal class ApmLoggerBridge : IApmLogger
        {
            public bool IsEnabled(LogLevel level)
            {
                return true;
            }

            public void Log<TState>(LogLevel level, TState state, Exception e, Func<TState, Exception, string> formatter)
            {
                string message = formatter(state, e);
                if (e == null)
                {
                    LogMessage(message);
                }
                else
                {
                    LogException(e);
                }
            }
        }

    }
}