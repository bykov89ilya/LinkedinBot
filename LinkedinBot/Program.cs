namespace LinkedinBot
{
    class Program
    {
        public static WebTraveler WebTraveler;

        static void Main(string[] args)
        {
            using (WebTraveler = new WebTraveler())
            {
                WebTraveler.Retry(
                    action: () => 
                    {
                        WebTraveler.NavigateLinkedin();
                        WebTraveler.Authorize();
                    },
                    delay: 2,
                    count: 5);
                WebTraveler.GoSearch();
            }
        }
    }
}
