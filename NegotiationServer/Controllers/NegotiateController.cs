// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;

namespace NegotiationServer.Controllers
{
    [ApiController]
    public class NegotiateController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public NegotiateController(IConfiguration configuration)
        {
            var connectionString = configuration["Azure:SignalR:ConnectionString"];
            _serviceManager = new ServiceManagerBuilder()
                .WithOptions(o =>
                {
	                o.ConnectionString = connectionString;
	                o.ServiceTransportType = ServiceTransportType.Persistent;
                })
                .Build();
        }

        [HttpPost("{hub}/negotiate")]
        public async Task<ActionResult> Index(string hub, string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                return BadRequest("User ID is null or empty.");
            }


            await MuckAroundWithGroups(hub, user);

            return new JsonResult(new Dictionary<string, string>()
            {
                { "url", _serviceManager.GetClientEndpoint(hub) },
                { "accessToken", _serviceManager.GenerateClientAccessToken(hub, user, new[] {new Claim("asrs.s.dc", "true") }) }
            });
        }

        [HttpGet("/test")]
        public async Task<ActionResult> Test()
        {
	        var hub = "phone_presence";
	        var user = "User";
	        await MuckAroundWithGroups(hub, user);
	        return new JsonResult(new {
		        message = "Groups have been properly mucked with."
	        });
        }

        private async Task MuckAroundWithGroups(string hub, string user)
        {
	        var hubContext = await _serviceManager.CreateHubContextAsync(hub);
	        await hubContext.UserGroups.RemoveFromAllGroupsAsync(user);
	        for (var i = 0; i < 20; i++)
	        {
		        await hubContext.UserGroups.AddToGroupAsync(user, "group" + (i + 1));
	        }

	        await hubContext.DisposeAsync();
        }
    }
}