using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SettingsApp.Common
{
    public class Integer16NullableBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null || string.IsNullOrEmpty(value.AttemptedValue))
                return null;
            else
            {
                return new Integer16Binder().BindModel(controllerContext, bindingContext);
            }
        }
    }
    
    public class Integer16Binder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            Int16 temp;
            if (value == null || string.IsNullOrEmpty(value.AttemptedValue) || !Int16.TryParse(value.AttemptedValue, out temp))
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Only numbers allowed max(32767).");
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);
                return null;
            }

            return temp;
        }
    }

    public class Integer32NullableBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null || string.IsNullOrEmpty(value.AttemptedValue))
                return null;
            else
            {
                return new Integer32Binder().BindModel(controllerContext, bindingContext);
            }
        }
    }

    public class Integer32Binder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            Int32 temp;
            if (value == null || string.IsNullOrEmpty(value.AttemptedValue) || !Int32.TryParse(value.AttemptedValue, out temp))
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Only numbers allowed max(2147483647).");
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);
                return null;
            }

            return temp;
        }
    }

    public class Integer64NullableBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null || string.IsNullOrEmpty(value.AttemptedValue))
                return null;
            else
            {
                return new Integer64Binder().BindModel(controllerContext, bindingContext);
            }
        }
    }

    public class Integer64Binder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            Int64 temp;
            if (value == null || string.IsNullOrEmpty(value.AttemptedValue) || !Int64.TryParse(value.AttemptedValue, out temp))
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Only numbers allowed max(9,223,372,036,854,775,808).");
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);
                return null;
            }

            return temp;
        }
    }

    public class DecimalNullableBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null || string.IsNullOrEmpty(value.AttemptedValue))
                return null;
            else
            {
                return new DecimalBinder().BindModel(controllerContext, bindingContext);
            }
        }
    }

    public class DecimalBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            decimal temp;
            if (value == null || string.IsNullOrEmpty(value.AttemptedValue) || !decimal.TryParse(value.AttemptedValue, out temp))
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Only numbers and decimal allowed.");
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);
                return null;
            }

            return temp;
        }
    }
}