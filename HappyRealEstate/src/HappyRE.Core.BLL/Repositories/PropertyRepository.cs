using log4net;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using HappyRE.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HappyRE.Core.Entities;
using System.Data;
using Dapper;
using System.Net.Http;
using System.IO;
using MBN.Utils;

namespace HappyRE.Core.BLL.Repositories
{
    public class PropertyRepository : BaseDPRepository<Property>, IPropertyRepository
    {
        private static readonly ILog _log = LogManager.GetLogger("PropertyRepository");
        private readonly static int MaxViewMobileInDay = int.Parse(WebUtils.AppSettings("MAX_VIEW_MOBILE", "20"));
        private readonly static int LimitProduct = int.Parse(WebUtils.AppSettings("LIMIT_PRODUCT", "0"));
        public PropertyRepository(IUow uow)
            : base(uow)
        {
        }

        public IEnumerable<KeyValuePropertyModel> GetAll()
        {
            return this.QueryNonAsync<KeyValuePropertyModel>("select Id,Code Name, PostedBy from PropertySearch (nolock) where Deleted=0 and IsTemp=0", new { }, CommandType.Text);
        }

        public async Task<Tuple<IEnumerable<PropertyListViewModel>, int>> Search(PropertyQuery query)
        {
            var p = new DynamicParameters();
            var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";

            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("fromDate", query.FromDate);
            p.Add("toDate", query.ToDate);
            p.Add("userName", userName);
            p.Add("keyword", query.Keyword);
            p.Add("typeId", query.TypeId_Filter);
            p.Add("contractId", query.ContractId_Filter);
            p.Add("statusId", query.StatusId_Filter);
            p.Add("legalId", query.LegalId_Filter);
            p.Add("sourceId", query.SourceId_Filter);
            p.Add("utilityId", query.UtilityId_Filter);
            p.Add("directionId", query.DirectionId_Filter);
            p.Add("isChecked", query.IsChecked_Filter);
            p.Add("cityId", query.CityId);
            p.Add("districtId", query.DistrictId);
            p.Add("wardId", query.WardId);            
            p.Add("streetId", query.StreetId);
            p.Add("price_bw", query.Price_bw);
            p.Add("area_bw", query.Area_bw);
            p.Add("width_bw", query.Width_bw);
            p.Add("streetWidth_bw", query.StreetWidth_bw);
            p.Add("numOfFloor_bw", query.NumOfFloor_bw);
            p.Add("numOfBedroom_bw", query.NumOfBedroom_bw);
            p.Add("numOfToilet_bw", query.NumOfToilet_bw);
            p.Add("priceUnit", query.PriceUnit);
            p.Add("postedBy", query.PostedBy);

            var res = await this.Query<PropertyListViewModel>("msp_Property_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");           
            return new Tuple<IEnumerable<PropertyListViewModel>, int>(res, total);
        }

