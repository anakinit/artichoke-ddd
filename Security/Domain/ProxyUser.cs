using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Data;
using Artichoke.Domain;

namespace Transaktion.Core.Security.Domain
{
    public class ProxyUser : EntityBase
    {
        private Guid _providerUserKey;
        private string _username;
        private string _applicationName;
        private string _email;
        private string _comment;
        private string _password;
        private string _passwordQuestion;
        private string _passwordAnswer;
        private bool _isApproved;
        private bool _isOnline;
        private bool _isLockedOut;
        private DateTime _lastActivityDate;
        private DateTime _lastLoginDate;
        private DateTime _lastPasswordChangedDate;
        private DateTime _lastLockoutDate;
        private DateTime _createDate;
        private int _failedPasswordAttempts;
        private DateTime _failedPasswordAttemptStart;
        private int _failedPasswordAnswers;
        private DateTime _failedPasswordAnswerStart;

        private IList<ProxyRole> _roles;


        protected ProxyUser() { }

        internal ProxyUser(string applicationName, Guid providerUserKey, string username, string email, string password, bool isApproved)
        {
            _applicationName = applicationName;
            _providerUserKey = providerUserKey;
            _username = username;
            _email = email;
            _password = password;
            _isApproved = isApproved;
            var createDate = DateTime.Now;
            _createDate = createDate;
            _lastActivityDate = createDate;
            _lastPasswordChangedDate = createDate;
            _lastLoginDate = DateTime.MinValue;
            _lastLockoutDate = DateTime.MinValue;
            _failedPasswordAttemptStart = DateTime.MaxValue;
            _failedPasswordAttempts = 0;
            _failedPasswordAnswerStart = DateTime.MaxValue;
            _failedPasswordAnswers = 0;

            _roles = new List<ProxyRole>();
        }

        public virtual Guid UserKey
        {
            get { return _providerUserKey; }
            set { _providerUserKey = value; }
        }

        public virtual string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public virtual string ApplicationName
        {
            get { return _applicationName; }
            protected set { _applicationName = value; }
        }

        public virtual string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public virtual string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public virtual string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public virtual string PasswordQuestion
        {
            get { return _passwordQuestion; }
            set { _passwordQuestion = value; }
        }

        public virtual string PasswordAnswer
        {
            get { return _passwordAnswer; }
            set { _passwordAnswer = value; }
        }

        public virtual bool IsApproved
        {
            get { return _isApproved; }
            set { _isApproved = value; }
        }

        public virtual bool IsOnline
        {
            get { return _isOnline; }
            set { _isOnline = value; }
        }

        public virtual bool IsLockedOut
        {
            get { return _isLockedOut; }
            set { _isLockedOut = value; }
        }

        public virtual DateTime LastActivityDate
        {
            get { return _lastActivityDate; }
            set { _lastActivityDate = value; }
        }

        public virtual DateTime LastLoginDate
        {
            get { return _lastLoginDate; }
            set { _lastLoginDate = value; }
        }

        public virtual DateTime LastPasswordChangedDate
        {
            get { return _lastPasswordChangedDate; }
            set { _lastPasswordChangedDate = value; }
        }

        public virtual DateTime LastLockoutDate
        {
            get { return _lastLockoutDate; }
            set { _lastLockoutDate = value; }
        }

        public virtual DateTime CreateDate
        {
            get { return _createDate; }
            set { _createDate = value; }
        }

        public virtual int FailedPasswordAttempts
        {
            get { return _failedPasswordAttempts; }
            set { _failedPasswordAttempts = value; }
        }

        public virtual DateTime FailedPasswordAttemptStart
        {
            get { return _failedPasswordAttemptStart; }
            set { _failedPasswordAttemptStart = value; }
        }

        public virtual int FailedPasswordAnswers
        {
            get { return _failedPasswordAnswers; }
            set { _failedPasswordAnswers = value; }
        }

        public virtual DateTime FailedPasswordAnswerStart
        {
            get { return _failedPasswordAnswerStart; }
            set { _failedPasswordAnswerStart = value; }
        }

        public virtual IList<ProxyRole> Roles
        {
            get { return _roles; }
            protected set { _roles = value; }
        }

        public override string ToString()
        {
            return string.Format("ProxyUser#{0}-{1}", ApplicationName.ToLower(), Username.ToLower());
        }
    }

    public class ProxyUserMap : EntityMap<ProxyUser>
    {
        public ProxyUserMap()
        {
            Table("security_user");
            Id(x => x.UserKey, "UserKey")
                .GeneratedBy.Assigned();
            Map(x => x.ApplicationName);
            Map(x => x.Username);
            Map(x => x.Email);
            Map(x => x.Password);
            Map(x => x.PasswordQuestion);
            Map(x => x.PasswordAnswer);
            Map(x => x.IsApproved);
            Map(x => x.IsOnline);
            Map(x => x.IsLockedOut);
            Map(x => x.LastActivityDate);
            Map(x => x.LastLoginDate);
            Map(x => x.LastPasswordChangedDate);
            Map(x => x.LastLockoutDate);
            Map(x => x.CreateDate);
            Map(x => x.FailedPasswordAnswers);
            Map(x => x.FailedPasswordAnswerStart);
            Map(x => x.FailedPasswordAttempts);
            Map(x => x.FailedPasswordAttemptStart);
            HasManyToMany<ProxyRole>(x => x.Roles)
                .Table("security_user_roles")
                .ParentKeyColumn("UserKey")
                .ChildKeyColumn("RoleKey")
                .Cascade.All();
        }

    }
}
