using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/main")]
    [ApiController]
    public class mainController : ControllerBase
    {
        // GET: api/<mainController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<mainController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<mainController>
        [HttpPost]
        [Route ("Login")]
        public void Login([FromBody] string value)
        {//https://localhost:44343/weatherforecast
            var lol = 0;
        }

        // PUT api/<mainController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<mainController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
