using Microsoft.Extensions.Logging;
using System;
using PnP.Core.Services;

namespace PnP.Core.Model.SharePoint
{
    /// <summary>
    /// PropertyValues class, write your custom code here
    /// </summary>
    [SharePointType("SP.PropertyValues", Uri = "_api/xxx", LinqGet = "_api/xxx")]
    internal partial class PropertyValues
    {
        public PropertyValues()
        {
            //MappingHandler = (FromJson input) =>
            //{
                //// implement custom mapping logic
                //switch (input.TargetType.Name)
                //{
                //    case "SearchScopes": return JsonMappingHelper.ToEnum<SearchScopes>(input.JsonElement);
                //    case "SearchBoxInNavBar": return JsonMappingHelper.ToEnum<SearchBoxInNavBar>(input.JsonElement);                    
                //}
                //
                //input.Log.LogDebug($"Field {input.FieldName} could not be mapped when converting from JSON");
                //
                //return null;
            //};
        }
    }
}
