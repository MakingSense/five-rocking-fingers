using AutoMapper;
using FRF.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace FRF.Web.Dtos.Artifacts
{
    public class XMLHelper : IValueResolver<Artifact, ArtifactDTO, Dictionary<string, string>>
    {
        public Dictionary<string, string> Resolve(Artifact source, ArtifactDTO destination, Dictionary<string, string> destMember, ResolutionContext context)
        {
            var settings = new Dictionary<string, string>();
            if(!source.Settings.HasElements)
            {
                return settings;
            }

            foreach (var element in source.Settings.Elements())
            {
                settings = GetKeyValuePair(element, settings);
            }

            return settings;
        }

        private Dictionary<string, string> GetKeyValuePair(XElement xelement, Dictionary<string, string> settings)
        {
            if(xelement.HasElements)
            {
                settings.Add(xelement.Name.ToString(), "");
            }
            else
            {
                if(!settings.ContainsKey(xelement.Name.ToString()))
                {
                    settings.Add(xelement.Name.ToString(), xelement.Value);
                }                
            }

            return settings;
        }
    }
}
