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
    public class MeghnaDepartmentController : Controller
    {
        // GET: MeghnaDepartment
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<MeghnaDepartment> meghnaDepartments = ApiUtility.GetAllMeghnaDepartments();
            ViewBag.MeghnaDepartments = meghnaDepartments;
            return View();
        }

        // POST: Create Meghna Department
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MeghnaDepartment meghnaDepartment)
        {
            try
            {
                var statusCode = CreateOrUpdateMeghnaDepartment(meghnaDepartment,"create");
                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Access Denied.";
                    return RedirectToAction("Login", "Home");
                }
                if (statusCode == HttpStatusCode.Created)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Meghna department created successfully.";
                }
                if (statusCode == HttpStatusCode.Conflict)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Meghna Department already exist.";
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
            return RedirectToAction("Index", "MeghnaDepartment");
        }

        // POST: Edit Meghna Department
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MeghnaDepartment meghnaDepartment)
        {
            try
            {
                var statusCode = CreateOrUpdateMeghnaDepartment(meghnaDepartment, "edit");
                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Access Denied.";
                    return RedirectToAction("Login", "Home");
                }
                if (statusCode == HttpStatusCode.OK)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Meghna department updated successfully.";
                }
                if (statusCode == HttpStatusCode.BadRequest)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Something went wrong.";
                }
                if (statusCode == HttpStatusCode.Conflict)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Meghna Department already exist.";                    
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "MeghnaDepartment");
        }

        // GET: Delete Meghna Department
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
                        var postTask = client.GetAsync("MeghnaDepartment/Delete?departmentId="+id +"&userId="+userId);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Meghna department deleted successfully.";
                            return RedirectToAction("Index", "MeghnaDepartment");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "MeghnaDepartment");
        }

        private HttpStatusCode CreateOrUpdateMeghnaDepartment(MeghnaDepartment meghnaDepartment, string mode)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null || Session["UserId"] == null)
            {
                return HttpStatusCode.Unauthorized;
            }
            if (meghnaDepartment != null)
            {
                long userId = Convert.ToInt64(Session["UserId"]);
                if (mode == "edit")
                {
                    meghnaDepartment.ModifiedBy = userId;
                }
                else
                {
                    meghnaDepartment.CreatedBy = userId;
                }

                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync("MeghnaDepartment/"+mode, meghnaDepartment);
                    postTask.Wait();
                    var result = postTask.Result;
                    return result.StatusCode;
                }
            }

            return HttpStatusCode.BadRequest;
        }

        public JsonResult GetDepartmentDetailsById(long departmentId)
        {
            MeghnaDepartment aMeghnaDepartment = new MeghnaDepartment();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MeghnaDepartment/GetById?id=" + departmentId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<MeghnaDepartment>();
                    readTask.Wait();
                    aMeghnaDepartment = readTask.Result;
                    return Json(new { Status = "Ok", MeghnaDepartment = aMeghnaDepartment });
                }
            }

            return Json(new { Status = "Failed" });
        }
    }
}