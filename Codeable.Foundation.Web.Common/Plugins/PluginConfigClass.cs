using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Codeable.Foundation.Common.Plugins;

namespace Codeable.Foundation.UI.Web.Common.Plugins
{
    public class PluginConfigClass : IPluginConfigClass
    {
        #region IPluginConfigClass Members

        public void DeserializeFrom(IDictionary<string, string> payload)
        {
            if (payload != null)
            {
                 PropertyInfo[] properties = this.GetType().GetProperties();
                 foreach (PropertyInfo item in properties)
                 {
                     if (payload.ContainsKey(item.Name))
                     {
                         string value = payload[item.Name];
                         try
                         {
                             // Exact
                             item.SetValue(this, value, null);
                             continue;
                         }
                         catch { }

                         // try parsing
                         object parsedValue = null;
                         try
                         {
                             parsedValue = ParseValue(item, value);
                         }
                         catch
                         {
                             continue; // can't parse, ah well.
                         }
                         try
                         {
                             // Parse then Exact
                             item.SetValue(this, parsedValue, null);
                             continue;
                         }
                         catch { }

                         try
                         {
                             // Parse then Coerce
                             item.SetValue(this, Convert.ChangeType(parsedValue, item.PropertyType), null);
                             continue;
                         }
                         catch { }
                     }
                 }
            }
        }
        public void SerializeInto(IDictionary<string, string> payload)
        {
            if (payload != null)
            {
                PropertyInfo[] properties = this.GetType().GetProperties();
                foreach (PropertyInfo item in properties)
                {
                    string stringValue = string.Empty;
                    object rawValue = item.GetValue(this, null);
                    if (rawValue != null)
                    {
                        stringValue = rawValue.ToString();
                    }
                    payload[item.Name] = stringValue;
                }
            }
        }

        #endregion

        protected virtual object ParseValue(PropertyInfo pInfo, object value)
        {
            if (value == null) { return null; }
            if (pInfo.PropertyType == typeof(bool))
            {
                return bool.Parse(value.ToString());
            }
            else if ((pInfo.PropertyType == typeof(long))
                    || (pInfo.PropertyType == typeof(short))
                    || (pInfo.PropertyType == typeof(int)))
            {
                return long.Parse(value.ToString(), System.Globalization.NumberStyles.Any);
            }
            else if ((pInfo.PropertyType == typeof(DateTime))
                || (pInfo.PropertyType.IsGenericType)) // assume datetime
            {
                return DateTime.Parse(value.ToString());
            }
            else if (pInfo.PropertyType == typeof(decimal))
            {
                // % fix
                string valueToParse = value.ToString();
                if (valueToParse.Contains("%"))
                {
                    return decimal.Parse(valueToParse.TrimEnd(new char[] { '%', ' ' })) / 100M;
                }
                return decimal.Parse(valueToParse, System.Globalization.NumberStyles.Any);
            }
            else if (pInfo.PropertyType == typeof(double))
            {
                return double.Parse(value.ToString(), System.Globalization.NumberStyles.Any);
            }
            else if (pInfo.PropertyType == typeof(float))
            {
                return float.Parse(value.ToString(), System.Globalization.NumberStyles.Any);
            }
            else
            {
                return value;
            }
        }
    }
}
