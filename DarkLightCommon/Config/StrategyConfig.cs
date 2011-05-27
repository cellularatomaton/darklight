using System;
using System.Configuration;

namespace DarkLight.Common.Config
{
    [Serializable]
    public class StrategyManagerConfigSection : ConfigurationSection
    {
        public const string _strategies = "Strategies";

        [ConfigurationProperty(_strategies, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(StrategyInstanceCollection),
            AddItemName = "AddStrategy",
            ClearItemsName = "ClearStrategies",
            RemoveItemName = "RemoveStrategy")]
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
        public StrategyInstanceCollection()
         {
              var strat = (StrategyInstanceElement)CreateNewElement();
              Add(strat);
         }

         public override ConfigurationElementCollectionType CollectionType
         {
              get 
              { 
                  return ConfigurationElementCollectionType.AddRemoveClearMap; 
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
              get { return (StrategyInstanceElement)BaseGet(index);}
              set
              {
                   if (BaseGet(index) != null)
                   {
                        BaseRemoveAt(index);
                   }
                   BaseAdd(index, value);
              }
         }
         public new StrategyInstanceElement this[string Name]
         {
              get { return (StrategyInstanceElement)BaseGet(Name); }
         }

         public int IndexOf(StrategyInstanceElement strategy) 
         {
             return BaseIndexOf(strategy); 
         }

         public void Add(StrategyInstanceElement message) 
         { 
             BaseAdd(message); 
         }

         protected override void BaseAdd(ConfigurationElement element)
         {
              BaseAdd(element, false);
         }

         public void Remove(StrategyInstanceElement message)
         {
              if (BaseIndexOf(message) >= 0)
              BaseRemove(message.Name);
         }
         public void RemoveAt(int index) 
         { 
             BaseRemoveAt(index); 
         }

         public void Remove(string name) 
         { 
             BaseRemove(name); 
         }

         public void Clear() 
         { 
             BaseClear(); 
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
