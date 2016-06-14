using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Users_pro_asp_exemple_login.Models
{
    public class AppRole : IdentityRole
    {
        public AppRole(): base() { }
        public AppRole(string name) : base(name) { }
    }
}