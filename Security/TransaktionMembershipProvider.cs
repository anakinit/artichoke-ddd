using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Configuration;
using Transaktion.Core.Security.Domain;
using System.Security.Cryptography;

namespace Transaktion.Core.Security
{
    public class TransaktionMembershipProvider :
        MembershipProvider
    {
        private static readonly ProxyUserDao userDao = new ProxyUserDao();

        private const int newPasswordLength = 8;

        private string _applicationName;
        private int _maxInvalidPasswordAttempts;
        private int _passwordAttemptWindow;
        private int _minRequiredNonAlphanumericCharacters;
        private int _minRequiredPasswordLength;
        private string _passwordStrengthRegularExpression;
        private bool _enablePasswordReset;
        private bool _enablePasswordRetrieval;
        private bool _requiresQuestionAndAnswer;
        private bool _requiresUniqueEmail;
        private MembershipPasswordFormat _passwordFormat;
        private MachineKeySection _machineKey;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "TransaktionMembershipProvider";

            base.Initialize(name, config);

            // Load Settings
            _applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            _passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            _minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphaNumericCharacters"], "1"));
            _minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            _passwordStrengthRegularExpression = GetConfigValue(config["passwordStrengthRegularExpression"], "");
            _enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "True"));
            _enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "True"));
            _requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "False"));
            _requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "True"));

            // Load Password Format
            string p = GetConfigValue(config["passwordFormat"], "Hashed");

            if (p.Equals("Hashed", StringComparison.CurrentCultureIgnoreCase))
                _passwordFormat = MembershipPasswordFormat.Hashed;
            else if (p.Equals("Encrypted", StringComparison.CurrentCultureIgnoreCase))
                _passwordFormat = MembershipPasswordFormat.Encrypted;
            else if (p.Equals("Clear", StringComparison.CurrentCultureIgnoreCase))
                _passwordFormat = MembershipPasswordFormat.Clear;
            else
                throw new ProviderException("Password format not supported.");

            // Load MachineKey - Validate Hashed/Encrypted passwords
            Configuration cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _machineKey = cfg.GetSection("system.web/machineKey") as MachineKeySection;

            if (_machineKey.ValidationKey.Contains("AutoGenerate") && PasswordFormat != MembershipPasswordFormat.Clear)
                throw new ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.");
        }

        private string GetConfigValue(string value, string defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return value;
        }


        public override string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set { }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
                return false;

            var args = new ValidatePasswordEventArgs(username, newPassword, false);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new ProviderException("Change password cancelled due to new password validation feature.");
            }

            var user = userDao.GetUser(ApplicationName, username);

            user.Password = EncodePassword(user.UserKey, newPassword);

            userDao.Save(user);

            return true;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            var user = userDao.GetUser(ApplicationName, username);

            user.PasswordQuestion = newPasswordQuestion;
            user.PasswordAnswer = EncodePassword(user.UserKey, newPasswordAnswer.Trim().ToUpper());

            userDao.Save(user);

            return true;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            // Check Email
            var user = GetUser(GetUserNameByEmail(email), false);
            if (RequiresUniqueEmail && user != null && user.IsApproved)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            // Check Username
            user = GetUser(username, false);
            if (user != null && user.IsApproved)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            // Check Provider Key
            if (providerUserKey == null)
                providerUserKey = Guid.NewGuid();
            else if (providerUserKey.GetType() != typeof(Guid))
            {
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }

            user = GetUser(providerUserKey, false);
            if (GetUser(providerUserKey, false) != null && user.IsApproved)
            {
                status = MembershipCreateStatus.DuplicateProviderUserKey;
                return null;
            }

            var userKey = (Guid)providerUserKey;

            var createDate = DateTime.Now;

            var proxyUserDao = new ProxyUserDao();
            var proxyUser = new ProxyUser(this.ApplicationName, userKey, username, email, EncodePassword(userKey, password), isApproved);
            proxyUser.PasswordQuestion = (passwordQuestion + "").Trim();
            proxyUser.PasswordAnswer = EncodePassword(userKey, (passwordAnswer + "").Trim().ToUpper());

            proxyUserDao.Save(proxyUser);

            status = MembershipCreateStatus.Success;
            return GetUser(userKey, false);
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var response = new MembershipUserCollection();
            totalRecords = 0;

            var daoUsers = userDao.SearchUsersByEmail(this.ApplicationName, emailToMatch, pageIndex, pageIndex, ref totalRecords);

            foreach (var daoUser in daoUsers)
            {
                response.Add(GetMembershipUser(daoUser));
            }

            return response;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var response = new MembershipUserCollection();
            totalRecords = 0;

            var daoUsers = userDao.SearchUsersByName(this.ApplicationName, usernameToMatch, pageIndex, pageIndex, ref totalRecords);

            foreach (var daoUser in daoUsers)
            {
                response.Add(GetMembershipUser(daoUser));
            }

            return response;

        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var response = new MembershipUserCollection();
            totalRecords = 0;

            var daoUsers = userDao.GetAllUsers(this.ApplicationName, pageIndex, pageIndex, ref totalRecords);

            foreach (var daoUser in daoUsers)
            {
                response.Add(GetMembershipUser(daoUser));
            }

            return response;
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
                throw new NotSupportedException("Password retrieval is not enabled.");

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
                throw new NotSupportedException("Unable to retrieve hashed passwords.");

            var user = userDao.GetUser(ApplicationName, username);

            if (user == null)
                throw new ProviderException("User not found.");
            if (user.IsLockedOut)
                throw new ProviderException("User is locked out.");

            if (RequiresQuestionAndAnswer)
            {
                if (string.IsNullOrEmpty(answer))
                {
                    throw new ProviderException("Password answer required for password retrieval.");
                }
                else if (!CheckAnswer(user.UserKey, answer, user.PasswordAnswer))
                {
                    throw new MembershipPasswordException("Incorrect password answer.");
                }
            }

            return DecodePassword(user.UserKey, user.Password);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var user = userDao.GetUser(ApplicationName, username);

            if (user == null)
                return null;

            if (userIsOnline && !user.IsOnline)
            {
                user.IsOnline = true;
                userDao.Save(user);
            }

            return GetMembershipUser(user);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            Guid userkey = Guid.Empty;

            if (!(providerUserKey is Guid))
                throw new ArgumentException("Invalid providerUserKey, must be Guid.");

            userkey = (Guid)providerUserKey;

            var user = userDao.GetUser(userkey);

            if (user == null)
                return null;

            if (userIsOnline && !user.IsOnline)
            {
                user.IsOnline = true;
                userDao.Save(user);
            }

            return GetMembershipUser(user);
        }

        public override string GetUserNameByEmail(string email)
        {
            return userDao.GetUsernameByEmail(email);
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
                throw new NotSupportedException("Password reset is not enabled.");

            var user = userDao.GetUser(ApplicationName, username);

            if (user == null)
                throw new ProviderException("User not found.");
            if (user.IsLockedOut)
                throw new ProviderException("User is locked out.");

            if (RequiresQuestionAndAnswer)
            {
                if (string.IsNullOrEmpty(answer))
                {
                    throw new ProviderException("Password answer required for password reset.");
                }
                else if (CheckAnswer(user.UserKey, answer, user.PasswordAnswer))
                {

                }
            }

            var newPassword = Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");
            }

            user.Password = EncodePassword(user.UserKey, newPassword);
            userDao.Save(user);

            return newPassword;
        }

        public override bool UnlockUser(string username)
        {
            var user = userDao.GetUser(ApplicationName, username);
            if (user == null)
                return false;

            if (user.IsLockedOut)
            {
                user.IsLockedOut = false;
                userDao.Save(user);
                return true;
            }

            return false;
        }

        public override void UpdateUser(MembershipUser user)
        {
            var userkey = user.ProviderUserKey;
            if (!(userkey is Guid))
                throw new ProviderException("Invalid provider user key, must be Guid.");

            var userProxy = userDao.GetUser((Guid)userkey);

            userProxy.Email = user.Email;
            userProxy.Comment = user.Comment;
            userProxy.IsApproved = user.IsApproved;

            userDao.Save(userProxy);
        }

        public override bool ValidateUser(string username, string password)
        {
            var user = userDao.GetUser(ApplicationName, username);

            if (user == null)
                throw new MembershipPasswordException("User not found.");

            if (!CheckPassword(user.UserKey, password, user.Password))
            {
                IncrementPasswordFailure(user);
                return false;
            }

            return true;
        }

        #region Password Support Methods

        private byte[] HexToByte(string hex)
        {
            var response = new byte[(hex.Length / 2) - 1];
            for (int i = 0; i < response.Length; i++)
                response[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return response;
        }

        private string EncodePassword(Guid providerUserKey, string password)
        {
            var response = password;

            if (PasswordFormat == MembershipPasswordFormat.Clear)
            {
                // Do Nothing
            }
            else if (PasswordFormat == MembershipPasswordFormat.Encrypted || PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                var salt = providerUserKey.ToString();
                var saltHalfway = salt.Length / 2;
                var saltedPassword = string.Concat(salt.Substring(0, saltHalfway), "#", password, "#", salt.Substring(saltHalfway));

                if (PasswordFormat == MembershipPasswordFormat.Encrypted)
                {
                    response = Convert.ToBase64String(EncryptPassword(Encoding.UTF8.GetBytes(saltedPassword)));
                }
                else
                {
                    var hash = new HMACSHA1();
                    hash.Key = HexToByte(_machineKey.ValidationKey);
                    response = Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword)));
                }
            }
            else
            {
                throw new ProviderException("Unsupported password format.");
            }

            return response;
        }

        private string DecodePassword(Guid providerUserKey, string encodedPassword)
        {
            var response = encodedPassword;
            if (PasswordFormat == MembershipPasswordFormat.Clear)
            {
                // Do Nothing
            }
            else if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                var salt = providerUserKey.ToString();
                var saltHalfway = salt.Length / 2;
                var saltedPassword = Encoding.UTF8.GetString(DecryptPassword(Convert.FromBase64String(encodedPassword)));
                response = saltedPassword
                    .Replace(string.Concat(salt.Substring(0, saltHalfway), "#"), "")
                    .Replace(string.Concat("#", salt.Substring(saltHalfway)), "");
            }
            else if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot decode a hashed password.");
            }
            else
            {
                throw new ProviderException("Unsupported password format.");
            }

            return response;
        }

        private bool CheckPassword(Guid providerUserKey, string password, string encodedPassword)
        {
            string pass1 = password;
            string pass2 = encodedPassword;

            if (PasswordFormat == MembershipPasswordFormat.Clear)
            {
                // Do Nothing
            }
            else if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                pass2 = DecodePassword(providerUserKey, encodedPassword);
            }
            else if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                pass1 = EncodePassword(providerUserKey, password);
            }
            else
            {
                throw new ProviderException("Unsupported password format.");
            }

            return pass1.Equals(pass2, StringComparison.InvariantCulture);
        }

        private bool CheckAnswer(Guid providerUserKey, string answer, string encodedAnswer)
        {
            string answer1 = answer.Trim().ToUpper();
            string answer2 = encodedAnswer;

            if (PasswordFormat == MembershipPasswordFormat.Clear)
            {
                // Do Nothing
            }
            else if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                answer2 = DecodePassword(providerUserKey, encodedAnswer);
            }
            else if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                answer1 = EncodePassword(providerUserKey, answer1);
            }
            else
            {
                throw new ProviderException("Unsupported password format.");
            }

            return answer1.Equals(answer2, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        private void IncrementAnswerFailure(ProxyUser user)
        {
            if (user.FailedPasswordAnswerStart > DateTime.Now.AddMinutes(this.PasswordAttemptWindow))
            {
                user.FailedPasswordAnswerStart = DateTime.Now;
                user.FailedPasswordAnswers = 1;
            }
            else
            {
                user.FailedPasswordAnswers += 1;
                if (user.FailedPasswordAnswers > this.MaxInvalidPasswordAttempts)
                {
                    user.IsLockedOut = true;
                    user.LastLockoutDate = DateTime.Now;
                }
            }

            userDao.Save(user);
        }

        private void IncrementPasswordFailure(ProxyUser user)
        {
            if (user.FailedPasswordAttemptStart > DateTime.Now.AddMinutes(this.PasswordAttemptWindow))
            {
                user.FailedPasswordAttemptStart = DateTime.Now;
                user.FailedPasswordAttempts = 1;
            }
            else
            {
                user.FailedPasswordAttempts += 1;
                if (user.FailedPasswordAttempts > this.MaxInvalidPasswordAttempts)
                {
                    user.IsLockedOut = true;
                    user.LastLockoutDate = DateTime.Now;
                }
            }

            userDao.Save(user);
        }

        private void ResetPasswordFailure(ProxyUser user)
        {
            user.FailedPasswordAnswers = 0;
            user.FailedPasswordAnswerStart = DateTime.MaxValue;
            user.FailedPasswordAttempts = 0;
            user.FailedPasswordAttemptStart = DateTime.MaxValue;

            userDao.Save(user);
        }

        private MembershipUserCollection GetMembershipUserCollection(IEnumerable<ProxyUser> users)
        {
            var response = new MembershipUserCollection();

            foreach (var user in users)
                response.Add(GetMembershipUser(user));

            return response;
        }


        private MembershipUser GetMembershipUser(ProxyUser user)
        {
            return new MembershipUser(
                    this.Name,
                    user.Username,
                    user.UserKey,
                    user.Email,
                    user.PasswordQuestion,
                    user.Comment,
                    user.IsApproved,
                    user.IsLockedOut,
                    user.CreateDate,
                    user.LastLoginDate,
                    user.LastActivityDate,
                    user.LastPasswordChangedDate,
                    user.LastLockoutDate);
        }
    }
}
