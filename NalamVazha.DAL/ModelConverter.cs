using System;
using System.Collections.Generic;
using System.Text;

namespace NalamVazha.DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    public class ModelConverter
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy", // Specify date format
            NullValueHandling = NullValueHandling.Ignore
        };

        public static T ConvertDataRowToModel<T>(DataRow row) where T : new()
        {
            var model = new T();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                {
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    // Handle JSON string to collection or complex type conversion
                    if (propertyType == typeof(string))
                    {
                        property.SetValue(model, row[property.Name]);
                    }
                    else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        var json = row[property.Name].ToString();
                        var itemType = propertyType.GetGenericArguments()[0];
                        var collectionType = typeof(List<>).MakeGenericType(itemType);
                        var value = JsonConvert.DeserializeObject(json, collectionType,JsonSettings);
                        property.SetValue(model, value);
                    }
                    else
                    {
                        var value = Convert.ChangeType(row[property.Name], propertyType);
                        property.SetValue(model, value);
                    }
                }
            }

            return model;
        }

        public static List<modelObject> ConvertDataTableToModelListA<modelObject>(DataTable dataTable) where modelObject : new()
        {
			var modelList = new List<modelObject>();

			var properties = typeof(modelObject).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (DataRow row in dataTable.Rows)
			{
				var model = new modelObject();

				foreach (var property in properties)
				{
					if (dataTable.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
					{
						var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

						// Handle JSON string to collection or complex type conversion
						if (propertyType == typeof(string))
						{
							property.SetValue(model, row[property.Name]);
						}
						else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
						{
							var json = row[property.Name].ToString();
							var itemType = propertyType.GetGenericArguments()[0];
							var collectionType = typeof(List<>).MakeGenericType(itemType);
							var value = JsonConvert.DeserializeObject(json, collectionType, JsonSettings);
							property.SetValue(model, value);
						}
						else
						{
							var value = Convert.ChangeType(row[property.Name], propertyType);
							property.SetValue(model, value);
						}
					}
				}

				modelList.Add(model);
			}

			return modelList;
		}

		public static List<T> ConvertDataTableToModelList<T>(DataTable dataTable) where T : new()
        {
            var modelList = new List<T>();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (DataRow row in dataTable.Rows)
            {
                var model = new T();

                foreach (var property in properties)
                {
                    if (dataTable.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                    {
                        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                        // Handle JSON string to collection or complex type conversion
                        if (propertyType == typeof(string))
                        {
                            property.SetValue(model, row[property.Name]);
                        }
                        else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                        {
                            var json = row[property.Name].ToString();
                            var itemType = propertyType.GetGenericArguments()[0];
                            var collectionType = typeof(List<>).MakeGenericType(itemType);
                            var value = JsonConvert.DeserializeObject(json, collectionType,JsonSettings);
                            property.SetValue(model, value);
                        }
                        else
                        {
                            var value = Convert.ChangeType(row[property.Name], propertyType);
                            property.SetValue(model, value);
                        }
                    }
                }

                modelList.Add(model);
            }

            return modelList;
        }
    }
 
}