        public async Task<IEnumerable<KeyValueDisplayModel>> GetListKeyValue(string keyword,string id)
        {
            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(id))
            {
                return new List<KeyValueDisplayModel>();
            }else if (string.IsNullOrEmpty(id) ==false)
            {
                return await this.Query<KeyValueDisplayModel>(@"select Id, (Code + ' ('+ AddressHtml+')') Name
                    from PropertySearch (nolock)
                    where Id=@id", new { id }, CommandType.Text);
            }
            var encodeForLike = keyword.Replace("[", "[[]").Replace("%", "[%]");
            keyword = "%" + encodeForLike + "%";
            return await this.Query<KeyValueDisplayModel>(@"select Id, (Code + ' ('+ AddressHtml+')') Name
                    from PropertySearch (nolock)
                    where Deleted=0 and IsTemp=0 and (AddressHtml like @keyword or Code like @keyword)", new { keyword }, CommandType.Text);
        }

        public async Task<Select2Results> SearchForSelect(string keyword, string id)
        {
            var r = new Select2Results();
            var list = new List<Select2Result>();
            r.results = list;
            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(id))
            {
                return r;
            }
            else if (string.IsNullOrEmpty(id) == false)
            {
                var a = await this.Query<Select2Result>(@"select id, Code text, postedby
                    from PropertySearch (nolock)
                    where Id=@id", new { id }, CommandType.Text);
                list = a.ToList();
            }
            else if (keyword.Length > 2)
            {
                var encodeForLike = keyword.Replace("[", "[[]").Replace("%", "[%]");
                keyword = "%" + encodeForLike + "%";
                var b = await this.Query<Select2Result>(@"select top 50 id, Code text, postedby
                    from PropertySearch (nolock)
                    where Deleted=0 and IsTemp=0 and (Code like @keyword)", new { keyword }, CommandType.Text);
                list = b.ToList();
                r.results = list;
            }
            return r;
        }
        public async Task<int?> IU(Property obj)
        {
            var m = await this.GetById(obj.Id);
            obj.Code = obj.Code.Trim().ToUpper();

            var isExistsCode = await IsExistsCode(obj);
            if (isExistsCode) throw new BusinessException("Mã BĐS đã tồn tại, vui lòng nhập mã khác!");
            var isExistsAddress = await IsExistsAddress(obj);
            if (isExistsAddress) throw new BusinessException("Địa chỉ BĐS này đã có trong hệ thống!");
            if (obj.DistrictId > 0)
            {
                var dist = await uow.District.GetById(obj.DistrictId);
                obj.CityId = dist == null ? 0 : dist.CityId;
            }
            if (m == null)
            {
                if (LimitProduct > 0)
                {
                    var canAdd = await base.CheckInsertLimit<Property>(LimitProduct);
                    if (canAdd == false)
                    {
                        throw new BusinessException($"Gói tài khoản sử dụng đang giới hạn quản lý {LimitProduct} sản phẩm!");
                    }
                }
                obj.PostedDate = DateTime.Now;
                var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
                if(obj.PostedBy==null) obj.PostedBy = userName;
                var id = await this.Insert(obj);
                if (id.HasValue)
                {
                    await uow.ImageFile.UpdateImages(new ImageFileQuery() {TableName="Property", TableKeyId= id.Value } , obj.PropertyImages);
                    await Merge_PropertySearch(id.Value);
                }
                return id;
            }
            else
            {
                var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
                if (obj.PostedBy == null) obj.PostedBy = userName;
                
                await TrackChange(obj, m);
                bool isChangeTpm = obj.IsTemp != m.IsTemp && m.IsTemp == true;

                m.Code = obj.Code;
                m.ContractId = obj.ContractId;
                m.ContractCode = obj.ContractCode;
                m.Commission = obj.Commission;
                m.IsChecked = obj.IsChecked;
                m.IsVerified = obj.IsVerified;
                m.StatusId = obj.StatusId;
                m.TypeId = obj.TypeId;
                m.CityId = obj.CityId;
                m.DistrictId = obj.DistrictId;
                m.WardId = obj.WardId;
                m.StreetId = obj.StreetId;
                m.Address = obj.Address;
                m.Office = obj.Office;
                m.RegionCode = obj.RegionCode;
                m.MapCode = obj.MapCode;
                m.OwnerName = obj.OwnerName;
                m.OwnerPhone = obj.OwnerPhone;
                m.OwnerPhoneExt = obj.OwnerPhoneExt;
                m.OwnerNote = obj.OwnerNote;
                m.LegalId = obj.LegalId;
                m.Price = obj.Price;
                m.PriceMatched = obj.PriceMatched;
                m.CurrencyType = obj.CurrencyType;
                m.CalcMethod = obj.CalcMethod;
                m.Width = obj.Width;
                m.Length = obj.Length;
                m.Area = obj.Area;
                m.AreaForBuild = obj.AreaForBuild;
                m.NumOfBedroom = obj.NumOfBedroom;
                m.NumOfToilet = obj.NumOfToilet;
                m.NumOfFloor = obj.NumOfFloor;
                m.StreetWidth = obj.StreetWidth;
                m.DirectionId = obj.DirectionId;
                m.UtilityId = obj.UtilityId;
                m.SourceId = obj.SourceId;
                m.Note = obj.Note;
                m.IsGood = obj.IsGood;
                m.StrongId = obj.StrongId;
                m.WeakId = obj.WeakId;
                m.ContructId = obj.ContructId;
                m.StructureId = obj.StructureId;
                m.PotentialId = obj.PotentialId;
                m.ImageUrl = obj.PropertyImages.FirstOrDefault();
                m.VideoUrl = obj.VideoUrl;
                m.IsTemp = obj.IsTemp;
                m.PostedBy = obj.PostedBy;
                m.IsVerified = obj.IsVerified;
                m.IsHot= obj.IsHot;

                if (isChangeTpm) m.PostedDate = DateTime.Now;
                await this.Update(m);
                //await uow.PropertyImage.UpdateProductImages(m.Id, obj.PropertyImages);
                await uow.ImageFile.UpdateImages(new ImageFileQuery() { TableName = "Property", TableKeyId = m.Id }, obj.PropertyImages);
                await Merge_PropertySearch(m.Id);

                if (m.IsGood && obj.IsTemp == false)
                {
                    Hangfire.BackgroundJob.Enqueue<IPropertyRepository>(x => x.NotifyGood(m));
                }
                return m.Id;
            }
        }

        public async Task NotifyGood(Property obj)
        {
            var q = "select count(*) from Notification (nolock) where Deleted=0 and Type=5 and Code = @code";
            var sl = await this.ExecuteScalar<int>(q, new { code = obj.Id.ToString() }, CommandType.Text);
            if (sl == 0 && obj.Id>0 && obj.IsGood && obj.IsTemp==false)
            {
                var client = new HttpClient();
                await client.GetAsync($"https://tqt.batdongsanhanhphuc.vn/job/AlertGoodProperty?propertyId={obj.Id}&code={obj.Code}");
            }
        }

        public async Task TrackChange(Property _new, Property _old)
        {
            try
            {
                List<TrackChange> tracks = new List<TrackChange>();
                foreach (var propertyInfo in _new.GetType().GetProperties().Where(p => !Attribute.IsDefined(p, typeof(NonTrack)) && Attribute.IsDefined(p, typeof(System.ComponentModel.DisplayNameAttribute))))
                {
                    // do stuff here
                    var n = propertyInfo.GetValue(_new);
                    var o = propertyInfo.GetValue(_old);
                    if (!Equals(n, o))
                    {
                        var t = new Entities.Model.TrackChange()
                        {
                            TableId="Property",
                            TableKeyId= _old.Id,
                            ColumnId = propertyInfo.Name,
                            ColumnName = propertyInfo.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), false).Cast<System.ComponentModel.DisplayNameAttribute>().Single().DisplayName,
                            Old = o?.ToString(),
                            New = n?.ToString(),
                            UpdatedBy = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System",
                            UpdatedDate = DateTime.Now
                        };

                        await this.ExecuteScalar<int>(@"INSERT INTO [HappyRETracking].[dbo].[TrackChange]
                                                   ([TableId]
                                                    ,[TableKeyId]
                                                   ,[ColumnId]
                                                   ,[ColumnName]
                                                   ,[New]
                                                   ,[Old]
                                                   ,[UpdatedBy]
                                                   ,[UpdatedDate])
                                             VALUES
                                                   (@TableId
                                                   ,@TableKeyId
                                                   ,@ColumnId
                                                   ,@ColumnName
                                                   ,@New
                                                   ,@Old
                                                   ,@UpdatedBy
                                                   ,@UpdatedDate)", new { t.TableId,t.TableKeyId, t.ColumnId, t.ColumnName, t.New, t.Old, t.UpdatedBy, t.UpdatedDate }, CommandType.Text);
                    }
                }
            }catch(Exception ex) { }
        }

        public async Task<int> Merge_PropertySearch(int id)
        {
            return await this.ExecuteScalar<int>("msp_FetchPropertySearch", new { id }, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<PropertyListViewModel>> Export(PropertyQuery query)
        {
            query.Page = 1;
            query.Limit = 1000000;
            var res = await Search(query);
            return res.Item1;
        }

        public async Task<PropertyListViewModel> GetDetail(int id)
        {
            var query = @"select w.*,a.[Name] ContractName,d.[Name] SourceName,g.[Name] DirectionName,u.[Name] LegalName,
                            case when w.PostedBy =@userName then 1 else isNull(l.Id,0) end ViewedMobileToday
                        from PropertySearch (nolock) w 
	                    left join dbo.SysCode (nolock) a on w.contractId = a.Id and a.TableId = 'ContractType' and a.Deleted=0
	                    left join dbo.SysCode (nolock) d on w.sourceId = d.Id and d.TableId = 'PropertySourceType' and d.Deleted=0
	                    left join dbo.SysCode (nolock) g on w.directionId = g.Id and g.TableId = 'PropertyDirectionType' and g.Deleted=0
                        left join dbo.SysCode (nolock) u on w.legalId = u.Id and u.TableId = 'PropertyLegalType' and u.Deleted=0
                        left join dbo.PropertyShowMobileLog (nolock) l on w.Id = l.PropertyId and l.CreatedBy =@userName and CONVERT(nvarchar,l.CreatedDate, 102)  = CONVERT(nvarchar,getdate(), 102)
                        where w.Deleted=0 and w.Id =@id";
            var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
            var res = await this.Query<PropertyListViewModel>(query, new { id, userName }, CommandType.Text);
            return res.FirstOrDefault();
        }

        public async Task<string> GetPhoneNumber(int id)
        {
            var res= await this.Query<string>("select OwnerPhone from PropertySearch (nolock) where Deleted=0 and Id=@id", new {id }, CommandType.Text);
            return res.FirstOrDefault();
        }

        #region ShowMobileLog
        public async Task<int> MobileViewedToday()
        {
            var q = @"with temp(sl)as(
                    select count(distinct PropertyId) sl 
                    from PropertyShowMobileLog (nolock) 
                    where createdBy=@userName and ViewDate=@viewDate
                    --union all
                    --select count(distinct CustomerId) sl
                    --from CustomerShowMobileLog (nolock) 
                    --where createdBy=@userName and ViewDate=@viewDate
                    )
                    select sum(sl) from temp";
            var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
            var res = await this.ExecuteScalar<int>(q, new { userName, viewDate = DateTime.Today }, CommandType.Text);
            return res;
        }

        public async Task<string> ShowMobile(int PropertyId, bool isAdmin=false)
        {
            if (isAdmin)
            {
                var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
                var sl = await this.ExecuteScalar<int>("select count(*) from PropertyShowMobileLog (nolock) where createdBy=@userName and PropertyId=@PropertyId and ViewDate=@viewDate", new { userName, PropertyId, viewDate = DateTime.Today }, CommandType.Text);
                if (sl == 0)
                {
                    await this.ExecuteScalar<int>("insert into PropertyShowMobileLog(PropertyId,ViewDate,CreatedBy,CreatedDate,Deleted) values(@PropertyId,@viewDate,@createdBy,@createdDate,0)", new { PropertyId, viewDate = DateTime.Now, createdBy = userName, createdDate = DateTime.Now }, CommandType.Text);
                }
                return await GetPhoneNumber(PropertyId);
            }
            else
            {
                var viewed = await MobileViewedToday();
                if (viewed < MaxViewMobileInDay)
                {
                    var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
                    var sl = await this.ExecuteScalar<int>("select count(*) from PropertyShowMobileLog (nolock) where createdBy=@userName and PropertyId=@PropertyId and ViewDate=@viewDate", new { userName, PropertyId, viewDate = DateTime.Today }, CommandType.Text);
                    if (sl == 0)
                    {
                        await this.ExecuteScalar<int>("insert into PropertyShowMobileLog(PropertyId,ViewDate,CreatedBy,CreatedDate,Deleted) values(@PropertyId,@viewDate,@createdBy,@createdDate,0)", new { PropertyId, viewDate = DateTime.Now, createdBy = userName, createdDate = DateTime.Now }, CommandType.Text);
                    }
                    return await GetPhoneNumber(PropertyId);
                }
                else
                {
                    throw new BusinessException($"Chỉ được xem {MaxViewMobileInDay} số điện thoại mỗi ngày");
                }
            }
        }

        public async Task<int> ForceHideMobile(int id, bool isForced)
        {
            var Property = await this.GetById(id);
            if (Property != null)
            {
                Property.IsForceHiddenPhone = isForced;
                var res = await this.Update(Property);
                await Merge_PropertySearch(id);
                return res;
            }
            return 0;
        }

        public async Task<int> ChangeHot(int id, bool isHot)
        {
            var Property = await this.GetById(id);
            if (Property != null)
            {
                Property.IsHot = isHot;
                var res = await this.Update(Property);
                await Merge_PropertySearch(id);
                return res;
            }
            return 0;
        }
        #endregion

        #region Condition
        public async Task<bool> IsExistsCode(Property obj)
        {
            var query = "select count(*) from PropertySearch (nolock) where Id <> @id and Code = @code and Deleted=0 and IsTemp=0";
            var res= await this.ExecuteScalar<int>(query, new { id = obj.Id, code = obj.Code });
            return res > 0;
        }

        public async Task<bool> IsExistsAddress(Property obj)
        {
            var query = "select count(*) from PropertySearch (nolock) where Id <> @id and CityId = @cityId and DistrictId =@districtId and Address=@address and Deleted=0 and IsTemp=0";
            var res = await this.ExecuteScalar<int>(query, new { id = obj.Id, cityId = obj.CityId, districtId = obj.DistrictId, address= obj.Address });
            return res > 0;
        }

        public override async Task DeleteCheck(Property obj)
        {
            var c = await this.ExecuteScalar<int>("select count(*) from SaleOrderSearch (nolock) where Deleted=0 and PropertyId=@propertyId", new { propertyId=obj.Id }, CommandType.Text);
            if (c > 0) throw new BusinessException($"Không thể xóa BĐS này vì có giao dịch liên quan!");
        }

        public override async Task DeleteAfter(Property obj)
        {
           if(obj.Id>0) await Merge_PropertySearch(obj.Id);
        }
        #endregion

        #region Tranfer Images
        public async Task TranferImages(int? from=0, int? to=0)
        {
            //string root_folder = @"C:\DATA\BACKUP\HappyRE\Product\Product";
            //string root_folder_target = @"C:\DATA\BACKUP\HappyRE\Images";

            string root_folder = @"C:\DATA\BDS\static.lenmay.vn\img\_temp\product";
            string root_folder_target = @"C:\DATA\BDS\static.lenmay.vn\img\product";
            //Lấy tất cả bđs chưa đc cập nhật hình
            var bds = await this.Query<PropertyRef>("select Id, RefId, CreatedDate from Property (nolock) where Deleted=0 and Id >=@from and Id<=@to", new {from=from,to=to }, CommandType.Text);
            List<string> imgs = new List<string>();
            _log.Warn($"BĐS from {from} to {to}: {bds.Count()}");
            foreach (var item in bds)
            {
                var imgFolder = System.IO.Path.Combine(root_folder, item.CreatedDate.ToString("yyyyMM"), item.RefId.ToString());
                if (Directory.Exists(imgFolder)==true) {
                    //get file in folder
                    try
                    {
                        DirectoryInfo dir = new DirectoryInfo(imgFolder);
                        imgs = new List<string>();
                        foreach (var file in dir.GetFiles())
                        {
                            //copy folder
                            var f = item.CreatedDate.ToString("MMyyyy");
                            try
                            {
                                if (!Directory.Exists(Path.Combine(root_folder_target, f)))
                                {
                                    System.IO.Directory.CreateDirectory(Path.Combine(root_folder_target, f));
                                }
                                string newFn = "img_" + file.Name.Replace(".jpeg",".jpg");
                                if (System.IO.File.Exists(Path.Combine(root_folder_target, f, newFn)) == false)
                                {
                                    System.IO.File.Copy(file.FullName, Path.Combine(root_folder_target, f, newFn), true);

                                    var fileName = $"https://static.lenmay.vn/img/s200x200/product/{f}/{newFn}";
                                    imgs.Add(fileName);
                                }
                                else
                                {
                                    var fileName = $"https://static.lenmay.vn/img/s200x200/product/{f}/{newFn}";
                                    if (await uow.ImageFile.IsExists(new ImageFileQuery()
                                    {
                                        TableName="Property",
                                        TableKeyId= item.Id,
                                        Src= fileName
                                    })==false)
                                    {
                                        imgs.Add(fileName);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _log.Error(ex);
                            }
                        }

                        _log.Warn($"BĐS {item.Id} add : {imgs.Count()} ảnh");
                        uow.ImageFile.AddImages(new ImageFileQuery()
                        {
                            TableName = "Property",
                            TableKeyId = item.Id,
                        }, imgs);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                }
            }

        }

        public async Task CleanImages(int? from = 0, int? to = 0)
        {
            //string root_folder = @"C:\DATA\BACKUP\HappyRE\Product\Product";
            //string root_folder_target = @"C:\DATA\BACKUP\HappyRE\Images";

            string root_folder = @"C:\DATA\BDS\static.lenmay.vn\img\_temp\product";
            string root_folder_target = @"C:\DATA\BDS\static.lenmay.vn\img\product";

            var bds = await this.Query<PropertyRef>(@"select a.Id ImgId, b.Id, b.CreatedDate, b.RefId, a.Src
                                                    from ImageFile (nolock)a
                                                    inner join Property(nolock) b on a.TableKeyId = b.Id
                                                    where a.CreatedDate <= '2022-03-19' and a.CreatedBy = 'system' and a.Deleted =0 and b.Id >=@from and b.Id<=@to", new { from = from, to = to }, CommandType.Text);
            _log.Warn($"BĐS from {from} to {to}: {bds.Count()}");
            foreach (var item in bds)
            {
                try {
                    var idx = item.Src.IndexOf("img_");
                    if (idx > 0)
                    {
                        var fileName = item.Src.Substring(idx+4, (item.Src.Length -4) - idx);
                        var path = System.IO.Path.Combine(root_folder, item.CreatedDate.ToString("yyyyMM"), item.RefId.ToString(), fileName);
                        if (System.IO.File.Exists(path) == false)
                        {
                            await uow.ImageFile.Delete(new ImageFile() { Id = item.ImgId });
                        }
                    }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                }

        }
        #endregion
    }

    public class PropertyRef
    {
        public int ImgId { get; set; }
        public int Id { get; set; }
        public int RefId { get; set; }
        public string Src { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
