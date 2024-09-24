using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using WebApplication_Students_Teachers_.Infrastructure.Identity;
using WebApplication_Students_Teachers_.Models;
using WebApplication_Students_Teachers_.Models.ViewModels;
using WebApplication_Students_Teachers_.Services;

namespace WebApplication_Students_Teachers_.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly ApplicationSignInManager _signInManager;
        private readonly ApplicationUserManager _userManager;

        public AuthorizationController(ApplicationUserManager applicationUserManager, ApplicationSignInManager applicationSignInManager)
        {
            _userManager = applicationUserManager;
            _signInManager = applicationSignInManager;
        }

        // GET: Authorization
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            var registrationViewModel = new RegisterViewModel { ModalMessageViewModel = new ModalMessageViewModel { IsError = false } };
            return View(registrationViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                registerViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Error", Body = "Received data is invalid!" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/Register.cshtml",
                    ViewData = new ViewDataDictionary(registerViewModel)
                };
            }

            var user = new User { UserName = registerViewModel.Login, PasswordHash = registerViewModel.Password, Name = registerViewModel.Name, Email = registerViewModel.Email };
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (!result.Succeeded)
            {
                var error = result.Errors.ToList()[0];

                registerViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Error", Body = error };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/Register.cshtml",
                    ViewData = new ViewDataDictionary(registerViewModel)
                };
            }

            result = await _userManager.AddToRoleAsync(user.Id, registerViewModel.Role);
            if (!result.Succeeded)
            {
                var error = result.Errors.ToList()[0];

                registerViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Error", Body = error };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/Register.cshtml",
                    ViewData = new ViewDataDictionary(registerViewModel)
                };
            }

            await _userManager.SendEmailAsync(user.Id, "Successfull Registration", "We are glad to notify you about successful creation of an account!");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Authorization", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await _userManager.SendEmailAsync(user.Id, "Confirm Email", "<p>Please, <a href='" +  callbackUrl + "'>Click Here</a> to confirm your email.</p>");

            registerViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Confirm Email", Body = "Please, check your mail-box and confirm your email" };
            return new ViewResult()
            {
                ViewName = "~/Views/Authorization/Register.cshtml",
                ViewData = new ViewDataDictionary(registerViewModel)
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            var loginViewModel = new LoginViewModel { ModalMessageViewModel = new ModalMessageViewModel { IsError = false } };
            return View(loginViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                loginViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Error", Body = "Received data is invalid!" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/Login.cshtml",
                    ViewData = new ViewDataDictionary(loginViewModel)
                };
            }

            if (loginViewModel.RememberMe == null) loginViewModel.RememberMe = false;

            var result = await _signInManager.PasswordSignInAsync(loginViewModel.Login, loginViewModel.Password, (bool)loginViewModel.RememberMe, true);

            if (result == SignInStatus.LockedOut)
            {
                loginViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "LockOut", Body = "You have run out of attempts! Wait a minute before trying again!" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/Login.cshtml",
                    ViewData = new ViewDataDictionary(loginViewModel)
                };
            }
            else if (result == SignInStatus.Failure)
            {
                loginViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "A failure occured while signing in!", Body = "Please, check your login and password" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/Login.cshtml",
                    ViewData = new ViewDataDictionary(loginViewModel)
                };
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginViewModel.Login);

            return await AuthorizeAsync(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            _signInManager.AuthenticationManager.SignOut();

            return RedirectToRoute("Default", new { controller = "Authorization", action = "Login" });
        }

        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null && code == null)
            {
                var loginViewModel = new LoginViewModel();
                loginViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Error", Body = "Email was not confirmed!" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/Login.cshtml",
                    ViewData = new ViewDataDictionary(loginViewModel)
                };
            }

            var resultConfirmEmail = await _userManager.ConfirmEmailAsync(userId, code);

            if (!resultConfirmEmail.Succeeded)
            {
                var error = resultConfirmEmail.Errors.ToList()[0];

                var loginViewModel = new LoginViewModel();
                loginViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Email was not confirmed!", Body = error };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/Login.cshtml",
                    ViewData = new ViewDataDictionary(loginViewModel)
                };
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

            await _signInManager.SignInAsync(user, false, false);

            return await AuthorizeAsync(user);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
             return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Authorization", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        [Route("/signin-google")]
        [Route("/signin-facebook")]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await _signInManager.AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                var loginViewModel = new LoginViewModel();
                loginViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Error", Body = "Can't reach external login info!" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/Login.cshtml",
                    ViewData = new ViewDataDictionary(loginViewModel)
                };
            }

            var result = await _signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await _userManager.FindByEmailAsync(loginInfo.Email);
                    return await AuthorizeAsync(user);
                case SignInStatus.LockedOut:
                    var loginViewModel = new LoginViewModel();
                    loginViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "LockOut", Body = "You have run out of attempts! Wait a minute before trying again!" };

                    return new ViewResult()
                    {
                        ViewName = "~/Views/Authorization/Login.cshtml",
                        ViewData = new ViewDataDictionary(loginViewModel)
                    };
                case SignInStatus.Failure:
                default:
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email, ModalMessageViewModel = new ModalMessageViewModel { IsError = false } });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = await _userManager.FindByEmailAsync(model.Email);
            //    return Authorize(user);
            //}

            if (ModelState.IsValid)
            {
                var info = await _signInManager.AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    var loginViewModel = new LoginViewModel();
                    loginViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Login Confirmation Failure", Body = "Login confirmation was not succeeded!" };

                    return new ViewResult()
                    {
                        ViewName = "~/Views/Authorization/Login.cshtml",
                        ViewData = new ViewDataDictionary(loginViewModel)
                    };
                }

                var user = new User { UserName = model.Email, PasswordHash = model.Password, Email = model.Email };
                var result = await _userManager.CreateAsync(user, user.PasswordHash);
                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user.Id, model.Role);
                    if (!result.Succeeded)
                    {
                        var error = result.Errors.ToList()[0];

                        var loginViewModel = new LoginViewModel();
                        loginViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Login Confirmation Failure", Body = error };

                        return new ViewResult()
                        {
                            ViewName = "~/Views/Authorization/Login.cshtml",
                            ViewData = new ViewDataDictionary(loginViewModel)
                        };
                    }

                    await _userManager.SendEmailAsync(user.Id, "Successfull Registration", "We are glad to notify you about successful creation of an account!");
                    result = await _userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return await AuthorizeAsync(user);
                    }
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            model.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Error", Body = "Received data is invalid!" };

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel { ModalMessage = new ModalMessageViewModel { Show = false }});
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ModalMessage = new ModalMessageViewModel { Show = true, IsError = true, Header = "Error", Body = "Received data is invalid!" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/ForgotPassword.cshtml",
                    ViewData = new ViewDataDictionary(model)
                };
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                model.ModalMessage = new ModalMessageViewModel { Show = true, IsError = true, Header = "Error", Body = "User with such an email does not exist!" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/ForgotPassword.cshtml",
                    ViewData = new ViewDataDictionary(model)
                };
            }

            if (!(await _userManager.IsEmailConfirmedAsync(user.Id)))
            {
                var confirmEmailCode = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var confirmEmailCallbackUrl = Url.Action("ConfirmEmail", "Authorization", new { userId = user.Id, code = confirmEmailCode }, protocol: Request.Url.Scheme);
                await _userManager.SendEmailAsync(user.Id, "Confirm Email", "<p>Please, <a href='" + confirmEmailCallbackUrl + "'>Click Here</a> to confirm your email.</p> <p>You will be login automatically. When logging into your account again, you are able to reset your password now clicking on 'Forgot Password?' link and following the instructions.");

                model.ModalMessage = new ModalMessageViewModel { Show = true, IsError = true, Header = "Email is not confirmed!", Body = "Please, check your mail-box and confirm your email first!" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/ForgotPassword.cshtml",
                    ViewData = new ViewDataDictionary(model)
                };
            }

            var resetPasswordCode = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
            var resetPasswordCallbackUrl = Url.Action("ResetPassword", "Authorization", new {userId = user.Id, code = resetPasswordCode }, protocol: Request.Url.Scheme);
            await _userManager.SendEmailAsync(user.Id, "Reset Password", "<p>Please, <a href='" + resetPasswordCallbackUrl + "'>Click Here</a> to reset your password.</p>");

            model.ModalMessage = new ModalMessageViewModel { Show = true, IsError = false, Header = "Email was sent!", Body = "Please, check your mail-box and reset your password!" };
            return new ViewResult()
            {
                ViewName = "~/Views/Authorization/ForgotPassword.cshtml",
                ViewData = new ViewDataDictionary(model)
            };
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            if (code == null)
            {
                var model = new ForgotPasswordViewModel();
                model.ModalMessage = new ModalMessageViewModel { Show = true, IsError = false, Header = "Error", Body = "Code was not received!" };
                
                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/ForgotPassword.cshtml",
                    ViewData = new ViewDataDictionary(model)
                };
            }

            return View(new ResetPasswordViewModel { Code = code, ModalMessage = new ModalMessageViewModel { Show = false } });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ModalMessage = new ModalMessageViewModel { Show = true, IsError = false, Header = "Error", Body = "Received data is invalid!" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/ResetPassword.cshtml",
                    ViewData = new ViewDataDictionary(model)
                };
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                model.ModalMessage = new ModalMessageViewModel { Show = true, IsError = true, Header = "Error", Body = "User with such an email does not exist!" };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/ResetPassword.cshtml",
                    ViewData = new ViewDataDictionary(model)
                };
            }

            var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            if (!result.Succeeded)
            {
                model.ModalMessage = new ModalMessageViewModel { Show = true, IsError = true, Header = "Error", Body = result.Errors.First() };

                return new ViewResult()
                {
                    ViewName = "~/Views/Authorization/ResetPassword.cshtml",
                    ViewData = new ViewDataDictionary(model)
                };
            }

            var loginViewModel = new LoginViewModel();
            loginViewModel.ModalMessageViewModel = new ModalMessageViewModel { IsError = true, Header = "Success", Body = "Password was reset successfully!" };

            return new ViewResult()
            {
                ViewName = "~/Views/Authorization/Login.cshtml",
                ViewData = new ViewDataDictionary(loginViewModel)
            };
        }

        private async Task<ActionResult> AuthorizeAsync(User user)
        {
            var userRole = (await _userManager.GetRolesAsync(user.Id)).FirstOrDefault();

            if (userRole == Role.Teacher.ToString())
            {
                return RedirectToRoute("Teacher_default", new { controller = "TimeTable", action = "Index" });
            }
            else if (userRole == Role.Student.ToString()) 
            {
                return RedirectToRoute("Student_default", new { controller = "TimeTable", action = "Index" });
            }
            else
            {
                return new ViewResult()
                {
                    ViewName = "~/Views/Shared/Error.cshtml",
                    ViewData = new ViewDataDictionary(new Exception("Not identified user role"))
                };
            }
        }

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return RedirectToAction("Login", "Authorization");
        //}

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}


        private const string XsrfKey = "XsrfId";
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null) { }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}