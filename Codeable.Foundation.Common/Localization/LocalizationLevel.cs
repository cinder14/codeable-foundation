using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common.Localization
{
    public enum LocalizationLevel
    {
        /// <summary>
        /// Undefined level, catch all.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Translations for data stored in DomainModel classes
        /// </summary>
        DomainModel = 1,
        /// <summary>
        /// Translations for data stored in ViewModel classes
        /// </summary>
        [Obsolete("I believe this is not required, we can piggy back on domain model. Perhaps another to determine the diff?", true)]
        ViewModel = 2,
        /// <summary>
        /// Translastions for text found in a UI
        /// </summary>
        UIText = 3,
        /// <summary>
        /// Custom area, specific per use
        /// </summary>
        Custom = 4
    }
}
