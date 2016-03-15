namespace FundaScraper
{
    public class FundaObject
    {
        public StraatAdres StraatAdres { get; set; }

        public PostCodePlaats PostCodePlaats { get; set; }
        public int Surface { get; set; }
        public string Huurprijs { get; set; }
        public string HuurprijsSpec { get; set; }
        public string Koopprijs { get; set; }
        public string KoopprijsSpec { get; set; }
    }
}