using AutoMapper;
using FRF.Core.Models;
using System.Collections.Generic;

namespace FRF.Web.Dtos.Artifacts
{
    public class DictionaryResolver : IValueResolver<Artifact, ArtifactDTO, Dictionary<string, string>>
    {
        public Dictionary<string, string> Resolve(Artifact source, ArtifactDTO destination, Dictionary<string, string> destMember, ResolutionContext context)
        {
            var fields = new Dictionary<string, string>();

            if (source.RelationalFields == null) return null;

            foreach(var field in source.RelationalFields)
            {
                fields.Add(field.Key, field.Value);
            }

            return fields;
        }
    }
}
