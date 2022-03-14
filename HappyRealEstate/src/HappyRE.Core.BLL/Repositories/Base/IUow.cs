using HappyRE.FileServiceProxy;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IUow
    {
        bool IsNested { get; }
        DbContext dbContext { get; }

        /// <summary>
        /// Connection string
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Commit data save into db
        /// </summary>
        void Commit();

        void LoadCache();
        void CreateNestedContainer();

        IRoleRepository Role { get; }
        IRoleGroupRepository RoleGroup { get; }
        ICityRepository City { get; }
        IDistrictRepository District { get; }
        IWardRepository Ward { get; }
        IStreetRepository Street { get; }
        IIpAllowedsRepository IpAlloweds { get; }
        ISysCodeRepository SysCode { get; }
        IDepartmentRepository Department { get; }
        ICustomerRepository Customer { get; }
        ICustomerInfoRepository CustomerInfo { get; }
        ICustomerRegionTargetRepository CustomerRegionTarget { get; }
        IPropertyRepository Property { get; }
        IImageFileRepository ImageFile { get; }
        ISaleOrderRepository SaleOrder { get; }
        IFileService FileService { get; }
        IFileRepository File { get; }
        IUserProfileRepository UserProfile { get; }
        IHistoryLogRepository HistoryLog { get; }
        ITokenRepository Token { get; }
        INotificationRepository Notification { get; }
        INotificationReadRepository NotificationRead { get; }


    }
}
