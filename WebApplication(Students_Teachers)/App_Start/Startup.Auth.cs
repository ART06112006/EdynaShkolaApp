using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using System;
using WebApplication_Students_Teachers_.Context;
using WebApplication_Students_Teachers_.Infrastructure.Identity;
using WebApplication_Students_Teachers_.Models;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Extensions.Configuration;

namespace WebApplication_Students_Teachers_
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Authorization/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, User>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            //External Login
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            //Google
            var googleSettings = configuration.GetSection("GoogleAuthenticationSettings").Get<GoogleAuthenticationSettings>();

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = googleSettings.ClientId,
                ClientSecret = googleSettings.ClientSecret,
                CallbackPath = new PathString(googleSettings.CallbackPath)
            });

            //Facebook
            var facebookSettings = configuration.GetSection("FacebookAuthenticationSettings").Get<FacebookAuthenticationSettings>();

            app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            {
                AppId = facebookSettings.AppId,
                AppSecret = facebookSettings.AppSecret,
                CallbackPath = new PathString(facebookSettings.CallbackPath)
            });
        }
    }
}