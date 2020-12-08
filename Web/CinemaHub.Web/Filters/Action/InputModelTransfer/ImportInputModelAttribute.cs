namespace CinemaHub.Web.Filters.Action.InputModelTransfer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    using Newtonsoft.Json;

    public class ImportInputModelAttribute : ActionFilterAttribute
    {
        public string ClassName { get; set; }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var controller = context.Controller as Controller;
            var serializedModel = controller?.TempData[this.ClassName] as string;
            controller.TempData.Remove(this.ClassName);
            var typeName = controller?.TempData[this.ClassName + "Type"] as string;
            controller.TempData.Remove(this.ClassName + "Type");

            if (serializedModel != null && typeName != null && context.Result is ViewResult result)
            {
                var type = Type.GetType(typeName);

                var model = JsonConvert.DeserializeObject(serializedModel, type);

                context.Result = new ViewResult
                                     {
                                         ViewName = result.ViewName,
                                         ViewData = new ViewDataDictionary(result.ViewData)
                                                        {
                                                            Model = model,
                                                        },
                                     };
            }

            base.OnActionExecuted(context);
        }
    }
}
