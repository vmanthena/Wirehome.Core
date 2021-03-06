﻿using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Wirehome.Core.Python;
using Wirehome.Core.Python.Models;

namespace Wirehome.Core.HTTP.Controllers
{
    public class PythonScratchpadController : Controller
    {
        private readonly PythonScriptHostFactoryService _pythonScriptHostFactoryService;
        
        public PythonScratchpadController(PythonScriptHostFactoryService pythonScriptHostFactoryService)
        {
            _pythonScriptHostFactoryService = pythonScriptHostFactoryService ?? throw new ArgumentNullException(nameof(pythonScriptHostFactoryService));
        }

        [HttpPost]
        [Route("api/v1/python_scratchpad/execute")]
        [ApiExplorerSettings(GroupName = "v1")]
        public object ExecuteScript(string function_name = "main")
        {
            try
            {
                string script;
                using (var streamReader = new StreamReader(HttpContext.Request.Body))
                {
                    script = streamReader.ReadToEnd();
                }

                var scriptHost = _pythonScriptHostFactoryService.CreateScriptHost(null);
                scriptHost.Compile(script);

                if (string.IsNullOrEmpty(function_name))
                {
                    return null;
                }
                
                return scriptHost.InvokeFunction(function_name);
            }
            catch (Exception exception)
            {
                return new ExceptionPythonModel(exception).ConvertToPythonDictionary();
            }
        }
    }
}
