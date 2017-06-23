using System.Configuration;

namespace LinkedinBot.Configuration
{
    public class MakeContactElement : ConfigurationElement
    {
        [ConfigurationProperty("text")]
        public string Text
        {
            get
            {
                return (string)this["text"];
            }
        }
    }
}
