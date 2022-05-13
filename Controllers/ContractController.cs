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
using EFreshAdmin.Utility;
using EFreshStore.Models.Context;
using EFreshStore.Utility;

namespace EFreshAdmin.Controllers
{
    public class ContractController : Controller
    {

        public ActionResult CorporateUser()
        {
            ViewBag.Contacts = Dropdown.Contracts();
            return View();
        }

        [HttpPost]
        public ActionResult CorporateUser(long id)
        {
            ViewBag.Contacts = Dropdown.Contracts();
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            IEnumerable<CorporateUser> corporateUsers = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Category/GetAll");
                var responseTask = client.GetAsync("Contract/GetByCorporateId/" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CorporateUser>>();
                    readTask.Wait();
                    corporateUsers = readTask.Result;
                }
                else
                {
                    corporateUsers = new List<CorporateUser>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.CorporateUsers = corporateUsers;
            return View(corporateUsers);
        }
        public ActionResult Create()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(CorporateContract aContract)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (aContract != null)
            {
                aContract.CreatedOn = DateTime.Now;
                aContract.IsDeleted = false;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<CorporateContract>("Contract/Add", aContract);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Corporate contract created successfully.";
                        return RedirectToAction("Views", "Contract");
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "Contract with this company already exist.";
                        return View();
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return View(aContract);
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
            List<CorporateContract> corporateContracts = Dropdown.Contracts();

            ViewBag.contract = corporateContracts;
            return View(corporateContracts);
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

            CorporateContract contract = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Contract/GetById/" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<CorporateContract>();
                    readTask.Wait();
                    contract = readTask.Result;
                }
                else
                {
                    TempData["Class"] = UtilityClass.Error;
                    TempData["Message"] = "Corporate contract not found.";
                }
            }
            return View(contract);
        }

