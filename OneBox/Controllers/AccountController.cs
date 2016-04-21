using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OneBox_BusinessLogic.AzureStorage;
using OneBox_DataAccess.Domain;
using OneBox_DataAccess.Infrastucture;
using OneBox_DataAccess.Utilities;
using OneBox_Infrastructure.DataTransferObjects;
using OneBox_WebServices.Infrastructure;
using OneBox_WebServices.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OneBox_WebServices.Controllers
{
    [Authorize]
    /// Display the info for the user after authentification.
    public class AccountController : Controller
    {
        private IAzureServices azureServices;
        
        public AccountController(IAzureServices azureServices)
        {
            this.azureServices = azureServices;
            
        }

        public PartialViewResult ListOfFiles(string filePath)
        {
            return PartialView(GetFiles(filePath));
        }

        /// <summary>
        /// Returns what the user can do after authentification.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string filePath="")
        {
            if (filePath == "")
            {
                filePath = azureServices.GetContainerName();
            }
            return View(GetFiles(filePath));
        }
    
        public ActionResult DownloadFile(string filePath)
        {

            Stream fileStream = azureServices.GetStream(filePath);            
            // Buffer to read 10K bytes in chunk:
            byte[] buffer = new Byte[10000];

            // Length of the file:
            int length;

            // Total bytes to read:
            long dataToRead;

            try
            {

                // Total bytes to read:
                dataToRead = fileStream.Length;
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Utility.GetFileName(filePath));
                // Read the bytes.
                while (dataToRead > 0)
                {
                    // Verify that the client is connected.
                    if (Response.IsClientConnected)
                    {
                        // Read the data in buffer.
                        length = fileStream.Read(buffer, 0, 10000);

                        // Write the data to the current output stream.
                        Response.OutputStream.Write(buffer, 0, length);

                        // Flush the data to the HTML output.
                        Response.Flush();

                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        //prevent infinite loop if user disconnects
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Trap the error, if any.
                Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    //Close the file.
                    fileStream.Close();
                }
                Response.Close();
            }

            //            HttpContext.Response.ContentType = new FileStreamResult(fileStream);
            
            string mime = MimeMapping.GetMimeMapping(filePath);
            return new FileStreamResult(fileStream, mime);
        }

        public ActionResult CreateNewFolder(string currentPath,  string newFolder)
        {
            if (newFolder == null || newFolder == "")
            {
                return Content("false");
            }

            azureServices.CreateNewFolder(currentPath, newFolder);

            return Content("true");

        }

        private FileSystemViewModel GetFiles(string filePath)
        {
            FileSystemViewModel fileSystem = new FileSystemViewModel();
            foreach(FileDto item in azureServices.GetFiles(filePath))
            {
                string fileType = "folder";
                if (item.ifFile)
                {
                    fileType = "file";
                }
                fileSystem.fileSystemList.Add(new FileViewModel(item.fullPath, item.name, item.sizeInBytes, fileType));
            }
            List<string> folders = OneBox_DataAccess.Utilities.Utility.Split(filePath, '/');
            for(int i=0; i<folders.Count; ++i)
            {
                fileSystem.pathList.Add(folders[i]);
            }

            return fileSystem;
        }

        public ActionResult AccountInfo()
        {
            return View(GetInfoData());
        }

        /// <summary>
        /// The login action. 
        /// If the user has already tried to log in and failed the error are going to be sent to the view.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>A view for inserting the credential</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                /// If already is loged in then redirect to prinpal action.
                return RedirectToAction("Index");
            }

            ///TODO: This should be changed. The return url
            ViewBag.Url = returnUrl;
            ///TODO:This should be changed.
            AddErrosFromResult((IdentityResult)TempData["Errors"]);
            return View();
        }
        /// <summary>
        /// Call the middleware login authentification service.
        /// </summary>
        /// <param name="returnUrl">The return path.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GoogleLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallback", new { returnUrl = returnUrl })
            };
            HttpContext.GetOwinContext().Authentication.Challenge(properties, "Google");
            ///TODO:De ce dau return asta ?
            return new HttpUnauthorizedResult();
        }

        /// <summary>
        /// The action that Google calls back with user credential
        /// TODO : ce se intampla daca nu poate sa se logheze cu google ? sau daca is creeaza cont.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>redirect to Index or Login(again)</returns>
        [AllowAnonymous]
        public async Task<ActionResult> GoogleLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await AuthManager.GetExternalLoginInfoAsync();

            ///Find the user. Maybe has already login with google before.
            AppUser user = await UserManager.FindAsync(loginInfo.Login);
            if (user == null)
            {
                /// If this the first time when the user has logged in with google on "onebox". 
                user = new AppUser
                {
                    Email = loginInfo.Email,
                    UserName = loginInfo.DefaultUserName,
                };

                ///Create new user.
                IdentityResult result = await UserManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    /// If the creation has not succeeded( for exemple another user exist with the same email).
                    /// Save the error.
                    TempData["Errors"] = result;
                    /// Redirect to Login action(and display the erros).
                    return RedirectToAction("Login");
                }
                else {
                    ///Try to add login to the created user.
                    result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    if (!result.Succeeded)
                    {
                        /// Save the errors and redirect to Login action.
                        TempData["Errors"] = result;
                        return RedirectToAction("Login");
                    }
                }
            }
            /// TODO: tre sa vad mai clar ce se intampla pe aici ( adica mai sus face addlogin async).
            ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            ident.AddClaims(loginInfo.ExternalIdentity.Claims);
            AuthManager.SignOut();
            AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);

            ///It is a simple user so add the user to "Users" role.
            if (!UserManager.IsInRole(user.Id, Utility.UsersRole))
            {
                UserManager.AddToRole(user.Id, Utility.UsersRole);
            }

            SettingsUserSuccesfullyLoggedIn(user.Email);
            /// Redirect to main page of the authentificated user.
            return RedirectToAction("Index");
        }

        private void SettingsUserSuccesfullyLoggedIn(string email)
        {
            // TODO : ceva erori pe aici ?
            //AppUser user = UserManager.FindById(User.Identity.GetUserId());
            azureServices.ConfigureServices(email);
            //defaultPath = "/" + azureServices.GetContainerName();
        }

        /// <summary>
        /// Obtain the current User.Identity from the current REQUEST !!!!.
        /// </summary>
        /// <returns></returns>
        private IIdentity GetIdentityUser()
        {
            return HttpContext.User.Identity;
        }

        /// <summary>
        /// Get some basic data of the authentifacted user.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetInfoData()
        {
            Dictionary<string, object> info = new Dictionary<string, object>();
            info.Add("User", GetIdentityUser().Name);
            info.Add("Authentificated", GetIdentityUser().IsAuthenticated);
            info.Add("Authnetification type", GetIdentityUser().AuthenticationType);
            info.Add("container name:", azureServices.GetContainerName());
            var id = GetIdentityUser().GetUserId();

            var user = UserManager.FindById(id);

            info.Add("email", user.Email);
            info.Add("Is in users role", HttpContext.User.IsInRole("Users"));
            return info;
        }

        /// <summary>
        /// Try to create a new user.
        /// </summary>
        /// <param name="userModel">The info of the user.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(UserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = userModel.Name,
                    Email = userModel.Email
                };
                IdentityResult result = await UserManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    user = UserManager.Find(userModel.Name, userModel.Password);
                    UserManager.AddToRole(user.Id, Utility.UsersRole);
                    UserManager.AddToRole(user.Id, Utility.LocalUsersRoleName);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrosFromResult(result);
                }
            }
            return View();
        }


        private void AddErrosFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        /// <summary>
        /// Check whether or not the given user can login within the application or not.
        /// </summary>
        /// <param name="details">the credential of the user.</param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //check if the input from the user is in valid state.
                AppUser user = await UserManager.FindAsync(details.Name, details.Password);
                if (user == null)
                {
                    // if the user doen not exist.
                    ModelState.AddModelError("", "Invalid name or password.");
                }
                else
                {
                    // user exist.
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
                    // user has signed in.
                    // redirect to previous page.
                    if (returnUrl == null || returnUrl == string.Empty)
                    {
                        //GetInfoData();
                        //returnView("Index", GetInfoData());
                        SettingsUserSuccesfullyLoggedIn(user.Email);
                        return RedirectToAction("Index");
                    }
                    else {
                        //GetInfoData();
                        SettingsUserSuccesfullyLoggedIn(user.Email);
                        return Redirect(returnUrl);
                    }
                }
            }
            return View(details);
        }

        /// <summary>
        /// Log out the current user.
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Change password. Just the users who have password can change their password.
        /// </summary>
        /// <returns></returns>
        [LocalUsersAuthorize(Utility.LocalUsersRoleName)]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Change password of the authentificated user.
        /// Return to home page if the action succeded.
        /// </summary>
        /// <param name="newPassViewModel">the model for new password.</param>
        /// <returns></returns>
        [HttpPost]
        [LocalUsersAuthorize(Utility.LocalUsersRoleName)]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel newPassViewModel){
            if(ModelState.IsValid)
            {
                string userId =  HttpContext.User.Identity.GetUserId();
                if (userId == null)
                {
                    // TODO: something went wrong; sign out and redirect to home page.
                }

                AppUser user = await UserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // TODO: something went wrong; sign out and redirect to home page.
                }

                ///Check if the given old password match the current password.
                AppUser passwordCheckerUser = await UserManager.FindAsync(user.UserName, newPassViewModel.OldPassword);
                if (passwordCheckerUser == null) {
                    ModelState.AddModelError("", "The current password is wrong!");
                    return View();    
                }

                /// Change the new password.
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(newPassViewModel.NewPassword);
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    /// If the updated succesded redirect to main page.
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong. Please try again.");
                    return View();
                }
            }

            return View();
        }

        /// <summary>
        /// Reset password action. Presents the user the page accesed through the link received in the email.
        /// I need two methods. Because there is the case : initial case when the user try to recover for the first time.
        /// Given that this is an http post with an object of ResetPasswordViewmodel; for the first time the given model will be empty so there will be errors but those errors are not made by the user.
        /// </summary>
        /// <param name="code">the corresponding code generated when a reset</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordPage(ResetPasswordViewModel model)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ResetPasswordLogic(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                /// TODO: check if the model has the email set. For exemple if the page was accessed from a different source then the one given into email.
                if (model.Email == null)
                {
                    /// do some retirect.
                }
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {

                    // Don't reveal that the user does not exist
                    ModelState.AddModelError("", "email is wrong");
                    return View();
                }

                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrosFromResult(result);
                    return View();
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "there is no account with the given email.");
                    return View();
                }
                else
                {
                    string code = UserManager.GeneratePasswordResetToken(user.Id);
                    //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    /// acum fac callback-ul 
                    /// ce cred ca se intampla : imi fac un obiect resetPasswordViewModel care ii facut de model binder cand se face apelul inapoi
                    var callbackUrl = Url.Action("ResetPasswordPage", "Account", new ResetPasswordViewModel() {Code = code, Email=user.Email }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    //return RedirectToAction("ForgotPasswordConfirmation", "Account");
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();  
        }
        
        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
    }

}