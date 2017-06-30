using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;

namespace WebApplication4
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Mapper.CreateMap<ViewModels.PostAdd, Models.Post>();
            Mapper.CreateMap<ViewModels.CommentAdd, Models.Post>();

            Mapper.CreateMap<Models.Post, ViewModels.PostFull>();
            //Mapper.CreateMap<Models.Post, ViewModels.PostList>();
        }
    }
}
