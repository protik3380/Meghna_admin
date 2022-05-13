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
    public class CouponController : Controller
    {
        // GET: Coupon
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<Coupon> coupons = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Coupon/GetAll");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Coupon>>();
                    readTask.Wait();
                    coupons = readTask.Result;
                }
                else
                {
                    coupons = new List<Coupon>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.Coupons = coupons;
            ViewBag.UserTypes = Dropdown.UserTypes();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Coupon aCoupon)
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
                if (aCoupon != null)
                {
                    aCoupon.CreatedBy = userId;
                    aCoupon.CreatedOn = DateTime.UtcNow.AddHours(6);
                    aCoupon.IsDeleted = false;
                    aCoupon.IsActive = true;
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync<Coupon>("Coupon/Create", aCoupon);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Coupon created successfully.";
                            return RedirectToAction("Index", "Coupon");
                        }
                        if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "This coupon already exists,";
                            return RedirectToAction("Index", "Coupon");
                        }
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }
            return RedirectToAction("Index", "Coupon");
        }

        [HttpPost]
        public ActionResult Edit(Coupon aCoupon)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            long userId = Convert.ToInt64(Session["UserId"]);
            if (aCoupon != null)
            {
                aCoupon.ModifiedBy = userId;
                aCoupon.ModifiedOn = DateTime.UtcNow.AddHours(6);
                aCoupon.IsDeleted = false;
                aCoupon.IsActive = true;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var putTask = client.PostAsJsonAsync<Coupon>("Coupon/Edit", aCoupon);

                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Coupon updated successfully.";
                        return RedirectToAction("Index", "Coupon");
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "This coupon already exist.";
                        return RedirectToAction("Index", "Coupon");
                    }
                    ModelState.AddModelError(string.Empty, result.Content.ReadAsStringAsync().Result);
                }
            }
            return RedirectToAction("Index", "Coupon");
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
                        var postTask = client.GetAsync("Coupon/Delete?couponId=" + id + "&userId=" + userId);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Coupon deleted successfully.";
                            return RedirectToAction("Index", "Coupon");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "Coupon");
        }

        public JsonResult GetCouponDetailsById(long couponId)
        {
            Coupon coupon = new Coupon();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Coupon/GetById?id=" + couponId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Coupon>();
                    readTask.Wait();
                    coupon = readTask.Result;
                    return Json(new { Status = "Ok", Coupon = coupon });
                }
            }

            return Json(new { Status = "Failed" });
        }
    }
}