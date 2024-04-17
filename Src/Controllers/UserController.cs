using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ServiceSSO;
using System.Security.Claims;

namespace EFID_V2.Controllers
{
    public class UserController : Controller
    {
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }

        public async Task<IActionResult> Login(IFormCollection form)
        {
            SSOWSSoapClient _sso = new SSOWSSoapClient(0);
            var username = form["Username"];
            var password = form["Password"];
            var user = await _sso.ValidateUserAsync(username, password);
            if (user.Body.ValidateUserResult)
            {
                var principal = await Authenticate(username);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
                });
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["msg"] = "Wrong username or password. Please check and try again.";
            }
            return View();
        }

        public async Task<ClaimsPrincipal> Authenticate(string username)
        {
            try
            {
                var claims = new List<Claim>
                        {
                            new Claim("UserId", username),

                        };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                return principal;
            }
            catch (Exception ex)
            {
                throw new Exception("error : " + ex.ToString());
            }
        }
    }
}
