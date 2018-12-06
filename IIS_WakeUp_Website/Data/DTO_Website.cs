using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YamlDotNet.Serialization;

namespace IIS_WakeUp_Website.Data
{
    [Serializable]
    public class DTO_Website
    {
        [Required(ErrorMessage = "The 'Name' value for the website item is required.")]
        public String Name { get; set; }

        [Required(ErrorMessage = "The 'Url' value for the website item is required.")]
        public String Url { get; set; }

        [Required(ErrorMessage = "The 'Ports' values for the website are required.")]
        [YamlMember(Alias = "Ports", ApplyNamingConventions = false)]
        public List<String> PortLst { get; set; }
    }
}
