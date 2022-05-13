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
using EFreshStore.Models.Context;
using EFreshStore.Utility;
using Newtonsoft.Json.Linq;

namespace EFreshAdmin.Controllers
{
    public class LPGComboDiscountController : Controller
    {
        // GET: LPGComboDiscount
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

            LPGComboDiscount discount = ApiUtility.GetLPGComboDiscount(token);

            if (discount != null)
            {
                return RedirectToAction("Edit");
            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LPGComboDiscount lpgComboDiscount)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            if (lpgComboDiscount.ActiveDate > lpgComboDiscount.Validity)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Start to date must be greater than end date";
                return View(lpgComboDiscount);
            }
            long userId = Convert.ToInt64(Session["UserId"]);
            try
            {
                if (lpgComboDiscount != null)
                {
                    lpgComboDiscount.CreatedBy = userId;
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync<LPGComboDiscount>("LPGComboDiscount/Create", lpgComboDiscount);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "LPG Combo discount created successfully.";
                            return RedirectToAction("Edit");
                        }
                        if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "LPG Combo discount already exist.";
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

            LPGComboDiscount discount = ApiUtility.GetLPGComboDiscount(token);
            if (discount == null)
            {
                return RedirectToAction("Create");
            }

            return View(discount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LPGComboDiscount lpgComboDiscount)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            if (lpgComboDiscount.ActiveDate > lpgComboDiscount.Validity)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Start to date must be greater than end date";
                return View(lpgComboDiscount);
            }
            long userId = Convert.ToInt64(Session["UserId"]);
            try
            {
                lpgComboDiscount.ModifiedBy = userId;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<LPGComboDiscount>("LPGComboDiscount/Edit", lpgComboDiscount);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "LPG Combo discount updated successfully.";
                    }
                    else if (result.StatusCode == HttpStatusCode.NotFound)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "LPG Combo discount wasn't found";
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