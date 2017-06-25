namespace LinkedinBot
{
    class Program
    {
        public static WebTraveler WebTraveler;

        static void Main(string[] args)
        {
            using (WebTraveler = new WebTraveler())
            {
                WebTraveler.NavigateLinkedin();
                WebTraveler.Authorize();
                WebTraveler.GoSearch();
            }
        }
    }
}
