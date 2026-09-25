using ClosedXML.Excel;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Admin
{
	public static class ExcelHelper
	{
		public static byte[] GenerateEmptyExcelTemplate<T>() where T : new()
		{
			using (var workbook = new XLWorkbook())
			{
				var worksheet = workbook.Worksheets.Add("Template");
				var properties = typeof(T).GetProperties();

				int colIndex = 1; // Column index in Excel

				foreach (var prop in properties)
				{
					// Check if the property should be excluded
					if (prop.Name == "craftmyapp_actionmethodname" ||
						prop.Name == "cma_client_row_id" ||
						prop.Name == "record_order")
						continue;

					// Get Display Name attribute (or fallback to property name)
					var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
					string columnName = displayAttr?.Name ?? prop.Name;

					worksheet.Cell(1, colIndex).Value = columnName; // Set column header
					colIndex++; // Move to next column
				}

				using (var stream = new MemoryStream())
				{
					workbook.SaveAs(stream);
					return stream.ToArray(); // Return as byte array
				}
			}
		}

		public static List<T> ReadExcelToModel<T>(string filePath) where T : new()
		{
			List<T> modelList = new List<T>();

			using (var workbook = new XLWorkbook(filePath))
			{
				var worksheet = workbook.Worksheet(1); // Assuming first worksheet
				var rows = worksheet.RowsUsed();

				if (rows == null || !rows.Any()) return modelList; // Return empty list if no data

				// Read header row to get column names
				var headers = rows.First().Cells().Select(c => c.Value.ToString().Trim()).ToList();

				// Iterate through data rows (skip header row)
				foreach (var row in rows.Skip(1))
				{
					T obj = new T();
					var properties = typeof(T).GetProperties();

					for (int i = 0; i < headers.Count; i++)
					{
						string columnName = headers[i];
						var prop = properties.FirstOrDefault(p =>
						{
							var displayAttr = p.GetCustomAttribute<DisplayAttribute>();
							string displayName = displayAttr?.Name ?? p.Name; // Use .Name for DisplayAttribute
							return displayName.Equals(columnName, StringComparison.OrdinalIgnoreCase);
						});

						if (prop != null && row.Cell(i + 1) != null)
						{
							object cellValue = Convert.ChangeType(row.Cell(i + 1).GetString().Trim(), prop.PropertyType);
							prop.SetValue(obj, cellValue);
						}
					}

					modelList.Add(obj);
				}
			}

			return modelList;
		}
	}
}



