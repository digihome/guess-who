using DigiHome.GuessWho.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerWebAPIFramework
{
    public class ValuesController1 : ApiController
    {
        // GET api/<controller>/5
        public string Get()
        {
            var result = AppTypeDetector.Detect();
            return $"Detected application type: {result.Display}";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}