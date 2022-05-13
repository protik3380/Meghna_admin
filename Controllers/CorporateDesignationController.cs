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
    public class CorporateDesignationController : Controller
    {
        // GET: CorporateDesignation
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<CorporateDesignation> corporateDesignations = ApiUtility.GetAllCorporateDesignations();
            ViewBag.CorporateDesignations = corporateDesignations;
            return View();
        }

        // POST: Create CorporateDesignation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CorporateDesignation corporateDesignation)
        {
            try
            {
                var statusCode = CreateOrUpdateCorporateDesignation(corporateDesignation, "create");
                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Access Denied.";
                    return RedirectToAction("Login", "Home");
                }
                if (statusCode == HttpStatusCode.Created)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Corporate designation created successfully.";
                }
                if (statusCode == HttpStatusCode.Conflict)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Corporate designation already exist.";
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
            return RedirectToAction("Index", "CorporateDesignation");
        }

        // POST: Edit CorporateDesignation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CorporateDesignation corporateDesignation)
        {
            try
            {
                var statusCode = CreateOrUpdateCorporateDesignation(corporateDesignation, "edit");
                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Access Denied.";
                    return RedirectToAction("Login", "Home");
                }
                if (statusCode == HttpStatusCode.OK)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Corporate designation updated successfully.";
                }
                if (statusCode == HttpStatusCode.Conflict)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Corporate designation already exist.";
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
            return RedirectToAction("Index", "CorporateDesignation");
        }

        // GET: Delete CorporateDesignation
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
                        var postTask = client.GetAsync("CorporateDesignation/Delete?designationId=" + id + "&userId=" + userId);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Corporate designation deleted successfully.";
                            return RedirectToAction("Index", "CorporateDesignation");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "CorporateDesignation");
        }

        private HttpStatusCode CreateOrUpdateCorporateDesignation(CorporateDesignation corporateDesignation, string mode)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null || Session["UserId"] == null)
            {
                return HttpStatusCode.Unauthorized;
            }
            if (corporateDesignation != null)
            {
                long userId = Convert.ToInt64(Session["UserId"]);
                if (mode == "edit")
                {
                    corporateDesignation.ModifiedBy = userId;
                }
                else
                {
                    corporateDesignation.CreatedBy = userId;
                }
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync("CorporateDesignation/" + mode, corporateDesignation);
                    postTask.Wait();
                    var result = postTask.Result;
                    return result.StatusCode;
                }
            }

            return HttpStatusCode.BadRequest;
        }

        public JsonResult GetDesignationDetailsById(long designationId)
        {
            CorporateDesignation aCorporateDesignation = new CorporateDesignation();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("CorporateDesignation/GetById?id=" + designationId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<CorporateDesignation>();
                    readTask.Wait();
                    aCorporateDesignation = readTask.Result;
                    return Json(new { Status = "Ok", CorporateDesignation = aCorporateDesignation });
                }
            }

            return Json(new { Status = "Failed" });
        }
    }
}