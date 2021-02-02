namespace FRF.Core.Models
{
    public class CustomArtifact : Artifact
    {
        public override decimal GetPrice()
        {
            if (Settings.Element("price") != null)
            {
                return decimal.Parse(Settings.Element("price").Value);
            }
            else
            {
                return 0;
            }
        }
    }
}
