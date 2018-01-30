using System;
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
            _options = optionsAccessor;
        }

        // GET: script
        [HttpGet]
        public string Get(string name)
        {
            _logger.LogDebug($"GET: /script/{name}");
            return StartScriptProcess(name);
        }

        // POST: script
        [HttpPost]
        public string Post([FromBody]string value)
        {
            _logger.LogDebug("POST: /script");
            return StartScriptProcess(null);
        }

        #region helper methods
        private string StartScriptProcess(string script)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();

                if (script == null)
                {
                    script = _options.Value.Script;
                }
                else
                {
                    script = _options.Value.Folder  + script;
                }
                _logger.LogInformation($"Running script {script}.");

                psi.FileName = script; 
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                //psi.CreateNoWindow = true;
                psi.Arguments = _options.Value.Arguments;
                Process p = Process.Start(psi);

                string strOutput = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                _logger.LogInformation(strOutput);
                _logger.LogInformation("Done executing script");

                return strOutput;
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} {e.StackTrace}");
                return e.Message;
            }
        }
        #endregion 

        #region private fields
        private readonly IOptions<ScriptOptions> _options;

        private readonly ILogger<ScriptController> _logger;
        #endregion
    }
}
