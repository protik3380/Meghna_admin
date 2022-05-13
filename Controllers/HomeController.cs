using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using EFreshAdmin.Models;
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using EFreshStore.Utility;

namespace EFreshAdmin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //login, logout, forget, reset password
        public ActionResult Login()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(User aUser)
        {
            //string returnUrl = (string)Session["requesturl"];
            User user = ApiUtility.ValidateUser(aUser.Username, aUser.Password);
            if (user != null)
            {
                Session["email"] = aUser.Username;
                Session["UserId"] = user.Id;
                Session["UserTypeId"] = user.UserTypeId;
                Session["requesturl"] = null;
                if (user.UserTypeId == (long)UserTypeEnum.Admin)
                {
                    LoginCredentials token = ApiUtility.ValidateUserToken(aUser.Username, aUser.Password);
                    Session["userToken"] = token;
                    return RedirectToAction("Index");
                }
                if (user.UserTypeId == (long)UserTypeEnum.MasterDepotUser)
                {
                    LoginCredentials token = ApiUtility.ValidateUserToken(aUser.Username, aUser.Password);
                    Session["userToken"] = token;
                    return RedirectToAction("Index", "Home");
                }
                //else
                //{
                //    user = null;
                //    Session["requesturl"] = null;
                //    if (string.IsNullOrEmpty(returnUrl))
                //    {
                //        FlashMessage.Queue($"Access Denied.", "Warning: ", FlashMessageType.Warning, false);
                //        returnUrl = "~/";
                //    }
                //    FlashMessage.Queue($"Access Denied.", "Warning: ", FlashMessageType.Warning, false);
                //    return RedirectToAction("Login","Home");
                //}
            }

            Session["email"] = null;
            Session["UserId"] = null;
            Session["UserTypeId"] = null;
            TempData["Class"] = UtilityClass.Error;
            TempData["Message"] = "Username or password doesn't match. Please try again.";
            return RedirectToAction("Login","Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            //Roles.DeleteCookie();
            Session["email"] = null;
            Session["UserId"] = null;
            Session["UserTypeId"] = null;
            Session["userToken"] = null;
            Session.Clear();
            Session.Abandon(); // it will clear the session at the end of request

            //if (Request.UrlReferrer != null)
            //{
            //    string redirectUrl = Request.UrlReferrer.ToString();
            //    return Redirect(redirectUrl);
            //}
            return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            try
            {
                User aUser = ApiUtility.GetByUserEmail(email);

                if (aUser != null)
                {
                    long userType = (long) aUser.UserTypeId;
                    if (userType == (long) UserTypeEnum.Admin || userType == (long) UserTypeEnum.MasterDepotUser)
                    {
                        Session["ID"] = aUser.Username;
                        string subject = "[Meghna e-Commerce] Reset Password";
                        string body = "Dear user" + Environment.NewLine;
                        body += Environment.NewLine;
                        body += "Click the link below to change your password" +
                                Environment.NewLine;
                        body += BaseUrl.homeUrl + "Home/ResetPassword/?email=" + aUser.Username + Environment.NewLine;

                        MailAddress mailAddress = new MailAddress(aUser.Username, aUser.Username);
                        if (!string.IsNullOrWhiteSpace(aUser.Username))
                        {
                            Email.SendEmail(subject, body, mailAddress);
                        }
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "A mail has been sent to this email address to reset password.";
                    }
                    else
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Sorry, this email address is not a valid admin/master depot user email.";
                    }
                }
                else
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Sorry, this email address dosen't exist in the record.";
                }
                return View();
            }
            catch (Exception)
            {
                TempData["Class"] = UtilityClass.Success;
                TempData["Message"] = "Sorry! Email can't be sent to this address.";
            }
            return View();
        }

        public ActionResult ResetPassword(string email)
        {
            User aUser = new User {Username = email};
            return View(aUser);
        }

        [HttpPost]
        public ActionResult ResetPassword(User user)
        {
            try
            {
                if (user != null)
                {
                    //user.Password = password;
                    using (var client = new HttpClientDemo())
                    {
                        var postTask = client.PostAsJsonAsync<User>("User/ChangePassword", user);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Password changed successfully.";
                            return RedirectToAction("Logout", "Home");
                        }
                        else
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Sorry! Password change failed.";
                            return View();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
            }
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string password)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Login to change password";
                    return RedirectToAction("Login", "Home");
                }
                long userId = Convert.ToInt64(Session["UserId"]);
                //string userName = Session["email"].ToString();
                User user = new User();
                user.Id = userId;
                user.Password = password;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl.url + "User/ChangePassword");
                    var postTask = client.PostAsJsonAsync<User>("ChangePassword", user);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Password changed successfully.";

                        Session["email"] = null;
                        Session["UserId"] = null;
                        Session["UserTypeId"] = null;
                        Session.Clear();
                        Session.Abandon(); // it will clear the session at the end of request
                        Response.Cookies["username"].Expires = System.DateTime.Now.AddDays(-1);
                        Response.Cookies["userid"].Expires = System.DateTime.Now.AddDays(-1);
                        Request.Cookies.Clear();

                        return RedirectToAction("Login", "Home");
                    }
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Sorry! Password change failed.";
                    return RedirectToAction("ResetPassword", "Home");

                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
            }
            return RedirectToAction("ResetPassword", "Home");

        }
    }
}