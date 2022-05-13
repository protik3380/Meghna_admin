using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using EFreshAdmin.Models;
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using EFreshStore.Utility;

namespace EFreshAdmin.Controllers
{
    public class BrandController : Controller
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
        public ActionResult Create(Brand aBrand)
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
                if (aBrand != null)
                {
                    aBrand.CreatedBy = userId;
                    aBrand.CreatedOn = DateTime.UtcNow.AddHours(6);
                    if (aBrand.ImageLocation != null)
                    {
                        MemoryStream target = new MemoryStream();
                        aBrand.ImageLocation.InputStream.CopyTo(target);
                        var data = target.ToArray();
                        aBrand.ImageByte = data;
                        aBrand.ImageLocation = null;
                    }
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync("Brand/Create", aBrand);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Brand created successfully.";
                            var data= result.Content.ReadAsAsync<Brand>().Result;
                            return RedirectToAction("Index", "Brand");
                        }
                        if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "This brand is already exist.";
                            return RedirectToAction("Index", "Brand");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "Brand");
        }

        [HttpGet]
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<Brand> brands = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Brand/GetAll");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Brand>>();
                    readTask.Wait();
                    brands = readTask.Result;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.Brands = brands;
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
            Brand brand = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Brand/GetById/" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Brand>();
                    readTask.Wait();
                    brand = readTask.Result;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(brand);
        }

        [HttpPost]
        public ActionResult Edit(Brand aBrand)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            long userId = Convert.ToInt64(Session["UserId"]);
            if (aBrand != null)
            {
                aBrand.ModifiedBy = userId;
                aBrand.ModifiedOn = DateTime.UtcNow.AddHours(6);
                aBrand.IsDeleted = false;
                if (aBrand.ImageLocation != null)
                {
                    MemoryStream target = new MemoryStream();
                    aBrand.ImageLocation.InputStream.CopyTo(target);
                    var data = target.ToArray();
                    aBrand.ImageByte = data;
                    aBrand.ImageLocation = null;
                }
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var putTask = client.PostAsJsonAsync<Brand>("Brand/Edit", aBrand);
                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Brand updated successfully.";
                        return RedirectToAction("Index", "Brand");
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "This brand already exist.";
                        return RedirectToAction("Index", "Brand");
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return RedirectToAction("Index", "Brand");
        }

        public JsonResult GetBrandDetailsById(long brandId)
        {
            Brand brand = new Brand();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Brand/GetById?id=" + brandId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Brand>();
                    readTask.Wait();
                    brand = readTask.Result;
                    return Json(new { Status = "Ok", Brand = brand });
                }
            }

            return Json(new { Status = "Failed" });
        }
    }
}
