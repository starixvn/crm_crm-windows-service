namespace NexCel.CRM.CRMReportingService.Core.Infrastructure.Configurations
{
    using System.Configuration;

    public class StoreProcedureSection : ConfigurationSection
    {
        public enum StoreCollection
        {
            Report
        }
        
        [ConfigurationProperty("Report", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(StoreProcedureCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public StoreProcedureCollection Report
        {
            get
            {
                var spCollection =
                    (StoreProcedureCollection)base["Report"];
                return spCollection;
            }
        }


        public static StoreProcedureSection Instance
        {
            get
            {
                var spSection =
                 ConfigurationManager.GetSection("StoreProcedure") as StoreProcedureSection;
                return spSection;
            }
        }


        public StoreProcedureCollection this[StoreCollection name]
        {
            get
            {

                var spCollection = (StoreProcedureCollection)this.GetType().GetProperty(name.ToString()).GetValue(this, System.Reflection.BindingFlags.GetField, null, null, null);
                return spCollection;
            }
        }


    }

    public class StoreProcedureCollection : ConfigurationElementCollection
    {

        public StoreProcedureCollection()
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

        public void Add(StoreProcedureElement storeProcedureElement)
        {
            BaseAdd(storeProcedureElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(StoreProcedureElement storeProcedureElement)
        {
            if (BaseIndexOf(storeProcedureElement) >= 0)
                BaseRemove(storeProcedureElement.Alias);
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


        public StoreProcedureElement this[int index]
        {
            get
            {
                return (StoreProcedureElement)BaseGet(index);
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

        new public StoreProcedureElement this[string Alias]
        {
            get
            {
                return (StoreProcedureElement)BaseGet(Alias);
            }
        }

        public int IndexOf(StoreProcedureElement storeProcedureElement)
        {
            return BaseIndexOf(storeProcedureElement);
        }


        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new StoreProcedureElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StoreProcedureElement)element).Alias;
        }
    }

    public class StoreProcedureElement : ConfigurationElement
    {



        public StoreProcedureElement()
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

        [ConfigurationProperty("db", IsRequired = true)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string DbName
        {
            get
            {
                return (string)this["db"];
            }
            set
            {
                this["db"] = value;
            }
        }

        [ConfigurationProperty("function", IsRequired = false)]
        //[RegexStringValidator(@"\w+:\/\/[\w.]+\S*")]
        public string Function
        {
            get
            {
                return (string)this["function"];
            }
            set
            {
                this["function"] = value;
            }
        }

        [ConfigurationProperty("stores", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(StoreProcedureCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public StoreProcedureCollection Stores
        {
            get
            {
                var stores =
                    (StoreProcedureCollection)base["stores"];
                return stores;
            }
        }
    }
}
