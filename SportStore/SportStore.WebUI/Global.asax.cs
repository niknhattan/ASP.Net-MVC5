﻿namespace SportStore.WebUI
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using SportStore.Domain.Entities;
    using SportStore.WebUI.Infrastructure.Binders;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());
        }
    }
}