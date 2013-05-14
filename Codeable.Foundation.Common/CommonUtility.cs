using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace Codeable.Foundation.Common
{
    public static class CommonUtility
    {
        #region Assembly Information

        private static Dictionary<string, string> _assemblyInformation = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public static string GetInformationalVersion(Assembly assembly)
        {
            string result = string.Empty;
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            if (attributes.Length > 0)
            {
                result = ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion.ToString();
            }
            return result;
        }

        #endregion

        #region Stream Functions

        public static void CopyStreamNonDisposed(Stream input, Stream output)
        {
            CopyStreamNonDisposed(input, output, 0x1000);//4kb
        }
        public static void CopyStreamNonDisposed(Stream input, Stream output, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        #endregion

        #region Format Helpers
        
        public static string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);
                }
                max /= scale;
            }
            return "0 Bytes";
        }
        public static string FormatTimeAgo(DateTime dateTime)
        {
            StringBuilder sb = new StringBuilder();
            DateTime now = DateTime.Now;
            TimeSpan timespan = now - dateTime;

            if (now <= dateTime)
            {
                sb.Append("in ");
                timespan = dateTime - now;
            }

            // A year or more?  Do "[Y] years and [M] months ago"
            if ((int)timespan.TotalDays >= 365)
            {
                // Years
                int nYears = (int)timespan.TotalDays / 365;
                sb.Append(nYears);
                if (nYears > 1)
                    sb.Append(" years");
                else
                    sb.Append(" year");

                // Months
                int remainingDays = (int)timespan.TotalDays - (nYears * 365);
                int nMonths = remainingDays / 30;
                if (nMonths == 1)
                    sb.Append(" and ").Append(nMonths).Append(" month");
                else if (nMonths > 1)
                    sb.Append(" and ").Append(nMonths).Append(" months");
            }
            // More than 60 days? (appx 2 months or 8 weeks)
            else if ((int)timespan.TotalDays >= 60)
            {
                // Do months
                int nMonths = (int)timespan.TotalDays / 30;
                sb.Append(nMonths).Append(" months");
            }
            // Weeks? (7 days or more)
            else if ((int)timespan.TotalDays >= 7)
            {
                int nWeeks = (int)timespan.TotalDays / 7;
                sb.Append(nWeeks);
                if (nWeeks == 1)
                    sb.Append(" week");
                else
                    sb.Append(" weeks");
            }
            // Days? (1 or more)
            else if ((int)timespan.TotalDays >= 1)
            {
                int nDays = (int)timespan.TotalDays;
                sb.Append(nDays);
                if (nDays == 1)
                    sb.Append(" day");
                else
                    sb.Append(" days");
            }
            // Hours?
            else if ((int)timespan.TotalHours >= 1)
            {
                int nHours = (int)timespan.TotalHours;
                sb.Append(nHours);
                if (nHours == 1)
                    sb.Append(" hour");
                else
                    sb.Append(" hours");
            }
            // Minutes?
            else if ((int)timespan.TotalMinutes >= 1)
            {
                int nMinutes = (int)timespan.TotalMinutes;
                sb.Append(nMinutes);
                if (nMinutes == 1)
                    sb.Append(" minute");
                else
                    sb.Append(" minutes");
            }
            // Seconds?
            else if ((int)timespan.TotalSeconds >= 1)
            {
                int nSeconds = (int)timespan.TotalSeconds;
                sb.Append(nSeconds);
                if (nSeconds == 1)
                    sb.Append(" second");
                else
                    sb.Append(" seconds");
            }
            // Just say "1 second" as the smallest unit of time
            else
            {
                sb.Append("1 second");
            }

            if (now > dateTime)
            {
                sb.Append(" ago");
            }


            // For anything more than 6 months back, put " ([Month] [Year])" at the end, for better reference
            if ((int)timespan.TotalDays >= 30 * 6)
            {
                sb.Append(" (" + dateTime.ToString("MMMM") + " " + dateTime.Year + ")");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Very ugly mechanism to truncate html.. should use an html library
        /// </summary>
        public static string TruncateHTMLFuzzy(string html, int visibleCharacterCount, bool emptyIfUnderThreshold)
        {
            
            string result = string.Empty;
            string visibleWords = string.Empty;
            string wrapToken = "CF";

            html = string.Format("<{0}>{1}</{0}>", wrapToken, html);

            MatchCollection words = Regex.Matches(html, @"<(.|\n)*?>");
            bool wasTruncated = false;
            int lastIndex = -1;
            List<string> tagOrder = new List<string>();
            foreach (Match match in words)
            {
                if (match.Success)
                {
                    Match tagMatch = Regex.Match(match.Value, @"<([/a-z0-9]+?)[\s>]", RegexOptions.IgnoreCase);
                    if (tagMatch.Success)
                    {
                        string rawTag = tagMatch.Groups[1].Value;
                        bool isCloseTag = tagMatch.Groups[1].Value.Contains("/");
                        if (isCloseTag)
                        {
                            int ix = tagOrder.LastIndexOf(rawTag.Replace("/",string.Empty));
                            if (ix >= 0)
                            {
                                tagOrder.RemoveAt(ix);
                            }
                        }
                        else
                        {
                            tagOrder.Add(rawTag);
                        }
                        if ((lastIndex != -1) && (lastIndex != match.Index))
                        {
                            string wordsToAdd = html.Substring(lastIndex, (match.Index - lastIndex));
                            if ((visibleWords.Length + wordsToAdd.Length) > visibleCharacterCount)
                            {
                                wordsToAdd = wordsToAdd.Substring(0, (wordsToAdd.Length - ((visibleWords.Length + wordsToAdd.Length) - visibleCharacterCount)));
                            }
                            visibleWords += wordsToAdd;
                            result += wordsToAdd;
                        }
                    }
                    result += match.Value;
                    lastIndex = (match.Index + match.Length);
                }
                if (visibleWords.Length >= visibleCharacterCount)
                {
                    wasTruncated = true;
                    break;
                }
            }

            if (emptyIfUnderThreshold && !wasTruncated)
            {
                return string.Empty;
            }
            for (int i = tagOrder.Count - 1; i >= 0; i--)
            {
                result += "</" + tagOrder[i] + ">";
            }

            return result.Replace(string.Format("<{0}>", wrapToken), string.Empty).Replace(string.Format("</{0}>", wrapToken), string.Empty);
        }

        #endregion

        public static string FromBase64(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            byte[] encodedDataAsBytes = global::System.Convert.FromBase64String(data);
            return global::System.Text.ASCIIEncoding.UTF8.GetString(encodedDataAsBytes);
        }
        public static string ToBase64(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            byte[] bytes = global::System.Text.ASCIIEncoding.UTF8.GetBytes(text);
            return global::System.Convert.ToBase64String(bytes);
        }
        public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek)
        {
            int diff = dateTime.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dateTime.AddDays(-1 * diff).Date;
        }
    }
}
