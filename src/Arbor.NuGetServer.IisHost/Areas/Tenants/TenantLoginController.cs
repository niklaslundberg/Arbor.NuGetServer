﻿using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Arbor.NuGetServer.Api.Areas.Configuration;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using MediatR;
using Microsoft.Owin.Security;

namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    [AllowAnonymous]
    [RouteArea(TenantsAreaRegistration.TenantsAreaName)]
    public class TenantLoginController : Controller
    {
        private bool _loginEnabled;
        private IMediator _mediator;

        public TenantLoginController(IMediator mediator, IKeyValueConfiguration keyValueConfiguration)
        {
            _loginEnabled = keyValueConfiguration[ConfigurationKeys.LoginEnabled].ParseAsBoolOrDefault(false);
            _mediator = mediator;
        }

        [HttpGet]
        [Route(TenantRouteConstants.TenantHttpGetLoginRoute, Name = TenantRouteConstants.TenantHttpGetLoginRouteName)]
        public ActionResult Index()
        {
            if (!_loginEnabled)
            {
                return HttpNotFound();
            }

            return View(new TenantLoginViewModel());
        }

        [HttpPost]
        [Route(TenantRouteConstants.TenantHttpPostLoginRoute, Name = TenantRouteConstants.TenantHttpPostLoginRouteName)]
        public async Task<ActionResult> Index(string tenant, LoginInput loginInput)
        {
            if (!_loginEnabled)
            {
                return HttpNotFound();
            }

            loginInput.TenantId = tenant;

            LoginResult loginResult = await _mediator.Send(new TenantLoginRequest(loginInput));

            if (loginResult.IsSuccessful)
            {
                Request.GetOwinContext().Authentication.SignIn(new AuthenticationProperties(), loginResult.Identity);
                return RedirectToAction(nameof(TenantsController.Index), nameof(Tenants));
            }

            return View(new TenantLoginViewModel(loginResult));
        }
    }
}
