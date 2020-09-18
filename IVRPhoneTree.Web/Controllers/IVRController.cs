using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using IVRPhoneTree.Web.Controllers;
using System.Web.UI.WebControls;
using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Twilio.AspNet.Common;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Exceptions;
using Twilio;

namespace IVRPhoneTree.Web.Controllers
{
    public class IVRController : TwilioController
    {
        // GET: IVR
        public ActionResult Index()
        {
            return View();
        }

        // POST: IVR/Welcome
        [HttpPost]
        public TwiMLResult Welcome()
        {
            var response = new VoiceResponse();
            var gather = new Gather(action: Url.ActionUri("Show", "Menu"), numDigits: 1);
            gather.Say("Thank you for calling the Office of Personnel Management," +
                       "Press 1 to create a support ticket, press 2 to connect to your support agent.");
            response.Append(gather);

            return TwiML(response);
        }

        // POST: IVR/Welcomes
        [HttpPost]
        //public async Task<ActionResult>  Welcomes()
        public TwiMLResult Welcomes()
        {
            var bdy = Request["Body"] != null ? Request["Body"] : "";
            var t = Request["From"] != null ? Request["From"] : "";

            if (Session["bdy"+t] != null)
            {
                bdy = (string)Session["bdy"+t] + " " + bdy;
            }

            Session["bdy"+t] = bdy;


            var response = new MessagingResponse();
            var mes = "Thank you for contacting the Office of Personnel Management. " +
                             "Text 101 to create a support ticket, text 202 to connect to your support agent.";
            try
            {
                if (Request["Body"].StartsWith("101") && Request["Body"].Length == 3)
                {
                    mes = "A ticket has been created successfully.";
                    CreateTicketFromSMS(Request["From"],bdy);
                    Session["bdy"+t] = "";
                    //  response.Message(mes);
                }
                else if (Request["Body"].StartsWith("202") && Request["Body"].Length == 3)
                {
                    mes = "Your agent has been notified, will contact you soon!";
                   //
                }
            }
            catch (ApiException e) {
                // else { response.Message(mes); }
                mes = "Thanks for contacting Office of Personnerl Management.";
            }
            finally
            {
                response.Message(mes);
            }

            return TwiML(response);
        }

        private TwiMLResult CreateTicketFromSMS(string p, string s)
        {
            createticket("IVR-SMS", p, s);
            var response = new MessagingResponse();
            response.Message("A new support ticket has been created. Thanks again!");

            return TwiML(response);
        }

        private void createticket(string source, string phone, string description)
        {
            IOrganizationService _service = null;
            try
            {
                ClientCredentials ccred = new ClientCredentials();
                ccred.UserName.UserName = "getyouraccount@cteccorptrial.onmicrosoft.com";
                ccred.UserName.Password = "getyourpass";

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                _service = (IOrganizationService)new OrganizationServiceProxy(new Uri("https://cteccorptrial.crm.dynamics.com/XRMServices/2011/Organization.svc"), null, ccred, null);
                if (_service != null)
                {
                    Entity e = new Entity("incident");
                    e["customerid"] = new EntityReference("account", new Guid("a16b3f4b-1be7-e611-8101-e0071b6af231"));
                    e["title"] = source + "-(" + phone + ")-" + DateTime.Now.ToString();
                    e["description"] = description ; 
                    Guid g = _service.Create(e);
                }
            }

            catch (ApiException e)
            {
                return;
            }
        }

    }
}