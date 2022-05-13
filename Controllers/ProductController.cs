using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using EFreshAdmin.Models;
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using EFreshStore.Models.ViewModels;
using EFreshStore.Utility;
using EFreshStoreCore.Model.Context;

namespace EFreshAdmin.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Index()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<Product> products = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Product/GetAll");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Product>>();
                    readTask.Wait();
                    products = readTask.Result;
                }
                else
                {
                    products = new List<Product>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.Products = products;
            ViewBag.category = Dropdown.Categories();
            ViewBag.brand = Dropdown.Brands();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product aProduct)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.category = Dropdown.Categories();
            ViewBag.brand = Dropdown.Brands();
            long userId = Convert.ToInt64(Session["UserId"]);
            if (aProduct!=null)
            {
                aProduct.CreatedOn = DateTime.UtcNow.AddHours(6);
                aProduct.CreatedBy = userId;
                aProduct.IsDeleted = false;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<Product>("Product/Add", aProduct);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Product added successfully.";
                        return RedirectToAction("Index","Product");
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "This Product is already exist.";
                        return RedirectToAction("Index", "Product");
                    }
                } 
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return RedirectToAction("Index", "Product");
        }

        public ActionResult AddDetails()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";                
                return RedirectToAction("Login", "Home");
            }
            ViewBag.product = Dropdown.Products();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDetails(ProductUnitVm productUnitvm)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (productUnitvm != null)
            {
                long userId = Convert.ToInt64(Session["UserId"]);
                productUnitvm.IsDeleted = false;
                productUnitvm.IsActive = true;
                productUnitvm.CreatedBy = userId;
                foreach (var pic in productUnitvm.ProductImages)
                {
                    if (pic.ImageLocation != null)
                    {
                        MemoryStream target = new MemoryStream();
                        pic.ImageLocation.InputStream.CopyTo(target);
                        var data = target.ToArray();
                        productUnitvm.ImageBytes.Add(data);
                    }
                }
                
                productUnitvm.ProductImages = null;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<ProductUnitVm>("Product/AddDetails", productUnitvm);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Product details has been saved successfully.";
                        return RedirectToAction("ViewProductUnit", "Product");
                    }
                }
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Failed to save product details.";
            }
            ViewBag.product = Dropdown.Products();
            return View();
        }

        public ActionResult EditDetails(long id)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.product = Dropdown.Products();
            ProductUnit productDetails = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Product/GetProductDetails?id=" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ProductUnit>();
                    readTask.Wait();
                    productDetails = readTask.Result;
                }
                else
                {
                    productDetails = new ProductUnit();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(productDetails);
        }

        //Edit Product Details action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDetails(ProductUnitVm productUnitvm)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.product = Dropdown.Products();
            long userId = Convert.ToInt64(Session["UserId"]);
            if (productUnitvm != null)
            {
                productUnitvm.IsDeleted = false;
                productUnitvm.IsActive = true;
                productUnitvm.ModifiedBy = userId;
                foreach (var pic in productUnitvm.ProductImages)
                {
                    if (pic.ImageLocation != null)
                    {
                        MemoryStream target = new MemoryStream();
                        pic.ImageLocation.InputStream.CopyTo(target);
                        var data = target.ToArray();
                        productUnitvm.ImageBytes.Add(data);
                    }
                }

                productUnitvm.ProductImages = null;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var putTask = client.PostAsJsonAsync<ProductUnitVm>("Product/EditDetails", productUnitvm);
                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Product details updated successfully.";
                        return RedirectToAction("ViewProductUnit", "Product");
                    }
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Something went wrong.";
                    return View();
                }
            }
            return View();
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
            Product product =null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Product/GetById?id=" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Product>();
                    readTask.Wait();
                    product = readTask.Result;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
          
            ViewBag.category = Dropdown.Categories();
            ViewBag.brand = Dropdown.Brands();
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product aProduct)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.category = Dropdown.Categories();
            ViewBag.brand = Dropdown.Brands();
            long userId = Convert.ToInt64(Session["UserId"]);
            if (aProduct != null)
            {
                aProduct.ModifiedBy = userId;                
                aProduct.ModifiedOn = DateTime.UtcNow.AddHours(6);
                aProduct.IsDeleted = false;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var putTask = client.PostAsJsonAsync<Product>("Product/Edit", aProduct);
                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Product updated successfully.";
                        return RedirectToAction("Index", "Product");
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "This product already exist.";
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return View(aProduct);
        }
        public JsonResult GetProductInfoById(long productId)
        {
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Product/GetById?id=" + productId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Product>();
                    readTask.Wait();
                    var product = readTask.Result;
                    return Json(new { Status = "Ok", Product = product }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Status = "Failed" });
        }
        public ActionResult ViewProductUnit()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            List<ProductUnit> productUnits = GetAllProductUnits();
            ViewBag.productUnits = productUnits;
            return View(productUnits);
        }

        public ProductUnit GetProductUnitByProductId(long productId)
        {
            ProductUnit product = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Product/GetProductDetails?id=" + productId);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ProductUnit>();
                    readTask.Wait();
                    product = readTask.Result;
                }
                else
                {
                    product = new ProductUnit();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
                return product;
            }
        }

        public List<ProductUnit> GetAllProductUnits()
        {
            IEnumerable<ProductUnit> products;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Product/GetAllProducts");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductUnit>>();
                    readTask.Wait();
                    products = readTask.Result;
                }
                else
                {
                    products = new List<ProductUnit>();
                }
            }
            return products.ToList();
        }

        //Discount and promotions

        //public ActionResult Discount()
        //{
        //    LoginCredentials token = (LoginCredentials)Session["userToken"];
        //    if (token == null)
        //    {
        //        TempData["Class"] = UtilityClass.Error;
        //        TempData["Message"] = "Access Denied.";
        //        return RedirectToAction("Login", "Home");
        //    }

        //    ViewBag.products = Dropdown.Products();
        //    ViewBag.categories = Dropdown.Categories();
        //    ViewBag.productTypes = Dropdown.ProductTypes();
        //    return View();
        //}

        [HttpPost]
        public ActionResult CreateDiscount(ProductDiscount productDiscount)
        {
            ViewBag.products = Dropdown.Products();
            ViewBag.categories = Dropdown.Categories();
            ViewBag.productTypes = Dropdown.ProductTypes();
            LoginCredentials token = (LoginCredentials) Session["userToken"];
            if (token == null)
            {
                ViewBag.Message = "Access Denied";
                ViewBag.Class = UtilityClass.Error;
                return RedirectToAction("Login", "Home");
            }
            try
            {
                if (productDiscount != null)
                {
                    long userId = Convert.ToInt64(Session["UserId"]);
                    productDiscount.CreatedBy = userId;
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                            token.AccessToken);
                        var postTask = client.PostAsJsonAsync<ProductDiscount>("product/AddDiscount", productDiscount);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Product discount added successfully.";
                            return RedirectToAction("ViewProductDiscounts", "Product");
                        }
                        else
                        {
                            ViewBag.Class = UtilityClass.Error;
                            ViewBag.Message = "Something went wrong.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ViewProductDiscounts", "Product");

            }
            return RedirectToAction("ViewProductDiscounts", "Product");
        }

        [HttpPost]
        public ActionResult EditDiscount(long? productId, ProductDiscount productDiscount)
        {
            ViewBag.products = Dropdown.Products();
            ViewBag.categories = Dropdown.Categories();
            ViewBag.productTypes = Dropdown.ProductTypes();
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                ViewBag.Message = "Access Denied";
                ViewBag.Class = UtilityClass.Error;
                return RedirectToAction("Login", "Home");
            }
            try
            {
                if (productDiscount != null)
                {
                    long userId = Convert.ToInt64(Session["UserId"]);
                    using (var client = new HttpClientDemo())
                    {
                        productDiscount.ModifiedBy = userId;
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                            token.AccessToken);
                        var postTask = client.PostAsJsonAsync<ProductDiscount>("product/EditDiscount", productDiscount);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Product discount updated successfully.";
                            return RedirectToAction("ViewProductDiscounts", "Product");
                        }
                        else if (result.StatusCode == HttpStatusCode.NotFound)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Couldn't update discount for selected product!";
                            return RedirectToAction("ViewProductDiscounts", "Product");
                        }
                        else
                        {
                            ViewBag.Class = UtilityClass.Error;
                            ViewBag.Message = "Something went wrong.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ViewProductDiscounts", "Product");
            }
            return RedirectToAction("ViewProductDiscounts", "Product");
        }

        public JsonResult GetProductUnitsByProduct(long? id)
        {
            IList<ProductUnit> productUnits = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Product/GetByProductId/" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductUnit>>();
                    readTask.Wait();
                    productUnits = readTask.Result;
                }
                else
                {
                    productUnits = new List<ProductUnit>();
                }
            }

            return Json(productUnits, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductByProductTypeAndCategory(long productTypeId,long categoryId)
        {
            IList<Product> products = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Product/GetByProductTypeAndCategory?productTypeId=" + productTypeId+ "&categoryId="+ categoryId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Product>>();
                    readTask.Wait();
                    products = readTask.Result;
                }
                else
                {
                    products = new List<Product>();
                }
            }

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDiscountByProductUnit(long? id)
        {
            ProductDiscount productDiscount = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Product/GetDiscountByProductUnit/" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ProductDiscount>();
                    readTask.Wait();
                    productDiscount = readTask.Result;
                }
                else
                {
                    productDiscount = new ProductDiscount();
                }
            }
            return Json(productDiscount, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductDiscountByDiscountId(long? discountId)
        {
            ProductDiscount productDiscount = new ProductDiscount();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Product/GetProductDiscountByDiscountId/" + discountId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ProductDiscount>();
                    readTask.Wait();
                    productDiscount = readTask.Result;
                    return Json(new { Status = "Ok", ProductDiscount = productDiscount });
                }
            }
            return Json(new { Status = "Failed" });
        }

        public ActionResult ViewProductDiscounts()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<ProductDiscount> productDiscounts = null;

            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Product/GetAllDiscountedProducts");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductDiscount>>();
                    readTask.Wait();
                    productDiscounts = readTask.Result;
                }
                else
                {
                    productDiscounts = new List<ProductDiscount>();
                }
            }
            ViewBag.ProductDiscounts = productDiscounts;
            ViewBag.products = Dropdown.Products();
            ViewBag.categories = Dropdown.Categories();
            ViewBag.productTypes = Dropdown.ProductTypes();
            return View();
        }
    }
}