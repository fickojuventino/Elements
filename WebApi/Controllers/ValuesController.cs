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
                // setting a connection string to our DB
                string conString = "User Id=S15315;Password=S15315;Data Source=gislab-oracle.elfak.ni.ac.rs:1521/SBP_PDB;";

                // creation of a connection
                using (OracleConnection con = new OracleConnection(conString))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        try
                        {
                            // opening of a connection
                            con.Open();
                            cmd.BindByName = true;                            

                            // format za date dd/mm/yyyy
                            string iString = $"{date} {time}";
                            // Console.WriteLine(date + " " + time);

                            // setting a desired date format
                            var outputCulture = CultureInfo.CreateSpecificCulture("es-es");
                            DateTime now = DateTime.Parse(DateTime.Now.ToString());
                            DateTime input = DateTime.Parse(iString, outputCulture);

                            // current and inserted date comparison
                            if(now < input)
                                return "You can not insert Date higher than current.";

                            // setting a sql query
                            cmd.CommandText = "select elementp.identifikacioni_kod, elementc.id, elementp.redni_broj, elementc.grupa, elementc.vrednost, elementp.vreme_pretrage from elementp"
                            + " join elementc on elementp.identifikacioni_kod = elementc.idkod";

                            // execution of a command
                            OracleDataReader reader = cmd.ExecuteReader();
                            string output = "";
                            int i = -1;
                            // using OracleDataReader to read returned data
                            // while there is data to read
                            while(reader.Read()){
                                string vreme = reader["vreme_pretrage"].ToString().Split(' ')[0] + " " + reader["vreme_pretrage"].ToString().Split(' ')[1];

                                // skip step if inserted date is higher than date of data creation from DB
                                if(input > DateTime.Parse(vreme, outputCulture)) continue;
                                else{
                                    string cString = "\tElementC: " + reader["id"] + " grupa: " + reader["grupa"] + " vrednost: " + reader["vrednost"]
                                        + " vreme pretrage: " + reader["vreme_pretrage"] + ".\n";
                                    if(i != int.Parse(reader["identifikacioni_kod"].ToString())){
                                        i = int.Parse(reader["identifikacioni_kod"].ToString());
                                        output += "ElementP: " + reader["identifikacioni_kod"] + " redni broj:" + reader["redni_broj"] + 
                                        ". Elementi:\n";
                                        output += cString;
                                    }
                                    else{
                                        output += cString;
                                    }
                                }
                            }

                            // release the resources that are used by this object
                            reader.Dispose();
                            // close the connection to DB
                            con.Close();
                            // if we found something return that data
                            if(output != "") return output;
                            return "No entry found!";
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    }
                }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Elements value)
        {
            // stop if received data is not correct
            if(!ModelState.IsValid){
                throw new InvalidOperationException("Invalid");
            }
 
            // setting a connection string
            string conString = "User Id=S15315;Password=S15315;Data Source=gislab-oracle.elfak.ni.ac.rs:1521/SBP_PDB;";
            // creation of a connection
            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {           
                        // opening of a connection
                        con.Open();
                        cmd.BindByName = true;                            
                        
                        foreach (ElementP ep in value.elementi)
                        {
                            // current date and time for insertion into DB
                            string now = DateTime.Now.ToString();
                            string dateAndTime = now.Split(' ')[0] + " ";
                            dateAndTime += now.Split(' ')[1];

                            // insert data for elementP in DB
                            cmd.CommandText = $"insert into elementp (redni_broj, vreme_pretrage) values ({ep.redni_broj}, to_date('{dateAndTime}', 'dd/mm/yyyy hh:mi:ss'))";
                            OracleDataReader odr = cmd.ExecuteReader();
                            // getting a generated key from DB for elementP
                            cmd.CommandText = $"select identifikacioni_kod from elementp where vreme_pretrage = to_date('{dateAndTime}', 'dd/mm/yyyy hh:mi:ss') and redni_broj = {ep.redni_broj}";
                            odr = cmd.ExecuteReader();
                            odr.Read();
                            int idKod = int.Parse(odr.GetValue(0).ToString());
                            
                            // insert data for elements of type elementC into DB
                            foreach(ElementC ec in ep.elementi)
                            {
                                cmd.CommandText = $"insert into elementc (vrednost, grupa, idkod) values ({ec.vrednost}, '{ec.grupa.ToString()[0]}', {idKod})";
                                odr = cmd.ExecuteReader();
                            }
                        }

                        // closing of a connection
                        con.Close();
                        Console.WriteLine("Successful");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }

    #region input_json_objects
    public class Elements{
        public string output { get; set;}
        public List<ElementP> elementi{ get; set; }
    }

    public class ElementP{
        // public int identifikacioni_kod{ get; set; }
        public int redni_broj { get; set; }
        public List<ElementC> elementi {get; set; }
    }

    public class ElementC{
        public char grupa{ get; set; }
        public int vrednost{ get; set; }
    }
    #endregion
}
