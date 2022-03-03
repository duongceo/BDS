using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
//using HtmlAgilityPack;

namespace HappyRE.App.Infrastructures
{
    public class ReplaceImageLink
    {
        //public static string ProcessImageDescHtml(string descSource)
        //{            
        //    if (string.IsNullOrWhiteSpace(descSource)) { return ""; };

        //    if (descSource.Length > 60000)
        //    {               
        //    }
            
        //    string theHTML = descSource.ToLower();
        //    string re2 = "url\\((.[^)]*)\\)";
        //    MatchCollection matches = Regex.Matches(theHTML, re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        //    foreach (Match match in matches)
        //    {
        //        if (!__isFromMediaServer(match.Groups[1].Value))
        //        {
        //            List<string> row = new List<string>();
        //            row.Add(match.Groups[1].Value);
        //            var resultInline = rpFile.UploadFileFromURL(row.ToArray());
        //            if (resultInline[0].IsError)
        //                descSource = Regex.Replace(theHTML, match.Groups[1].Value, "#");
        //            else
        //                descSource = Regex.Replace(theHTML, match.Groups[1].Value, resultInline[0].Data);
        //        }

        //    }

        //    HtmlDocument document = new HtmlDocument();
        //    document.OptionFixNestedTags = true;
        //    document.OptionAutoCloseOnEnd = true;
        //    document.OptionDefaultStreamEncoding = Encoding.UTF8;
        //    document.LoadHtml(descSource);

        //    HtmlNode allNodes = document.DocumentNode;

        //    IEnumerable<HtmlNode> nodes = (from n in allNodes.DescendantsAndSelf()
        //                                   where n.Name == "img"
        //                                   select n);

        //    if (nodes.Count() > 30)
        //    {
        //        return "Số lượng ảnh không vượt quá 30";                
        //    }


        //    var listForUpload = new List<Tuple<HtmlNode, int, int>>();
        //    bool isHasRemoveImg = false;

        //    foreach (var img in nodes.ToArray())
        //    {
        //        if (!img.HasAttributes) continue;

        //        int height = -1;
        //        int width = -1;

        //        if (img.Attributes.Contains("height"))
        //        {
        //            int.TryParse(img.Attributes["height"].Value, out height);
        //        }

        //        if (img.Attributes.Contains("width"))
        //        {
        //            int.TryParse(img.Attributes["width"].Value, out width);
        //        }

        //        if (img.Attributes.Contains("src"))
        //        {
        //            var src = img.Attributes["src"].Value;

        //            if (string.IsNullOrWhiteSpace(src))
        //            {
        //                img.Remove();
        //            }

        //            if (__isFromMediaServer(src))
        //            {
        //                if (width != -1 && height != -1)
        //                {
        //                    var newSrc = ImageMaxSizePath(src, width, height);
        //                    img.Attributes["src"].Value = newSrc;
        //                }
        //            }
        //            else
        //            {
        //                listForUpload.Add(new Tuple<HtmlNode, int, int>(img, width, height));
        //            }
        //        }
        //        else
        //        {
        //            isHasRemoveImg = true;
        //            img.Remove();
        //        }
        //    }

        //    if (listForUpload.Count() > 0)
        //    {
        //        var response = rpFile.UploadFileFromURL(listForUpload.Select(m => m.Item1.Attributes["src"].Value).ToArray());

        //        for (int n = 0; n < listForUpload.Count(); n++)
        //        {
        //            if (response[n].IsError)
        //            {
        //                isHasRemoveImg = true;
        //                listForUpload[n].Item1.Remove();
        //            }
        //            else
        //            {

        //                if (listForUpload[n].Item2 != -1 && listForUpload[n].Item3 != -1)
        //                {
        //                    listForUpload[n].Item1.Attributes["src"].Value = ImageMaxSizePath(response[n].Data, listForUpload[n].Item2, listForUpload[n].Item3);
        //                    listForUpload[n].Item1.Attributes["src"].Value = ImageMaxSizePath(response[n].Data, listForUpload[n].Item2, listForUpload[n].Item3);
        //                }
        //                else
        //                {
        //                    listForUpload[n].Item1.Attributes["src"].Value = response[n].Data;
        //                    listForUpload[n].Item1.Attributes["src"].Value = response[n].Data;
        //                }
        //            }
        //        }
        //    }


        //    if (isHasRemoveImg)
        //    {
        //        rp.Infos.Add("Chú ý, trong quá trình xử lý có loại bỏ một số ảnh không thể xử lý");
        //    }

        //    //remove external link

        //    IEnumerable<HtmlNode> linkNodes = (from n in allNodes.DescendantsAndSelf()
        //                                       where n.Name == "a"
        //                                       select n);

        //    foreach (var link in linkNodes.ToArray())
        //    {
        //        if (!link.HasAttributes) continue;

        //        if (link.Attributes.Contains("href"))
        //        {
        //            var url = link.Attributes["href"].Value;

        //            if (url != null)
        //            {
        //                var result = Uri.IsWellFormedUriString(url, UriKind.Absolute);
        //                if (result)
        //                {
        //                    var uri = new Uri(url);
        //                    if (uri != null && !uri.Host.Contains("sendo.vn"))
        //                        link.Attributes["href"].Value = string.Empty;
        //                }
        //            }
        //        }
        //    }


        //    rp.Data = allNodes.InnerHtml;

        //    return rp;
        //}

        private static bool __isFromMediaServer(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            return url.IndexOf("batdongsanhanhphuc.vn") > 0;
        }
    }
}