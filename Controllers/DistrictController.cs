using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using EFreshAdmin.Models;
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using EFreshStore.Utility;

namespace EFreshAdmin.Controllers
{
    public class DistrictController : Controller
    {
        public ActionResult Create()
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(District aDistrict)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            long userId = Convert.ToInt64(Session["UserId"]);
            try
            {
                if (aDistrict != null)
                {
                    aDistrict.CreatedBy = userId;
                    aDistrict.CreatedOn = DateTime.Now;
                    aDistrict.IsDeleted = false;
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync<District>("District/Create", aDistrict);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "District added successfully.";
                            return RedirectToAction("Views", "District");
                        }
                        if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "District already exist.";
                            return RedirectToAction("Views", "District");
                        }
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "Something went wrong.";
                        return RedirectToAction("Views", "District");
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }
            return View(aDistrict);
        }

        [HttpGet]
        public ActionResult Views()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<District> districts = Dropdown.Districts();

            ViewBag.Districts = districts;
            return View();
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            District district = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("District/GetById/" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<District>();
                    readTask.Wait();
                    district = readTask.Result;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(district);
        }

        [HttpPost]
        public ActionResult Edit(District aDistrict)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            long userId = Convert.ToInt64(Session["UserId"]);
            if (aDistrict != null)
            {
                aDistrict.ModifiedBy = userId;
                aDistrict.ModifiedOn = DateTime.Now;
                aDistrict.IsDeleted = false;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var putTask = client.PostAsJsonAsync<District>("District/Edit", aDistrict);

                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "District updated successfully.";
                        return RedirectToAction("Views", "District");
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "District already exist.";
                        return RedirectToAction("Views", "District");
                    }
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Something went wrong.";
                    return RedirectToAction("Views", "District");
                }
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return RedirectToAction("Views", "District");
        }

        public JsonResult GetDistrictDetailsById(long districtId)
        {
            District district = new District();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("District/GetById?id=" + districtId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<District>();
                    readTask.Wait();
                    district = readTask.Result;
                    return Json(new { Status = "Ok", District = district });
                }
            }

            return Json(new { Status = "Failed" });
        }
    }
}