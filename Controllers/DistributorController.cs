using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using EFreshAdmin.Models;
using EFreshAdmin.Models.EntityModels;
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using EFreshStore.Utility;

namespace EFreshAdmin.Controllers
{
    public class DistributorController : Controller
    {
        // GET: Distributor
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
            IEnumerable<Distributor> distributors = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Distributor/GetAll");
                var responseTask = client.GetAsync("Distributor/GetAll");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Distributor>>();
                    readTask.Wait();
                    distributors = readTask.Result;
                }
                else
                {
                    distributors = new List<Distributor>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.Distributors = distributors;
            ViewBag.masterDepot = Dropdown.MasterDepo();
            ViewBag.thana = Dropdown.Thanas();
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
            ViewBag.masterDepot = Dropdown.MasterDepo();
            ViewBag.thana = Dropdown.Thanas();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Distributor aDistributor)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.masterDepot = Dropdown.MasterDepo();
            ViewBag.thana = Dropdown.Thanas();
            long userId = Convert.ToInt64(Session["UserId"]);
            if (aDistributor != null)
            {
                aDistributor.CreatedBy = userId;
                aDistributor.CreatedOn = DateTime.Now;
                aDistributor.IsDeleted = false;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<Distributor>("Distributor/Create", aDistributor);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Distributor created successfully.";
                        var data = result.Content.ReadAsAsync<Distributor>().Result;
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "This email already exist.";
                    }
                    return RedirectToAction("Index", "Distributor");

                }
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return RedirectToAction("Index", "Distributor");

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
            Distributor distributor = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Distributor/GetById?id=" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Distributor>();
                    readTask.Wait();
                    distributor = readTask.Result;
                }
                else
                {
                    distributor = new Distributor();
                }
            }

            ViewBag.masterDepot = Dropdown.MasterDepo();
            ViewBag.thana = Dropdown.Thanas();
            return View(distributor);
        }

        [HttpPost]
        public ActionResult Edit(Distributor aDistributor)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.masterDepot = Dropdown.MasterDepo();
            ViewBag.thana = Dropdown.Thanas();
            long userId = Convert.ToInt64(Session["UserId"]);
            if (aDistributor != null)
            {
                aDistributor.ModifiedBy = userId;
                aDistributor.ModifiedOn = DateTime.Now;
                aDistributor.IsDeleted = false;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var putTask = client.PostAsJsonAsync<Distributor>("Distributor/Edit", aDistributor);
                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Distributor updated successfully.";
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "This email already exist.";
                    }
                    return RedirectToAction("Index", "Distributor");
                }
            }
            TempData["Class"] = UtilityClass.Success;
            TempData["Message"] = "Distrobutor information is already up to date";
            //ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return RedirectToAction("Index", "Distributor");

        }

        public JsonResult GetAllThanaByMasterDepot(long id)
        {
            IEnumerable<ThanaWiseMasterDepot> thanas = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Thana/GetByMasterDepotId/" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ThanaWiseMasterDepot>>();
                    readTask.Wait();
                    thanas = readTask.Result;
                }
                else
                {
                    thanas = new List<ThanaWiseMasterDepot>();
                }
            }
            return Json(thanas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDistributorProductLineSubscriptions(long distributorId)
        {
            var distributorProductLines = GetDistributorProductLines(distributorId);

            var productLinesByDistributorId = distributorProductLines;
            //var productLinesForDropDown = Dropdown.ProductLines().FindAll(pl => productLinesByDistributorId.All(d => d.ProductLineId != pl.Id));
            var productLinesForDropDown = Dropdown.ProductLines();
            return Json(new { AllProductLines = productLinesForDropDown, ProductLinesByDistributorId = productLinesByDistributorId });
        }
        private IEnumerable<DistributorProductLine> GetDistributorProductLines(long distributorId)
        {
            IEnumerable<DistributorProductLine> distributorSubscriptions = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Distributor/GetProductLineByDistributorId?id=" + distributorId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<DistributorProductLine>>();
                    readTask.Wait();
                    distributorSubscriptions = readTask.Result;
                }
                else
                {
                    distributorSubscriptions = new List<DistributorProductLine>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return distributorSubscriptions;
        }

        public ActionResult SubscribeToProductLine(DistributorProductLine distributorProductLine)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (distributorProductLine != null)
            {
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<DistributorProductLine>("Distributor/SubscribeToProductLine", distributorProductLine);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return Json(new { status = "Saved" });
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        return Json(new { status = "Conflict" });
                    }
                }
            }
            return Json(new { status = "Failed" });
        }

        public ActionResult DeleteSubscription(DistributorProductLine distributorProductLine)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (distributorProductLine != null)
            {
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<DistributorProductLine>("Distributor/DeleteSubscription", distributorProductLine);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var distributorProductLines = GetDistributorProductLines((long)distributorProductLine.DistributorId);
                        return Json(new { status = "Ok", distributorProductLines = distributorProductLines });
                    }
                }
            }
            return Json(new { status = "Failed" });
        }

        public JsonResult GetDistributorDetailsById(long distributorId)
        {
            Distributor distributor = new Distributor();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Distributor/GetById?id=" + distributorId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Distributor>();
                    readTask.Wait();
                    distributor = readTask.Result;
                    return Json(new { Status = "Ok", Distributor = distributor });
                }
            }

            return Json(new { Status = "Failed" });
        }

    }
}
