using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace Codeable.Foundation.UI.Web.Core.MVC.Binders
{
    public class BrowserNamedPostModelBinder : DefaultModelBinder
    {
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            string prefix = GenerateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
            if (!bindingContext.ValueProvider.ContainsPrefix(prefix))
            {
                return;
            }

            IModelBinder propertyBinder = this.Binders.GetBinder(propertyDescriptor.PropertyType);
            object existingPropertyValue = propertyDescriptor.GetValue(bindingContext.Model);
            ModelMetadata propertyMetaData = bindingContext.PropertyMetadata[propertyDescriptor.Name];
            propertyMetaData.Model = existingPropertyValue;
            
            ModelBindingContext context = new ModelBindingContext()
            {
                ModelMetadata = propertyMetaData,
                ModelName = prefix,
                ModelState = bindingContext.ModelState,
                ValueProvider = bindingContext.ValueProvider
            };
            object newValue = this.GetPropertyValue(controllerContext, context, propertyDescriptor, propertyBinder);
            propertyMetaData.Model = newValue;
            ModelState state = bindingContext.ModelState[prefix];
            if ((state == null) || (state.Errors.Count == 0))
            {
                if (this.OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, newValue))
                {
                    this.SetProperty(controllerContext, bindingContext, propertyDescriptor, newValue);
                    this.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, newValue);
                    return;
                }
                return;
            }
            this.SetProperty(controllerContext, bindingContext, propertyDescriptor, newValue);

            List<ModelError> errors = state.Errors.Where<ModelError>(delegate(ModelError err)
            {
                return (string.IsNullOrEmpty(err.ErrorMessage) && (err.Exception != null));
            }).ToList<ModelError>();

            foreach (ModelError error in errors)
            {
                for (Exception exception = error.Exception; exception != null; exception = exception.InnerException)
                {
                    if (exception is FormatException)
                    {
                        string displayName = propertyMetaData.GetDisplayName();
                        string valueInvalidResource = "The Value '{0}' is not valid for {1}";
                        string errorMessage = string.Format(CultureInfo.CurrentCulture, valueInvalidResource, new object[] { state.Value.AttemptedValue, displayName });
                        state.Errors.Remove(error);
                        state.Errors.Add(errorMessage);
                        break;
                    }
                }
            }

        }

        protected internal string GenerateSubPropertyName(string prefix, string propertyName)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return propertyName;
            }
            if (string.IsNullOrEmpty(propertyName))
            {
                return prefix;
            }
            return string.Format("{0}[{1}]", prefix, propertyName);
        }


    }
}