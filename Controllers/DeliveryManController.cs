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
    public class DeliveryManController : Controller
    {
        // GET: DeliveryMan
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<DeliveryMan> deliveryMen = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            long userTypeId = Convert.ToInt64(Session["UserTypeId"]);

            using (var client = new HttpClientDemo())
            {
                var queryString = "DeliveryMan/GetAllWithMasterDepots?id=";
                if (userTypeId == (long)UserTypeEnum.MasterDepotUser)
                {
                    queryString = "DeliveryMan/GetAllWithMasterDepots?id=" + userId;
                }
                var responseTask = client.GetAsync(queryString);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<DeliveryMan>>();
                    readTask.Wait();
                    deliveryMen = readTask.Result;
                }
                else
                {
                    deliveryMen = new List<DeliveryMan>();
                }
            }
            ViewBag.DeliveryMen = deliveryMen;
            ViewBag.MasterDepots = Dropdown.MasterDepo();
            ViewBag.Thanas = new List<Thana>();
            ViewBag.Districts = Dropdown.Districts();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DeliveryMan deliveryMan)
        {
            LoginCredentials token = (LoginCredentials) Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            long userId = Convert.ToInt64(Session["UserId"]);
            long userTypeId = Convert.ToInt64(Session["UserTypeId"]);

            if (userTypeId == (long)UserTypeEnum.MasterDepotUser)
            {
                deliveryMan.CurrentUserId = userId;
            }
            try
            {
                if (deliveryMan != null)
                {
                    deliveryMan.CreatedBy = userId;
                    if (deliveryMan.ImageLocation != null)
                    {
                        MemoryStream target = new MemoryStream();
                        deliveryMan.ImageLocation.InputStream.CopyTo(target);
                        var data = target.ToArray();
                        deliveryMan.ImageByte = data;
                        deliveryMan.ImageLocation = null;
                    }
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync("DeliveryMan/Create", deliveryMan);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Delivery man created successfully.";
                            return RedirectToAction("Index", "DeliveryMan");
                        }
                        if (result.StatusCode == HttpStatusCode.BadRequest)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            dynamic response = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                            TempData["Message"] = response.message;
                            return RedirectToAction("Index", "DeliveryMan");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "DeliveryMan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DeliveryMan deliveryMan)
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

            if (userTypeId == (long)UserTypeEnum.MasterDepotUser)
            {
                deliveryMan.CurrentUserId = userId;
            }
            try
            {
                if (deliveryMan != null)
                {
                    deliveryMan.ModifiedBy = userId;
                    deliveryMan.ModifiedOn = DateTime.UtcNow.AddHours(6);
                    deliveryMan.IsDeleted = false;
                    if (deliveryMan.ImageLocation != null)
                    {
                        MemoryStream target = new MemoryStream();
                        deliveryMan.ImageLocation.InputStream.CopyTo(target);
                        var data = target.ToArray();
                        deliveryMan.ImageByte = data;
                        deliveryMan.ImageLocation = null;
                    }
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync("DeliveryMan/Edit", deliveryMan);
                        postTask.Wait();
                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Delivery man updated successfully.";
                            return RedirectToAction("Index", "DeliveryMan");
                        }
                        if (result.StatusCode == HttpStatusCode.BadRequest)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            dynamic response = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                            TempData["Message"] = response.message;
                            return RedirectToAction("Index", "DeliveryMan");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index", "DeliveryMan");
        }
        public JsonResult GetDeliveryManDetailsById(long deliveryManId)
        {
            DeliveryMan deliveryMan = new DeliveryMan();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("DeliveryMan/GetById?id=" + deliveryManId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<DeliveryMan>();
                    readTask.Wait();
                    deliveryMan = readTask.Result;
                    deliveryMan.MasterDepotIds = deliveryMan.MasterDepotDeliveryMen.Select(m => (long)m.MasterDepotId).ToArray();
                    return Json(new { Status = "Ok", DeliveryMan = deliveryMan });
                }
            }

            return Json(new { Status = "Failed" });
        }
        
        public ActionResult SearchByMasterDepot(long[] masterDepotIds)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.MasterDepots = Dropdown.MasterDepo();

            IEnumerable<DeliveryMan> deliveryMen = null;

            if (masterDepotIds == null)
            {
                return View(deliveryMen);
            }

            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);

                var queryString = "DeliveryMan/GetAllDeliveryManByMasterDepots?";

                queryString = queryString +
                              UtilityClass.GenerateQueryStringFromArray(masterDepotIds, "masterDepotIds");

                var responseTask = client.GetAsync(queryString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<DeliveryMan>>();
                    readTask.Wait();
                    deliveryMen = readTask.Result;
                }
                else
                {
                    deliveryMen = new List<DeliveryMan>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(deliveryMen);
        }
    }
}