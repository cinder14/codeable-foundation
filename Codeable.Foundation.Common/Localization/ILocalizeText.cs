using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Localization
{
    public interface ILocalizeText
    {
        string I18N(string token, string englishDefaultValue);
    }
}
