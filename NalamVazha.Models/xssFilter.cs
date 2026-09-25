namespace NalamVazha.Models{
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class xssFilter : ValidationAttribute
{
public HashSet<string> BlackList = new HashSet<string>() 
{
{ "<script" },
{ "script/>" },
{ "<iframe" },
{ "iframe/>" },
{ "<form" },
{ "form/>" },
{ "<embed" },
{ "embed/>" },
{ "<link" },
{ "link>" },
{ "<head" },
{ "head>" },
{ "<meta" }
};
protected override ValidationResult IsValid(object value, ValidationContext validationContext)
{
string strValue = value as string;
StringBuilder sb =new StringBuilder();
if (!string.IsNullOrEmpty(strValue))
{
foreach(var blockedVal in BlackList)
{
if(strValue.ToLower().Contains(blockedVal)){
sb.AppendLine(blockedVal);
}
}
}
if(sb.ToString().Length ==0)
return ValidationResult.Success;
else
{
return new ValidationResult("Please Remove "+ sb.ToString() +" from "+validationContext.DisplayName.ToString());
}
}
}
}

