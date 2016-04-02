namespace OneBox_DataAccess.Migrations
{
    using Domain;
    using Infrastucture;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Utilities;
    internal sealed class Configuration : DbMigrationsConfiguration<OneBox_DataAccess.DatabaseContexts.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(OneBox_DataAccess.DatabaseContexts.ApplicationDbContext context)
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
