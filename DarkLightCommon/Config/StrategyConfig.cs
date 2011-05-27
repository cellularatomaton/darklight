using System;
using System.Configuration;

namespace DarkLight.Common.Config
{
    [Serializable]
    public class StrategyManagerConfigSection : ConfigurationSection
    {
        public const string _strategies = "Strategies";

        [ConfigurationProperty(_strategies, IsDefaultCollection = false)]
        public StrategyInstanceCollection Strategies
        {
            get
            {
                var items = (StrategyInstanceCollection)base[_strategies];
                return items;
            }
        }
    }

    [Serializable]
    public class StrategyInstanceCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new StrategyInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StrategyInstanceElement)element).Name;
        }

        public StrategyInstanceElement this[int index]
        {
            get { return (StrategyInstanceElement)BaseGet(index); }
        }
        public new StrategyInstanceElement this[string name]
        {
            get
            {
                if (IndexOf(name) < 0) return null;
                return (StrategyInstanceElement)BaseGet(name);
            }
        }

        public int IndexOf(string name)
        {
            name = name.ToLower();
            for (int idx = 0; idx < base.Count; idx++)
            {
                if (this[idx].Name.ToLower() == name)
                    return idx;
            }
            return -1;
        }

        protected override string ElementName
        {
            get { return "Strategy"; }
        }
    }

    [Serializable]
    public class StrategyInstanceElement : ConfigurationElement
    {
        private const string _name = "name";
        private const string _binding = "binding";
        private const string _productGroup = "productGroup";

        [ConfigurationProperty(_name, IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base[_name]; }
            set { base[_name] = value; }
        }

        [ConfigurationProperty(_binding, IsRequired = true)]
        public string Binding
        {
            get { return (string)base[_binding]; }
            set { base[_binding] = value; }
        }

        [ConfigurationProperty(_productGroup, IsRequired = true)]
        public string ProductGroup
        {
            get { return (string)base[_productGroup]; }
            set { base[_productGroup] = value; }
        }
    }
}
