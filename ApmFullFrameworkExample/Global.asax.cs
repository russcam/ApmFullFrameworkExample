using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Elastic.Apm;
using Elastic.Apm.Api;
using Elastic.Apm.AspNetFullFramework;
using Elastic.Apm.SqlClient;

namespace ApmFullFrameworkExample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // set up agent with components
            var agentComponents = ElasticApmModule.CreateAgentComponents();
            Agent.Setup(agentComponents);

            // subscribe to capture database spans to SQL server
            Agent.Subscribe(new SqlClientDiagnosticSubscriber());

            // add transaction filter
            Agent.AddFilter((ITransaction t) =>
            {
                t.SetLabel("foo", "bar");
                return t;
            });
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}