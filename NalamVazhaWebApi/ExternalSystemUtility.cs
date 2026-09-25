namespace NalamVazhaWebApi.Controllers
                {
                   
                            using System;
	                        using System.Data;
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
	                        using System.Reflection;
	                        using System.Collections;
	                        using System.Linq;
	                        using RestSharp;  // Updated namespace for RestSharp v108
	                        using System.Threading.Tasks;

                            public class ExternalSystemUtility
                            {
                                public ExternalSystemUtility(IOptions<ConnectionSettings> connectionSettings,IConfiguration configuration)
                                {
                                    _configuration = configuration;
                                    _connectionSettings = connectionSettings;
                                    obj_External_System_DAL = new External_System_DAL(_connectionSettings.Value.ConnectionString)
                                    ;
                                }
                                private External_System_DAL obj_External_System_DAL;
                                private IOptions<ConnectionSettings> _connectionSettings;
                                private IConfiguration _configuration;
         
                                public virtual bool Insert_external_tokens(string usersid,userloginModel model,string identityTokenIfAny="")
                                {

                                    try
                                    {
                                        return false;
                                    }
                                    catch
                                    {
                                        return false;
                                    }
            

                                    return true;
                                }

                              	// Asynchronous version of Get_post_response
		                        public async virtual Task<string> Get_post_response<T>(string api_url, string createduser, string authorizationcheckby, string externalSystem, T model)
		                        {
			                        string message = "";
			                        try
			                        {
				                        var client_R = new RestClient(api_url);  // RestClient initialization

				                        var request = new RestRequest(api_url,Method.Post);
				                        string user_token = "";
				                        string external_user = "";
				                        External_System_DAL objExternal_System_DAL = new External_System_DAL(_connectionSettings.Value.ConnectionString);
				                        string received_user_token = "";

				                        if (objExternal_System_DAL.get_active_token(createduser, externalSystem, ref user_token, ref external_user))
				                        {
					                        Type type = model.GetType();
					                        PropertyInfo property = type.GetProperty(authorizationcheckby);
					                        property.SetValue(model, new Guid(external_user), null);

					                        received_user_token = user_token;
					                        request.AddHeader("Authorization", $"Bearer {received_user_token}");
				                        }

				                        var model_message = Newtonsoft.Json.JsonConvert.SerializeObject(model);
				                        request.AddHeader("Content-Type", "application/json");
				                        request.AddParameter("application/json", model_message, ParameterType.RequestBody);

				                        var response_ = await client_R.ExecuteAsync(request);  // ExecuteAsync for async call
				                        message = response_.Content.ToString();

				                        if (_configuration.GetSection("api_access_get_logs").Value.ToString() == "YES")
				                        {
					                        access_logsModel objExternalapilogsModel = new access_logsModel
					                        {
						                        logged_date = DateTime.Now,
						                        expiry_date = DateTime.Now.AddMinutes(60),
						                        user_token = received_user_token,
						                        users_id = createduser,
						                        request_type = "POST",
						                        api_url = api_url,
						                        request_json = model_message,
						                        response_json = message,
						                        createduser = new Guid(createduser)
					                        };
					                        obj_External_System_DAL.Create_api_access_logs(objExternalapilogsModel);
				                        }
			                        }
			                        catch (Exception ex)
			                        {
				                        message = "External Exception | " + ex.Message;
			                        }

			                        return message;
		                        }

		                        // Asynchronous version of Get_response
		                        public async virtual Task<string> Get_response(string api_url, string createduser, string externalSystem, IDictionary<string, string> queryParameters)
		                        {
			                        string message = "";
			                        try
			                        {
				                        var client_R = new RestClient(api_url);
				                        var request = new RestRequest(api_url,Method.Get);

				                        string user_token = "";
				                        string external_user = "";
				                        string received_user_token = "";
				                        External_System_DAL objExternal_System_DAL = new External_System_DAL(_connectionSettings.Value.ConnectionString);

				                        if (objExternal_System_DAL.get_active_token(createduser, externalSystem, ref user_token, ref external_user))
				                        {
					                        received_user_token = user_token;
					                        request.AddHeader("Authorization", $"Bearer {received_user_token}");
					                        request.AddHeader("apikey", external_user);
					                        request.AddHeader("x-api-key", external_user);
				                        }

				                        foreach (KeyValuePair<string, string> kvp in queryParameters)
				                        {
					                        request.AddQueryParameter(kvp.Key, kvp.Value != null ? kvp.Value : "");
				                        }

				                        var entries = queryParameters.Select(d => $"\"{d.Key}\": \"{string.Join(",", d.Value)}\"");
				                        string RequestJSON = "{" + string.Join(",", entries) + "}";

				                        request.AddHeader("Content-Type", "application/json");

				                        var response_ = await client_R.ExecuteAsync(request);  // ExecuteAsync for async call
				                        message = response_.Content.ToString();

				                        if (_configuration.GetSection("api_access_get_logs").Value.ToString() == "YES")
				                        {
					                        access_logsModel objExternalapilogsModel = new access_logsModel
					                        {
						                        logged_date = DateTime.Now,
						                        expiry_date = DateTime.Now.AddMinutes(60),
						                        user_token = received_user_token,
						                        users_id = createduser,
						                        request_type = "GET",
						                        api_url = api_url,
						                        request_json = RequestJSON,
						                        response_json = message,
						                        createduser = new Guid(createduser)
 
					                        };
					                        obj_External_System_DAL.Create_api_access_logs(objExternalapilogsModel);
				                        }
			                        }
			                        catch (Exception ex)
			                        {
				                        message = "External Exception | " + ex.Message;
			                        }

			                        return message;
		                        }
	                        }
                        }


                
