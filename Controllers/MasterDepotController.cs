using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using EFreshAdmin.Models;
using EFreshAdmin.Models.EntityModels;
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using EFreshStore.Utility;
using EFreshStoreCore.Model.Context;
using Newtonsoft.Json;
using EFreshAdmin.Utility.Enums;

namespace EFreshAdmin.Controllers
{
    public class MasterDepotController : Controller
    {
        // GET: Master Depot
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

        //Create Master Depot
        [HttpPost]
        public ActionResult Create(MasterDepot aMasterDepot)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (aMasterDepot != null)
            {
                long userId = Convert.ToInt64(Session["UserId"]);
                using (var client = new HttpClientDemo())
                {
                    aMasterDepot.CreatedOn = DateTime.UtcNow.AddHours(6);
                    aMasterDepot.CreatedBy = userId;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<MasterDepot>("MasterDepot/Add", aMasterDepot);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Master depot created successfully.";
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "This email already exist.";
                    }
                    return RedirectToAction("Index", "MasterDepot");
                }
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return RedirectToAction("Index", "MasterDepot");
        }

        //Get all master depot
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
            IEnumerable<MasterDepot> masterDepots = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MasterDepot/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<MasterDepot>>();
                    readTask.Wait();
                    masterDepots = readTask.Result;
                }
                else
                {
                    masterDepots = new List<MasterDepot>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.MasterDepots = masterDepots;
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
            MasterDepot masterDepot = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("MasterDepot/GetById/" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<MasterDepot>();
                    readTask.Wait();
                    masterDepot = readTask.Result;
                }
                else
                {
                    masterDepot = new MasterDepot();
                    return RedirectToAction("Create", "MasterDepot");
                }
            }
            return View(masterDepot);
        }

        [HttpPost]
        public ActionResult Edit(MasterDepot aMasterDepot)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            if (aMasterDepot != null)
            {
                long userId = Convert.ToInt64(Session["UserId"]);
                aMasterDepot.ModifiedOn = DateTime.UtcNow.AddHours(6);
                aMasterDepot.ModifiedBy = userId;
                using (var client = new HttpClientDemo())
                {
                    string serailizeddto = JsonConvert.SerializeObject(aMasterDepot);
                    var inputMessage = new HttpRequestMessage
                    {
                        Content = new StringContent(serailizeddto, Encoding.UTF8, "application/json")
                    };
                    inputMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);

                    HttpResponseMessage message =
                        client.PostAsync("MasterDepot/Edit", inputMessage.Content).Result;

                    if (message.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Master depot updated successfully.";
                    }
                    else if (message.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "This email already exist.";
                    }
                    else
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "Sorry master depot cannot be added.";
                    }
                    return RedirectToAction("Index", "MasterDepot");
                }
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return RedirectToAction("Index", "MasterDepot");
        }

        //link master depot with thana
        public ActionResult LinkMasterDepotWithThana()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.MasterDepotId = new SelectList(Dropdown.MasterDepo(), "Id", "Name");
            ViewBag.DistrictId = Dropdown.AllActiveDistricts();
            ViewBag.ThanaId = Dropdown.AllActiveThanas();
            return View();
        }

        public JsonResult GetThanaByDistrictId(long districtId)
        {
            List<Thana> thanaList = Dropdown.GetAllThanaById((long)districtId);
            List<Thana> thanas = thanaList.Select(thana => new Thana
            {
                Id = thana.Id,
                Name = thana.Name
            }).ToList();

            return Json(thanas, JsonRequestBehavior.AllowGet);
        }

        //POST link master depot with thana
        [HttpPost]
        public ActionResult LinkMasterDepotWithThana(ThanaWiseMasterDepot thanaWiseMasterDepot)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.MasterDepotId = new SelectList(Dropdown.MasterDepo(), "Id", "Name");
            ViewBag.DistrictId = Dropdown.Districts();
            if (thanaWiseMasterDepot != null)
            {
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<ThanaWiseMasterDepot>("MasterDepot/LinkMasterDepotWithThana", thanaWiseMasterDepot);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Thana and master depot has been linked successfully.";
                        //FlashMessage.Queue($"Thana and master depot has been linked successfully.", "", FlashMessageType.Confirmation, false);
                        return RedirectToAction("Views", "MasterDepot");
                    }
                    else
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "This thana already linked with this master depot.";
                        return View();
                    }
                }
            }
            return View();
        }

        public ActionResult Views()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<ThanaWiseMasterDepot> thanaWiseMasterDepots = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MasterDepot/GetAllThanaWithMasterDepot");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ThanaWiseMasterDepot>>();
                    readTask.Wait();
                    thanaWiseMasterDepots = readTask.Result;
                }
                else
                {
                    thanaWiseMasterDepots = new List<ThanaWiseMasterDepot>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.Products = thanaWiseMasterDepots;
            return View(thanaWiseMasterDepots);
        }

        public ActionResult PendingOrders()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<Order> masterDepotOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetOrderByMasterdepot/" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    masterDepotOrders = readTask.Result;
                    masterDepotOrders = masterDepotOrders.OrderByDescending(c => c.OrderDate).ToList();
                }
                else
                {
                    masterDepotOrders = new List<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.RejectStatus = Dropdown.OrderRejectStatus();
            ViewBag.Products = masterDepotOrders;
            return View((List<Order>)masterDepotOrders);
        }

        //order details by order id
        public JsonResult OrderDetails(long id)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            IList<OrderDetail> orderDetails = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Order/GetOrderDetailsByOderId");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetOrderDetailsByOderId?id=" + id);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<OrderDetail>>();
                    readTask.Wait();
                    orderDetails = readTask.Result;
                }
                else
                {
                    orderDetails = (IList<OrderDetail>)Enumerable.Empty<OrderDetail>(); ;
                }
            }
            return Json(orderDetails, JsonRequestBehavior.AllowGet);
        }

        //Accepted orders
        public ActionResult SavedOrders()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<Order> masterDepotOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Order/GetSavedOrders");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                //var responseTask = client.GetAsync("Order/GetSavedOrders/" + userId);
                //var responseTask = client.GetAsync("Order/GetAllOrders/");
                var responseTask = client.GetAsync("Order/GetOrderByMasterdepot/" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    masterDepotOrders = readTask.Result;
                    masterDepotOrders = masterDepotOrders.Where(c => c.OrderStateId == (long)OrderStatusEnum.Confirm).OrderByDescending(c => c.OrderDate)
                        .ToList();
                }
                else
                {
                    masterDepotOrders = new List<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.ProductStatus = Dropdown.OrderStates();
            //ViewBag.Products = masterDepotOrders;
            return View((List<Order>)masterDepotOrders);
        }
        //Confirm orders
        public ActionResult ConfirmedOrders()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<Order> masterDepotOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Order/GetSavedOrders");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetConfirmedOrders/" + userId);
                //var responseTask = client.GetAsync("Order/GetAllOrders/");
                //var responseTask = client.GetAsync("Order/GetOrderByMasterdepot/" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    masterDepotOrders = readTask.Result;
                    //masterDepotOrders = masterDepotOrders.Where(c => c.OrderStateId == (long)OrderStatusEnum.Confirm).OrderByDescending(c => c.OrderDate).ToList();
                }
                else
                {
                    masterDepotOrders = new List<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.ProductStatus = Dropdown.OrderStates();
            //ViewBag.Products = masterDepotOrders;
            return View((List<Order>)masterDepotOrders);
        }

        //Accepted orders
        public ActionResult ReceivedOrders()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<Order> masterDepotOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetReceivedOrders/" + userId);
                //var responseTask = client.GetAsync("Order/GetOrderByMasterdepot/" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    masterDepotOrders = readTask.Result;
                    masterDepotOrders = masterDepotOrders.Where(c => c.OrderStateId == (long)OrderStatusEnum.Received).OrderByDescending(c => c.OrderDate)
                        .ToList();
                }
                else
                {
                    masterDepotOrders = new List<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.ProductStatus = Dropdown.OrderStates();
            return View((List<Order>)masterDepotOrders);
        }

        //Rejected orders
        public ActionResult RejectedOrders()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<Order> masterDepotOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetRejectedOrders/" + userId);
                //var responseTask = client.GetAsync("Order/GetOrderByMasterdepot/" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    masterDepotOrders = readTask.Result;
                    masterDepotOrders = masterDepotOrders.Where(c=>c.OrderStateId == (long)OrderStatusEnum.Rejected).OrderByDescending(c => c.OrderDate)
                        .ToList();
                }
                else
                {
                    masterDepotOrders = new List<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.ProductStatus = Dropdown.OrderStates();
            return View((List<Order>)masterDepotOrders);
        }

        public ActionResult SaveOrRejectOrder(Order orders, long? orderRejectId)
        {
            if (orders == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Order is null.";
                return RedirectToAction("PendingOrders", "MasterDepot");
            }
            var userId = orders.UserId;
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            Order order = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetDetails?id=" + orders.Id + "&userId=" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Order>();
                    readTask.Wait();
                    order = readTask.Result;
                }
                else
                {
                    order = new Order();
                }
            }
            if (orderRejectId != -1)
            {
                order.OrderRejectId = orderRejectId;
                order.OrderStateId = (long)OrderStatusEnum.Rejected;
            }
            else
            {
                order.OrderStateId = (long)OrderStatusEnum.Confirm;
                order.OrderDetails = orders.OrderDetails;
            }
            order.OrderStatusChangedBy = Convert.ToInt64(Session["UserId"]);
            //update order status
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.PostAsJsonAsync<Order>("Order/UpdateOrderDetails", order);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = "Order state changed..";
                    return RedirectToAction("PendingOrders");
                }
            }
            return RedirectToAction("PendingOrders");
            //return PendingOrders();
        }


        public ActionResult Delivering()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<Order> masterDepotOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetOrderByMasterdepot/" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    masterDepotOrders = readTask.Result;
                    masterDepotOrders = masterDepotOrders.Where(c => c.OrderStateId == (long)OrderStatusEnum.Delivered).OrderByDescending(c => c.OrderDate)
                        .ToList();
                }
                else
                {
                    masterDepotOrders = new List<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.ProductStatus = Dropdown.OrderStates();
            return View((List<Order>)masterDepotOrders);
        }
        //delivered Orders
        public ActionResult DeliveredOrders()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<Order> masterDepotOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetDeliveredOrders/" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    masterDepotOrders = readTask.Result;
                    masterDepotOrders = masterDepotOrders.Where(c => c.OrderStateId == (long)OrderStatusEnum.Delivered).OrderByDescending(c => c.OrderDate)
                        .ToList();
                }
                else
                {
                    masterDepotOrders = new List<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.ProductStatus = Dropdown.OrderStates();
            //ViewBag.Products = masterDepotOrders;
            return View((List<Order>)masterDepotOrders);
        }

        //On Processing orders
        public ActionResult OnProcessingOrders()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<Order> masterDepotOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetOnProcessingOrders/" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    masterDepotOrders = readTask.Result;
                    masterDepotOrders = masterDepotOrders.Where(c => c.OrderStateId == (long)OrderStatusEnum.OnProcessing).OrderByDescending(c => c.OrderDate)
                        .ToList();
                }
                else
                {
                    masterDepotOrders = new List<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.ProductStatus = Dropdown.OrderStates();
            return View((List<Order>)masterDepotOrders);
        }
        //Shipped orders
        public ActionResult ShippedOrders()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<Order> masterDepotOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetShippedOrders/" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    masterDepotOrders = readTask.Result;
                    masterDepotOrders = masterDepotOrders.Where(c => c.OrderStateId == (long)OrderStatusEnum.Shipped).OrderByDescending(c => c.OrderDate)
                        .ToList();
                }
                else
                {
                    masterDepotOrders = new List<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.ProductStatus = Dropdown.OrderStates();
            return View((List<Order>)masterDepotOrders);
        }

        public ActionResult UpdateOrderStatus(List<Order> orders, long? OrderStateId)
        {
            if (Request.UrlReferrer != null)
            {
                Session["requesturl"] = Request.UrlReferrer.ToString();
            }
            int count = 0;
            if (orders == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Order is null.";
                return RedirectToAction("ConfirmedOrders", "MasterDepot");
            }
            orders = orders.Where(c => c.IsCheck).ToList();
            if (orders.Count == 0)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Please select order.";
                string returnUrl = (string)Session["requesturl"];
                return Redirect(returnUrl);
            }
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            Order order = null;
            foreach (var pendingOrder in orders)
            {

                var userId = pendingOrder.UserId;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var responseTask = client.GetAsync("Order/GetDetails?id=" + pendingOrder.Id);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<Order>();
                        readTask.Wait();
                        order = readTask.Result;
                    }
                    else
                    {
                        order = new Order();
                    }
                }
                order.OrderStateId = OrderStateId; 
                order.OrderStatusChangedBy = Convert.ToInt64(Session["UserId"]);
                //update order status
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var responseTask = client.PostAsJsonAsync<Order>("Order/UpdateOrderStatus", order);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        count++;
                    }
                }


            }
            if (count > 0)
            {
                TempData["Class"] = UtilityClass.Success;
                TempData["Message"] = "Order status changed..";
                if (OrderStateId == (long)OrderStatusEnum.Confirm)
                {
                    return RedirectToAction("ConfirmedOrders");
                }
                else if (OrderStateId == (long)OrderStatusEnum.OnProcessing)
                {
                    return RedirectToAction("OnProcessingOrders");
                }
                else if (OrderStateId == (long)OrderStatusEnum.Shipped)
                {
                    return RedirectToAction("ShippedOrders");
                }
                else if (OrderStateId == (long)OrderStatusEnum.Delivered)
                {
                    return RedirectToAction("DeliveredOrders");
                }

                else if (OrderStateId == (long)OrderStatusEnum.Received)
                {
                    return RedirectToAction("ReceivedOrders");
                }
                else if (OrderStateId == (long)OrderStatusEnum.Rejected)
                {
                    return RedirectToAction("RejectedOrders");
                }
                return RedirectToAction("ConfirmedOrders");
            }


            TempData["Class"] = UtilityClass.Error;
            TempData["Message"] = "Order status changing failed..";
            if (OrderStateId == (long)OrderStatusEnum.Received)
            {
                return RedirectToAction("ConfirmedOrders");
            }
            return RedirectToAction("ConfirmedOrders");
        }
        public ActionResult DeleteProductFromPendingOrder(OrderDetail orderDetail)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (orderDetail != null)
            {
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<OrderDetail>("MasterDepot/DeleteProductFromPendingOrder", orderDetail);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return Json(new { status = "Ok" });
                    }
                }
            }
            return Json(new { status = "Failed" });
        }

        public JsonResult GetMasterDepotDetailsById(long masterDepotId)
        {
            MasterDepot masterDepot = new MasterDepot();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MasterDepot/GetById?id=" + masterDepotId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<MasterDepot>();
                    readTask.Wait();
                    masterDepot = readTask.Result;
                    return Json(new { Status = "Ok", MasterDepot = masterDepot });
                }
            }

            return Json(new { Status = "Failed" });
        }

        public ActionResult AssignDeliveryMan()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<Order> masterDepotOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetOnProcessingOrders/" + userId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Order>>();
                    readTask.Wait();
                    masterDepotOrders = readTask.Result;
                    masterDepotOrders =
                        masterDepotOrders.Where(c => c.AssignOrders.Count == 0).OrderByDescending(c => c.OrderDate).ToList();
                    //    .ToList();
                }
                else
                {
                    masterDepotOrders = new List<Order>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            //ViewBag.ProductStatus = Dropdown.OrderStates();
            ViewBag.DeliveryMen = Dropdown.DeliveryMenByMasterDepotId(userId);
            return View((List<Order>)masterDepotOrders);
        }

        [HttpPost]
        public ActionResult AssignOrder(AssignOrder assignOrder)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            long userTypeId = Convert.ToInt64(Session["UserTypeId"]);
            if (userTypeId != Convert.ToInt64(UserTypeEnum.MasterDepotUser))
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Sorry you cannot assign orders to any delivery man";
                return RedirectToAction("AssignDeliveryMan");
            }

            if (assignOrder != null)
            {
                long userId = Convert.ToInt64(Session["UserId"]);
                assignOrder.AssignedOn = DateTime.Now;
                assignOrder.AssignedBy = userId;
                using (var client = new HttpClientDemo())
                {

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<AssignOrder>("AssignOrder/Create", assignOrder);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Order assigned successfully.";
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "This order is already assigned.";
                    }
                    return Json(true,JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("AssignDeliveryMan","MasterDepot");
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("AssignDeliveryMan", "MasterDepot");
        }

        [HttpPost]
        public ActionResult ChangeAssignedDeliveryMan(AssignOrder assignOrder)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            long userTypeId = Convert.ToInt64(Session["UserTypeId"]);
            if (userTypeId != Convert.ToInt64(UserTypeEnum.MasterDepotUser))
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Sorry you cannot assign orders to any delivery man";
                return RedirectToAction("AssignDeliveryMan");
            }

            if (assignOrder != null)
            {
                long userId = Convert.ToInt64(Session["UserId"]);
                assignOrder.ModifiedOn = DateTime.Now;
                assignOrder.ModifiedBy = userId;
                using (var client = new HttpClientDemo())
                {

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<AssignOrder>("AssignOrder/Edit", assignOrder);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Order assigned to new delivery man successfully.";
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("AssignDeliveryMan","MasterDepot");
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("AssignDeliveryMan", "MasterDepot");
        }

        public ActionResult ViewAssignedOrders()
        {

            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }

            IEnumerable<AssignOrder> assignOrders = null;
            long userId = Convert.ToInt64(Session["UserId"]);
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("AssignOrder/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<AssignOrder>>();
                    readTask.Wait();
                    assignOrders = readTask.Result;
                   
                }
                else
                {
                    assignOrders = new List<AssignOrder>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.DeliveryMen = Dropdown.DeliveryMenByMasterDepotId(userId); ;
            
            return View(assignOrders);
        }

        public JsonResult GeDeliveryManDetailsByOrderId(long orderId)
        {
            DeliveryMan deliveryMan = new DeliveryMan();
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("AssignOrder/GetDeliveryManByOrderId?orderId=" + orderId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<DeliveryMan>();
                    readTask.Wait();
                    deliveryMan = readTask.Result;
                    return Json(new { Status = "Ok", MasterDepot = deliveryMan });
                }
            }

            return Json(new { Status = "Failed" });
        }

    }
}