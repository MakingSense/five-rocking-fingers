﻿using System.ComponentModel.DataAnnotations;

namespace FRF.DataAccess.EntityModels
{
    public class ConfigurationSettings
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}