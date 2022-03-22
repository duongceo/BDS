using MBN.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Serialization;

namespace HappyRE.Web.Helpers
{
    [Serializable]
    public class RouteConfig
    {
        const string FILE_NAME = "route.config";
        RouteConfigItem[] _items;
        Hashtable HItems = new Hashtable();
        private static object runningLock = new object();
        private static RouteConfig _Instance = null;
        public static RouteConfig Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = Load(string.Empty);
                }
                return _Instance;
            }
        }
        public static RouteConfig Load(string fileName)
        {
            RouteConfig _conf = null;

            if (fileName == string.Empty)
            {
                fileName = FILE_NAME;
            }
            if (fileName.IndexOf(":", 0, 3) == -1)
            {
                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            }
            try
            {
                if (!File.Exists(fileName))
                {
                    RouteConfig obj = new RouteConfig();

                    // stop 
                    RouteConfigItem objItem1 = new RouteConfigItem();
                    objItem1.Name = "Resource";
                    objItem1.Url = "{resource}.axd/{*pathInfo}";
                    objItem1.IgnoreRoute = true;
                    objItem1.Enabled = true;

                    // stop 
                    RouteConfigItem objItem2 = new RouteConfigItem();
                    objItem2.Name = "Service";
                    objItem2.Url = "{service}.asmx/{*pathInfo}";
                    objItem2.IgnoreRoute = true;
                    objItem2.Enabled = true;

                    // Sample
                    RouteConfigItem objItem = new RouteConfigItem();
                    objItem.Name = "Default";
                    objItem.Url = "{controller}/{action}/{id}";
                    objItem.Controller = "Home";
                    objItem.Action = "Index";
                    objItem.Enabled = true;
                    objItem.Params = new RouteParamItem[] { new RouteParamItem() { Name = "id", Value = "1" } };
                    obj.Items = new RouteConfigItem[] { objItem1, objItem2, objItem };

                    XmlSerializer s = new XmlSerializer(typeof(RouteConfig));
                    StreamWriter w = new StreamWriter(fileName);
                    s.Serialize(w, obj);
                    w.Close();
                    w = null;

                }

                StreamReader reader = null;
                try
                {
                    lock (runningLock)
                    {
                        reader = new StreamReader(fileName);
                        XmlSerializer ser = new XmlSerializer(typeof(RouteConfig));
                        _conf = (RouteConfig)ser.Deserialize(reader);
                        reader.Close();
                        reader = null;
                        HttpRuntime.Cache.Add(FILE_NAME, true, new CacheDependency(fileName),
                            System.Web.Caching.Cache.NoAbsoluteExpiration,
                            System.Web.Caching.Cache.NoSlidingExpiration,
                            CacheItemPriority.High,
                            new CacheItemRemovedCallback(LoadCallback));

                    }
                }
                catch (Exception ex)
                {
                    _conf = null;
                    MBN.Utils.WebLog.Log.Error("RouteConfig: " + fileName, ex.Message);
                }
                finally
                {
                    if (reader != null) reader.Close();
                }

            }
            catch (Exception ex)
            {
                _conf = null;
                MBN.Utils.WebLog.Log.Error("RouteConfig: " + fileName, ex.Message);
            }
            if (_conf == null) _conf = new RouteConfig();

            return _conf;
        }
        private static void LoadCallback(string key, object v, CacheItemRemovedReason reason)
        {
            if (reason == CacheItemRemovedReason.DependencyChanged)
            {
                _Instance = null;
                try
                {
                    Instance.RegisterRoutes(RouteTable.Routes); // Re Register
                }
                catch (Exception ex)
                {
                    MBN.Utils.WebLog.Log.Error("RouteConfig-LoadCallback", ex.Message);
                }
            }
        }



        public RouteConfig()
        {
            // Load
        }

        private RouteCollection _routes = null;

        /// <summary>
        /// Đăng ký Routing (Call in Application_Start)
        /// </summary>
        /// <param name="routes"></param>
        public void RegisterRoutes(RouteCollection routes)
        {
            _routes = routes;
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            foreach (RouteConfigItem item in Items)
            {
                if (!item.Enabled) continue;
                if (item.IgnoreRoute)
                {
                    _routes.IgnoreRoute(item.Url);
                }
                else
                {
                    routeValues = new System.Web.Routing.RouteValueDictionary();
                    routeValues.Add("controller", item.Controller);
                    routeValues.Add("action", item.Action);
                    if (item.Params != null && item.Params.Length > 0)
                    {
                        foreach (RouteParamItem p in item.Params)
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(p.Value))
                                {
                                    routeValues.Add(p.Name, UrlParameter.Optional);
                                }
                                else
                                {
                                    routeValues.Add(p.Name, p.Value);
                                }
                            }
                            catch (Exception ex)
                            {
                                WebLog.Log.Error("RegisterRoutes-RoutingName:" + p.Name, ex.Message);
                            }

                        }
                    }

					// Constraints - (?<CityUrl>.*)/(?<DistrictUrl>.*)/mua\-.*/.*-(prj(?<ProjectId>\d+)|wid(?<WardId>\d+)|sid(?<StreetId>\d+)|pid(?<PlaceId>\d+))
					var route = new Route(item.Url, new MvcRouteHandler()) { Defaults = routeValues };
					if (string.IsNullOrEmpty(item.Constraints) == false)
					{
						var contraints = new RouteValueDictionary();
						contraints.Add("url", new RegexNamedGroupRoutingConstraint(item.Constraints.Split('#')));
						route.Url = "{*url}";
						route.Constraints = contraints;
					}

					if (routes[item.Name] != null)
					{
						routes.Remove(routes[item.Name]);
					}
					routes.Add(item.Name, route);

                    //routes.MapRoute(
                    //    item.Name, // Route name
                    //    item.Url,// URL with parameters
                    //    new { controller = item.Controller, action = item.Action, item.Url = item.Action, codeUrl = UrlParameter.Optional } // Parameter defaults
                    //    );
                }
            }
        }

        [XmlElement("Routes", typeof(RouteConfigItem[]))]
        public RouteConfigItem[] Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public RouteConfigItem this[string name]
        {
            get
            {
                return GetRouteConfigItem(name);
            }
        }

        private RouteConfigItem GetRouteConfigItem(string name)
        {
            if (Items != null)
            {
                if (HItems.Count == 0)
                {
                    Hashtable h = new Hashtable();
                    foreach (RouteConfigItem item in Items)
                    {
                        h.Add(item.Name, item);
                    }
                    HItems = h;
                }
                object result = HItems[name];
                return (result == null ? null : (result as RouteConfigItem));
            }
            return null;
        }
    }

    [Serializable]
    public class RouteConfigItem
    {
        private string _name = string.Empty;
        private string _url = string.Empty;
        private string _controller = string.Empty;
        private string _action = string.Empty;
        private bool _ignoreRoute = false;
        private bool _enabled = true;
		private string _constraints = string.Empty;

		[XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute("Url")]
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        [XmlAttribute("Controller")]
        public string Controller
        {
            get { return _controller; }
            set { _controller = value; }
        }

        [XmlAttribute("Action")]
        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }

        [XmlAttribute("IgnoreRoute")]
        public bool IgnoreRoute
        {
            get { return _ignoreRoute; }
            set { _ignoreRoute = value; }
        }

        [XmlAttribute("Enabled")]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        private RouteParamItem[] _params = null;
        [XmlElement("Params", typeof(RouteParamItem[]))]
        public RouteParamItem[] Params
        {
            get { return _params; }
            set { _params = value; }
        }

		[XmlAttribute("Constraints")]
		public string Constraints
		{
			get { return _constraints; }
			set { _constraints = value; }
		}
	}

    [Serializable]
    public class RouteParamItem
    {
        private string _name = string.Empty;
        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _value = string.Empty;
        [XmlAttribute("Value")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

    }


	public class RegexNamedGroupRoutingConstraint : IRouteConstraint
	{
		private readonly List<Regex> regexes = new List<Regex>();

		public RegexNamedGroupRoutingConstraint(params string[] regexes)
		{
			foreach (var regex in regexes)
			{
				this.regexes.Add(new Regex(regex.Trim(), RegexOptions.IgnoreCase));
			}
		}

		public bool Match(
			HttpContextBase httpContext, 
			Route route, 
			string parameterName, 
			RouteValueDictionary values, 
			RouteDirection routeDirection)
		{
			if (values[parameterName] == null)
			{
				return false;
			}

			var url = values[parameterName].ToString();

			foreach (var regex in regexes)
			{
				var match = regex.Match(url);
				if (match.Success)
				{
					
					foreach (Group group in match.Groups)
					{
						string name = ((dynamic)group).Name;
						values.Add(name, group.Value);
					}
					return true;
				}
			}

			return false;
		}
	}
}