using HappyRE.Core.Entities;
using HappyRE.Core.Model;
using HappyRE.FileServiceProxy;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public class Uow : IUow, IDisposable
    {
        private bool disposed = false; // Track whether Dispose has been called. 
        private IContainer container;
        private bool _isNested = false;
        public bool IsNested { get { return _isNested; } }
        private HappyREContext _dbContext;

        public Uow()
        {
            container = ObjectFactory.Container;
        }


        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _dbContext.Database.Connection.ConnectionString;
            }
        }


        #region DataContext
        public DbContext dbContext
        {
            get
            {
                if (_dbContext == null)
                {
                    _dbContext = new HappyREContext();
                    _dbContext.Configuration.ProxyCreationEnabled = false;
                    _dbContext.Configuration.LazyLoadingEnabled = false;
                    _dbContext.Configuration.ValidateOnSaveEnabled = false;
                }
                return _dbContext;
            }
        }

        #endregion

        public void Commit()
        {
            if (_dbContext != null)
            {
                _dbContext.SaveChanges();
            }
        }

        private T GetRepository<T>() where T : class
        {
            return container.With<IUow>(this).GetInstance<T>();
        }

        public void CreateNestedContainer()
        {
            if (_isNested == true) return;

            _isNested = true;
            container = ObjectFactory.Container.GetNestedContainer();
        }

        private IRoleRepository _Role = null;
        public IRoleRepository Role
        {
            get { return _Role ?? (_Role = this.GetRepository<IRoleRepository>()); }
        }

        private IRoleGroupRepository _RoleGroup = null;
        public IRoleGroupRepository RoleGroup
        {
            get { return _RoleGroup ?? (_RoleGroup = this.GetRepository<IRoleGroupRepository>()); }
        }

        private ICityRepository _City = null;
        public ICityRepository City
        {
            get { return _City ?? (_City = this.GetRepository<ICityRepository>()); }
        }

        private IDistrictRepository _District = null;
        public IDistrictRepository District
        {
            get { return _District ?? (_District = this.GetRepository<IDistrictRepository>()); }
        }

        private IWardRepository _Ward = null;
        public IWardRepository Ward
        {
            get { return _Ward ?? (_Ward = this.GetRepository<IWardRepository>()); }
        }

        private IStreetRepository _Street = null;
        public IStreetRepository Street
        {
            get { return _Street ?? (_Street = this.GetRepository<IStreetRepository>()); }
        }

        private ISysCodeRepository _SysCode = null;
        public ISysCodeRepository SysCode
        {
            get { return _SysCode ?? (_SysCode = this.GetRepository<ISysCodeRepository>()); }
        }

        private IIpAllowedsRepository _IpAlloweds = null;
        public IIpAllowedsRepository IpAlloweds
        {
            get { return _IpAlloweds ?? (_IpAlloweds = this.GetRepository<IIpAllowedsRepository>()); }
        }

        private IDepartmentRepository _Department = null;
        public IDepartmentRepository Department
        {
            get { return _Department ?? (_Department = this.GetRepository<IDepartmentRepository>()); }
        }
        private ICustomerRepository _Customer = null;
        public ICustomerRepository Customer
        {
            get { return _Customer ?? (_Customer = this.GetRepository<ICustomerRepository>()); }
        }
        private ICustomerInfoRepository _CustomerInfo = null;
        public ICustomerInfoRepository CustomerInfo
        {
            get { return _CustomerInfo ?? (_CustomerInfo = this.GetRepository<ICustomerInfoRepository>()); }
        }
        private ICustomerRegionTargetRepository _CustomerRegionTarget = null;
        public ICustomerRegionTargetRepository CustomerRegionTarget
        {
            get { return _CustomerRegionTarget ?? (_CustomerRegionTarget = this.GetRepository<ICustomerRegionTargetRepository>()); }
        }

        private IPropertyRepository _Property = null;
        public IPropertyRepository Property
        {
            get { return _Property ?? (_Property = this.GetRepository<IPropertyRepository>()); }
        }

        private IImageFileRepository _ImageFile = null;
        public IImageFileRepository ImageFile
        {
            get { return _ImageFile ?? (_ImageFile = this.GetRepository<IImageFileRepository>()); }
        }

        private ISaleOrderRepository _SaleOrder = null;
        public ISaleOrderRepository SaleOrder
        {
            get { return _SaleOrder ?? (_SaleOrder = this.GetRepository<ISaleOrderRepository>()); }
        }

        private IFileRepository _File = null;
        public IFileRepository File
        {
            get { return _File ?? (_File = this.GetRepository<IFileRepository>()); }
        }

        private IUserProfileRepository _UserProfile = null;
        public IUserProfileRepository UserProfile
        {
            get { return _UserProfile ?? (_UserProfile = this.GetRepository<IUserProfileRepository>()); }
        }

        private IHistoryLogRepository _HistoryLog = null;
        public IHistoryLogRepository HistoryLog
        {
            get { return _HistoryLog ?? (_HistoryLog = this.GetRepository<IHistoryLogRepository>()); }
        }

        private INotificationRepository _Notification = null;
        public INotificationRepository Notification
        {
            get { return _Notification ?? (_Notification = this.GetRepository<INotificationRepository>()); }
        }
        private INotificationReadRepository _NotificationRead = null;
        public INotificationReadRepository NotificationRead
        {
            get { return _NotificationRead ?? (_NotificationRead = this.GetRepository<INotificationReadRepository>()); }
        }


        private ITokenRepository _Token = null;
        public ITokenRepository Token
        {
            get { return _Token ?? (_Token = this.GetRepository<ITokenRepository>()); }
        }

        #region Services
        private IFileService _FileService = null;
        public IFileService FileService
        {
            get { return _FileService ?? (_FileService = this.GetRepository<IFileService>()); }
        }
        #endregion
        public void LoadCache()
        {
            throw new NotImplementedException();
        }

        #region Dispose
        ~Uow()
        {
            this.Dispose(false);
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed == true) return;

            if (disposing)
            {
                if (_dbContext != null) _dbContext.Dispose();
                if (_isNested == true) container.Dispose();
            }

            this.disposed = true;
        }
        #endregion
    }
}
