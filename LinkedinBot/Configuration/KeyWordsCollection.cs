using System.Configuration;

namespace LinkedinBot.Configuration
{
    [ConfigurationCollection(typeof(KeyWordElement), AddItemName = "keyWord")]
    public class KeyWordsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new KeyWordElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((KeyWordElement)(element)).Word;
        }

        public KeyWordElement this[int idx]
        {
            get { return (KeyWordElement)BaseGet(idx); }
        }
    }
}
