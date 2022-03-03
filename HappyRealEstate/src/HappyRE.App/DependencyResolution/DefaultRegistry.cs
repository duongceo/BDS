// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HappyRE.App.DependencyResolution {
    using Core.BLL.Repositories;
    using FileServiceProxy;
    using HappyRE.App.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Owin.Security;
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
    using StructureMap.Web.Pipeline;
    using System.Data.Entity;
    using System.Web;

    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {

            //For<IUserStore<ApplicationUser>>().LifecycleIs<HybridLifecycle>().Use<UserStore<ApplicationUser>>();
            For<IUserStore<ApplicationUser>>().Use<UserStore<ApplicationUser>>();
            For<DbContext>().Use(() => new ApplicationDbContext());
            For<IAuthenticationManager>().Use(() => HttpContext.Current.GetOwinContext().Authentication);

     //       Scan(
     //           scan => {
     //               scan.TheCallingAssembly();
     //               scan.WithDefaultConventions();
					//scan.With(new ControllerConvention());
     //           });

            
        }

        #endregion
    }
}