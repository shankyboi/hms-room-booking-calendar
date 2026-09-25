using Microsoft.AspNetCore.Mvc.ModelBinding;
                        using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
                        using System;
                        using System.Collections.Generic;
                        using System.Globalization;
                        using System.Text;
                        using System.Threading.Tasks;
                        using Microsoft.Extensions.Logging;
                        namespace NalamVazha.Models
                        {
	                        public class DateTimeModelBinder : IModelBinder
	                        {
		                        private readonly IModelBinder baseBinder;
		                        private readonly ILoggerFactory factory;

		                        // Define the supported date and datetime formats
		                        private static readonly string[] SupportedFormats = new string[]
		                        {
		                        "dd/MM/yyyy",         // Date format (e.g., "25/12/2024")
                                "MM/dd/yyyy",         // Date format (e.g., "12/25/2024")
                                "dd-MMM-yyyy",        // Date format (e.g., "25-Dec-2024")
                                "yyyy-MM-dd",         // ISO 8601 date format (e.g., "2024-12-25")
                                "dd/MM/yyyy HH:mm",   // DateTime format (e.g., "25/12/2024 14:30")
                                "MM/dd/yyyy hh:mm tt" // DateTime format (e.g., "12/25/2024 02:30 PM")
		                        };

		                        public DateTimeModelBinder(ILoggerFactory factory)
		                        {
			                        this.factory = factory;
			                        baseBinder = new SimpleTypeModelBinder(typeof(DateTime), factory);
		                        }

		                        public Task BindModelAsync(ModelBindingContext bindingContext)
		                        {
			                        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			                        if (valueProviderResult != ValueProviderResult.None)
			                        {
				                        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

				                        if (!string.IsNullOrEmpty(valueProviderResult.FirstValue))
				                        {
					                        var valueAsString = valueProviderResult.FirstValue;
					                        DateTime parsedDateTime;

					                        // Try parsing the value using different formats
					                        foreach (var format in SupportedFormats)
					                        {
						                        if (DateTime.TryParseExact(valueAsString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
						                        {
							                        // Success: Return the parsed value
							                        bindingContext.Result = ModelBindingResult.Success(parsedDateTime);
							                        return Task.CompletedTask;
						                        }
					                        }

					                        // If parsing failed, add an error to the model state
					                        bindingContext.ModelState.AddModelError(
						                        bindingContext.ModelName,
						                        $"Invalid date or datetime format. Supported formats are: {string.Join(", ", SupportedFormats)}.");
				                        }
			                        }

			                        // Fallback to the base model binder
			                        return baseBinder.BindModelAsync(bindingContext);
		                        }
	                        }

                        }


 



                        
