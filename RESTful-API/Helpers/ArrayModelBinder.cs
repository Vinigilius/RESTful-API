using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI.Helpers
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext) {

            /* Check if we get enumerable type */
            if (!bindingContext.ModelMetadata.IsEnumerableType) {
                return Task.CompletedTask;
            }

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            if (string.IsNullOrWhiteSpace(value)) {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            var elementType = bindingContext.ModelType.GetType().GenericTypeArguments[0];
            var converter = TypeDescriptor.GetConverter(elementType);

            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(str => converter.ConvertFromString(str.Trim())).ToArray();

            var typedValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValues, 0);

            bindingContext.Model = typedValues;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
