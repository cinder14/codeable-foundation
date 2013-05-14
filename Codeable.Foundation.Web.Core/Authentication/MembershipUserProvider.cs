using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeable.Foundation.Common;
using System.Web.Security;
using System.Web;
using Codeable.Foundation.Core.Aspect;
using Microsoft.Practices.Unity;
using System.Reflection;
using Codeable.Foundation.Core;
using Codeable.Foundation.Common.System;
using Codeable.Foundation.Common.Aspect;
using Codeable.Foundation.UI.Web.Common.Authentication;
using Codeable.Foundation.Common.Emailing;
using Codeable.Foundation.UI.Web.Core.Emailing;

namespace Codeable.Foundation.UI.Web.Core.Authentication
{
    public class MembershipUserProvider : ChokeableClass, IUserProvider
    {
        #region Constructor
        public MembershipUserProvider(IFoundation iFoundation)
            : base(iFoundation)
        {
        }
        #endregion

        #region Public Methods

        public virtual IUserProviderInfo GetUser(string userName)
        {
            return base.ExecuteFunction<IUserProviderInfo>("GetUser", delegate()
            {
                MembershipUser membershipUser = Membership.GetUser(userName);
                if (membershipUser != null)
                {
                    MembershipUserProviderInfo result = new MembershipUserProviderInfo();
                    string[] roles = new string[0];
                    try 
	                {	        
                        Roles.GetRolesForUser(userName);
	                }
	                catch {}

                    result.UserAccountName = membershipUser.UserName;
                    result.UserName = membershipUser.UserName;
                    result.UserEmail = membershipUser.Email;
                    result.UserId = membershipUser.ProviderUserKey.ToString();
                    result.Roles = roles.ToList<String>();
                    return result;      
                } 
                return null;
          
            });
        }
        public virtual bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            return base.ExecuteFunction<bool>("ChangePassword", delegate()
            {
                MembershipUser user = Membership.GetUser(userName, false);
                return user.ChangePassword(currentPassword, newPassword);
            });
        }
        

        public virtual bool DeleteUser(string userName)
        {
            return base.ExecuteFunction<bool>("DeleteUser", delegate()
            {
                return Membership.DeleteUser(userName, true);
            });
        }
        public virtual IUserProviderInfo CreateUser(string userName, string password, string email)
        {
            return base.ExecuteFunction<IUserProviderInfo>("CreateUser", delegate()
            {
                try
                {

                    MembershipUser membershipUser = Membership.CreateUser(userName, password, email);
                    if (membershipUser != null)
                    {
                        return GetUser(membershipUser.UserName);
                    }
                    return null;
                }
                catch (MembershipCreateUserException ex)
                {
                    throw new Exception("Unable to create user: " + ex.StatusCode.ToString());
                }
            });
        }

        public virtual bool StartPasswordReset(string userName)
        {
            return base.ExecuteFunction<bool>("StartPasswordReset", delegate()
            {
                IUserProviderInfo user = GetUser(userName);
                if (user != null)
                {
                    IEmailTransport emailTransport = this.IFoundation.SafeResolve<IEmailTransport>();
                    if (emailTransport != null)
                    {
                        string newPassword = GenerateNewPassword(userName);
                        EmailRecipient recipient = new EmailRecipient(user.UserEmail);
                        EmailTemplate template = GetEmailTemplate();
                        IEmail email = emailTransport.CreateEmail();
                        PasswordResetEmailFormatter formatter = new PasswordResetEmailFormatter();
                        email = formatter.FormatEmail(email, template, recipient, user, newPassword);
                        emailTransport.SendEmail(email, recipient, false);
                        return true;
                    }
                    return false;
                }
                return false;
            });
        }

        #endregion

        #region Protected Methods

        protected virtual string GenerateNewPassword(string userName)
        {
            return base.ExecuteFunction<string>("GenerateNewPassword", delegate()
            {
                MembershipUser user = Membership.GetUser(userName, false);
                return user.ResetPassword();
            });
        }
        protected virtual EmailTemplate GetEmailTemplate()
        {
            return base.ExecuteFunction<EmailTemplate>("GetEmailTemplate", delegate()
            {
                EmailTemplate result = null;
                Assembly assembly = Assembly.GetExecutingAssembly();
                if (assembly != null)
                {
                    string template = string.Empty;
                    using (System.IO.Stream stream = assembly.GetManifestResourceStream("Codeable.Foundation.UI.Web.Core.Content.email-template-password-reset-v1.xml"))
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                        {
                            template = reader.ReadToEnd();
                        }
                    }
                    if (!string.IsNullOrEmpty(template))
                    {
                        try
                        {
                            result = CoreUtility.DeserializeFromXml<EmailTemplate>(template);
                        }
                        catch (Exception ex)
                        {
                            this.IFoundation.GetLogger().Write(string.Format("Error loading default email template: {0}", ex.Message), Category.Error);
                        }
                    }
                }
                if (result == null)
                {
                    // hopefully this is never used
                    result = new EmailTemplate()
                    {
                        FromEmail = "DoNotReply@codeable.net",
                        FromName = "DoNotReply@codeable.net",
                        Subject = "codeable.net: Password reset request",
                        HTMLBody = "&lt;table&gt;&lt;tr&gt;&lt;td valign=\"top\"&gt;A password reset request has been received. Your new password is:.&lt;/td&gt;&lt;/tr&gt; &lt;tr&gt; &lt;td &gt; %NEWPASSWORD% &lt;/td&gt; &lt;/tr&gt; &lt;/table&gt;&lt;span style=\"color:#808080\"&gt;DO NOT REPLY TO THIS EMAIL. &lt;/span&gt;"
                    };
                }
                return result;
            });
        }

        #endregion
    }
}
