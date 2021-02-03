using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace FRF.Core.XmlValidation
{
    public class SettingsValidator
    {
        public static bool ValidateSettings(XElement settings)
        {
            var areSettingsValid = true;

            string rootPath = Path.GetDirectoryName(AppContext.BaseDirectory);

            var path = Path.Combine(rootPath, "CustomArtifact.xsd");

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, path);

            XDocument settingsDoc = new XDocument(settings);

            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.ValidationType = ValidationType.Schema;
            xrs.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            xrs.Schemas = schemaSet;
            xrs.ValidationEventHandler += (s, e) => {
                if (e != null)
                {
                    areSettingsValid = false;
                }
            };

            XmlReader xr = XmlReader.Create(settingsDoc.CreateReader(), xrs);

            while (xr.Read()) { }

            return areSettingsValid;
        }
    }
}
