namespace RatesApi.Models
{
    public class RatesInputModel
    {
        public bool Valid { get; set; }
        public double Updated { get; set; }
        public string Base { get; set; }
        public Rates Rates { get; set; }
    }
}