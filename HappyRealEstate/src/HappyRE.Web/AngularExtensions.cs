using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Properties;
using System.Text;

namespace HappyRE.Web
{
    public static class AngularExtensions
    {
        internal static readonly Hashtable HValidationRules = Hashtable.Synchronized(new Hashtable());
        private static readonly Dictionary<string, string> ValidationAttributes = LoadValidationAttributes();
        private static Dictionary<string, string> LoadValidationAttributes()
        {
            return new Dictionary<string, string>()
            {
                // message
                {"msg-required","required" },
                {"msg-minlength-min","minlength" },
                {"msg-maxlength-max","maxlength" },
                {"msg-regex-pattern","pattern" },
                {"msg-length-min","minlength" },
                {"msg-length-max","maxlength" },
                {"msg-range-min","min" },
                {"msg-range-max","max" },
                // attribute
                {"required","required" },
                {"required-val","required" },
                {"minlength-min","ng-minlength" },
                {"maxlength-max","ng-maxlength" },
                {"regex-pattern","pattern" },
                {"length-min","ng-minlength" },
                {"length-max","ng-maxlength" },
                {"range-min","min" },
                {"range-max","max" }
            };
        }

        private static FieldValidationMetadata GetFieldValidationMetadata(HtmlHelper htmlHelper, string name)
        {
            string key = htmlHelper.ViewData.ModelMetadata.ModelType.FullName + "." + name;
            FieldValidationMetadata fieldMetadata = null;
            if (HValidationRules.ContainsKey(key) == false)
            {
                FormContext formContext = htmlHelper.ViewContext.FormContext;
                fieldMetadata = formContext.GetValidationMetadataForField(name, true /* createIfNotFound */);
                IEnumerable<ModelValidator> validators = ModelValidatorProviders.Providers.GetValidators(ModelMetadata.FromStringExpression(name, htmlHelper.ViewData), htmlHelper.ViewContext);
                foreach (ModelClientValidationRule rule in validators.SelectMany(v => v.GetClientValidationRules()))
                {
                    fieldMetadata.ValidationRules.Add(rule);
                }
                try
                {
                    HValidationRules.Add(key, fieldMetadata);
                }
                catch { }
            }
            else
            {
                fieldMetadata = (FieldValidationMetadata)HValidationRules[key];
            }

            return fieldMetadata;
        }

        #region ValidationMessage
        public static MvcHtmlString AngularMessage(this HtmlHelper htmlHelper, string name) {
            return AngularMessage(htmlHelper, name, null, null);
        }
        public static MvcHtmlString AngularMessage(this HtmlHelper htmlHelper, string name, Dictionary<string, string> attributes = null) {
            return AngularMessage(htmlHelper, name, null, attributes);
        }
        public static MvcHtmlString AngularMessage(this HtmlHelper htmlHelper, string name, string formName) {
            return AngularMessage(htmlHelper, name, formName, null);
        }
        public static MvcHtmlString AngularMessage(this HtmlHelper htmlHelper, string name, string formName, Dictionary<string, string> attributes = null, bool submitOnly = false)
        {
            FieldValidationMetadata fieldMetadata = GetFieldValidationMetadata(htmlHelper, name);
            if (fieldMetadata == null || fieldMetadata.ValidationRules == null)
            {
                return MvcHtmlString.Empty;
            }
            string atts = string.Empty;
            if (attributes != null)
            {
                foreach (var item in attributes) atts += " " + item.Key + "=\"" + item.Value + "\"";
            }
            formName = (formName ?? "myForm");
            StringBuilder sb = new StringBuilder();
			if (submitOnly)
			{
				sb.AppendFormat("<div ng-messages=\"{0}.{1}.$error\" ng-if=\"{0}.$submitted\"{2}>", formName, name, atts);
			}
			else
			{
				sb.AppendFormat("<div ng-messages=\"{0}.{1}.$error\" ng-if=\"{0}.{1}.$touched || {0}.$submitted\"{2}>", formName, name, atts);
			}
            foreach (var item in fieldMetadata.ValidationRules)
            {
                string k = "msg-" + item.ValidationType;
                if (item.ValidationParameters == null || item.ValidationParameters.Count == 0)
                {
                    if (ValidationAttributes.ContainsKey(k) == false) continue;
                    sb.AppendFormat("\r\n\t<div ng-message=\"{0}\" class=\"validMessage\">{1}</div>", ValidationAttributes[k], item.ErrorMessage);
                }
                else
                {
                    foreach (var p in item.ValidationParameters)
                    {
                        k = "msg-" + item.ValidationType + "-" + p.Key;
                        if (ValidationAttributes.ContainsKey(k) == false) continue;
                        sb.AppendFormat("\r\n\t<div ng-message=\"{0}\" class=\"validMessage\">{1}</div>", ValidationAttributes[k], item.ErrorMessage);
                    }
                }
            }
            sb.Append("\r\n</div>");

            return MvcHtmlString.Create(sb.ToString());
        }
        #endregion
       
        #region ValidationAttibute
        public static Dictionary<string, object> AngularAttibute(this HtmlHelper htmlHelper, string name) {
            return AngularAttibute(htmlHelper, name, null, null);
        }
        public static Dictionary<string, object> AngularAttibute(this HtmlHelper htmlHelper, string name, string modelName) {
            return AngularAttibute(htmlHelper, name, modelName, null);
        }
        public static Dictionary<string, object> AngularAttibute(this HtmlHelper htmlHelper, string name, Dictionary<string, string> attributes) {
            return AngularAttibute(htmlHelper, name, null, attributes);
        }
        public static Dictionary<string, object> AngularAttibute(this HtmlHelper htmlHelper, string name, string modelName, Dictionary<string, string> attributes)
        {
            Dictionary<string, object> res = null;

            FieldValidationMetadata fieldMetadata = GetFieldValidationMetadata(htmlHelper, name);
            if (fieldMetadata == null || fieldMetadata.ValidationRules == null)
            {
                return res;
            }
            res = new Dictionary<string, object>();
            res.Add("ng-model", (string.IsNullOrEmpty(modelName) ? "data" : modelName) + "." + name);
            foreach (var item in fieldMetadata.ValidationRules)
            {
                string k = item.ValidationType;
                if (item.ValidationParameters == null || item.ValidationParameters.Count == 0)
                {
                    if (ValidationAttributes.ContainsKey(k) == false) continue;
                    res.Add(ValidationAttributes[k], ValidationAttributes[k + "-val"]);
                }
                else
                {
                    foreach (var p in item.ValidationParameters)
                    {
                        k = item.ValidationType + "-" + p.Key;
                        if (ValidationAttributes.ContainsKey(k) == false) continue;
                        res.Add(ValidationAttributes[k], p.Value);
                    }
                }
            }

            // attributes
            if (attributes != null)
            {
                foreach(var item in attributes)
                {
                    res[item.Key] = item.Value;
                }
            }

            return res;
        }
        #endregion
    }
}