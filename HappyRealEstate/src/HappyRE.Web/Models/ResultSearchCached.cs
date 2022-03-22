using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    [Serializable]
    public class ResultSearchCached
    {
        public string ListUrl { get; set; }
        public int Total { get; set; }
        public List<ItemResult> Items { get; set; }
        public Core.MapModels.SearchFilter Filter { get; set; }

        public ItemResult[] GetNexAndPrev(int propertyId)
        {
            if (this.Items == null) return null;
            ItemResult[] res = new ItemResult[2];

            int i = 0, t = this.Items.Count;
            for (i = 0; i < t; i++)
            {
                if (this.Items[i].Id == propertyId) break;
            }
            if (i > 0) res[0] = this.Items[i - 1];
            if (i < t - 1) res[1] = this.Items[i + 1];

            return res;
        }

        public bool IsFirst(int propertyId)
        {
            if (this.Items != null && this.Items.Count > 0)
            {
                return (this.Items[0].Id == propertyId);
            }
            return false;
        }
        
        public bool IsLast(int propertyId)
        {
            if (this.Items != null && this.Items.Count > 0)
            {
                return (this.Items[this.Items.Count - 1].Id == propertyId);
            }
            return false;
        }
        
        public ItemResult GetNexItem(int propertyId)
        {
            int i = 0, t = this.Items.Count;
            for (i = 0; i < t; i++)
            {
                if (this.Items[i].Id == propertyId) break;
            }
            if (i < t - 1) return this.Items[i + 1];

            return null;
        }

        public ItemResult GetPrevItem(int propertyId)
        {
            int i = 0, t = this.Items.Count;
            for (i = 0; i < t; i++)
            {
                if (this.Items[i].Id == propertyId) break;
            }
            if (i > 0) return this.Items[i - 1];

            return null;
        }

        public bool HasItem(int propertyId)
        {
            if (this.Items == null) return false;
            return this.Items.Any(c => c.Id == propertyId);            
        }
        public ItemResult GetFirstItem()
        {
            if (this.Items != null && this.Items.Count > 0) return this.Items[0];
            return null;
        }

        public ItemResult GetLastItem()
        {
            if (this.Items != null && this.Items.Count > 0) return this.Items[this.Items.Count - 1];
            return null;
        }
    }
    [Serializable]
    public class ItemResult
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }     
        public int Sort { get; set; }
    }
}