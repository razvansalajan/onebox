using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OneBox_WebServices.Models;
using OneBox_WebServices.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OneBox_WebServices.Infrastructure
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext() : base("oneboxdatabase", throwIfV1Schema: false) { }
        static AppIdentityDbContext()
        {
            Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
        }
        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }
    public class IdentityDbInit
 : DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
    {
        protected override void Seed(AppIdentityDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }
        public void PerformInitialSetup(AppIdentityDbContext context)
        {
            AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));
            string roleName = Utility.AdminRoles;
            string userName = Utility.AdminName;
            string password = Utility.AdminPassword;
            string email = Utility.AdminEmail;
            if (!roleMgr.RoleExists(roleName))
            {
                roleMgr.Create(new AppRole(roleName));
            }

            if (!roleMgr.RoleExists(Utility.LocalUsersRoleName))
            {
                roleMgr.Create(new AppRole(Utility.LocalUsersRoleName));
            }

            if (!roleMgr.RoleExists(Utility.UsersRole))
            {
                roleMgr.Create(new AppRole(Utility.UsersRole));
            }
            AppUser user = userMgr.FindByName(userName);
            if (user == null)
            {
                userMgr.Create(new AppUser { UserName = userName, Email = email },
                password);
                user = userMgr.FindByName(userName);

            }
            if (!userMgr.IsInRole(user.Id, roleName))
            {
                userMgr.AddToRole(user.Id, roleName);
            }
        }
    }
}