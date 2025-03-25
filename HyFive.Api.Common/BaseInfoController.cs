using System;
using System.Reflection;
using HyFive.Api.Common.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Api.Common
{
    public abstract class BaseInfoController : ControllerBase
    {
        // GET: api/info/version
        [Route("version")]
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetVersion()
        {
            var version = new
            {
                BuildDateString = Assembly.GetExecutingAssembly().GetBuildTime().ToString("f"),
                BuildVersion = Assembly.GetExecutingAssembly().GetName().Version
            };
            return Ok(version);
        }
    }
}