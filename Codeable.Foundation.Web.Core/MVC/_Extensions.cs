using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using System.ComponentModel;

namespace Codeable.Foundation.UI.Web.Core.MVC
{
    public static class _Extensions
    {
        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }
        
        public static SelectList MakeSelection(this SelectList list, object selection)
        {
            return new SelectList(list.Items, list.DataValueField, list.DataTextField, selection);
        }

        public static SelectList ToSelectList(this Enum enumObj)
        {
            return enumObj.ToSelectList(true);
        }

        public static SelectList ToSelectList(this Enum enumObj, bool useTextAsValue)
        {
            var values = from Enum e in Enum.GetValues(enumObj.GetType())
                         select new { Id = (useTextAsValue) ? GetDescription(e) : (object)((int)(object)e), Name = GetDescription(e) };

            return new SelectList(values, "Id", "Name", enumObj);
        }
    }
}
