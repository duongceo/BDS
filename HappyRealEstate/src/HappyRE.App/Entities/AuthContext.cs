using HappyRE.App.Entities;
using HappyRE.Core.Entities.Model;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HappyRE.App
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext()
            : base("DBC_HappyRE")
        {
     
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<IpAlloweds> IpAllowed { get; set; }
    }

    public class AspNetUsers
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool LockoutEnabled { get; set; }
    }

}