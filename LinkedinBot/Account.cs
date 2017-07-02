using System.Collections.Generic;

namespace LinkedinBot
{
    public class Account
    {
        public string Link {get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Skills { get; set; }
        public string Region { get; set; }
    }
}
