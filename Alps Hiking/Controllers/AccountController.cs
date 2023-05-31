using Alps_Hiking.Entities;
using Alps_Hiking.Utilities.Roles;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Net;

namespace Alps_Hiking.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        public SignInManager<User> _signInManager { get; }
        public RoleManager<IdentityRole> _roleManager { get; }
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
      

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM account)
        {
            if (!ModelState.IsValid) return View();
            User user = new User
            {
                Fullname = string.Concat(account.Firstname, " ", account.Lastname),
                Email = account.Email,
                UserName = account.Username
            };
            IdentityResult result = await _userManager.CreateAsync(user, account.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError message in result.Errors)
                {
                    ModelState.AddModelError("", message.Description);
                }
                return View();
            }
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action(nameof(VerifyEmail), "Account", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());
            string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string callbackUrl = Url.Action("VerifyEmail", "Account", new { email = user.Email, token = confirmationToken }, Request.Scheme);
            string emailBody = $"Welcome, {user.Fullname}! Your Register is Successul. Please for confirm your account<a href='{callbackUrl}'>click here</a>.";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("alpshiking1994@gmail.com", "Alps Hiking");
            mail.To.Add(new MailAddress(user.Email));
            mail.Subject = "Verify Email";
            mail.Body = string.Empty;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/assets/ConfirmEmail/confirmemail.html"))
            {
                body = reader.ReadToEnd();
            }
            mail.Body = body.Replace("{{link}}", link);
            mail.IsBodyHtml = true;
            mail.IsBodyHtml = true;
            
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("alpshiking1994@gmail.com", "jipcexnfxtvvsdiy");
            smtp.Send(mail);

            await _userManager.AddToRoleAsync(user, "Member");
            return RedirectToAction("Index", "Home");
            ;
        }


        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest();

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return BadRequest();

            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("Index", "Home");
        }



        //public async Task CreateRoles()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
        //    await _roleManager.CreateAsync(new IdentityRole(Roles.Moderator.ToString()));
        //    await _roleManager.CreateAsync(new IdentityRole(Roles.Member.ToString()));
        //}


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM account)
        {

            if (!ModelState.IsValid) return View();

            User user = await _userManager.FindByNameAsync(account.Username);
            if (user is null)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains(Roles.Member.ToString()))
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, account.Password, account.RememberMe, true);

                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError("", "Due to your efforts, our account was blocked for 5 minutes");
                    }
                    ModelState.AddModelError("", "Username or password is incorrect");
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                TempData["Login"] = true;
            }
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
