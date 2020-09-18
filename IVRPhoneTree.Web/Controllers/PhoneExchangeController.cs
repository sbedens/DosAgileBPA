using System.Collections.Generic;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;

namespace IVRPhoneTree.Web.Controllers
{
    public class PhoneExchangeController : ControllerBase
    {
        // POST: PhoneExchange/Interconnect
        [HttpPost]
        public ActionResult Interconnect(string digits)
        {
            var userOption = digits;
            var optionPhones = new Dictionary<string, string>
            {
                {"4", "+18134861340"},
                {"5", "+13013188087"},
                {"6", "+17087070312"}
            };

            return optionPhones.ContainsKey(userOption)
                ? Dial(optionPhones[userOption]) : RedirectWelcome();
        }

        private TwiMLResult Dial(string phoneNumber)
        {
            var response = new VoiceResponse();
            response.Say("Connecting your to your agent now ..., please wait ", voice: "alice", language: "en-GB", loop: 1);
            response.Dial(phoneNumber);

            return TwiML(response);
        }
    }
}
