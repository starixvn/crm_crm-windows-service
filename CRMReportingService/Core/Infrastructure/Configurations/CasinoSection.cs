namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.Configurations
{
    using System.Configuration;

    public class CasinoSection : ConfigurationSection
    {
        public enum SystemConfigCollection
        {
            Systems, Communications
        }

        [ConfigurationProperty("systems", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(CasinoCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public CasinoCollection Systems
        {
            get
            {
                var spCollection =
                    (CasinoCollection)base["systems"];
                return spCollection;
            }
        }

        [ConfigurationProperty("communications", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(CasinoCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public CasinoCollection Communications
        {
            get
            {
                var spCollection =
                    (CasinoCollection)base["communications"];
                return spCollection;
            }
        }


        public static CasinoSection Instance
        {
            get
            {
                var spSection =
                 ConfigurationManager.GetSection("CasinoSection") as CasinoSection;
                return spSection;
            }
        }


        public CasinoCollection this[SystemConfigCollection name]
        {
            get
            {

                var spCollection = (CasinoCollection)this.GetType().GetProperty(name.ToString()).GetValue(this, System.Reflection.BindingFlags.GetField, null, null, null);
                return spCollection;
            }
        }


    }

    public class CasinoCollection : ConfigurationElementCollection
    {

        public CasinoCollection()
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

        public void Add(CasinoElement casinoElement)
        {
            BaseAdd(casinoElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(CasinoElement casinoElement)
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


        public CasinoElement this[int index]
        {
            get
            {
                return (CasinoElement)BaseGet(index);
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

        new public CasinoElement this[string Alias]
        {
            get
            {
                return (CasinoElement)BaseGet(Alias);
            }
        }

        public int IndexOf(CasinoElement casinoElement)
        {
            return BaseIndexOf(casinoElement);
        }


        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new CasinoElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CasinoElement)element).Alias;
        }
    }

    public class CasinoElement : ConfigurationElement
    {



        public CasinoElement()
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
        
        [ConfigurationProperty("prefix", IsRequired = true)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string Prefix
        {
            get
            {
                return (string)this["prefix"];
            }
            set
            {
                this["prefix"] = value;
            }
        }

        [ConfigurationProperty("subfix", IsRequired = true)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string Subfix
        {
            get
            {
                return (string)this["subfix"];
            }
            set
            {
                this["subfix"] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired = true)]
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

        [ConfigurationProperty("id", IsRequired = true)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public int Id
        {
            get
            {
                return (int)this["id"];
            }
            set
            {
                this["id"] = value;
            }
        }
        

        [ConfigurationProperty("stores", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(CasinoCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public CasinoCollection Stores
        {
            get
            {
                var stores =
                    (CasinoCollection)base["stores"];
                return stores;
            }
        }
    }
}
