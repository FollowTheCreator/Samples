using System.Collections.Generic;
using Kernel.AspNetMvc.DynamicData.Models;
using Kernel.AspNetMvc.DynamicData.Services.Interfaces;

namespace IRT.Modules.DataTransfer.Generic.Domain.DynamicFields
{
    public class GenericIntegrationFields : IFormFieldContributor
    {
        public string TargetFormKey => "GenericIntegrationFields";

        public IEnumerable<DynamicFormFieldDefinition> GetFields()
        {
            return new[]
            {
                  new DynamicFormFieldDefinition
                  {
                      FieldName = "ApacheHopUrl",
                      Label = Resources.GenericIntegrationFields.ApacheHopUrl,
                      EditorTemplate = "TextBox",
                      DefaultValue = string.Empty,
                      IsRequired = true,
                      HtmlAttributes = new Dictionary<string, object>
                      {
                          { "class", "form-control" },
                      },

                      ValidationAttributes = new Dictionary<string, string>
                      {
                      }
                  }
            };
        }
    }
}
