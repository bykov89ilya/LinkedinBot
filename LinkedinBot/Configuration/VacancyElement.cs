using System.Configuration;

namespace LinkedinBot.Configuration
{
    public class VacancyElement : ConfigurationElement
    {
        [ConfigurationProperty("id")]
        public int Id
        {
            get
            {
                return (int)this["id"];
            }
        }

        [ConfigurationProperty("pageFrom")]
        public int PageFrom
        {
            get
            {
                return (int)this["pageFrom"];
            }
        }

        [ConfigurationProperty("pageTo")]
        public int PageTo
        {
            get
            {
                return (int)this["pageTo"];
            }
        }

        [ConfigurationProperty("description")]
        public int Description
        {
            get
            {
                return (int)this["description"];
            }
        }

        [ConfigurationProperty("searchWord")]
        public string SearchWord
        {
            get
            {
                return (string)this["searchWord"];
            }
        }

        [ConfigurationProperty("keyWords")]
        public KeyWordsCollection keyWords
        {
            get
            {
                return (KeyWordsCollection)this["keyWords"];
            }
        }
    }
}
