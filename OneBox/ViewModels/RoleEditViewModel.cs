using OneBox_WebServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneBox_WebServices.ViewModels
{
    public class RoleEditViewModel
    {
        public AppRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }
}