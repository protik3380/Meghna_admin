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
    public class CorporateDepartmentController : Controller
    {
        // GET: CorporateDepartment
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<CorporateDepartment> corporateDepartments = ApiUtility.GetAllCorporateDepartments();
            ViewBag.CorporateDepartments = corporateDepartments;
            return View();
        }

        // POST: Create Corporate Department
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CorporateDepartment corporateDepartment)
        {
            try
            {
                var statusCode = CreateOrUpdateCorporateDepartment(corporateDepartment, "create");
                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Access Denied.";
                    return RedirectToAction("Login", "Home");
                }
                if (statusCode == HttpStatusCode.Created)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Corporate department created successfully.";
                }
                if (statusCode == HttpStatusCode.Conflict)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Corporate department already exist.";
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
            return RedirectToAction("Index", "CorporateDepartment");
        }

        // POST: Edit Corporate Department
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CorporateDepartment corporateDepartment)
        {
            try
            {
                var statusCode = CreateOrUpdateCorporateDepartment(corporateDepartment, "edit");
                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Access Denied.";
                    return RedirectToAction("Login", "Home");
                }
                if (statusCode == HttpStatusCode.OK)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Corporate department updated successfully.";
                }
                if (statusCode == HttpStatusCode.Conflict)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Corporate department already exist.";
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
            return RedirectToAction("Index", "CorporateDepartment");
        }

        // GET: Delete Corporate Department
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
                        var postTask = client.GetAsync("CorporateDepartment/Delete?departmentId=" + id + "&userId=" + userId);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Corporate department deleted successfully.";
                            return RedirectToAction("Index", "CorporateDepartment");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "CorporateDepartment");
        }

        private HttpStatusCode CreateOrUpdateCorporateDepartment(CorporateDepartment corporateDepartment, string mode)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null || Session["UserId"] == null)
            {
                return HttpStatusCode.Unauthorized;
            }
            if (corporateDepartment != null)
            {
                long userId = Convert.ToInt64(Session["UserId"]);
                if (mode == "edit")
                {
                    corporateDepartment.ModifiedBy = userId;
                }
                else
                {
                    corporateDepartment.CreatedBy = userId;
                }

                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync("CorporateDepartment/" + mode, corporateDepartment);
                    postTask.Wait();
                    var result = postTask.Result;
                    return result.StatusCode;
                }
            }

            return HttpStatusCode.BadRequest;
        }

        public JsonResult GetDepartmentDetailsById(long departmentId)
        {
            CorporateDepartment aCorporateDepartment = new CorporateDepartment();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("CorporateDepartment/GetById?id=" + departmentId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<CorporateDepartment>();
                    readTask.Wait();
                   aCorporateDepartment = readTask.Result;
                   return Json(new { Status = "Ok", CorporateDepartment = aCorporateDepartment });
                }
            }

            return Json(new {Status = "Failed"});
        }
    }
}