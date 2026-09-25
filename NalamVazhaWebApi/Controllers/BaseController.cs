
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Policy;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IdentityModel;

namespace NalamVazhaWebApi.Controllers
{
    				[Authorize(AuthenticationSchemes ="JwtBearer")]

	public abstract class BaseController : Controller
	{
		
	}
}

