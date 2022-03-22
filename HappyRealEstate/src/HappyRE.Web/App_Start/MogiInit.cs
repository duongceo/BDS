
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MBN.Utils.Extension;

namespace HappyRE.Web.App_Start
{
    public class MogiInit
    {
        public static void InitApp()
        {
            IUow uow = Core.ObjectFactory.GetInstance<IUow>();

			// JS: Common, PostingUI
			BLL.Queues.CacheVersionTask.JS_Common_Web(uow, false); 

            // Common Queue
            MBN.Utils.TaskScheduler.BackgroundQueue.Start();

            // RefreshCache
            BLL.Queues.CacheVersionTask.Start();

            // LoadCache Queue
            new BLL.Queues.LoadCacheQueue().Enqueue();

            // Message
            new BLL.Queues.MessageUpdateLatestQueue().Enqueue();

            #region LoadCache
            uow.Language.LoadCache();
            uow.PropertyType.LoadCache();
            uow.Captcha.LoadCache();

			// CMS
            uow.CMSGroup.LoadCache();
            uow.CMSCategory.LoadCache();
			uow.CMSTemplate.LoadCache();

            uow.MediaReferCode.LoadCache();
            uow.MediaServer.LoadCache();

            uow.SysTable.LoadCache();
            uow.SysCode.LoadCache();
            #endregion
            
        }

        //private static void JS_PostingUI(IUow uow)
        //{
        //    string fileJS = uow.GetFullPathResourceFile("mogi-res-postingui.js");
        //    FileInfo file = new FileInfo(fileJS);
        //    if (file.Exists && file.Length > 10) return;

        //    string json = uow.PostingUI.ToJson();

        //    uow.SaveFileJS(fileJS, json);
        //}

        //private static Dictionary<string,string> JS_Msg()
        //{
        //    Dictionary<string, string> msg = new Dictionary<string, string>();
        //    msg.Add("Routing_Buy_CodeUrl", Core.Resources.Message.Routing_Buy_CodeUrl);
        //    msg.Add("Routing_Rent_CodeUrl", Core.Resources.Message.Routing_Rent_CodeUrl);
        //    msg.Add("Routing_PropertyType_All", Core.Resources.Message.Routing_PropertyType_All);
        //    msg.Add("Msg_Update", Core.Resources.Message.Msg_Update);
        //    msg.Add("Msg_Cancel", Core.Resources.Message.Msg_Cancel);
        //    msg.Add("Listing_ResultMap", Core.Resources.Message.Listing_ResultMap);
        //    msg.Add("Listing_Result_NotFound", Core.Resources.Message.Listing_Result_NotFound);
        //    msg.Add("Listing_Filter_All", Resources.Message.Listing_Filter_All);
        //    msg.Add("Listing_Filter_Country", Resources.Message.Listing_Filter_Country);
        //    msg.Add("GoogleMap_Return", Core.Resources.Message.LblComback);
        //    msg.Add("GoogleMap_Header", Core.Resources.Message.Listing_View_Map);
        //    msg.Add("GoogleMap_Footer", Core.Resources.Message.LblPlace);
        //    return msg;
        //}
    }
}