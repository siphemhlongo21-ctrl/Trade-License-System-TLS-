using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.DataAccessLayer.CesarDb;
using C8.TradeLicense.Models;
using C8.TradeLicense.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
//using C8.Exchange.CommonCore;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

namespace C8.TradeLicense.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly TradeLicenseDbContext _context = new TradeLicenseDbContext();
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private CesarDbContext core = new CesarDbContext();

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new TradeLicenseDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public AccountController(TradeLicenseDbContext context)
        {
            _context = context;
            UserManager =
                new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(_context));
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    //Checks if account is deactivated
                    if (user.User.IsLocked == true && user.User.IsDeleted == true)
                    {

                        ViewBag.Error = "Your Account Is Deactivated, Please Contact The System Administrator To Re-Activate Your Account.";
                        AuthenticationManager.SignOut();
                        return View(model);
                    }
                    //LF.20150316 - Force to change password on the first login.
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        await SignInAsync(user, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }                   
                    else if (user.User.IsPasswordReset == false)
                    {
                        await SignInAsync(user, model.RememberMe);
                        return RedirectToLocal("~/Account/Manage");
                    }
                    else
                    {

                        if (UserManager.IsInRole(user.Id, "System Administrators"))
                        {
                            await SignInAsync(user, model.RememberMe);
                            return RedirectToAction("Index", "Home");
                        }
                        if (UserManager.IsInRole(user.Id, "System Admin"))
                        {
                            await SignInAsync(user, model.RememberMe);
                            return RedirectToAction("Index", "Home");
                        }
                        if (UserManager.IsInRole(user.Id, "Department Inspector"))
                        {
                            await SignInAsync(user, model.RememberMe);
                            return RedirectToAction("Index", "InspectionResponse");
                        }
                        if (UserManager.IsInRole(user.Id, "Department Clerk"))
                        {
                            await SignInAsync(user, model.RememberMe);
                            return RedirectToAction("Index", "InspectionResponse");
                        }
                        if (UserManager.IsInRole(user.Id, "Department Manager"))
                        {
                            await SignInAsync(user, model.RememberMe);
                            return RedirectToAction("DepartmentManagerIndex", "ManagerReviews");
                        }
                        if (UserManager.IsInRole(user.Id, "Licensing Administrator"))
                        {
                            await SignInAsync(user, model.RememberMe);
                            return RedirectToAction("Index", "AdministratorReviews");
                        }
                        if (UserManager.IsInRole(user.Id, "Chief Inspector"))
                        {
                            await SignInAsync(user, model.RememberMe);
                            return RedirectToAction("Index", "ChiefReviews");
                        }
                        if (UserManager.IsInRole(user.Id, "Licensing Manager"))
                        {
                            await SignInAsync(user, model.RememberMe);
                            return RedirectToAction("Index", "ManagerReviews");
                        }
                        else
                        {
                            await SignInAsync(user, model.RememberMe);
                            return RedirectToAction("Index", "Home");
                            //return RedirectToAction("Dashboard", "Profile");
                        }
                      
                    }
                
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    model.Password = "";
                    model.UserName = "";
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // JK.20140724a - Passing values from the ViewModel to the Model.
                    var user = new ApplicationUser() { UserName = model.UserName, Email = model.EmailAddress };

                    // JK.20140724a - Custom profile information.
                    user.User = new User()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Username = model.UserName,                       
                        EmailAddress = model.EmailAddress,
                        LandLine = model.LandLine,
                        IsActive = true,
                        IsDeleted = false,
                        IsLocked = false,
                        ModifiedDateTime = DateTime.Now
                    };

                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                // JK.20140724a - Used to iterate through entity framework valiations.
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.Print("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.Print("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                throw e;
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {

            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            var identityManager = new IdentityManager();

            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var user = identityManager.CurrentUser(User.Identity.GetUserId());
                        //var user = db.Users.Find(aspuser.)
                        user.ModifiedByUserId = user.UserId;
                        user.ModifiedDateTime = DateTime.Today;
                        user.IsPasswordReset = true;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            TempData["Error"] = null;
            return View();
        }

        //<summary>
        //TG20150408 - Reset User Password
        // </summary>
        // <param name="model"></param>
        // <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                //TG20150408a.
                //This below code updates the Asp.NetUser tables with the users new password by getting the userid based on the users email address.
                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                var cUser = await UserManager.FindByEmailAsync(model.Email);

                if (cUser != null)
                {
                    String userId = User.Identity.GetUserId();//"<YourLogicAssignsRequestedUserId>";
                    String newPassword = GeneratePassword(); //"<PasswordAsTypedByUser>";
                    String hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);
                    //ApplicationUser cUser = await store.FindByIdAsync(userId);
                    await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                    await store.UpdateAsync(cUser);

                    //TG20150409.
                    //Updates password in users table with the new password
                    User user = db.Users.Find(cUser.User.UserId);
                    user.IsPasswordReset = false;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();

                    //string code = await UserManager.GeneratePasswordResetTokenAsync(cUser.Id);
                    //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = cUser.Id, code = code }, protocol: Request.Url.Scheme);

                    #region Construct Email
                    var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                    var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                    var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
                    var body = String.Empty;
                    //LF2014a 
                    //User email helper to get well formatted email.
                    // var mail = new EmailHelper();
                    if (template != null)
                    {
                        body = template.EmailBody;
                    } else
                    {
                        body = "";// html for email template
                    }

                    //L.M.20150303a - Replace variables with actual email content
                    body = body.Replace("#NAME#", cUser.User.FullName);
                    body = body.Replace("#BODYTEXT#", "You're new password is \"" + newPassword + "\"");//model.Password
                    var email = new tb_EmailQueue
                    {
                        QueueDateTime = DateTime.Now,
                        ApplicationId = applicationId,
                        EmailAccountId = 5,//Hard coded
                        ToList = cUser.Email,
                        CcList = null,
                        BccList = null,
                        Subject = "TLS: Reset Password.",
                        Body = body,
                        IsHtml = true,
                        FailureCount = 0,
                        ReferenceId = cUser.UserId.ToString(),
                        HasAttachments = false
                    };
                    try
                    {
                        core.tb_EmailQueue.Add(email);
                        core.SaveChanges();

                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                
                    #endregion email

                    //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");


                    return RedirectToAction("ForgotPasswordConfirmation", "Account");


                }
                else
                {
                    TempData["Error"] = "TLS Email Error - Please enter correct email address or contact system administrator.";
                }
                // If we got this far, something failed, redisplay form

            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            ViewBag.Link = TempData["ViewBagLink"];
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Generate Default Password
        /// <summary>
        /// Generates the password for each user.
        /// </summary>
        /// <returns></returns>
        public string GeneratePassword()
        {
            int codeCount = 8;
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(36);
                if (temp != -1 && temp == t)
                {
                    return GeneratePassword();
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }
        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                
              
                    return RedirectToAction("Index", "Home");
               
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}