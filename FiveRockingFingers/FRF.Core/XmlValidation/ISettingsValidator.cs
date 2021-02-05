using FRF.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.XmlValidation
{
    public interface ISettingsValidator
    {
        bool ValidateSettings(Artifact artifact);
    }
}
