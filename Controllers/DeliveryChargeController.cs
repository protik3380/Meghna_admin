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
using Newtonsoft.Json.Linq;

namespace EFreshAdmin.Controllers
{
    public class DeliveryChargeController : Controller
    {
        // GET: DeliveryCharge
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            DeliveryCharge deliveryCharge = ApiUtility.GetDeliveryCharge(token);

            if (deliveryCharge != null)
            {
                return RedirectToAction("Edit");
            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DeliveryCharge deliveryCharge)
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
                if (deliveryCharge != null)
                {
                    deliveryCharge.CreatedBy = userId;
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync<DeliveryCharge>("DeliveryCharge/Create", deliveryCharge);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Delivery charge created successfully.";
                            return RedirectToAction("Edit");
                        }
                        if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Delivery charge already exist.";
                            return RedirectToAction("Edit");
                        }

                        dynamic response = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = response.message.ToString();
                        return View();
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }
            return View();
        }

        public ActionResult Edit()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            DeliveryCharge discount = ApiUtility.GetDeliveryCharge(token);
            if (discount == null)
            {
                return RedirectToAction("Create");
            }

            return View(discount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DeliveryCharge deliveryCharge)
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
                deliveryCharge.ModifiedBy = userId;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<DeliveryCharge>("DeliveryCharge/Edit", deliveryCharge);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Delivery charge updated successfully.";
                    }
                    else if (result.StatusCode == HttpStatusCode.NotFound)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "Delivery charge wasn't found";
                    }
                    else
                    {
                        dynamic response = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = response.message.ToString();
                    }
                }

                return RedirectToAction("Edit");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }
            return View();
        }

    }
}