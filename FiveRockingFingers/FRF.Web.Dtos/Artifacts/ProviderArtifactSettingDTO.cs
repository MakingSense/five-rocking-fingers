using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos.Artifacts
{
    public class ProviderArtifactSettingDTO
    {
        public KeyValuePair<string, string> Name { get; set; }
        public IList<string> Values { get; set; }
    }
}
