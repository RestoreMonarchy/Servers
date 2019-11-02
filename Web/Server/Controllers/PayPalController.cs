using Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Web.Server.Utilities.Database;
using Web.Server.Utilities.DiscordMessager;

namespace Web.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PayPalController : ControllerBase
    {        
        private readonly IConfiguration _configuration;
        private readonly DatabaseManager _database;
        private readonly DiscordMessager _messager;
        private readonly bool useSandbox = false;
        private readonly string rootPath;

        public PayPalController(IConfiguration configuration, DatabaseManager database, DiscordMessager messager)
        {
            this._configuration = configuration;
            this._database = database;
            this._messager = messager;
            this.useSandbox = configuration.GetValue<bool>("UsePayPalSandbox");
            this.rootPath = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
        }

        [HttpPost]
        public ActionResult Receive()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            PayPalRespond response = GetPayPalResponse();

            if (response.RespondType == RespondTypeEnum.Verified)
            {
                System.IO.File.AppendAllText(rootPath + Path.DirectorySeparatorChar + "data.txt", $"{DateTime.Now.ToString()} {response.JsonData}." + Environment.NewLine);

                Sale sale = _database.GetSale;

                //check the amount paid
                if (order.Total <= response.AmountPaid)
                {
                    // IPN Order successfully transacted. Save changes to database

                    return Ok();
                }
                else
                {
                    // Amount Paid is incorrect
                }
            }
            else
            {
                // Not verified
            }

            return Content("");
        }

        PayPalRespond GetPayPalResponse()
        {
            PayPalRespond output = new PayPalRespond();

            var formVals = new Dictionary<string, string>();
            formVals.Add("cmd", "_notify-validate");

            string paypalUrl = useSandbox ? "https://www.sandbox.paypal.com/cgi-bin/webscr" : "https://www.paypal.com/cgi-bin/webscr";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(paypalUrl);

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            byte[] param;
            using (var ms = new MemoryStream(2048))
            {
                Request.Body.CopyTo(ms);
                param = ms.ToArray();
            }

            string strRequest = Encoding.ASCII.GetString(param);

            var QueryValues = System.Web.HttpUtility.ParseQueryString(strRequest);

            output.Data = new List<QueryValue>();
            foreach (var item in QueryValues.AllKeys)
            {
                if (item.Equals("txn_id"))
                    output.TransactionID = QueryValues[item];
                else if (item.Equals("mc_gross"))
                {
                    CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
                    NumberStyles style = NumberStyles.Number;

                    Decimal amountPaid = 0;
                    Decimal.TryParse(QueryValues[item], style, culture, out amountPaid);
                    output.AmountPaid = amountPaid;
                }
                else if (item.Equals("custom"))
                    output.OrderID = QueryValues[item];

                output.Data.Add(new QueryValue { Name = item, Value = QueryValues[item] });
            }
            output.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(output.Data);

            StringBuilder sb = new StringBuilder();
            sb.Append(strRequest);

            foreach (string key in formVals.Keys)
            {
                sb.AppendFormat("&{0}={1}", key, formVals[key]);
            }
            strRequest += sb.ToString();
            req.ContentLength = strRequest.Length;

            //Send the request to PayPal and get the response
            string response = "";
            using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
            {
                streamOut.Write(strRequest);
                streamOut.Close();
                using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                }
            }

            output.RespondType = response.Equals("VERIFIED") ? RespondTypeEnum.Verified : RespondTypeEnum.Invalid;

            return output;
        }

        public enum RespondTypeEnum { Verified, Invalid }

        public class PayPalRespond
        {
            public RespondTypeEnum RespondType { get; set; }
            public List<QueryValue> Data { get; set; }
            public string JsonData { get; set; }
            public string TransactionID { get; set; }
            public string OrderID { get; set; }
            public Decimal AmountPaid { get; set; }
        }

        public class QueryValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }


        
    }
}
