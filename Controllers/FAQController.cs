using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using EFreshAdmin.Models;
using EFreshAdmin.Models.EntityModels;
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using EFreshStore.Utility;
using Newtonsoft.Json.Linq;

namespace EFreshAdmin.Controllers
{
    public class FAQController : Controller
    {
        // GET: FAQ
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<FAQ> faqs = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            long userTypeId = Convert.ToInt64(Session["UserTypeId"]);

            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("FAQ/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<FAQ>>();
                    readTask.Wait();
                    faqs = readTask.Result;
                }
                else
                {
                    faqs = new List<FAQ>();
                }
            }
            ViewBag.FAQS = faqs;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FAQ faq)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            long userId = Convert.ToInt64(Session["UserId"]);
            long userTypeId = Convert.ToInt64(Session["UserTypeId"]);

            
            try
            {
                if (faq != null)
                {
                    faq.CreatedBy = userId;
                    faq.CreatedOn = DateTime.Now;
                    faq.IsDeleted = false;
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync("FAQ/Create", faq);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "FAQ added successfully.";
                            return RedirectToAction("Index", "FAQ");
                        }
                        if (result.StatusCode == HttpStatusCode.BadRequest)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            dynamic response = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                            TempData["Message"] = response.message;
                            return RedirectToAction("Index", "FAQ");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "FAQ");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FAQ anFaq)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            long userId = Convert.ToInt64(Session["UserId"]);
            long userTypeId = Convert.ToInt64(Session["UserTypeId"]);
            try
            {
                if (anFaq != null)
                {
                    anFaq.ModifiedBy = userId;
                    anFaq.ModifiedOn = DateTime.UtcNow.AddHours(6);
                    anFaq.IsDeleted = false;
                   
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync("FAQ/Edit", anFaq);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "FAQ updated successfully.";
                            return RedirectToAction("Index", "FAQ");
                        }
                        if (result.StatusCode == HttpStatusCode.BadRequest)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            dynamic response = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                            TempData["Message"] = response.message;
                            return RedirectToAction("Index", "FAQ");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "FAQ");
        }

        public JsonResult GetFAQDetailsById(long faqId)
        {
            FAQ faq = new FAQ();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("FAQ/GetById?id=" + faqId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<FAQ>();
                    readTask.Wait();
                    faq = readTask.Result;
                    return Json(new { Status = "Ok", FAQ = faq });
                }
            }

            return Json(new { Status = "Failed" });
        }
    }
}