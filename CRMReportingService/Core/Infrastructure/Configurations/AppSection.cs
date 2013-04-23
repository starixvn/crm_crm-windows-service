namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.Configurations
{
    using System.Configuration;

    public class AppSection : ConfigurationSection
    {
        public enum SystemConfigCollection
        {
            Systems, Communications
        }

        [ConfigurationProperty("HtmlReplace", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(AppCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public AppCollection HtmlReplace
        {
            get
            {
                var spCollection =
                    (AppCollection)base["HtmlReplace"];
                return spCollection;
            }
        }

        public static AppSection Instance
        {
            get
            {
                var spSection =
                 ConfigurationManager.GetSection("AppSection") as AppSection;
                return spSection;
            }
        }


        public AppCollection this[SystemConfigCollection name]
        {
            get
            {

                var spCollection = (AppCollection)this.GetType().GetProperty(name.ToString()).GetValue(this, System.Reflection.BindingFlags.GetField, null, null, null);
                return spCollection;
            }
        }


    }

    public class AppCollection : ConfigurationElementCollection
    {

        public AppCollection()
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

        public void Add(AppElement casinoElement)
        {
            BaseAdd(casinoElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(AppElement casinoElement)
        {
            if (BaseIndexOf(casinoElement) >= 0)
                BaseRemove(casinoElement.Alias);
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


        public AppElement this[int index]
        {
            get
            {
                return (AppElement)BaseGet(index);
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

        new public AppElement this[string Alias]
        {
            get
            {
                return (AppElement)BaseGet(Alias);
            }
        }

        public int IndexOf(AppElement casinoElement)
        {
            return BaseIndexOf(casinoElement);
        }


        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new AppElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AppElement)element).Alias;
        }
    }

    public class AppElement : ConfigurationElement
    {



        public AppElement()
        {

        }

        [ConfigurationProperty("alias", IsRequired = true, IsKey = true)]
        public string Alias
        {
            get
            {
                return (string)this["alias"];
            }
            set
            {
                this["alias"] = value;
            }
        }

        [ConfigurationProperty("html", IsRequired = false)]
        public string HtmlTag
        {
            get
            {
                return (string)this["html"];
            }
            set
            {
                this["html"] = value;
            }
        }

        [ConfigurationProperty("changeto", IsRequired = false)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string Changeto
        {
            get
            {
                return (string)this["changeto"];
            }
            set
            {
                this["changeto"] = value;
            }
        }
        
        [ConfigurationProperty("stores", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(AppCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public AppCollection Stores
        {
            get
            {
                var stores =
                    (AppCollection)base["stores"];
                return stores;
            }
        }
    }
}