        [HttpPost]
        public ActionResult Edit(CorporateContract aContract)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            if (aContract != null)
            {
                aContract.ModifiedOn = DateTime.Now;
                aContract.IsDeleted = false;
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var putTask = client.PostAsJsonAsync<CorporateContract>("Contract/Edit", aContract);

                    putTask.Wait();
                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Corporate contract updated successfully.";
                        return RedirectToAction("Views", "Contract");
                    }
                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["Class"] = UtilityClass.Error;
                        TempData["Message"] = "Contract with this company already exist.";
                    }
                    ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                }
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
            ViewBag.Contracts = Dropdown.Contracts();
            ViewBag.Departments = Dropdown.CorporateDepartments();
            ViewBag.Deisgnations = Dropdown.CorporateDesignations();
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(CorporateUser aUser)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Contracts = Dropdown.Contracts();
            ViewBag.Departments = Dropdown.CorporateDepartments();
            ViewBag.Deisgnations = Dropdown.CorporateDesignations();
            aUser.CreatedOn = DateTime.UtcNow.AddHours(6);
            aUser.CreatedBy = (long)Session["UserId"];
            aUser.IsActive = true;
            aUser.IsDeleted = false;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var client = new HttpClientDemo())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                        var postTask = client.PostAsJsonAsync<CorporateUser>("Contract/AddUser", aUser);
                        postTask.Wait();
                        ModelState.Clear();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Class"] = UtilityClass.Success;
                            TempData["Message"] = "Corporate user created successfully.";
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
            ModelState.Clear();
            
            return View();
        }

        //Bulk corporate User
        public ActionResult AddBulkUser()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Contracts = Dropdown.Contracts();
            return View();
        }

        [HttpPost]
        public ActionResult AddBulkUser(long CorporateContractId)
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Contracts = Dropdown.Contracts();
            List<CorporateUser> savedUsers = new List<CorporateUser>();
            List<CorporateUser> duplicateEmails = new List<CorporateUser>();
            List<CorporateUser> invalidMobileNos = new List<CorporateUser>();
            if (Request.Files["ecxelFile"].ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["ecxelFile"].FileName).ToLower();
                string query = null;
                string connString = "";
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
                        var isExistColumn = UtilityClass.IsAllColumnExist(dt, UtilityClass.CorporateUserProperty());
                        if (!isExistColumn)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Some columns are missing.";
                            return View();
                        }
                        var corporateUsers = GetCorporateUsersFromUploadedFile(CorporateContractId, dt);
                        savedUsers = CheckInvalidDataAndAddCorporateBulkUsers(corporateUsers, filePath, token, savedUsers, duplicateEmails, invalidMobileNos);
                    }
                    else if (extension.Trim() == ".xls")
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath +
                                     ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = UtilityClass.ConvertXLSXtoDataTable(filePath, connString);
                        ViewBag.Data = dt;
                        var isExistColumn = UtilityClass.IsAllColumnExist(dt, UtilityClass.CorporateUserProperty());
                        if (!isExistColumn)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Some columns are missing.";
                            return View();
                        }
                        var corporateUsers = GetCorporateUsersFromUploadedFile(CorporateContractId, dt);
                        savedUsers = CheckInvalidDataAndAddCorporateBulkUsers(corporateUsers, filePath, token, savedUsers, duplicateEmails, invalidMobileNos);
                    }
                    else if (extension.Trim() == ".xlsx")
                    {
                        connString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + filePath +
                                     "';Extended Properties='Excel 12.0;IMEX=1'";
                        DataTable dt = UtilityClass.ConvertXLSXtoDataTable(filePath, connString);
                        var isExistColumn = UtilityClass.IsAllColumnExist(dt, UtilityClass.CorporateUserProperty());
                        if (!isExistColumn)
                        {
                            TempData["Class"] = UtilityClass.Error;
                            TempData["Message"] = "Some columns are missing.";
                            return View();
                        }
                        var corporateUsers = GetCorporateUsersFromUploadedFile(CorporateContractId, dt);
                        savedUsers = CheckInvalidDataAndAddCorporateBulkUsers(corporateUsers, filePath, token, savedUsers, duplicateEmails, invalidMobileNos);
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
                    TempData["Message"] = savedUsers.Count + " corporate user(s) added. " + (invalidMobileNos.Count + duplicateEmails.Count) + " failed.";
                    return View();
                }
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = " No user added. ";
                return View();
            }
            return View();
        }

        [HttpGet]
        public ActionResult ViewUser()
        {
            LoginCredentials token = (LoginCredentials)Session["userToken"];
            if (token == null)
            {
                TempData["Class"] = UtilityClass.Error;
                TempData["Message"] = "Access Denied.";
                return RedirectToAction("Login", "Home");
            }
            List<CorporateUser> corporateUsers;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Contract/GetAll");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Contract/GetAll");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CorporateUser>>();
                    readTask.Wait();
                    corporateUsers = (List<CorporateUser>)readTask.Result;
                }
                else
                {
                    corporateUsers = new List<CorporateUser>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
                ViewBag.contract = corporateUsers;
                return View();
            }
        }

        public ActionResult ChangePassword()
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
        public ActionResult ChangePassword(string password)
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
                long userId = Convert.ToInt64(Session["UserId"]);
                User user = new User();
                user.Id = userId;
                user.Password = password;
                using (var client = new HttpClientDemo())
                {
                    //client.BaseAddress = new Uri(BaseUrl.url + "User/ChangePassword");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<User>("User/ChangePassword", user);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Class"] = UtilityClass.Success;
                        TempData["Message"] = "Password changed successfully.";
                        return RedirectToAction("ViewUser", "Contract");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }
            return View();
        }

        private List<CorporateUser> GetCorporateUsersFromUploadedFile(long corporateContractId, DataTable dt)
        {
            List<CorporateUser> corporateUsers = (from DataRow row in dt.Rows
                                                  select new CorporateUser
                                                  {
                                                      EmployeeId = row["EmployeeId"].ToString(),
                                                      CorporateContractId = corporateContractId,
                                                      Name = row["Name"].ToString(),
                                                      MobileNo = row["MobileNo"].ToString().Length == 10 && !row["MobileNo"].ToString().StartsWith("0")
                                                          ? "0" + row["MobileNo"].ToString()
                                                          : row["MobileNo"].ToString(),
                                                      Email = row["Email"].ToString(),
                                                      Designation = row["Designation"].ToString(),
                                                      DeliveryAddress1 = row["DeliveryAddress1"].ToString(),
                                                      DeliveryAddress2 = row["DeliveryAddress2"].ToString(),
                                                      CorporateDepartmentId = row["DepartmentId"].ToString() != "" ? Convert.ToInt64(row["DepartmentId"]) : (long?)null,
                                                      CorporateDesignationId = row["DesignationId"].ToString() != "" ? Convert.ToInt64(row["DesignationId"]) : (long?)null,
                                                      IsActive = true,
                                                      IsDeleted = false,
                                                      CreatedOn = DateTime.Now
                                                  }).ToList();
            return corporateUsers;
        }
        private List<CorporateUser> CheckInvalidDataAndAddCorporateBulkUsers(List<CorporateUser> corporateUsers, string filePath, LoginCredentials token, List<CorporateUser> savedUsers,
            List<CorporateUser> duplicateEmails, List<CorporateUser> invalidMobileNos)
        {
            invalidMobileNos = corporateUsers.FindAll(c => c.MobileNo.Length < 10);
            corporateUsers.RemoveAll(c => c.MobileNo.Length < 10);


            List<CorporateUser> invalidDepartment = CheckInvalidDepartment(corporateUsers);
            corporateUsers = corporateUsers.Except(invalidDepartment).ToList();

            List<CorporateUser> invalidDesignations = CheckInvalidDesignation(corporateUsers);
            corporateUsers = corporateUsers.Except(invalidDesignations).ToList();

            System.IO.File.Delete(filePath);
            int count = 0;
            foreach (var user in corporateUsers)
            {
                using (var client = new HttpClientDemo())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                    var postTask = client.PostAsJsonAsync<CorporateUser>("Contract/AddUser", user);
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
        private List<CorporateUser> CheckInvalidDesignation(List<CorporateUser> corporateUsers)
        {
            var allCorporateDesignation = Dropdown.CorporateDesignations().ToList();
            var corporateUserWithDesignations = corporateUsers.Where(c => c.CorporateDesignationId != null).ToList();
            var invalidDesignations = corporateUserWithDesignations.Where(x => allCorporateDesignation.All(y => y.Id != x.CorporateDesignationId)).ToList();
            return invalidDesignations;
        }
        private List<CorporateUser> CheckInvalidDepartment(List<CorporateUser> corporateUsers)
        {
            var allCorporateDept = Dropdown.CorporateDepartments().ToList();
            var corporateUserWithDept = corporateUsers.Where(c => c.CorporateDepartmentId != null).ToList();
            var invalidDept = corporateUserWithDept.Where(x => allCorporateDept.All(y => y.Id != x.CorporateDepartmentId)).ToList();
            return invalidDept;
        }
    }
}