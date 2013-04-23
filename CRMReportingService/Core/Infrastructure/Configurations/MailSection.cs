namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.Configurations
{
    using System.Configuration;

    using DNAide.Core;
    using DNAide.Utils;

    /// <summary>
    /// Summary description for MailSetting
    /// </summary>
    public class MailSection : ConfigurationSection
    {
        public enum Setting
        {
            Server, Send
        }

        public enum HostName
        {
            Gmail, IBC
        }

        [ConfigurationProperty("Active", IsDefaultCollection = false, DefaultValue = "Gmail")]
        public string Active
        {
            get
            {
                var active =
                    (string)base["Active"];
                return active;
            }
        }

        [ConfigurationProperty("Server", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MailCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public MailCollection Server
        {
            get
            {
                var spCollection =
                    (MailCollection)base["Server"];
                return spCollection;
            }
        }

        [ConfigurationProperty("Send", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MailCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public MailCollection Send
        {
            get
            {
                var spCollection =
                    (MailCollection)base["Send"];
                return spCollection;
            }
        }


        public static MailSection Instance
        {
            get
            {
                var spSection =
                 ConfigurationManager.GetSection("MailSection") as MailSection;
                return spSection;
            }
        }


        public MailCollection this[Setting name]
        {
            get
            {

                var spCollection = (MailCollection)this.GetType().GetProperty(name.ToString()).GetValue(this, System.Reflection.BindingFlags.GetField, null, null, null);
                return spCollection;
            }
        }


        #region Old Version
        public static string MailServer
        {
            get { return ConfigurationManager.AppSettings["mailServer"]; }
        }

        public static string MailPwd
        {
            get { return Encryption.CfsDeCrypt(ConfigurationManager.AppSettings["emailPwd"]); }
        }

        public static string FromMail
        {
            get { return ConfigurationManager.AppSettings["fromMail"]; }
        }

        public static int Port
        {
            get { return CastAide.AsInt32(ConfigurationManager.AppSettings["port"]); }
        }

        public static string CCMail
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ccMail"]); }
        }

        public static string BCCMail
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["bccMail"]); }
        }

        public static string ToMailAsiaBetCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailAsiaBetCSD"]); }
        }

        public static string CCMailAsiaBetCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailAsiaBetCSD"]); }
        }

        public static string BCCMailAsiaBetCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailAsiaBetCSD"]); }
        }

        public static string ToMail12BetCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMail12BetCSD"]); }
        }

        public static string CCMail12BetCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMail12BetCSD"]); }
        }

        public static string BCCMail12BetCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMail12BetCSD"]); }
        }

        public static string ToMail12EUCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMail12EUCSD"]); }
        }

        public static string CCMail12EUCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMail12EUCSD"]); }
        }

        public static string BCCMail12EUCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMail12EUCSD"]); }
        }

        public static string ToMailMansion88CSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailMansion88CSD"]); }
        }

        public static string CCMailMansion88CSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailMansion88CSD"]); }
        }

        public static string BCCMailMansion88CSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailMansion88CSD"]); }
        }

        public static string ToMailHRGroup
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailHRGroup"]); }
        }

        public static string CCMailHRGroup
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailHRGroup"]); }
        }

        public static string BCCMailHRGroup
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailHRGroup"]); }
        }

        public static string ToMailFIAutoDummyTransfer
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailFIAutoDummyTransfer"]); }
        }

        public static string CCMailFIAutoDummyTransfer
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailFIAutoDummyTransfer"]); }
        }

        public static string BCCMailFIAutoDummyTransfer
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailFIAutoDummyTransfer"]); }
        }

        public static string ToMailExchangeRate
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailExchangeRate"]); }
        }

        public static string CCMailExchangeRate
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailExchangeRate"]); }
        }

        public static string BCCMailExchangeRate
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailExchangeRate"]); }
        }

        public static string ToMailAlogCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailAlogCSD"]); }
        }

        public static string CCMailAlogCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailAlogCSD"]); }
        }

        public static string BCCMailAlogCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailAlogCSD"]); }
        }

        public static string ToMailBugSystem
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailBugSystem"]); }
        }

        public static string CCMailBugSystem
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailBugSystem"]); }
        }

        public static string BCCMailBugSystem
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailBugSystem"]); }
        }

        public static string ToMailTLCCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailTLCCSD"]); }
        }

        public static string CCMailTLCCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailTLCCSD"]); }
        }

        public static string BCCMailTLCCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailTLCCSD"]); }
        }

        public static string ToMailDelaCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailDelaCSD"]); }
        }

        public static string CCMailDelaCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailDelaCSD"]); }
        }

        public static string BCCMailDelaCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailDelaCSD"]); }
        }

        public static string ToMailSpondemoCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailSpondemoCSD"]); }
        }

        public static string CCMailSpondemoCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailSpondemoCSD"]); }
        }

        public static string BCCMailSpondemoCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailSpondemoCSD"]); }
        }

        public static string ToMailZzun88CSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailZzun88CSD"]); }
        }

        public static string CCMailZzun88CSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailZzun88CSD"]); }
        }

        public static string BCCMailZzun88CSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailZzun88CSD"]); }
        }

        public static string ToMailMayfairCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailMayfairCSD"]); }
        }

        public static string CCMailMayfairCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailMayfairCSD"]); }
        }

        public static string BCCMailMayfairCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailMayfairCSD"]); }
        }

        public static string ToMailSuncityCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailSuncityCSD"]); }
        }

        public static string CCMailSuncityCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailSuncityCSD"]); }
        }

        public static string BCCMailSuncityCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailSuncityCSD"]); }
        }

        public static string ToMailRecoverBetCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["ToMailRecoverBetCSD"]); }
        }

        public static string CCMailRecoverBetCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["CCMailRecoverBetCSD"]); }
        }

        public static string BCCMailRecoverBetCSD
        {
            get { return CastAide.AsString(ConfigurationManager.AppSettings["BCCMailRecoverBetCSD"]); }
        }

        #endregion
    }


    public class MailCollection : ConfigurationElementCollection
    {

        public MailCollection()
        {
            //var storeProcedure = (StoreProcedureElement) CreateNewElement();
            //Add(storeProcedure);
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public void Add(MailElement mailElement)
        {
            BaseAdd(mailElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(MailElement mailElement)
        {
            if (BaseIndexOf(mailElement) >= 0)
                BaseRemove(mailElement.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string alias)
        {
            BaseRemove(alias);
        }

        public void Clear()
        {
            BaseClear();
        }


        public MailElement this[int index]
        {
            get
            {
                return (MailElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public MailElement this[string Name]
        {
            get
            {
                return (MailElement)BaseGet(Name);
            }
        }

        public int IndexOf(MailElement mailElement)
        {
            return BaseIndexOf(mailElement);
        }


        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new MailElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MailElement)element).Name;
        }
    }

    public class MailElement : ConfigurationElement
    {



        public MailElement()
        {

        }


        [ConfigurationProperty("host", IsRequired = false)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string Host
        {
            get
            {
                return (string)this["host"];
            }
            set
            {
                this["host"] = value;
            }
        }

        [ConfigurationProperty("port", IsRequired = false, DefaultValue = 25)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
            set
            {
                this["port"] = value;
            }
        }

        [ConfigurationProperty("username", IsRequired = false)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string Username
        {
            get
            {
                return (string)this["username"];
            }
            set
            {
                this["username"] = value;
            }
        }


        [ConfigurationProperty("password", IsRequired = false)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }
        [ConfigurationProperty("auth", IsRequired = false, DefaultValue = true)]
        public bool Auth
        {
            get
            {
                return (bool)this["auth"];
            }
            set
            {
                this["auth"] = value;
            }
        }


        [ConfigurationProperty("name", IsRequired = false)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("from", IsRequired = false)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string From
        {
            get
            {
                return (string)this["from"];
            }
            set
            {
                this["from"] = value;
            }
        }

        [ConfigurationProperty("to", IsRequired = false)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string To
        {
            get
            {
                return (string)this["to"];
            }
            set
            {
                this["to"] = value;
            }
        }

        [ConfigurationProperty("bcc", IsRequired = false)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string Bcc
        {
            get
            {
                return (string)this["bcc"];
            }
            set
            {
                this["bcc"] = value;
            }
        }

        [ConfigurationProperty("cc", IsRequired = false)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string Cc
        {
            get
            {
                return (string)this["cc"];
            }
            set
            {
                this["cc"] = value;
            }
        }

        [ConfigurationProperty("title", DefaultValue = "", IsRequired = false)]
        public string Title
        {
            get
            {
                return (string)this["title"];
            }
        }

        [ConfigurationProperty("subject", DefaultValue = "", IsRequired = false)]
        public string Subject
        {
            get
            {
                return (string)this["subject"];
            }
        }

        [ConfigurationProperty("mails", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MailCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public MailCollection Mails
        {
            get
            {
                var stores =
                    (MailCollection)base["mails"];
                return stores;
            }
        }
    }
}
