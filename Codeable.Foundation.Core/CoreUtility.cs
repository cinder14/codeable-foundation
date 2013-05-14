using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Security;
using System.Text.RegularExpressions;

namespace Codeable.Foundation.Core
{
    public static class CoreUtility
    {

        #region Public Property Comparison

        public static bool AreSameTypePublicInstancePropertiesEqual<T>(T left, T right, params string[] ignore) where T : class
        {
            return AreSameTypePublicInstancePropertiesEqual<T, T>(left, right, ignore);
        }
        public static bool AreSameTypePublicInstancePropertiesEqual<TLeft, TRight>(TLeft left, TRight right, params string[] ignore) where TLeft : class where TRight : class
        {
            if (left != null && right != null)
            {
                Type leftType = typeof(TLeft);
                Type rightType = typeof(TRight);
                Dictionary<string, PropertyInfo> leftProperties = leftType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
                Dictionary<string, PropertyInfo> rightProperties = rightType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
                List<string> ignoreList = new List<string>(ignore);
                foreach (PropertyInfo pi in leftProperties.Values)
                {
                    if (!ignoreList.Contains(pi.Name) && rightProperties.ContainsKey(pi.Name))
                    {
                        PropertyInfo leftProp = leftType.GetProperty(pi.Name);
                        PropertyInfo rightProp = rightType.GetProperty(pi.Name);
                        if(leftProp.PropertyType == rightProp.PropertyType)
                        {
                            object leftValue = leftProp.GetValue(left, null);
                            object rightValue = rightProp.GetValue(right, null);
                            if (leftProp.PropertyType == typeof(DateTime) || (leftProp.PropertyType.IsGenericType && leftProp.PropertyType.GetGenericArguments()[0] == typeof(DateTime) ))
                            {
                                // too many issues can cause datetime difference, just focus on the big stuff
                                bool match = (((DateTime)leftValue).ToString("g") == ((DateTime)rightValue).ToString("g"));
                                if (!match)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (leftValue != rightValue && (leftValue == null || !leftValue.Equals(rightValue)))
                                {
                                    return false;
                                }
                            }
                        }
                        
                    }
                }
                return true;
            }
            return left == right;
        }

        #endregion

        #region Serialization

        public static string SerializeToXml<T>(T item)
        {
            if (item == null) { return string.Empty; }
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (XmlTextWriter tw = new XmlTextWriter(sw))
                {
                    tw.Formatting = Formatting.Indented;
                    new XmlSerializer(typeof(T)).Serialize(tw, item);
                }
            }
            return sb.ToString();
        }
        public static void SerializeToXml<T>(T item, FileInfo outFile) where T : new()
        {
            if (item == null) { item = new T(); }
            using (TextWriter tw = new StreamWriter(outFile.FullName))
            {
                using (XmlTextWriter xw = new XmlTextWriter(tw))
                {
                    xw.Formatting = Formatting.Indented;
                    new XmlSerializer(typeof(T)).Serialize(xw, item);
                }
            }
        }

        public static T DeserializeFromXml<T>(string serializedXML) where T : new()
        {
            using (StringReader sr = new StringReader(serializedXML))
            {
                using (XmlReader xr = new XmlTextReader(sr))
                {
                    return (T)new XmlSerializer(typeof(T)).Deserialize(xr);
                }
            }
        }
        public static T DeserializeFromXml<T>(FileInfo inFile) where T : new()
        {
            using (TextReader tr = new StreamReader(inFile.FullName))
            {
                using (XmlReader xr = new XmlTextReader(tr))
                {
                    return (T)new XmlSerializer(typeof(T)).Deserialize(xr);
                }
            }
        }

        public static T XmlClone<T>(T item) where T : new()
        {
            if (item == null) { return item; }
            return DeserializeFromXml<T>(SerializeToXml<T>(item));
        }
        #endregion

        #region Exception Helpers
        public static string FormatException(Exception ex)
        {
            return FormatException(ex, string.Empty);
        } 
        public static string FormatException(Exception ex, string tag)
        {
            string message = string.Format("<Exception tag=\"{3}\" message=\"{0}\" type=\"{1}\" stack=\"{2}\">", SecurityElement.Escape(ex.Message), ex.GetType().ToString(), SecurityElement.Escape(ex.StackTrace), tag);
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                message += string.Format("\r\n<InnerException message=\"{0}\" type=\"{1}\" stack=\"{2}\" />\r\n", SecurityElement.Escape(ex.Message), ex.GetType().ToString(), SecurityElement.Escape(ex.StackTrace));
            }
            message += "</Exception>";
            return message;
        } 
        #endregion
        
        #region File Helpers

        public static string[] UnsafeExtensions = new string[] { ".ade", ".adp", ".app", ".asa", ".ashx", ".asmx", ".asp", ".bas", ".bat", ".cdx", ".cer", ".chm", ".class", ".cmd", ".com", ".config", ".cpl", ".crt", ".csh", ".dll", ".exe", ".fxp", ".hlp", ".hta", ".htr", ".htw", ".ida", ".idc", ".idq", ".ins", ".isp", ".its", ".jse", ".ksh", ".lnk", ".mad", ".maf", ".mag", ".mam", ".maq", ".mar", ".mas", ".mat", ".mau", ".mav", ".maw", ".mda", ".mdb", ".mde", ".mdt", ".mdw", ".mdz", ".msc", ".msh", ".msh1", ".msh1xml", ".msh2", ".msh2xml", ".mshxml", ".msi", ".msp", ".mst", ".ops", ".pcd", ".pif", ".prf", ".prg", ".printer", ".pst", ".reg", ".rem", ".scf", ".scr", ".sct", ".shb", ".shs", ".shtm", ".shtml", ".soap", ".stm", ".url", ".vb", ".vbe", ".vbs", ".ws", ".wsc", ".wsf", ".wsh" };

        public static string CleanFileName(string fileName)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()) + "&?=";
            foreach (char c in invalid)
            {
                fileName = fileName.Replace(c.ToString(), "");
            }
            return fileName;
        } 
        public static string CleanPhoneNumber(string phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                return Regex.Replace(phoneNumber, @"[\W]", string.Empty);
            }
            return string.Empty;
        }
        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                phoneNumber = CleanPhoneNumber(phoneNumber);
                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    if (phoneNumber.Length == 11)
                    {
                        phoneNumber = string.Format("{0}-{1}-{2}-{3}", phoneNumber.Substring(0,1), phoneNumber.Substring(1, 3), phoneNumber.Substring(4,3), phoneNumber.Substring(7));
                    }
                    if (phoneNumber.Length == 10)
                    {
                        phoneNumber = string.Format("{0}-{1}-{2}", phoneNumber.Substring(0,3), phoneNumber.Substring(3,3), phoneNumber.Substring(6));
                    }
                    if (phoneNumber.Length == 7)
                    {
                        phoneNumber = string.Format("{0}-{1}", phoneNumber.Substring(0,3), phoneNumber.Substring(3));
                    }
                }
            }
            return phoneNumber;
        } 
        #endregion

    }
}
