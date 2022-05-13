using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProductLineController : Controller
    {
        // GET: ProductLine
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<ProductLine> productLines = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("ProductLine/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductLine>>();
                    readTask.Wait();
                    productLines = readTask.Result;
                }
                else
                {
                    productLines = new List<ProductLine>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.ProductLines = productLines;
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductLine productLine)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            using (var client = new HttpClientDemo())
            {
                long userId = Convert.ToInt64(Session["UserId"]);
                productLine.CreatedBy = userId;
                productLine.CreatedOn = DateTime.UtcNow.AddHours(6); ;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var postTask = client.PostAsJsonAsync<ProductLine>("ProductLine/Add", productLine);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Product line saved successfully.";
                    return RedirectToAction("Index", "ProductLine");
                }
                else
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = result.Content.ReadAsStringAsync().Result;
                    //FlashMessage.Warning(result.Content.ReadAsStringAsync().Result);
                    return RedirectToAction("Index", "ProductLine");
                }
            }
        }

        public ActionResult Edit(long? id)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ProductLine productLine = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("ProductLine/GetById/" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ProductLine>();
                    readTask.Wait();
                    productLine = readTask.Result;
                }
                else
                {
                    productLine = null;
                    return RedirectToAction("Index", "ProductLine");
                }
            }

            if (productLine == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Product line not found.";
                return RedirectToAction("Index", "ProductLine");
            }

            return RedirectToAction("Index", "ProductLine");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductLine productLine)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            using (var client = new HttpClientDemo())
            {
                if (productLine.Description == null)
                {
                    productLine.Description = "";
                }
                long userId = Convert.ToInt64(Session["UserId"]);
                productLine.ModifiedBy = userId;
                productLine.ModifiedOn = DateTime.UtcNow.AddHours(6);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var postTask = client.PostAsJsonAsync<ProductLine>("ProductLine/Edit", productLine);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Product line updated successfully.";
                    return RedirectToAction("Index", "ProductLine");
                }
                if (result.StatusCode == HttpStatusCode.Conflict)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "This product line alreasy exist.";
                    return RedirectToAction("Index", "ProductLine");
                }
                else
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = result.Content.ReadAsStringAsync().Result;
                    TempData["Message"] = "Product line is already up to date";
                    return RedirectToAction("Index", "ProductLine");
                }
            }
        }

        //GET link product action method
        public ActionResult Detail()
        {
            IEnumerable<ProductLineDetail> productLineDetails = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("ProductLine/Detail");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductLineDetail>>();
                    readTask.Wait();
                    productLineDetails = readTask.Result;
                }
                else
                {
                    productLineDetails = new List<ProductLineDetail>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.Products = productLineDetails;
            return View(productLineDetails);
        }

        public ActionResult AddDetail()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.product = Dropdown.Products();
            ViewBag.productLine = Dropdown.ProductLines();
            return View();
        }

        public JsonResult GetProductLineDetails(long productLineId)
        {
            var productLineDetails = GetLineDetails(productLineId);

            var productsByLineId = productLineDetails;
            //var productsForDropDown = Dropdown.Products().FindAll(p => productsByLineId.All(i => i.ProductId != p.Id));
            var productsForDropDown = Dropdown.Products();
            return Json(new { AllProducts = productsForDropDown, ProductsByLineId = productsByLineId });
        }

        private IEnumerable<ProductLineDetail> GetLineDetails(long productLineId)
        {
            IEnumerable<ProductLineDetail> productLineDetails = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("ProductLine/GetProductsByLineId?id=" + productLineId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductLineDetail>>();
                    readTask.Wait();
                    productLineDetails = readTask.Result;
                }
                else
                {
                    productLineDetails = new List<ProductLineDetail>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return productLineDetails;
        }

        [HttpPost]
        public ActionResult AddDetail(ProductLineDetail productLineDetail)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.product = Dropdown.Products();
            ViewBag.productLine = Dropdown.ProductLines();
            if (productLineDetail != null)
            {
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<ProductLineDetail>("ProductLine/AddDetail", productLineDetail);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Product line details saved successfully.";
                        return RedirectToAction("Detail", "ProductLine");
                    }
                    if(result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "Product line details already exist.";
                        return View();
                    }
                }
            }
            return View();
        }

        public ActionResult AddProductToLine(ProductLineDetail productLineDetail)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (productLineDetail != null)
            {
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<ProductLineDetail>("ProductLine/AddDetail", productLineDetail);
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
            return Json(new {status="Failed"});
        }

        [HttpGet]
        public ActionResult EditDetail(long id)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.product = Dropdown.Products();
            ViewBag.productLine = Dropdown.ProductLines();
            ProductLineDetail productLineDetail = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("ProductLine/GetByDetailId/" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ProductLineDetail>();
                    readTask.Wait();
                    productLineDetail = readTask.Result;
                }
            }

            if (productLineDetail == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Product line not found.";
                return View();
            }
            return View(productLineDetail);
        }

        [HttpPost]
        public ActionResult EditDetail(ProductLineDetail productLineDetail)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.product = Dropdown.Products();
            ViewBag.productLine = Dropdown.ProductLines();
            if (productLineDetail != null)
            {
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<ProductLineDetail>("ProductLine/EditDetail", productLineDetail);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Product line has been updated successfully.";
                        return RedirectToAction("Detail", "ProductLine");
                    }
                    if(result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "This product line already exist.";
                        return View();
                    }
                }
            }
            return View();
        }

        public ActionResult DeleteDetail(ProductLineDetail productLineDetail)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (productLineDetail != null)
            {
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<ProductLineDetail>("ProductLine/DeleteDetail", productLineDetail);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var productLineDetails = GetLineDetails((long)productLineDetail.ProductLineId);
                        return Json(new { status = "Ok" , productLineDetails = productLineDetails });
                    }
                }
            }
            return Json(new { status = "Failed" });
        }
        public JsonResult GetProductLineById(long productLineId)
        {
            ProductLine productLine = new ProductLine();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("ProductLine/GetById?id=" + productLineId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ProductLine>();
                    readTask.Wait();
                    productLine = readTask.Result;
                    return Json(new { Status = "Ok", ProductLine = productLine });
                }
            }

            return Json(new { Status = "Failed" });
        }
    }
}