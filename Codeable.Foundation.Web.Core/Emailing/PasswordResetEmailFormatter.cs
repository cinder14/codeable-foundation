using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codeable.Foundation.Common.Emailing;
using Codeable.Foundation.UI.Web.Core;
using Codeable.Foundation.UI.Web.Common.Authentication;

namespace Codeable.Foundation.UI.Web.Core.Emailing
{
    public class PasswordResetEmailFormatter
    {
        public const string EMAIL_ADDRESS = "%EMAILADDRESS%";
        public const string USER_ID = "%USERID%";
        public const string USER_ACCOUNT = "%USERACCOUNT%";
        public const string USER_NAME = "%USERNAME%";
        public const string NEW_PASSWORD = "%NEWPASSWORD%";


        public virtual string[] GetAvailableTokens()
        {
            return new string[] { EMAIL_ADDRESS, USER_ID, USER_ACCOUNT, USER_NAME, NEW_PASSWORD };
        }

        public IEmail FormatEmail(IEmail email, EmailTemplate template, IEmailRecipient recipient, IUserProviderInfo userInfo, string newPassword)
        {
            email.InternalMessageType = WebAssumptions.EMAIL_TYPE_PASSWORDRESET;
            email.InternalTypeID = userInfo.UserEmail;
            email.FromName = ReplaceTokens(template.FromName, template, recipient, userInfo, newPassword);
            email.FromEmail = ReplaceTokens(template.FromEmail, template, recipient, userInfo, newPassword);
            email.Subject = ReplaceTokens(template.Subject, template, recipient, userInfo, newPassword);
            email.HTMLBody = ReplaceTokens(template.HTMLBody, template, recipient, userInfo, newPassword);
            return email;
        }

        protected virtual string ReplaceTokens(string template, EmailTemplate emailTemplate, IEmailRecipient recipient, IUserProviderInfo userInfo, string newPassword)
        {
            template = template.Replace(EMAIL_ADDRESS, userInfo.UserEmail);
            template = template.Replace(USER_ID, userInfo.UserId);
            template = template.Replace(USER_ACCOUNT, userInfo.UserAccountName);
            template = template.Replace(USER_NAME, userInfo.UserName);
            template = template.Replace(NEW_PASSWORD, newPassword);
            return template;
        }
    }

}