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

namespace HappyRE.Core.BLL.Repositories
{
    public class ImageFileRepository : BaseDPRepository<ImageFile>, IImageFileRepository
    {
        public ImageFileRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<ImageFileViewModel>, int>> Search(ImageFileQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            p.Add("tableName", query.TableName);
            p.Add("tableKeyId", query.TableKeyId);
            var res = await this.Query<ImageFileViewModel>("msp_FileImageHistory_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<ImageFileViewModel>, int>(res, total);
        }

        public async Task<IEnumerable<string>> GetImages(ImageFileQuery query)
        {
            var q = @"select Src from [ImageFile] (nolock) where TableName = @TableName and TableKeyId=@TableKeyId and Deleted=0";
            return await this.Query<string>(q, new { query.TableName, query.TableKeyId }, System.Data.CommandType.Text);
        }

        public async Task<bool> IsExists(ImageFileQuery query)
        {
            var q = @"select count(*) from [ImageFile] (nolock) where TableName = @TableName and TableKeyId=@TableKeyId and src=@Src";
            var r= await this.ExecuteScalar<int>(q, new { query.TableName, query.TableKeyId, query.Src }, System.Data.CommandType.Text);
            return r > 0;
        }

        public async Task<int?> IU(ImageFile obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                //m.TableName = obj.TableName;
                m.Src = obj.Src;
                await this.Update(m);
                return m.Id;
            }
        }

        public async Task UpdateImages(ImageFileQuery query, List<string> images)
        {
            var q = @"Delete ImageFile where TableName =@TableName and TableKeyId =@TableKeyId and src not in @images";
            await this.ExecuteScalar<int>(q, new { query.TableName, query.TableKeyId, images }, System.Data.CommandType.Text);
            var createdBy = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
            DateTime createdDate = DateTime.Now;
            foreach (var src in images)
            {
                q = "if not exists(select Id from ImageFile (nolock) where TableName =@TableName and TableKeyId =@TableKeyId and Src=@src) begin Insert into ImageFile(TableName, TableKeyId, Src,Size, Deleted, CreatedBy, CreatedDate, UpdatedDate) values(@TableName,@TableKeyId,@src,0,0,@createdBy,@createdDate,@createdDate) end";
                await this.ExecuteScalar<int>(q, new { query.TableName, query.TableKeyId, src, createdBy, createdDate }, System.Data.CommandType.Text);
            }
        }

        public void AddImages(ImageFileQuery query, List<string> images)
        {
            var groupCode = DateTime.Now.GetHashCode().ToString("x");
            foreach (var img in images)
            {
                var f = new ImageFile()
                {
                    Src = img,
                    TableName = "Property",
                    TableKeyId = query.TableKeyId,
                    IsMore = false,
                    GroupCode = groupCode,
                    Deleted = false
                };
                Hangfire.BackgroundJob.Enqueue<IImageFileRepository>(x => x.IU(f));
            }
        }
    }
}
