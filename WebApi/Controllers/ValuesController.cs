using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Globalization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public string Get(string date, string time)
        {
                string conString = "User Id=S15315;Password=S15315;Data Source=gislab-oracle.elfak.ni.ac.rs:1521/SBP_PDB;";

                using (OracleConnection con = new OracleConnection(conString))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        try
                        {
                            con.Open();
                            cmd.BindByName = true;                            

                            // format za date dd/mm/yyyy
                            string iString = $"{date} {time}";
                            var outputCulture = CultureInfo.CreateSpecificCulture("es-es");
                            DateTime now = DateTime.Parse(DateTime.Now.ToString());
                            DateTime input = DateTime.Parse(iString, outputCulture);

                            if(now < input)
                                return "You can not insert Date higher than current.";

                            cmd.CommandText = "select elementp.identifikacioni_kod, elementc.id, elementp.redni_broj, elementc.grupa, elementc.vrednost, elementp.vreme_pretrage from elementp"
                            + " join elementc on elementp.identifikacioni_kod = elementc.idkod";

                            //Execute the command and use DataReader to display the data
                            OracleDataReader reader = cmd.ExecuteReader();
                            string output = "";
                            int i = -1;
                            while(reader.Read()){
                                if(i != int.Parse(reader["identifikacioni_kod"].ToString())){
                                    string vreme = reader["vreme_pretrage"].ToString().Split(' ')[0] + " " + reader["vreme_pretrage"].ToString().Split(' ')[1];
                                    i = int.Parse(reader["identifikacioni_kod"].ToString());

                                    if(input <= DateTime.Parse(vreme, outputCulture)) {
                                        output += "ElementP: " + reader["identifikacioni_kod"] + " redni broj:" + reader["redni_broj"] + 
                                        ". Elementi:\n\tElementc: " + reader["id"] + " grupa: " + reader["grupa"] + " vrednost: " + reader["vrednost"] +
                                        " vreme pretrage: " + reader["vreme_pretrage"] + ".\n";
                                    }
                                    else{
                                        return "No entry with inputed date.";
                                    }
                                }
                                else{
                                    output += "\tElementc: " + reader["id"] + " grupa: " + reader["grupa"] + " vrednost: " 
                                    + reader["vrednost"] + " vreme pretrage: " + reader["vreme_pretrage"] + ".\n";
                                }
                            }

                            reader.Dispose();
                            return output;
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    }
                }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Elements value)
        {
            if(!ModelState.IsValid)
                throw new InvalidOperationException("Invalid");
            Console.WriteLine(value.output);
            Console.WriteLine(value.elementi[0].identifikacioniKod);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class Elements{
        public string output { get; set;}
        public List<ElementP> elementi{ get; set; }
    }

    public class ElementP{
        public int identifikacioniKod{ get; set; }
        public DateTime vremePretrage{ get; set; }
        public List<ElementC> elementi {get; set; }
    }

    public class ElementC{
        public char grupa{ get; set; }
        public int vrednost{ get; set; }
    }
}
