using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos.Artifacts
{
    class ProviderArtifactSettingDTO
    {
        public string Name { get; set; }
        public IList<string> Values { get; set; }
    }
}
