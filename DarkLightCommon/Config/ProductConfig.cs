using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DarkLight.Common.Config
{
    public class ProductManagerConfigSection : ConfigurationSection
    {
        public const string _productGroups = "ProductGroups";

        [ConfigurationProperty(_productGroups, IsDefaultCollection = true)]
        public ProductGroupCollection ProductGroups
        {
            get
            {
                var items = (ProductGroupCollection)base[_productGroups];
                return items;
            }
        }
    }

    [Serializable]
    public class ProductGroupElement : ConfigurationElement
    {
        private const string _name = "name";
        private const string _products = "Products";

        [ConfigurationProperty(_name, IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this[_name]; }
            set { this[_name] = value; }
        }

        [ConfigurationProperty(_products, IsDefaultCollection = false)]
        public ProductInstanceCollection Products
        {
            get
            {
                var items = (ProductInstanceCollection)base[_products];
                return items;
            }
        }
    }

    [Serializable]
    public class ProductInstanceElement : ConfigurationElement
    {
        private const string _ticker = "ticker";

        [ConfigurationProperty(_ticker, IsKey = true, IsRequired = true)]
        public string Ticker
        {
            get { return (string)this[_ticker]; }
            set { this[_ticker] = value; }
        }
    }

    [Serializable]
    public class ProductInstanceCollection : ConfigurationElementCollection
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
            return new ProductInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProductInstanceElement)element).Ticker;
        }

        public ProductInstanceElement this[int index]
        {
            get { return (ProductInstanceElement)BaseGet(index); }
        }
        public new ProductInstanceElement this[string name]
        {
            get
            {
                if (IndexOf(name) < 0) return null;
                return (ProductInstanceElement)BaseGet(name);
            }
        }

        public int IndexOf(string name)
        {
            name = name.ToLower();
            for (int idx = 0; idx < base.Count; idx++)
            {
                if (this[idx].Ticker.ToLower() == name)
                    return idx;
            }
            return -1;
        }

        protected override string ElementName
        {
            get { return "Product"; }
        }
    }

    [Serializable]
    public class ProductGroupCollection : ConfigurationElementCollection
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
             return new ProductGroupElement();
         }

         protected override object GetElementKey(ConfigurationElement element)
         {
              return ((ProductGroupElement)element).Name;
         }

         public ProductGroupElement this[int index]
         {
             get { return (ProductGroupElement)BaseGet(index); }
         }
         public new ProductGroupElement this[string name]
         {
            get
            {
                if( IndexOf(name) < 0 ) return null;
                return (ProductGroupElement)BaseGet(name);
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
             get { return "ProductGroup"; }
         }
    }
}
