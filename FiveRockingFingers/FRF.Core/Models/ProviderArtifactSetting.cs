using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Models
{
    public class ProviderArtifactSetting
    {
        public string Name { get; set; }
        public IList<string> Values { get; set; }
    }
}
