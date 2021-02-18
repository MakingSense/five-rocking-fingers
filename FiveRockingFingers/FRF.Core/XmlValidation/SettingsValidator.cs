﻿using FRF.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace FRF.Core.XmlValidation
{
    public class SettingsValidator : ISettingsValidator
    {
        public bool ValidateSettings(Artifact artifact)
        {
            var areSettingsValid = true;

            string rootPath = Path.GetDirectoryName(AppContext.BaseDirectory);

            var path = "";

            switch (artifact.ArtifactType.Provider.Name)
            {
                case ArtifactTypes.Custom:
                    path = Path.Combine(rootPath, "CustomArtifact.xsd");
                    break;
                case ArtifactTypes.Aws:
                    switch (artifact.ArtifactType.Name)
                    {
                        case AwsS3Descriptions.Service:
                            path = Path.Combine(path, "AwsS3.xsd");
                            break;
                        case AwsEc2Descriptions.ServiceValue:
                            path = Path.Combine(rootPath, "AwsEc2.xsd");
                            break;
                    }
                    break;
                default:
                    return !areSettingsValid;
            }

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, path);

            XDocument settingsDoc = new XDocument(artifact.Settings);

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
