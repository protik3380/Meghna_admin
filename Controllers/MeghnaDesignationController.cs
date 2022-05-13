using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using EFreshAdmin.Models;
using EFreshAdmin.Models.EntityModels;
using EFreshAdmin.Utility;
using EFreshStore.Utility;

namespace EFreshAdmin.Controllers
{
    public class MeghnaDesignationController : Controller
    {
        // GET: MeghnaDesignation
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<MeghnaDesignation> meghnaDesignations = ApiUtility.GetAllMeghnaDesignations();
            ViewBag.MeghnaDesignations = meghnaDesignations;
            return View();
        }

        // POST: Create MeghnaDesignation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MeghnaDesignation meghnaDesignation)
        {
            try
            {
                var statusCode = CreateOrUpdateMeghnaDesignation(meghnaDesignation, "create");
                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Access Denied.";
                    return RedirectToAction("Login", "Home");
                }
                if (statusCode == HttpStatusCode.Created)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Meghna designation created successfully.";
                }
                if (statusCode == HttpStatusCode.Conflict)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Meghna designation already exist.";
                }
                if (statusCode == HttpStatusCode.BadRequest)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Something went wrong.";
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "MeghnaDesignation");
        }

        // POST: Edit MeghnaDesignation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MeghnaDesignation meghnaDesignation)
        {
            try
            {
                var statusCode = CreateOrUpdateMeghnaDesignation(meghnaDesignation, "edit");
                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Access Denied.";
                    return RedirectToAction("Login", "Home");
                }
                if (statusCode == HttpStatusCode.OK)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Meghna designation updated successfully.";
                }
                if (statusCode == HttpStatusCode.Conflict)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Meghna designation already exist.";
                }
                if (statusCode == HttpStatusCode.BadRequest)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Something went wrong.";
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "MeghnaDesignation");
        }

        // GET: Delete MeghnaDesignation
        public ActionResult Delete(long? id)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                if (id != null)
                {
                    long userId = Convert.ToInt64(Session["UserId"]);
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.GetAsync("MeghnaDesignation/Delete?designationId=" + id + "&userId=" + userId);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Meghna designation deleted successfully.";
                            return RedirectToAction("Index", "MeghnaDesignation");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "MeghnaDesignation");
        }

        private HttpStatusCode CreateOrUpdateMeghnaDesignation(MeghnaDesignation meghnaDesignation, string mode)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null || Session["UserId"] == null)
            {
                return HttpStatusCode.Unauthorized;
            }
            if (meghnaDesignation != null)
            {
                long userId = Convert.ToInt64(Session["UserId"]);
                if (mode == "edit")
                {
                    meghnaDesignation.ModifiedBy = userId;
                }
                else
                {
                    meghnaDesignation.CreatedBy = userId;
                }
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync("MeghnaDesignation/" + mode, meghnaDesignation);
                    postTask.Wait();
                    var result = postTask.Result;
                    return result.StatusCode;
                }
            }

            return HttpStatusCode.BadRequest;
        }

        public JsonResult GetDesignationDetailsById(long designationId)
        {
            MeghnaDesignation aMeghnaDesignation = new MeghnaDesignation();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MeghnaDesignation/GetById?id=" + designationId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<MeghnaDesignation>();
                    readTask.Wait();
                    aMeghnaDesignation = readTask.Result;
                    return Json(new { Status = "Ok", MeghnaDesignation = aMeghnaDesignation });
                }
            }

            return Json(new { Status = "Failed" });
        }
    }
}