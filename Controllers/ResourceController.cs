using AuthServerNew.Data;
using Bogus;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OAuthServer.Models;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OAuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public class ResourceController : Controller
    {
        public readonly IConfiguration _configuration;
        public ResourceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("message")]
        public IActionResult GetMessage()
        {
            var clientId = User.GetClaim(Claims.Subject);
            return Ok($"Client '{clientId}' is authenticated.");
        }

        //Header validation for this endpoint
        [HttpPost("searchCustomerProfileByFilter")]
        public IActionResult SearchCustomerProfileByFilter([FromBody] ParaCustomerProfile _paylod) 
        {
            #region header validation
            if (!Request.Headers.TryGetValue("accept", out var accept))
            {
                return BadRequest("Missing accept header.");
            }

            if (!Request.Headers.TryGetValue("X-DBS-ReqUID", out var  request_uid))
            {
                return BadRequest("Missing X-DBS-ReqUID header.");
            }

            if (!Request.Headers.TryGetValue("X-DBS-ReqClientId", out var req_client_id))
            {
                return BadRequest("Missing X-DBS-ReqClientId header.");
            }

            if (!Request.Headers.TryGetValue("X-DBS-Country", out var country))
            {
                return BadRequest("Missing X-Custom-Header header.");
            }

            if(accept!= _configuration.GetValue<string>("CMCPSettings:CMCP_HEADER_ACCEPT"))
            {
                return BadRequest("Invalid accept header.");
            }

            if (req_client_id != _configuration.GetValue<string>("CMCPSettings:CMCP_HEADER_ReqClientId"))
            {
                return BadRequest("Invalid CMCP_HEADER_ReqClientId header.");
            }

            if (country != _configuration.GetValue<string>("CMCPSettings:CMCP_HEADER_Country"))
            {
                return BadRequest("Invalid CMCP_HEADER_Country header.");
            }
            #endregion

            var card_number = _paylod.query.contactFilter.cardNumber;

            if (string.IsNullOrEmpty(card_number))
            {
                return BadRequest("Card number is required header.");
            }

            // Check if card_number is exactly 16 digits
            if (!System.Text.RegularExpressions.Regex.IsMatch(card_number, @"^\d{16}$"))
            {
                return BadRequest("Card number must be exactly 16 digits.");
            }

            // call wcf service to search customer profile by card number

            // Generate Random Name
            var faker = new Faker();

            long unixTimestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            var helper = new Helper();
            var customerProfile = new
            {
                CardNumber = card_number,
                Branchcode = "0001",
                Timestamp = unixTimestamp,
                Salutation = "Mr",
                Name = faker.Name.FullName(),
                NRIC = helper.GenerateSingaporeNRIC(),
                Mobile = helper.GenerateSingaporePhoneNumber(faker),
                RMName= helper.GenerateRMName(faker),
                segment = "PB"
            };
            return Ok(customerProfile);
        }
    }
}
