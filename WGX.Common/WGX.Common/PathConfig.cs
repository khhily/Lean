using System.Configuration;

namespace WGX.Common
{
    public sealed class PathConfig : ConfigurationSection
    {

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public Paths Paths
        {
            get
            {
                return (Paths)this[""];
            }
        }

    }

    [ConfigurationCollection(typeof(PathItem))]
    public class Paths : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new PathItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PathItem)element).Key;
        }

        public PathItem Get(string key)
        {
            var item = (PathItem)BaseGet(key) ?? new PathItem();
            return item;
        }

    }

    public class PathItem : ConfigurationElement
    {

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            return true;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            return true;
        }


        /// <summary>
        /// Key
        /// </summary>
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get
            {
                return this["key"].ToString();
            }
            set
            {
                this["key"] = value;
            }
        }


        /// <summary>
        /// 路径
        /// </summary>
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return this["path"].ToString();
            }
            set
            {
                this["path"] = value;
            }
        }
    }
}
