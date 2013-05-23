using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Codeable.Foundation.Web.Core.Validation
{
    [AttributeUsage (AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.Parameter, AllowMultiple = false)]
	public class CaseInsensitiveRegularExpressionAttribute : ValidationAttribute
	{
        public CaseInsensitiveRegularExpressionAttribute(string pattern)
			: base(GetDefaultErrorMessage)
		{
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			Pattern = pattern;
		}

		public string Pattern { get; private set; }

		static string GetDefaultErrorMessage ()
		{
			return "The field {0} must match the regular expression {1}.";
		}

		public override string FormatErrorMessage (string name)
		{
			return string.Format(ErrorMessageString, name, Pattern);
		}

		public override bool IsValid (object value)
		{
			if (value == null) 
				return true;

			string str = (string) value;
			Regex regex = new Regex(Pattern, RegexOptions.IgnoreCase);
			return regex.IsMatch(str);
		}
	}
}
