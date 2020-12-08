namespace CinemaHub.Web.Filters.Action.InputModelTransfer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Newtonsoft.Json;

    public class ExportInputModelAttribute : ActionFilterAttribute
    {
        private object inputModel;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.inputModel = context.ActionArguments.Values.FirstOrDefault(x => x.GetType().IsClass);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!context.ModelState.IsValid && 
                this.inputModel != null &&
                (context.Result is RedirectResult ||
                 context.Result is RedirectToActionResult ||
                 context.Result is RedirectToRouteResult))
            {
                var controller = (Controller)context.Controller;
                var modelType = this.inputModel.GetType();
                var serializedModel = JsonConvert.SerializeObject(this.inputModel);

                controller.TempData[modelType.Name] = serializedModel;
                controller.TempData[modelType.Name + "Type"] = modelType.AssemblyQualifiedName;
            }

            base.OnActionExecuted(context);
        }
    }
}
