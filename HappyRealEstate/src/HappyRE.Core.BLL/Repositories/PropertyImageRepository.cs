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

namespace HappyRE.Core.BLL.Repositories
{
    public class PropertyImageRepository : BaseDPRepository<PropertyImage>, IPropertyImageRepository
    {
        public PropertyImageRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<IEnumerable<string>> GetImages(int propertyId)
        {
            var query = @"select Src from PropertyImage where PropertyId = @propertyId and Deleted=0";
            return await this.Query<string>(query, new { propertyId }, System.Data.CommandType.Text);
        }

        public async Task<int?> IU(PropertyImage obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                m.Src = obj.Src;
                await this.Update(m);
                return m.Id;
            }
        }

        public async Task UpdateProductImages(int propertyId, List<string> images)
        {
            var query = @"Delete PropertyImage where PropertyId = @propertyId and src not in @images";
            await this.ExecuteScalar<int>(query, new { propertyId, images }, System.Data.CommandType.Text);
            var createdBy = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
            DateTime createdDate = DateTime.Now;
            foreach (var src in images)
            {
                query = "if not exists(select Id from PropertyImage(nolock) where PropertyId =@propertyId and Src=@src) begin Insert into PropertyImage(PropertyId, Src, Deleted, CreatedBy, CreatedDate, UpdatedDate) values(@propertyId,@src,0,@createdBy,@createdDate,@createdDate) end";
                await this.ExecuteScalar<int>(query, new { propertyId, src, createdBy, createdDate }, System.Data.CommandType.Text);
            }
        }
    }
}
