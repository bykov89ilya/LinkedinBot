using System.Configuration;

namespace LinkedinBot.Configuration
{
    public class RegionElement : ConfigurationElement
    {
        [ConfigurationProperty("place")]
        public string Place
        {
            get
            {
                return (string)this["place"];
            }
        }

        [ConfigurationProperty("description")]
        public string Description
        {
            get
            {
                return (string)this["description"];
            }
        }
    }
}
