using System.Collections.Generic;
using System.Xml.Linq;

namespace FRF.Core.Models
{
    public class CustomArtifact : Artifact
    {
        public CustomArtifact(XElement settings)
        {
            Settings = settings;

            RelationalFields = new Dictionary<string, string>();
            foreach (XElement xe in Settings.Elements())
            {
                if (xe.Attribute("type") != null)
                {
                    RelationalFields.Add(xe.Name.ToString(), xe.Attribute("type").Value.ToString());
                }
            }
        }
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
