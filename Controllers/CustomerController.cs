using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using EFreshAdmin.Models;
using EFreshAdmin.Models.EntityModels;
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using Vereyon.Web;

namespace EFreshAdmin.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult Create()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                FlashMessage.Queue($"Access Denied.", "Warning: ", FlashMessageType.Warning, false);
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer aCustomer)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                FlashMessage.Queue($"Access Denied.", "Warning: ", FlashMessageType.Warning, false);
                return RedirectToAction("Login", "Home");
            }

            try
            {
                if (aCustomer != null)
                {
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync<Customer>("Customer/Add", aCustomer);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            FlashMessage.Queue($"Customer created successfully.", "", FlashMessageType.Confirmation, false);
                        }
                        else
                        {
                            FlashMessage.Queue($"This email is already registered.", "Warning: ", FlashMessageType.Warning, false);
                        }
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }
            return View(aCustomer);
        }

    }
}
