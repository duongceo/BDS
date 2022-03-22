using HappyRE.Core.MapModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    //public class ProjectSearchCache
    //{
    //    public string Url { get; set; }
    //    public int Total { get; set; }
    //    public List<int> ItemIds { get; set; }
    //    public ProjectQuery Query { get; set; }
    //    public int? GetNextItem(int projectId)
    //    {

    //        var index = ItemIds.IndexOf(projectId);

    //        if (index + 1 < ItemIds.Count) return ItemIds[index + 1];

    //        return null;
    //    }
    //    public int? GetBackItem(int projectId)
    //    {
    //        var index = ItemIds.IndexOf(projectId);
    //        if (index -1 > 0) return ItemIds[index - 1];
    //        return null;
    //    }
    //    public bool IsLast(int projectId)
    //    {
    //        if (!this.HasItem()) return false;
    //        var index = ItemIds.IndexOf(projectId);
    //        return (index == ItemIds.Count - 1);
    //    }
    //    public bool IsFirst(int projectId)
    //    {
    //        if (!this.HasItem()) return false;
    //        var index = ItemIds.IndexOf(projectId);
    //        return (index == 0);
    //    }
    //    public int? GetFisrtItem()
    //    {
    //        if (!this.HasItem()) return null;
    //        return ItemIds[0];
    //    }
    //    public int? GetLastItem()
    //    {
    //        if (!this.HasItem()) return null;
    //        return ItemIds[ItemIds.Count - 1];
    //    }
    //    private bool HasItem()
    //    {
    //        return ItemIds != null && ItemIds.Count > 0;
    //    }

    //    public bool HasItem(int projectId)
    //    {
    //        if (!this.HasItem()) return false;
    //        return this.ItemIds.Any(itemId => itemId == projectId);
    //    }
    //}

    
}

