namespace NalamVazhaWebApi.Controllers
			{
				using System;
				using System.Data;
				using System.Linq;
				using Microsoft.AspNetCore.Mvc;
				using System.Collections.Generic;
				using Microsoft.Extensions.Options;
				using Microsoft.Extensions.Logging;
				using Microsoft.AspNetCore.Authorization;
				using Microsoft.Extensions.Configuration;
				using System.IdentityModel.Tokens.Jwt;
				using System.Security.Claims;
				using System.Text;
				using Microsoft.IdentityModel.Tokens;
				using NalamVazha.Models;
				using NalamVazha.DAL;
				using FluentValidation.Results;

				using Microsoft.AspNetCore.Hosting;
				using System.IO;
				using System.Net.Http.Headers;
				[Authorize()]
				[Route("api/[controller]/[action]")]
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/09/2026 05:55:01
				public class userlockoutController : Controller
				{
				public userlockoutController(IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				{
					 _configuration = configuration;
					 _logger = loggerFactory.CreateLogger<userlockoutController>();
					 _connectionSettings = connectionSettings;
					 objuserlockoutDAL = new userlockoutDAL(_connectionSettings.Value.ConnectionString);
					 hostingEnv = hostingEnvironment;
				}
				private userlockoutDAL objuserlockoutDAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
 
			  

				
			   [AllowAnonymous()]	
 			  [HttpPost()]
			  [ActionName("ins_userlockout")]
			  public virtual string ins_userlockout([FromBody]userlockoutModel model)
			  { 
				    string message = "";
					try{

					if (model.username !=null && model.username!="")
					{

						 
								
							message = objuserlockoutDAL.ins_userlockout(model);	
 


					}
					else
					{
					 	message = "please provide username";

						_logger.LogError("usersModel - ins_userlockout, Validation Error :" + message);
					}






					}catch(Exception ex){
						message=ex.Message;
						_logger.LogError(ex.Message);
					}

					return message;

			   }
 
 			  [HttpPost()]
			  [ActionName("upd_userlockout")]
			  public virtual string upd_userlockout([FromBody]userlockoutModel model)
			  { 
				    string message = "";
					try{
 					if (model.lockoutid !=null && model.loginUser !=null)
					{

						 
								
							message = objuserlockoutDAL.upd_userlockout(model);	
 


					}
					else
					{
					 	message = "please provide username / login info";

						_logger.LogError("usersModel - upd_userlockout, Validation Error :" + message);
					}






					}catch(Exception ex){
						message=ex.Message;
						_logger.LogError(ex.Message);
					}

					return message;

			   }

			   
			  [AllowAnonymous()]	
 			  [HttpPost()]
			  [ActionName("verify_userlockout")]
			  public virtual string verify_userlockout([FromBody]userlockoutModel model)
			  { 
				    string message = "";
					try{
 					if (model.username !=null)
					{

						 
								
							message = objuserlockoutDAL.verify_userlockout(model);	
 


					}
					else
					{
					 	message = "please provide username";

						_logger.LogError("usersModel - verify_userlockout, Validation Error :" + message);
					}






					}catch(Exception ex){
						message=ex.Message;
						_logger.LogError(ex.Message);
					}

					return message;

			   }

			   [HttpGet()]
			
			[ActionName("get_userlockout")]
			public virtual System.Data.DataTable get_userlockout()
			{
					 
				  	DataTable dtusers = new DataTable();
					try
					{
						dtusers = objuserlockoutDAL.get_userlockout();
					}
					catch (Exception ex)
					{
						_logger.LogError(ex.Message);
					}
					return dtusers;

			   }




		}


}

