using System.Configuration;

namespace LinkedinBot.Configuration
{
    public class KeyWordElement : ConfigurationElement
    {
        [ConfigurationProperty("word")]
        public string Word
        {
            get
            {
                return (string)this["word"];
            }
        }
    }
}
