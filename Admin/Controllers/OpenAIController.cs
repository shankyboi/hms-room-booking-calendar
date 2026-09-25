
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Admin.Controllers
{
	public class OpenAIController : BaseController
	{
		private IWebHostEnvironment hostingEnv;
		private IOptions<ApiSettings> _balSettings;
		private IOptions<MailSettings> _mailSettings;
		private string url = "";
		private string baseUrl = "";
		private string adminUrl = "";
		private string clientUrl = "";
		private string accesskey = "";
		private IHttpContextAccessor _accessor;
		public IConfiguration Configuration { get; }
		private readonly ILogger<usersController> _logger;
		private  OpenAIService _openAIService;

		public OpenAIController(IConfiguration configuration, IHttpContextAccessor accessor, IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<usersController> logger) : base(configuration)
		{
			_logger = logger;
			this.hostingEnv = env;
			_balSettings = ApiSettings;
			_mailSettings = MailSettings;
			url = _balSettings.Value.apiURL;
			baseUrl = _balSettings.Value.baseURL;
			adminUrl = _balSettings.Value.adminURL;
			clientUrl = _balSettings.Value.clientURL;
			accesskey = _balSettings.Value.accesskey;

			_accessor = accessor;
			Configuration = configuration;
			var openAIOptions = new OpenAIOptions();
			Configuration.GetSection("OpenAI").Bind(openAIOptions);
			_openAIService = new OpenAIService(Microsoft.Extensions.Options.Options.Create(openAIOptions));
		}
		public async Task<string> AnalyzeLocalImageForHuman(IFormCollection collection)
		{
			var files = collection.Files;
			foreach (var file in files)
			{
				if (file.Length == 0)
					continue;

				try
				{
					using (var memoryStream = new MemoryStream())
					{
						await file.CopyToAsync(memoryStream);
						byte[] imageBytes = memoryStream.ToArray();
						string base64Image = Convert.ToBase64String(imageBytes);

						//string result = await _openAIService.AnalyzeImageWithGPT(base64Image, "Is there a human in this image? Just answer 'yes' or 'no'.");
                       string result = await _openAIService.AnalyzeImageWithGPT(base64Image, "");

						string normalized = result?.Trim().ToLower();
						if (normalized.StartsWith("yes"))
							return "true";
						else
							return "false";
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error analyzing image: " + ex.Message);
					return "false";
				}
			}

			return "false"; // No valid file found
		}
	    public async Task<string> GenerateAlertContent(string prompt)
		{
			try
			{
				string result = await _openAIService.GenerateAlertContent(prompt);
				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating alert content");
				return "error";
			}
		}

	}
}


