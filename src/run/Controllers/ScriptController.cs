using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace run.Controllers
{
    [Route("script")]
    public class ScriptController : Controller
    {
        public ScriptController(ILogger<ScriptController> logger, IOptions<ScriptOptions> optionsAccessor)
        {
            _logger = logger;
            _optionsAccessor = optionsAccessor;
        }

        // GET: script
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInformation("GET: /script");
            return new string[] { "value1", "value3" };
            _logger.LogInformation("Done with GET request");
        }

        // POST: script
        [HttpPost]
        public IActionResult Post([FromBody]string value)
        {
            _logger.LogInformation("POST: /script");
            try
            {

                ProcessStartInfo psi = new ProcessStartInfo();

                var fileName = _optionsAccessor.Value.Script;//configuration.GetSection("Script").Value;
                _logger.LogInformation($"Script path {fileName}.");

                psi.FileName = fileName; //"/tmp/bash.sh";
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;

                //psi.Arguments = "";
                Process p = Process.Start(psi);
                string strOutput = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                _logger.LogInformation(strOutput);
                return Ok(strOutput);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} {e.StackTrace}");
                return BadRequest(e.Message);
            }
            _logger.LogInformation("Done with POST request.");

        }

        private readonly IOptions<ScriptOptions> _optionsAccessor;

        private readonly ILogger<ScriptController> _logger;

    }
}
