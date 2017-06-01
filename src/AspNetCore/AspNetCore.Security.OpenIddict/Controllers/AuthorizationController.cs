using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Core;
using AspNetCore.Security.OpenIddict.Models;

namespace AspNetCore.Security.OpenIddict.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizationController(IOptions<IdentityOptions> identityOptions, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("~/connect/token")]
        public async Task<IActionResult> Token(OpenIdConnectRequest request)
        {
            if (request.IsPasswordGrantType())
            {
                var result = await HandlePasswordGrantTypeRequest(request);
                return result;
            }
            else if (request.IsRefreshTokenGrantType())
            {
                var result = await HandleRefreshTokenTypeRequest(request);
                return result;
            }

            return BadRequest(new OpenIdConnectResponse
            {
                Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported."
            });
        }

        private async Task<IActionResult> HandlePasswordGrantTypeRequest(OpenIdConnectRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);

            var error = await ValidateUserAndPassword(user, request);
            if (error != null)
                return error;

            var ticket = await CreateTicketAsync(request, user, new AuthenticationProperties());

            // Tokengenerieren und an den Aufrufer schicken
            return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }

        private async Task<BadRequestObjectResult> ValidateUserAndPassword(ApplicationUser user, OpenIdConnectRequest request)
        {
            if (user == null)
            {
                // Return bad request if the user doesn't exist
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "Invalid username or password"
                });
            }

            // Prüfen ob sich der Benutzer noch einloggen darf
            if (!await _signInManager.CanSignInAsync(user) ||
                (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user)))
            {
                // Return bad request is the user can't sign in
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The specified user cannot sign in."
                });
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                // Passwort überprüfen
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "Invalid username or password"
                });
            }

            if (_userManager.SupportsUserLockout)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            return null;
        }

        private async Task<IActionResult> HandleRefreshTokenTypeRequest(OpenIdConnectRequest request)
        {
            // Das ClaimPrincipal aus dem Refresh-Token extrahieren
            var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(OpenIdConnectServerDefaults.AuthenticationScheme);

            // Soll der Request-Token automatisch invalidiert werden, falls sich
            // das Passwort des Users zwischenzeitlich geändert hat,
            // kann die folgende Methode verwendet werden:
            // var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
            var user = await _userManager.GetUserAsync(info.Principal);
            var error = await ValidateUserForRefreshToken(user);
            if (error != null)
            {
                return error;
            }

            // Für das neue Ticket werden Eigenschaften wie berechtigte Scopes übernommen
            var ticket = await CreateTicketAsync(request, user, info.Properties);
            return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }

        private async Task<BadRequestObjectResult> ValidateUserForRefreshToken(ApplicationUser user)
        {
            if (user == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The refresh token is no longer valid."
                });
            }

            // Ensure the user is still allowed to sign in.
            if (!await _signInManager.CanSignInAsync(user))
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The user is no longer allowed to sign in."
                });
            }

            return null;
        }

        private async Task<AuthenticationTicket> CreateTicketAsync(OpenIdConnectRequest request, ApplicationUser user, AuthenticationProperties properties = null)
        {
            // Aus dem User ein ClaimsPrincipal erzeugen dem die Claims hinzugefügt werden
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Für die letztendliche Erzeugung eines Token nutzt OpenIddict einen Ticket-Typ der alle benötigten
            // Informationen zur Generierung enthält
            var ticket = new AuthenticationTicket(principal, properties, OpenIdConnectServerDefaults.AuthenticationScheme);

            SetSupportedScopes(request, ticket);

            // Optional: Es kann festgelegt werden für welche Ressourcen ein Token Gültigkeit hat
            // Damit lassen sich einem AccessToken klare Wirksamkeitsgrenzen setzen
            ticket.SetResources("resource_server");

            ProcessClaims(ticket);

            return ticket;
        }

        private static void SetSupportedScopes(OpenIdConnectRequest request, AuthenticationTicket ticket)
        {
            // Für den Refresh-Token werden die gleichen Scopes wiederverwendet
            // und müssen nicht explizit gesetzt werden
            if (!request.IsRefreshTokenGrantType())
            {
                // Hinweis: Der Scope "offline_access" wird benötigt das OpenIddict einen
                // Refresh-Token ausstellen darf.
                ticket.SetScopes(new[]
                {
                    OpenIdConnectConstants.Scopes.OpenId,
                    OpenIdConnectConstants.Scopes.Email,
                    OpenIdConnectConstants.Scopes.Profile,
                    OpenIdConnectConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Roles

                }.Intersect(request.GetScopes()));
            }
        }

        private void ProcessClaims(AuthenticationTicket ticket)
        {
            // Claims werden nicht automatisch von OpenIddict an den access oder identity Token gehängt.
            // Über die Destinations kann festgehalten werden an welchen Token die Claims serialisiert werden sollen.
            foreach (var claim in ticket.Principal.Claims)
            {
                // Der SecurityStampClaim sollte niemals an den User ausgeliefert werden
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType)
                {
                    continue;
                }

                // Only add the iterated claim to the id_token if the corresponding scope was granted to the client application.
                // The other claims will only be added to the access_token, which is encrypted when using the default format.
                if ((claim.Type == OpenIdConnectConstants.Claims.Name && ticket.HasScope(OpenIdConnectConstants.Scopes.Profile)) ||
                    (claim.Type == OpenIdConnectConstants.Claims.Email && ticket.HasScope(OpenIdConnectConstants.Scopes.Email)) ||
                    (claim.Type == OpenIdConnectConstants.Claims.Role && ticket.HasScope(OpenIddictConstants.Claims.Roles)))
                {
                    claim.SetDestinations(OpenIdConnectConstants.Destinations.IdentityToken, OpenIdConnectConstants.Destinations.AccessToken);
                }
                else
                {
                    claim.SetDestinations(OpenIdConnectConstants.Destinations.AccessToken);
                }
            }
        }


    }
}