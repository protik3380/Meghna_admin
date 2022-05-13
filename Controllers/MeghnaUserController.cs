using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    public class MeghnaUserController : Controller
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
            IEnumerable<MeghnaUser> meghnaUsers = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Category/GetAll");
                var responseTask = client.GetAsync("MeghnaUser/GetAll");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<MeghnaUser>>();
                    readTask.Wait();
                    meghnaUsers = readTask.Result;
                }
                else
                {
                    meghnaUsers = new List<MeghnaUser>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.meghnaUser = meghnaUsers;
            return View(meghnaUsers);
        }

        //Add Bulk Meghna User
        public ActionResult AddMeghnaUser()
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

        public ActionResult AddUser()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Departments = Dropdown.MeghnaDepartments();
            ViewBag.Deisgnations = Dropdown.MeghnaDesignations();
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(MeghnaUser aMeghnaUser)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Departments = Dropdown.MeghnaDepartments();
            ViewBag.Deisgnations = Dropdown.MeghnaDesignations();
            aMeghnaUser.CreatedOn = DateTime.UtcNow.AddHours(6);
            aMeghnaUser.IsActive = true;
            aMeghnaUser.IsDeleted = false;
            aMeghnaUser.CreatedBy = (long)Session["UserId"];
            try
            {
                if (ModelState.IsValid)
                {
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync<MeghnaUser>("MeghnaUser/AddMeghnaUser", aMeghnaUser);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "MGI user created successfully.";
                        }
                        else
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Error! Something went wrong.";
                        }
                        if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "This email is already exist.";
                            return View();
                        }
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = ex.Message;
            }
            return View();
        }
        [ActionName("AddMeghnaUser")]
        [HttpPost]
        public ActionResult AddBulkMeghnaUser()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            List<MeghnaUser> savedUsers = new List<MeghnaUser>();
            List<MeghnaUser> duplicateEmails = new List<MeghnaUser>();
            List<MeghnaUser> invalidMobileNos = new List<MeghnaUser>();

            if (Request.Files["ecxelFile"].ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["ecxelFile"].FileName).ToLower();
                string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                string filePath = string.Format("{0}/{1}", Server.MapPath("~/Content"), Request.Files["ecxelFile"].FileName);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content"));
                }
                if (validFileTypes.Contains(extension))
                {
                    Request.Files["ecxelFile"].SaveAs(filePath);
                    if (extension == ".csv")
                    {
                        DataTable dt = UtilityClass.ConvertCSVtoDataTable(filePath);
                        ViewBag.Data = dt;
                        var isExistColumn = UtilityClass.IsAllColumnExist(dt, UtilityClass.MeghnaUserProperty());
                        if (!isExistColumn)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Some columns are missing.";
                            return View();
                        }
                        var meghnaUsers = GetMeghnaUsersFromUploadedFile(dt);
                        savedUsers = CheckInvalidDataAndAddMeghnaBulkUsers(meghnaUsers, filePath, token, savedUsers, duplicateEmails, invalidMobileNos);
                    }

                    else
                    {
                        var connString = "";
                        if (extension.Trim() == ".xls")
                        {
                            connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath +
                                         ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            DataTable dt = UtilityClass.ConvertXLSXtoDataTable(filePath, connString);
                            ViewBag.Data = dt;
                            List<string> meghnaUserColumnList = UtilityClass.MeghnaUserProperty();
                            string columnNames = null;
                            foreach (var column in meghnaUserColumnList)
                            {
                                columnNames += column + ", ";
                            }
                            var isExistColumn = UtilityClass.IsAllColumnExist(dt, UtilityClass.MeghnaUserProperty());
                            if (!isExistColumn)
                            {
                                TempData["Class"] = UtilityClass.Error;
                                TempData["Message"] = "Some columns are missing.";
                                return View();
                            }
                            var meghnaUsers = GetMeghnaUsersFromUploadedFile(dt);
                            savedUsers = CheckInvalidDataAndAddMeghnaBulkUsers(meghnaUsers, filePath, token, savedUsers, duplicateEmails, invalidMobileNos);
                        }
                        else if (extension.Trim() == ".xlsx")
                        {
                            connString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + filePath +
                                         "';Extended Properties='Excel 12.0;IMEX=1'";
                            DataTable dt = UtilityClass.ConvertXLSXtoDataTable(filePath, connString);
                            var isExistColumn = UtilityClass.IsAllColumnExist(dt, UtilityClass.MeghnaUserProperty());
                            if (!isExistColumn)
                            {
                                TempData["Class"] = UtilityClass.Error;
                                TempData["Message"] = "Some columns are missing.";
                                return View();
                            }
                            var meghnaUsers = GetMeghnaUsersFromUploadedFile(dt);
                            savedUsers = CheckInvalidDataAndAddMeghnaBulkUsers(meghnaUsers, filePath, token, savedUsers, duplicateEmails, invalidMobileNos);
                        }
                    }
                }
                else
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Please upload files in .xls, .xlsx or .csv format.";
                }
                if (savedUsers.Count >= 0)
                {
                    TempData["Class"] = UtilityClass.Success;
                    TempData["Message"] = savedUsers.Count + " meghna user(s) added. " + (invalidMobileNos.Count + duplicateEmails.Count) + " failed.";
                    return View();
                }
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "No user added.";
                return View();
            }
            return View();
        }



        //Duscount
        public ActionResult MeghnaUserDiscount()
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
        public ActionResult MeghnaUserDiscount(UserDiscount userDiscount)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            try
            {
                if (userDiscount != null)
                {
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                            token.AccessToken);
                        var postTask = client.PostAsJsonAsync<UserDiscount>("MeghnaUser/AddDiscount", userDiscount);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Meghna user discount added successfully.";
                            return RedirectToAction("GetAllDiscounts", "MeghnaUser");
                        }
                        else
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Error! Something went wrong.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
            return View();
        }

        public JsonResult GetMeghnaUserDiscount()
        {
            UserDiscount userDiscount = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MeghnaUser/GetMeghnaUserDiscount");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<UserDiscount>();
                    readTask.Wait();
                    userDiscount = readTask.Result;
                }
                else
                {
                    userDiscount = new UserDiscount();
                }
            }
            return Json(userDiscount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllDiscounts()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<UserDiscount> userDiscounts = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MeghnaUser/GetAllMeghnaUserDiscounts");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<UserDiscount>>();
                    readTask.Wait();
                    userDiscounts = readTask.Result;
                }
                else
                {
                    userDiscounts = new List<UserDiscount>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(userDiscounts);
        }

        private List<MeghnaUser> GetMeghnaUsersFromUploadedFile(DataTable dt)
        {
            List<MeghnaUser> meghnaUsers = (from DataRow row in dt.Rows
                                            select new MeghnaUser
                                            {
                                                EmployeeId = row["EmployeeId"].ToString(),
                                                Name = row["Name"].ToString(),
                                                MobileNo = row["MobileNo"].ToString().Length == 10 && !row["MobileNo"].ToString().StartsWith("0")
                                                    ? "0" + row["MobileNo"].ToString()
                                                    : row["MobileNo"].ToString(),
                                                Email = row["Email"].ToString(),
                                                Designation = row["Designation"].ToString(),
                                                DeliveryAddress1 = row["DeliveryAddress1"].ToString(),
                                                DeliveryAddress2 = row["DeliveryAddress2"].ToString(),
                                                MeghnaDepartmentId = row["DepartmentId"].ToString() != "" ? Convert.ToInt64(row["DepartmentId"]) : (long?)null,
                                                MeghnaDesignationId = row["DesignationId"].ToString() != "" ? Convert.ToInt64(row["DesignationId"]) : (long?)null,
                                                IsActive = true,
                                                IsDeleted = false,
                                                CreatedOn = DateTime.Now
                                            }).ToList();
            return meghnaUsers;
        }

        private List<MeghnaUser> CheckInvalidDataAndAddMeghnaBulkUsers(List<MeghnaUser> meghnaUsers, string filePath, LoginCredentials token,
            List<MeghnaUser> savedUsers, List<MeghnaUser> duplicateEmails, List<MeghnaUser> invalidMobileNos)
        {
            invalidMobileNos = meghnaUsers.FindAll(c => c.MobileNo.Length < 10);
            meghnaUsers.RemoveAll(c => c.MobileNo.Length < 10);

            List<MeghnaUser> invalidDepartment = CheckInvalidDepartment(meghnaUsers);           
            meghnaUsers = meghnaUsers.Except(invalidDepartment).ToList();

            List<MeghnaUser> invalidDesignations = CheckInvalidDesignation(meghnaUsers);
            meghnaUsers = meghnaUsers.Except(invalidDesignations).ToList();

            System.IO.File.Delete(filePath);
            int count = 0;
            foreach (var user in meghnaUsers)
            {
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<MeghnaUser>("MeghnaUser/AddMeghnaUser", user);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        savedUsers.Add(user);
                        count += 1;
                    }

                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        duplicateEmails.Add(user);
                    }
                }
            }

            ViewBag.Count = count;
            ViewBag.SavedUser = savedUsers;
            ViewBag.DuplicateEmails = duplicateEmails;
            ViewBag.InvalidMobileNos = invalidMobileNos;
            ViewBag.InvalidDepartments = invalidDepartment;
            ViewBag.InvalidDesignations = invalidDesignations;
            return savedUsers;
        }

        private List<MeghnaUser> CheckInvalidDesignation(List<MeghnaUser> meghnaUsers)
        {
            var allMeghnaDesignation = Dropdown.MeghnaDesignations().ToList();
            var meghnaUserWithDesignations = meghnaUsers.Where(c => c.MeghnaDesignationId != null).ToList();
            var invalidDesignations = meghnaUserWithDesignations.Where(x => allMeghnaDesignation.All(y => y.Id != x.MeghnaDesignationId)).ToList();
            return invalidDesignations;
        }

        private List<MeghnaUser> CheckInvalidDepartment(List<MeghnaUser> meghnaUsers)
        {
            var allMeghnaDept = Dropdown.MeghnaDepartments().ToList();
            var meghnaUserWithDept = meghnaUsers.Where(c => c.MeghnaDepartmentId != null).ToList();
            var invalidDept = meghnaUserWithDept.Where(x => allMeghnaDept.All(y => y.Id != x.MeghnaDepartmentId)).ToList();
            return invalidDept;
        }
    }
}