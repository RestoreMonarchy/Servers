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
using System.Web;
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

        public PayPalController(IConfiguration configuration, DatabaseManager database, DiscordMessager messager)
        {
            this._configuration = configuration;
            this._database = database;
            this._messager = messager;
        }
        [HttpPost]
        public async Task<IActionResult> Receive()
        {
            IPNContext ipnContext = new IPNContext()
            {
                IPNRequest = Request
            };

            using (StreamReader reader = new StreamReader(ipnContext.IPNRequest.Body, Encoding.ASCII))
            {
                ipnContext.RequestBody = await reader.ReadToEndAsync();
            }

            //Store the IPN received from PayPal
            await LogRequest(ipnContext);

            //Fire and forget verification task
            await Task.Factory.StartNew(() => VerifyTask(ipnContext));

            //Reply back a 200 code
            return Ok();
        }

        private void VerifyTask(IPNContext ipnContext)
        {
            try
            {
                var verificationRequest = WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");

                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";

                string strRequest = "cmd=_notify-validate&" + ipnContext.RequestBody;
                verificationRequest.ContentLength = strRequest.Length;
                
                using (StreamWriter writer = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII))
                {
                    writer.Write(strRequest);
                }

                using (StreamReader reader = new StreamReader(verificationRequest.GetResponse().GetResponseStream()))
                {
                    ipnContext.Verification = reader.ReadToEnd();
                }
                
            }
            catch (Exception exception)
            {
                Task.Factory.StartNew(async () => await _messager.LogVerifyTaskExceptionAsync(exception));
            }
            Task.Factory.StartNew(async () => await ProcessVerificationResponse(ipnContext));
        }


        private async Task LogRequest(IPNContext ipnContext)
        {
            var parsed = HttpUtility.ParseQueryString(ipnContext.RequestBody);
            await _messager.SendPayPalMessage(ipnContext.RequestBody);
        }

        private async Task ProcessVerificationResponse(IPNContext ipnContext)
        {
            try
            {
                if (ipnContext.Verification.Equals("VERIFIED"))
                {
                    var body = HttpUtility.ParseQueryString(ipnContext.RequestBody);
                    int saleId;
                    int.TryParse(body["custom"], out saleId);
                    string transactionId = body["txn_id"];
                    string paymentStatus = body["payment_status"];
                    string receiverEmail = body["receiver_email"];
                    decimal paymentGross;
                    decimal.TryParse(body["mc_gross"], out paymentGross);
                    string currency = body["mc_currency"];

                    var sale = _database.GetSale(saleId);

                    if (sale != null && paymentStatus == "Completed" && receiverEmail == _configuration["PayPalEmail"] && !_database.HasBeenProcessed(transactionId))
                    {
                        if (sale.Price == paymentGross && sale.Currency == currency)
                        {
                            sale.PaymentType = body["payment_type"];
                            sale.PaymentStatus = paymentStatus;
                            sale.PayerEmail = body["payer_email"];
                            sale.TransactionId = transactionId;
                            sale.TransactionType = body["txn_type"];

                            _database.UpdateSale(sale);

                            await _messager.SendPayPalMessage($"Successfull purchase for {sale.Price} {currency}");
                        }
                        else
                        {
                            await _messager.SendPayPalMessage($"[SALE {sale.SaleId}] Price `{sale.Price}`!= `{paymentGross}` OR Currency `{sale.Currency}` != {currency}");
                        }
                    }
                    else
                    {
                        await _messager.SendPayPalMessage($"[SALE {saleId}] Status: `{paymentStatus}` Receiver: `{receiverEmail}` Already Processed: {_database.HasBeenProcessed(transactionId)}");
                    }
                }
                else if (ipnContext.Verification.Equals("INVALID"))
                {
                    await _messager.SendPayPalMessage($"Verification Invalid: ```{ipnContext.ToString()}```");
                }
                else
                {
                    await _messager.SendPayPalMessage($"Error: ```{ipnContext.ToString()}```");
                }
            } catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(e);
            }            
        }

        private class IPNContext
        {
            public HttpRequest IPNRequest { get; set; }

            public string RequestBody { get; set; }

            public string Verification { get; set; } = String.Empty;
        }
    }
}
