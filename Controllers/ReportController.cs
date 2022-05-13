using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EFreshAdmin.Models;
using EFreshAdmin.Models.ViewModels;
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using EFreshStore.Models.ViewModels;
using EFreshStore.Utility;
using EFreshStoreCore.Model.Context;

namespace EFreshAdmin.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult SalesByProduct(SalesByProductVM salesByProductVm)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }


            salesByProductVm.Categories = Dropdown.Categories();
            salesByProductVm.Brands = Dropdown.Brands();
            salesByProductVm.ProductTypes = Dropdown.ProductTypes();
            return View(salesByProductVm);
        }

        public ActionResult ShowSalesByProduct(SalesByProductVM salesByProductVm)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<SalesByProductVM> sales = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                //var responseTask = client.GetAsync("Report/SalesByProduct");
                var queryString = "Report/GetSalesByProduct?";
                queryString = queryString + GenerateSalesByProductQueryString(salesByProductVm);
                var responseTask = client.GetAsync(queryString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SalesByProductVM>>();
                    readTask.Wait();
                    sales = readTask.Result;
                }
                else
                {
                    sales = new List<SalesByProductVM>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            salesByProductVm.Categories = Dropdown.Categories();
            salesByProductVm.Brands = Dropdown.Brands();
            salesByProductVm.ProductTypes = Dropdown.ProductTypes();
            ViewBag.Sales = sales;
            return View("SalesByProduct", salesByProductVm);
        }
        public ActionResult Analysis(TotalOrdersVM totalOrdersVm)

        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            totalOrdersVm.Categories = Dropdown.Categories();
            totalOrdersVm.Brands = Dropdown.Brands();
            totalOrdersVm.ProductTypes = Dropdown.ProductTypes();
            totalOrdersVm.MasterDepots = Dropdown.MasterDepo();
            return View(totalOrdersVm);
        }

        public ActionResult GetAnalysisReport(TotalOrdersVM totalOrdersVm)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            long userId = Convert.ToInt64(Session["UserId"]);

            if (userId == (long)UserTypeEnum.MasterDepotUser)
            {
                totalOrdersVm.MasterDepotIds = new[] { userId };
            }

            IEnumerable<TotalOrdersVM> orders = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var queryString = "Report/GetAnalysisReport?";
                queryString = queryString + GenerateTotalOrdersQueryString(totalOrdersVm);
                var responseTask = client.GetAsync(queryString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TotalOrdersVM>>();
                    readTask.Wait();
                    orders = readTask.Result;
                }
                else
                {
                    orders = new List<TotalOrdersVM>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }


            if (orders != null)
            {
                var totalOrders = orders.Count();
                var totalProductQuantity = orders.Sum(c => c.TotalProducts);
                var totalPrice = orders.Sum(c => c.TotalPrice);
                var totalRevenue = totalProductQuantity * totalPrice;
                var averageOrderValue = totalRevenue / totalOrders;
                Session["Revenue"] = totalRevenue;
                Session["AverageOrderValue"] = averageOrderValue.ToString("#.##");

            }
            var returningCustomerRate = ReturningCustomerRate(totalOrdersVm);
            Session["ReturnCustomerRate"] = returningCustomerRate;
            totalOrdersVm.Categories = Dropdown.Categories();
            totalOrdersVm.Brands = Dropdown.Brands();
            totalOrdersVm.ProductTypes = Dropdown.ProductTypes();
            totalOrdersVm.MasterDepots = Dropdown.MasterDepo();
            Session["Orders"] = orders;
            return View("Analysis", totalOrdersVm);
        }

        public ActionResult TotalOrders(TotalOrdersVM totalOrdersVm)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            totalOrdersVm.Categories = Dropdown.Categories();
            totalOrdersVm.Brands = Dropdown.Brands();
            totalOrdersVm.ProductTypes = Dropdown.ProductTypes();
            totalOrdersVm.MasterDepots = Dropdown.MasterDepo();
            return View(totalOrdersVm);
        }

        public ActionResult ShowTotalOrders(TotalOrdersVM totalOrdersVm)
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
            

            totalOrdersVm.Categories = Dropdown.Categories();
            totalOrdersVm.Brands = Dropdown.Brands();
            totalOrdersVm.ProductTypes = Dropdown.ProductTypes();
            totalOrdersVm.MasterDepots = Dropdown.MasterDepo();

            if (totalOrdersVm.ToDate < totalOrdersVm.FromDate)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "To to date must be greater than from date";
                return View("TotalOrders", totalOrdersVm);
            }


            IEnumerable<TotalOrdersVM> orders = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                //var responseTask = client.GetAsync("Report/SalesByProduct");
                var queryString = "Report/GetTotalOrders?";
                if (userTypeId == (long)UserTypeEnum.MasterDepotUser)
                {
                    totalOrdersVm.UserId = userId;
                    queryString = queryString + "userId=" + userId + "&";
                    if (totalOrdersVm.MasterDepotIds != null)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "Permission denied";
                        return View("TotalOrders", totalOrdersVm);
                    }

                }
                queryString = queryString + GenerateTotalOrdersQueryString(totalOrdersVm);
                var responseTask = client.GetAsync(queryString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TotalOrdersVM>>();
                    readTask.Wait();
                    orders = readTask.Result;
                }
                else
                {
                    orders = new List<TotalOrdersVM>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            
            ViewBag.Orders = orders;
            return View("TotalOrders", totalOrdersVm);
        }

        public ActionResult SalesByLocation(SalesByLocationVM salesByLocationVm)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            salesByLocationVm.Categories = Dropdown.Categories();
            salesByLocationVm.Brands = Dropdown.Brands();
            salesByLocationVm.ProductTypes = Dropdown.ProductTypes();
            salesByLocationVm.MasterDepots = Dropdown.MasterDepo();
            salesByLocationVm.Thanas = Dropdown.Thanas();
            salesByLocationVm.Districts = Dropdown.Districts();

            return View(salesByLocationVm);
        }

        public ActionResult ShowSalesByLocation(SalesByLocationVM salesByLocationVm)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            long userTypeId = Convert.ToInt64(Session["UserTypeId"]);
            long userId = Convert.ToInt64(Session["UserId"]);

            salesByLocationVm.Categories = Dropdown.Categories();
            salesByLocationVm.Brands = Dropdown.Brands();
            salesByLocationVm.ProductTypes = Dropdown.ProductTypes();
            salesByLocationVm.MasterDepots = Dropdown.MasterDepo();
            salesByLocationVm.Thanas = Dropdown.Thanas();
            salesByLocationVm.Districts = Dropdown.Districts();

            if (salesByLocationVm.ToDate < salesByLocationVm.FromDate)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "To to date must be greater than from date";
                return View("SalesByLocation", salesByLocationVm);
            }

            IEnumerable<SalesByLocationVM> sales = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                //var responseTask = client.GetAsync("Report/SalesByProduct");

                var queryString = "Report/GetSalesByLocation?";
                if (userTypeId == (long)UserTypeEnum.MasterDepotUser)
                {
                    salesByLocationVm.UserId = userId;
                    queryString = queryString + "userId=" + userId + "&";

                    if (salesByLocationVm.MasterDepotIds != null)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "Permission denied";
                        return View("SalesByLocation", salesByLocationVm);
                    }
                }
                queryString = queryString + GenerateSalesByLocationQueryString(salesByLocationVm);
                var responseTask = client.GetAsync(queryString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SalesByLocationVM>>();
                    readTask.Wait();
                    sales = readTask.Result;
                }
                else
                {
                    sales = new List<SalesByLocationVM>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.Sales = sales;

            return View("SalesByLocation", salesByLocationVm);
        }

        public ActionResult OrdersByStatus(OrdersByStatusVM ordersByStatusVm)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            ordersByStatusVm.Categories = Dropdown.Categories();
            ordersByStatusVm.Brands = Dropdown.Brands();
            ordersByStatusVm.ProductTypes = Dropdown.ProductTypes();
            ordersByStatusVm.MasterDepots = Dropdown.MasterDepo();
            ordersByStatusVm.OrderStates = Dropdown.OrderStates();
            return View(ordersByStatusVm);
        }

        public ActionResult ShowOrdersByStatus(OrdersByStatusVM ordersByStatusVm)
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
            

            ordersByStatusVm.Categories = Dropdown.Categories();
            ordersByStatusVm.Brands = Dropdown.Brands();
            ordersByStatusVm.ProductTypes = Dropdown.ProductTypes();
            ordersByStatusVm.MasterDepots = Dropdown.MasterDepo();
            ordersByStatusVm.OrderStates = Dropdown.OrderStates();

            if (ordersByStatusVm.ToDate < ordersByStatusVm.FromDate)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "To to date must be greater than from date";
                return View("OrdersByStatus", ordersByStatusVm);
            }

            IEnumerable<OrdersByStatusVM> orders = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                //var responseTask = client.GetAsync("Report/SalesByProduct");
                var queryString = "Report/GetOrdersByOrderStatus?";
                if (userTypeId == (long)UserTypeEnum.MasterDepotUser)
                {
                    ordersByStatusVm.UserId = userId;
                    queryString = queryString + "userId=" + userId + "&";
                    if (ordersByStatusVm.MasterDepotIds != null)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "Permission denied";
                        return View("OrdersByStatus", ordersByStatusVm);
                    }
                }
                queryString = queryString + GenerateOrdersByStatusQueryString(ordersByStatusVm);
                var responseTask = client.GetAsync(queryString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<OrdersByStatusVM>>();
                    readTask.Wait();
                    orders = readTask.Result;
                }
                else
                {
                    orders = new List<OrdersByStatusVM>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.Orders = orders;

            return View("OrdersByStatus", ordersByStatusVm);
        }
       
        public ActionResult OrderRateOverTime()
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
        [HttpGet]
        public ActionResult OrderRateOverTime(SalesOverTimeVm salesOverTime)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (salesOverTime.ToMonth < salesOverTime.FromMonth || salesOverTime.ToYear < salesOverTime.FromYear)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "To month/year must be greater than from month/year";
                return View();
            }
            IEnumerable<OrderVsMonthOrYearVm> orderVsMonthOrYears = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);

                var queryString = "Report/SalesOverTime?salesOverTime.ReportType="
                                  + salesOverTime.ReportType + "&salesOverTime.FromMonth="
                                  + salesOverTime.FromMonth + "&salesOverTime.ToMonth="
                                  + salesOverTime.ToMonth + "&salesOverTime.FromYear="
                                  + salesOverTime.FromYear + "&salesOverTime.ToYear="
                                  + salesOverTime.ToYear + "&salesOverTime.Year="
                                  + salesOverTime.Year;

                var responseTask = client.GetAsync(queryString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<OrderVsMonthOrYearVm>>();
                    readTask.Wait();
                    orderVsMonthOrYears = readTask.Result;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View(orderVsMonthOrYears);
        }

        public ActionResult OrderGrowthRate(OrderGrowthRateVM orderGrowthRate)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            if (orderGrowthRate.PriorFromDate != null &&
                orderGrowthRate.PriorToDate != null &&
                orderGrowthRate.CurrentFromDate != null &&
                orderGrowthRate.CurrentToDate != null)
            {

                if (orderGrowthRate.PriorToDate < orderGrowthRate.PriorFromDate)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "To prior to date must be greater than prior from date";
                    return View(orderGrowthRate);
                }
                if (orderGrowthRate.CurrentFromDate < orderGrowthRate.PriorToDate)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "To current from date must be greater than prior to date";
                    return View(orderGrowthRate);
                }
                if (orderGrowthRate.CurrentToDate < orderGrowthRate.CurrentFromDate)
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "To current to date must be greater than current from date";
                    return View(orderGrowthRate);
                }

                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);

                    var queryString = "Report/GetOrderGrowthRate?orderGrowthRate.PriorFromDate=" 
                                      + orderGrowthRate.PriorFromDate+ "&orderGrowthRate.PriorToDate=" 
                                      + orderGrowthRate.PriorToDate + "&orderGrowthRate.CurrentFromDate=" 
                                      + orderGrowthRate.CurrentFromDate + "&orderGrowthRate.CurrentToDate=" 
                                      + orderGrowthRate.CurrentToDate;

                    var responseTask = client.GetAsync(queryString);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<double>();
                        readTask.Wait();
                        orderGrowthRate.OrderGrowthRate = readTask.Result;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            
            return View(orderGrowthRate);
        }

        private string GenerateTotalOrdersQueryString(TotalOrdersVM totalOrdersVm)
        {
            var queryString = "";
            if (totalOrdersVm.BrandIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(totalOrdersVm.BrandIds, "brandIds");
            }

            if (totalOrdersVm.CategoryIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(totalOrdersVm.CategoryIds, "categoryIds");
            }

            if (totalOrdersVm.ProductTypeIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(totalOrdersVm.ProductTypeIds, "productTypeIds");
            }

            if (totalOrdersVm.MasterDepotIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(totalOrdersVm.MasterDepotIds, "masterDepotIds");
            }

            if (totalOrdersVm.FromDate != null)
            {
                queryString = queryString + "fromDate=" + totalOrdersVm.FromDate + "&";
            }
            if (totalOrdersVm.ToDate != null)
            {
                queryString = queryString + "toDate=" + totalOrdersVm.ToDate;
            }

            return queryString;
        }

        private string GenerateSalesByProductQueryString(SalesByProductVM salesByProductVm)
        {
            var queryString = "";
            if (salesByProductVm.BrandIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(salesByProductVm.BrandIds, "brandIds");
            }

            if (salesByProductVm.CategoryIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(salesByProductVm.CategoryIds, "categoryIds");
            }

            if (salesByProductVm.ProductTypeIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(salesByProductVm.ProductTypeIds, "productTypeIds");
            }

            if (salesByProductVm.FromDate != null)
            {
                queryString = queryString + "fromDate=" + salesByProductVm.FromDate + "&";
            }
            if (salesByProductVm.ToDate != null)
            {
                queryString = queryString + "toDate=" + salesByProductVm.ToDate;
            }

            return queryString;
        }


        private double ReturningCustomerRate(TotalOrdersVM totalOrdersVm)
        {
            LoginCredentials token = (LoginCredentials) Session["userToken"];
            double returningCustomerRate = 0.0;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                //var responseTask = client.GetAsync("Report/SalesByProduct");
                var queryString = "Report/GetReturningCustomerRate?";
                queryString = queryString + GenerateTotalOrdersQueryString(totalOrdersVm);
                var responseTask = client.GetAsync(queryString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    returningCustomerRate = Convert.ToDouble(readTask.Result);
                }
                else
                {
                    returningCustomerRate = Convert.ToDouble("0");
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return returningCustomerRate;
        }

        private string GenerateSalesByLocationQueryString(SalesByLocationVM salesByLocationVm)
        {
            var queryString = "";

            if (salesByLocationVm.MasterDepotIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(salesByLocationVm.MasterDepotIds, "masterDepotIds");
            }
            if (salesByLocationVm.BrandIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(salesByLocationVm.BrandIds, "brandIds");
            }

            if (salesByLocationVm.CategoryIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(salesByLocationVm.CategoryIds, "categoryIds");
            }

            if (salesByLocationVm.ProductTypeIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(salesByLocationVm.ProductTypeIds, "productTypeIds");
            }

            if (salesByLocationVm.ThanaIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(salesByLocationVm.ThanaIds, "thanaIds");
            }

            if (salesByLocationVm.DistrictIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(salesByLocationVm.DistrictIds, "districtIds");
            }

            if (salesByLocationVm.FromDate != null)
            {
                queryString = queryString + "fromDate=" + salesByLocationVm.FromDate + "&";
            }

            if (salesByLocationVm.ToDate != null)
            {
                queryString = queryString + "toDate=" + salesByLocationVm.ToDate;
            }

            return queryString;
        }
        private string GenerateOrdersByStatusQueryString(OrdersByStatusVM ordersByStatusVm)
        {
            var queryString = "";
            if (ordersByStatusVm.MasterDepotIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(ordersByStatusVm.MasterDepotIds, "masterDepotIds");
            }
            if (ordersByStatusVm.BrandIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(ordersByStatusVm.BrandIds, "brandIds");
            }

            if (ordersByStatusVm.CategoryIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(ordersByStatusVm.CategoryIds, "categoryIds");
            }

            if (ordersByStatusVm.ProductTypeIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(ordersByStatusVm.ProductTypeIds, "productTypeIds");
            }

            if (ordersByStatusVm.OrderStateIds != null)
            {
                queryString = queryString + UtilityClass.GenerateQueryStringFromArray(ordersByStatusVm.OrderStateIds, "orderStateIds");
            }
            
            if (ordersByStatusVm.FromDate != null)
            {
                queryString = queryString + "fromDate=" + ordersByStatusVm.FromDate + "&";
            }

            if (ordersByStatusVm.ToDate != null)
            {
                queryString = queryString + "toDate=" + ordersByStatusVm.ToDate;
            }

            return queryString;
        }

        //public ActionResult ExportToExcel()
        //{
        //    IEnumerable<TotalOrdersVM> orders = (IEnumerable<TotalOrdersVM>) Session["Orders"];
        //    string filename = "ExcelDataFrom.xls";
        //    string folderPath = HttpContext.Server.MapPath("/ExcelFiles/");
        //    string filePath = Path.Combine(folderPath, filename);

        //    //Step-1: Checking: the file name exist in server, if it is found then remove from server.------------------
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        System.IO.File.Delete(filePath);
        //    }

        //    //Step-2: Get Html Data & Converted to String----------------------------------------------------------------
        //    string HtmlResult = RenderRazorViewToString("~/Views/Report/GenerateAnalysisExcel.cshtml", orders);

        //    //Step-4: Html Result store in Byte[] array------------------------------------------------------------------
        //    byte[] ExcelBytes = Encoding.ASCII.GetBytes(HtmlResult);

        //    //Step-5: byte[] array converted to file Stream and save in Server------------------------------------------- 
        //    using (Stream file = System.IO.File.OpenWrite(filePath))
        //    {
        //        file.Write(ExcelBytes, 0, ExcelBytes.Length);
        //    }

        //    //Step-6: Download Excel file 
        //    Response.ContentType = "application/vnd.ms-excel";
        //    Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filename));
        //    Response.WriteFile(filePath);
        //    Response.End();
        //    Response.Flush();
        //   return RedirectToAction("Analysis","Report");

        //}

        //protected string RenderRazorViewToString(string viewName, object model)
        //{
        //    if (model != null)
        //    {
        //        ViewData.Model = model;
        //    }
        //    using (StringWriter sw = new StringWriter())
        //    {
        //        ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
        //        ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}

        public void GetTotalOrderParammeters(TotalOrdersVM totalOrdersVm)
        {
            if (totalOrdersVm.BrandIds != null)
            {
                
            }
            if (totalOrdersVm.CategoryIds != null)
            {
                
            }
            if (totalOrdersVm.ProductTypeIds != null)
            {
                
            }
            if (totalOrdersVm.MasterDepotIds != null)
            {
                
            }
        }
    }
}