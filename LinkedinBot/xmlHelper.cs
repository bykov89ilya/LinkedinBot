using System.IO;
using System.Xml.Linq;

namespace LinkedinBot
{
    public static class xmlHelper
    {
        private static DirectoryInfo directoryForXml = new DirectoryInfo(
            Path.Combine(Directory.GetCurrentDirectory(), "Xml"));

        private static FileInfo xmlFile = new FileInfo(
            Path.Combine(Directory.GetCurrentDirectory(), "Xml", "employes.xml"));

        public static XDocument doc = new XDocument(); 

        static xmlHelper()
        {
            if (!Directory.Exists(directoryForXml.FullName))
            {
                Directory.CreateDirectory(directoryForXml.FullName);
            };

            if (!File.Exists(xmlFile.FullName))
            {
                using (File.Create(xmlFile.FullName));
                doc.Add(new XElement("employes"));
                doc.Save(xmlFile.FullName);
            };
        }

        public static void Load()
        {
            doc = XDocument.Load(xmlFile.FullName);
        }

        public static void Save()
        {
            doc.Save(xmlFile.FullName);
        }
    }
}
