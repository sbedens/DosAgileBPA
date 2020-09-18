using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Exceptions;

namespace IVRPhoneTree.Web.Controllers
{
    public class MenuController : ControllerBase
    {
        // POST: Menu/Show
        [HttpPost]
        public ActionResult Show(string digits)
        {
            var selectedOption = digits;
            var optionActions = new Dictionary<string, Func<ActionResult>>()
            {
                {"1", ReturnInstructions},
                {"2", Planets}//,
              //  {"3", CreateSupportTicket }
            };

            return optionActions.ContainsKey(selectedOption) ?
                optionActions[selectedOption]() :
                RedirectWelcome();
        }

        private TwiMLResult ReturnInstructions()
        {
            var bb = Request["From"] != null ? Request["From"] : "7087070312";

            createticket("IVR-Voice", bb);
            var response = new VoiceResponse();
            response.Say("A new support ticket has been created.");
           /* response.Say("To get to your extraction point, get on your bike and go down " +
                         "the street. Then Left down an alley. Avoid the police cars. Turn left " +
                         "into an unfinished housing development. Fly over the roadblock. Go " +
                         "passed the moon. Soon after you will see your mother ship.",
                         voice: "alice", language: "en-GB");*/

            response.Say("Thank you again for calling the Office of Personnel Management. Good bye.");

            response.Hangup();

            return TwiML(response);
        }

        private TwiMLResult Planets1()
        {
            var response = new VoiceResponse();
            var gather = new Gather(action: Url.ActionUri("Interconnect", "PhoneExchange"), numDigits: 1);
            gather.Say("Connecting your to your agent now ..., please wait ",
                     voice: "alice", language: "en-GB", loop: 1);

            response.Append(gather);
            return TwiML(response);
        }

        private TwiMLResult Planets()
        {
            var response = new VoiceResponse();
            var gather = new Gather(action: Url.ActionUri("Interconnect", "PhoneExchange"), numDigits: 1);
            gather.Say("Press 4 to reach out to the OPM Helpdesk, " +
                       "Press 5 to reach out to the OPM Retirement Information Center, " +
                       "Press 6 to reach out to the OPM Retirement Services, " +
                       "Press the star key to go back to the main menu. ",
                     voice: "alice", language: "en-GB", loop: 1);
            response.Append(gather);
            return TwiML(response);
        }

       
        private void createticket(string source, string phone)
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
                    e["title"] = source + "-("+ phone +")-" + DateTime.Now.ToString();
                    Guid g = _service.Create(e);
                }
            }
            
            catch (Exception e)
            {
                return;
            }
        }
    }
}