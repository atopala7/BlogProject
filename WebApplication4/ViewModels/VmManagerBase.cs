using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.Models
{
    public class VmManagerBase
    {
        public VmManagerBase()
        {
            dc = new ApplicationDbContext();

            // Turn off the Entity Framework (EF) proxy creation features
            // We do NOT want the EF to track changes - we'll do that ourselves
            dc.Configuration.ProxyCreationEnabled = false;

            // Also, turn off lazy loading...
            // We want to retain control over fetching related objects
            dc.Configuration.LazyLoadingEnabled = false;
        }

        public ApplicationDbContext dc;
    }
}