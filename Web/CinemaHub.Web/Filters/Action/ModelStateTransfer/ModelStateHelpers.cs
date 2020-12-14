namespace CinemaHub.Web.Filters.Action.ModelStateTransfer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.ModelBinding;

    using Newtonsoft.Json;

    public static class ModelStateHelpers
    {
        public static string SerialiseModelState(ModelStateDictionary modelState)
        {
            var errorList = modelState
                .Select(kvp => new ModelStateDto()
                                   {
                                       Key = kvp.Key,
                                       AttemptedValue = kvp.Value.AttemptedValue == "true, false" ? "true" : kvp.Value.AttemptedValue, // weird checkbox behavior - returns true,false instead of true and can't be deserialized
                                       RawValue = kvp.Value.RawValue,
                                       ErrorMessages = kvp.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                                   });

                
            return JsonConvert.SerializeObject(errorList);
        }

        public static ModelStateDictionary DeserialiseModelState(string serialisedErrorList)
        {
            var errorList = JsonConvert.DeserializeObject<List<ModelStateDto>>(serialisedErrorList);
            var modelState = new ModelStateDictionary();

            foreach (var item in errorList)
            {
                modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                foreach (var error in item.ErrorMessages)
                {
                    modelState.AddModelError(item.Key, error);
                }
            }
            return modelState;
        }
    }
}
