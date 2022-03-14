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

namespace HappyRE.Core.BLL.DI
{
    using HappyRE.Core.BLL.Repositories;
    using HappyRE.FileServiceProxy;
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
    using StructureMap.Pipeline;
    using StructureMap.Web.Pipeline;

    public class BLLRegistry : Registry
    {

        #region Constructors and Destructors

        public BLLRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });
            For<IUow>().LifecycleIs<HybridLifecycle>().Use<Uow>();

            For<IRoleRepository>().LifecycleIs<HybridLifecycle>().Use<RoleRepository>();
            For<IRoleGroupRepository>().LifecycleIs<HybridLifecycle>().Use<RoleGroupRepository>();
            For<ICityRepository>().LifecycleIs<HybridLifecycle>().Use<CityRepository>();
            For<IDistrictRepository>().LifecycleIs<HybridLifecycle>().Use<DistrictRepository>();
            For<IWardRepository>().LifecycleIs<HybridLifecycle>().Use<WardRepository>();
            For<IStreetRepository>().LifecycleIs<HybridLifecycle>().Use<StreetRepository>();
            For<ISysCodeRepository>().LifecycleIs<HybridLifecycle>().Use<SysCodeRepository>();
            For<IIpAllowedsRepository>().LifecycleIs<HybridLifecycle>().Use<IpAllowedsRepository>();
            For<IDepartmentRepository>().LifecycleIs<HybridLifecycle>().Use<DepartmentRepository>();
            For<ICustomerRepository>().LifecycleIs<HybridLifecycle>().Use<CustomerRepository>();
            For<ICustomerInfoRepository>().LifecycleIs<HybridLifecycle>().Use<CustomerInfoRepository>();
            For<ICustomerRegionTargetRepository>().LifecycleIs<HybridLifecycle>().Use<CustomerRegionTargetRepository>();
            For<IPropertyRepository>().LifecycleIs<HybridLifecycle>().Use<PropertyRepository>();
            For<IImageFileRepository>().LifecycleIs<HybridLifecycle>().Use<ImageFileRepository>();
            For<ISaleOrderRepository>().LifecycleIs<HybridLifecycle>().Use<SaleOrderRepository>();

            For<IFileService>().LifecycleIs<HybridLifecycle>().Use<FileService>();
            For<IFileRepository>().LifecycleIs<HybridLifecycle>().Use<FileRepository>();
            For<IUserProfileRepository>().LifecycleIs<HybridLifecycle>().Use<UserProfileRepository>();
            
            For<ITokenRepository>().LifecycleIs<HybridLifecycle>().Use<TokenRepository>();
            
            For<IHistoryLogRepository>().LifecycleIs<HybridLifecycle>().Use<HistoryLogRepository>();
            
        }

        #endregion
    }
}