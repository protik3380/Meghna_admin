using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using EFreshAdmin.Models;
using EFreshAdmin.Models.EntityModels;
using EFreshStore.Models.Context;

namespace EFreshAdmin.Utility
{

    public static class ApiUtility
    {
        public static User ValidateUser(string email, string password)
        {
            User user = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "User/Login");
                var responseTask = client.GetAsync("User/Login?email=" + email + "&password=" + password);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<User>();
                    readTask.Wait();
                    user = readTask.Result;
                }
                else
                {
                    user = null;
                }
            }
            return user;
        }

        //token validation
        public static LoginCredentials ValidateUserToken(string email, string password)
        {
            using (var client = new HttpClientDemo())
            {
                var userCredentials = new Dictionary<string, string>
                    {
                        {"grant_type", "password"},
                        {"username", email},
                        {"password", password}
                    };
                var tokenResponse = client.PostAsync(BaseUrl.url+ "token", new FormUrlEncodedContent(userCredentials)).Result;
                //var token = tokenResponse.Content.ReadAsStringAsync().Result;  
                var token = tokenResponse.Content.ReadAsAsync<LoginCredentials>(new[] { new JsonMediaTypeFormatter() }).Result;
                if (string.IsNullOrEmpty(token.Error))
                {
                    //FlashMessage.Confirmation("Login successful");
                    return token;
                }
                else
                {
                    //FlashMessage.Warning("Error :   ", token.Error);
                    return token;
                }
                //Console.Read();
            }
        }


        public static User GetByUserEmail(string email)
        {
            User user = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("User/GetByUserEmail?email=" + email);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<User>();
                    readTask.Wait();
                    user = readTask.Result;
                }
                else
                {
                    user = null;
                }
            }

            return user;
        }

        public static List<MeghnaDepartment> GetAllMeghnaDepartments()
        {
            IEnumerable<MeghnaDepartment> meghnaDepartments = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MeghnaDepartment/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<MeghnaDepartment>>();
                    readTask.Wait();
                    meghnaDepartments = readTask.Result;
                }
                else
                {
                    meghnaDepartments = new List<MeghnaDepartment>();
                }
            }
            return meghnaDepartments.ToList();
        }

        public static List<MeghnaDesignation> GetAllMeghnaDesignations()
        {
            IEnumerable<MeghnaDesignation> meghnaDesignations = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MeghnaDesignation/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<MeghnaDesignation>>();
                    readTask.Wait();
                    meghnaDesignations = readTask.Result;
                }
                else
                {
                    meghnaDesignations = new List<MeghnaDesignation>();
                }
            }
            return meghnaDesignations.ToList();
        }

        public static List<CorporateDepartment> GetAllCorporateDepartments()
        {
            IEnumerable<CorporateDepartment> corporateDepartments = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("CorporateDepartment/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CorporateDepartment>>();
                    readTask.Wait();
                    corporateDepartments = readTask.Result;
                }
                else
                {
                    corporateDepartments = new List<CorporateDepartment>();
                }
            }
            return corporateDepartments.ToList();
        }

        public static List<CorporateDesignation> GetAllCorporateDesignations()
        {
            IEnumerable<CorporateDesignation> corporateDesignations = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("CorporateDesignation/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CorporateDesignation>>();
                    readTask.Wait();
                    corporateDesignations = readTask.Result;
                }
                else
                {
                    corporateDesignations = new List<CorporateDesignation>();
                }
            }
            return corporateDesignations.ToList();
        }
        public static int CountTotalMasterDepot()
        {
            int totalBrand = 0;
           
            using (var client = new HttpClientDemo())
            {
             
                var responseTask = client.GetAsync("MasterDepot/CountTotalMasterDepot");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();
                    totalBrand = readTask.Result;

                }
                else
                {
                    totalBrand = 0;
                }
            }
            return totalBrand;
        }
        public static int CountTotalCorporateUser()
        { 
            int totalCorporateUser = 0;
      
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Contract/CountTotalCorporateUser");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();
                    totalCorporateUser = readTask.Result;

                }
                else
                {
                    totalCorporateUser = 0;
                }
            }
            return totalCorporateUser;
        }

        public static int CountTotalMGIUser()
        {

            int totalMGIUser = 0;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MeghnaUser/CountTotalMeghnaUser");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();
                    totalMGIUser = readTask.Result;

                }
                else
                {
                    totalMGIUser = 0;
                }
            }
            return totalMGIUser;
        }
        public static int CountTotalCustomer()
        {
            int totalCustomer = 0;
           
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Customer/CountTotalCustomer");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();
                    totalCustomer = readTask.Result;

                }
                else
                {
                    totalCustomer = 0;
                }
            }
            return totalCustomer;
        }
        public static int CountTotalProduct()
        {
            int totalProduct = 0;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Product/CountTotalProducts");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();
                    totalProduct = readTask.Result;

                }
                else
                {
                    totalProduct = 0;
                }
            }
            return totalProduct;
        }
        public static int CountTotalBrand()
        {
            int totalBrand = 0;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Brand/CountTotalBrand");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();
                    totalBrand = readTask.Result;

                }
                else
                {
                    totalBrand = 0;
                }
            }
            return totalBrand;
        }
        public static int CountTotalCategory()
        {
            int totalCategory = 0;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Category/CountTotalCategory");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();
                    totalCategory = readTask.Result;

                }
                else
                {
                    totalCategory = 0;
                }
            }
            return totalCategory;
        }
        public static int CountTotalOrdersByOrderStatus(long userId,long orderStateId)
        {
            LoginCredentials token = (LoginCredentials)HttpContext.Current.Session["userToken"];
            int totalOrder = 0;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Order/GetTotalOrderByStatus?id="+userId+"&orderStateId="+orderStateId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<int>();
                    readTask.Wait();
                    totalOrder = readTask.Result;

                }
                else
                {
                    totalOrder = 0;
                }
            }
            return totalOrder;
        }
        public static List<ThanaWiseMasterDepot> GetAllThanaAgainstMasterDepot()
        {
            var masterDepotDetails = GetMasterDepotByUserId();
            var masterDepotId = masterDepotDetails.Id;
            IEnumerable<ThanaWiseMasterDepot> thanaList = null;
           var id =  HttpContext.Current.Session["userId"];
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Thana/GetByMasterDepotId?id="+ masterDepotId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ThanaWiseMasterDepot>>();
                    readTask.Wait();
                    thanaList = readTask.Result;
                }
                else
                {
                    thanaList = new List<ThanaWiseMasterDepot>();
                }
            }
            return thanaList.ToList();
        }
        public static List<Distributor> GetAllDistributorAgainstMasterDepot()
        {
            var masterDepotDetails = GetMasterDepotByUserId();
            var masterDepotId = masterDepotDetails.Id;
            IEnumerable<Distributor> distributorList = null;
           var id =  HttpContext.Current.Session["userId"];
            using (var client = new HttpClientDemo())
            {
                LoginCredentials token = (LoginCredentials)HttpContext.Current.Session["userToken"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Distributor/GetAllDistributorAgainstMasterDepot?id=" + masterDepotId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Distributor>>();
                    readTask.Wait();
                    distributorList = readTask.Result;
                }
                else
                {
                    distributorList = new List<Distributor>();
                }
            }
            return distributorList.ToList();
        }
        public static MasterDepot GetMasterDepotByUserId()
        {
            MasterDepot masterDepot = null;
           var userId =  HttpContext.Current.Session["userId"];
            using (var client = new HttpClientDemo())
            {
                LoginCredentials token = (LoginCredentials)HttpContext.Current.Session["userToken"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("MasterDepot/GetByUserId?userId=" + userId);
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
                }
            }
            return masterDepot;
        }

        public static List<Configuration> GetAllConfigurations(LoginCredentials token)
        {
            IEnumerable<Configuration> configurations = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("Configuration/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Configuration>>();
                    readTask.Wait();
                    configurations = readTask.Result;
                }
                else
                {
                    configurations = new List<Configuration>();
                }
            }
            return configurations.ToList();
        }

        public static LPGComboDiscount GetLPGComboDiscount(LoginCredentials token)
        {
            LPGComboDiscount lpgComboDiscount = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("LPGComboDiscount/GetLPGComboDiscount");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<LPGComboDiscount>();
                    readTask.Wait();
                    lpgComboDiscount = readTask.Result;
                }
            }
            return lpgComboDiscount;
        }

        public static DeliveryCharge GetDeliveryCharge(LoginCredentials token)
        {
            DeliveryCharge deliveryCharge = null;
            using (var client = new HttpClientDemo())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                var responseTask = client.GetAsync("DeliveryCharge/GetDeliveryCharge");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<DeliveryCharge>();
                    readTask.Wait();
                    deliveryCharge = readTask.Result;
                }
            }
            return deliveryCharge;
        }

        public static List<User> GetAllUser()
        {
            return null;
        }


    }
}