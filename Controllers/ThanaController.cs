using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using EFreshAdmin.Models;
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using EFreshStore.Utility;
using Newtonsoft.Json;

namespace EFreshAdmin.Controllers
{
    public class ThanaController : Controller
    {
        public async Task<List<District>> DistrictList()
        {
            List<District> districts = new List<District>();

            using (var client = new HttpClientDemo())
            {
                //Passing service base url  
                client.BaseAddress = new Uri("http://dotnet.nerdcastlebd.com/EFreshStoreApp/Api");
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                var res = await client.GetAsync("District/GetAll");

                //Checking the response is successful or not which is sent using HttpClient  
                if (res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var districtsRes = res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    districts = JsonConvert.DeserializeObject<List<District>>(districtsRes);

                }
                //returning the employee list to view  
                return districts;
            }
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
            ViewBag.district = Dropdown.Districts();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Thana aThana)
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
                if (aThana != null)
                {
                    aThana.CreatedBy = userId;
                    aThana.CreatedOn = DateTime.UtcNow.AddHours(6);
                    aThana.IsDeleted = false;
                    using (var client = new HttpClientDemo())
                    {
                        var postTask = client.PostAsJsonAsync<Thana>("Thana/Create", aThana);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = @"Thana added successfully.";
                        }
                        if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "This thana already exists in this district.";
                        }
                        if (result.StatusCode == HttpStatusCode.BadRequest)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Something went wrong.";
                        }
                        return RedirectToAction("Views", "Thana");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Views", "Thana");
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
            IEnumerable<Thana> thanas = Dropdown.Thanas();
            ViewBag.Districts = Dropdown.Districts();
            ViewBag.Thanas = thanas;
            return View();
        }
        public ActionResult FilterThana(long? id)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return RedirectToAction("Views", "Thana");
            }
            IEnumerable<Thana> thanas = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Thana/GetByDistrictId/" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Thana>>();
                    readTask.Wait();
                    thanas = readTask.Result;
                }
                else
                {
                    thanas = new List<Thana>();
                }
            }
            ViewBag.Districts = Dropdown.Districts();
            ViewBag.Thanas = thanas;
            return View("Views");

        }
        public ActionResult Edit(long id)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            Thana thana = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Thana/GetById/" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Thana>();
                    readTask.Wait();
                    thana = readTask.Result;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            IEnumerable<District> aDistrict = Dropdown.Districts();

            ViewBag.district = aDistrict;
            return View(thana);
        }

        [HttpPost]
        public ActionResult Edit(Thana aThana)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.district = Dropdown.Districts();
            long userId = Convert.ToInt64(Session["UserId"]);
            try
            {
                if (aThana != null)
                {
                    aThana.ModifiedBy = userId;
                    aThana.ModifiedOn = DateTime.UtcNow.AddHours(6);
                    aThana.IsDeleted = false;
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var putTask = client.PostAsJsonAsync<Thana>("Thana/Edit", aThana);
                        putTask.Wait();

                        var result = putTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Thana updated successfully.";
                        }

                        if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "This thana already exists in this district.";
                        }

                        if (result.StatusCode == HttpStatusCode.BadRequest)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Something went wrong.";
                        }

                        return RedirectToAction("Views", "Thana");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Views", "Thana");
        }

        public JsonResult GetThanaDetailsById(long thanaId)
        {
            Thana thana = new Thana();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Thana/GetById?id=" + thanaId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Thana>();
                    readTask.Wait();
                    thana = readTask.Result;
                    return Json(new { Status = "Ok", Thana = thana });
                }
            }

            return Json(new { Status = "Failed" });
        }
    }
}