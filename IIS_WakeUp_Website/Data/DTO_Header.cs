using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using YamlDotNet.Serialization;

namespace IIS_WakeUp_Website.Data
{
    [Serializable]
    public class DTO_Header : IValidatableObject
    {
        [Required(ErrorMessage = "The 'AppName' value is required.")]
        public String AppName { get; set; }

        [Required(ErrorMessage = "The 'Version' value is required.")]
        public String Version { get; set; }

        [Required(ErrorMessage = "The 'Items' values are required.")]
        [YamlMember(Alias = "Items", ApplyNamingConventions = false)]
        public List<DTO_Website> WebSiteLst { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            String assemblyAppName = Assembly.GetExecutingAssembly().GetName().Name;
            Version assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
            Version yamlVersion = new Version(this.Version);

            if (this.AppName != assemblyAppName)
            {
                yield return new ValidationResult("The YAML file provided does not seem to be made for this application. Expected: '" 
                    + assemblyAppName + "', Got: '" + this.AppName + "'");
            }
            if(yamlVersion > assemblyVersion)
            {
                yield return new ValidationResult("The YAML file's version provided is higher than the application's. Expected: '"
                    + assemblyVersion + "', Got: '" + yamlVersion + "'");
            }
            else if (yamlVersion < assemblyVersion)
            {
                yield return new ValidationResult("The YAML file's version provided is lower than the application's. Expected: '"
                    + assemblyVersion + "', Got: '" + yamlVersion + "'");
            }
        }
    }
}
