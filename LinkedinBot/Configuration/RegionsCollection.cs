using System.Configuration;

namespace LinkedinBot.Configuration
{
    [ConfigurationCollection(typeof(VacancyElement), AddItemName = "region")]
    public class RegionsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RegionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RegionElement)(element)).Place;
        }

        public RegionElement this[int idx]
        {
            get { return (RegionElement)BaseGet(idx); }
        }
    }
}
