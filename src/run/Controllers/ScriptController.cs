using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace run.Controllers
{
    [Route("script/{name}")]
    public class ScriptController : Controller
    {
        public ScriptController(ILogger<ScriptController> logger, IOptions<ScriptOptions> optionsAccessor)
        {
            _logger = logger;
            _optionsAccessor = optionsAccessor;
        }

        // GET: script
        [HttpGet]
        public string Get(string name)
        {
            _logger.LogInformation($"GET: /script/{name}");
            
            return StartScriptProcess(name);


        }

        // POST: script
        [HttpPost]
        public string Post([FromBody]string value)
        {
            _logger.LogInformation("POST: /script");
            return StartScriptProcess(null);
        }

        private string StartScriptProcess(string script)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();

                if (script == null)
                {
                    script = _optionsAccessor.Value.Script; //configuration.GetSection("Script").Value;
                }
                else
                {
                    script = _optionsAccessor.Value.Folder + "\\" + script;
                }
                _logger.LogInformation($"Script path {script}.");

                psi.FileName = script; //"/tmp/bash.sh";
                psi.UseShellExecute = false;
                //psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;
                //psi.Arguments = "";
                Process p = Process.Start(psi);
                //string strOutput = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                //_logger.LogInformation(strOutput);
                _logger.LogInformation("Done with POST request.");
                return "OK";
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} {e.StackTrace}");
                _logger.LogInformation("Done with POST request.");
                return e.Message;
            }
        }

        private readonly IOptions<ScriptOptions> _optionsAccessor;

        private readonly ILogger<ScriptController> _logger;

    }
}
