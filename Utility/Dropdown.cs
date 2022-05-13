using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using EFreshAdmin.Models.EntityModels;
using EFreshStore.Models.Context;
using EFreshStore.Utility;
using EFreshStoreCore.Model.Context;

namespace EFreshAdmin.Utility
{
    public static class Dropdown
    {

        public static List<Product> Products()
        {
            IEnumerable<Product> products = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Product/GetAll");
                var responseTask = client.GetAsync("Product/GetAllActive");
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
                    products = Enumerable.Empty<Product>();
                }
            }
            return products.ToList();
        }

        public static List<CorporateContract> Contracts()
        {
            IEnumerable<CorporateContract> contracts = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Contract/GetAll");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CorporateContract>>();
                    readTask.Wait();
                    contracts = readTask.Result;
                }
                else
                {
                    contracts = Enumerable.Empty<CorporateContract>();
                }
            }
            return contracts.ToList();
        }

        public static List<Category> Categories()
        {
            IEnumerable<Category> categories = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Category/GetAllActive");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Category>>();
                    readTask.Wait();
                    categories = readTask.Result;
                }
                else
                {
                    categories = new List<Category>();
                }
            }
            return categories.ToList();
        }

        public static List<ProductType> ProductTypes()
        {
            IEnumerable<ProductType> productTypes = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("ProductType/GetAllActive");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductType>>();
                    readTask.Wait();
                    productTypes = readTask.Result;
                }
                else
                {
                    productTypes = new List<ProductType>();
                }
            }
            return productTypes.ToList();
        }

        public static List<Brand> Brands()
        {
            IEnumerable<Brand> brands = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Brand/GetAllActive");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Brand>>();
                    readTask.Wait();
                    brands = readTask.Result;
                }
                else
                {
                    brands = new List<Brand>();
                }
            }
            return brands.ToList();
        }

        public static List<District> Districts()
        {
            IEnumerable<District> districts = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "District/GetAll");
                var responseTask = client.GetAsync("District/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<District>>();
                    readTask.Wait();
                    districts = readTask.Result;
                }
                else
                {
                    districts = Enumerable.Empty<District>();
                }
            }
            return districts.ToList();
        }
        public static List<District> AllActiveDistricts()
        {
            IEnumerable<District> districts = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "District/GetAll");
                var responseTask = client.GetAsync("District/GetAllActive");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<District>>();
                    readTask.Wait();
                    districts = readTask.Result;
                }
                else
                {
                    districts = Enumerable.Empty<District>();
                }
            }
            return districts.ToList();
        }
        
        public static List<Thana> GetAllThanaById(long id)
        {
            IEnumerable<Thana> thanas = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Thana/GetByDistrictId");
                var responseTask = client.GetAsync("Thana/GetActiveThanaByDistrictId/" + id);
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
            return thanas.ToList();
        }

        public static List<MasterDepot> MasterDepo()
        {
            IEnumerable<MasterDepot> masterDepots = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "MasterDepot/GetAll");
                var responseTask = client.GetAsync("MasterDepot/GetAllActive");
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
                }
            }
            return masterDepots.ToList();
        }

        public static List<Thana> Thanas()
        {
            IEnumerable<Thana> thanas = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Thana/GetAll");
                var responseTask = client.GetAsync("Thana/GetAll");
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
            return thanas.ToList();
        }

        public static List<Thana> AllActiveThanas()
        {
            IEnumerable<Thana> thanas = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "Thana/GetAll");
                var responseTask = client.GetAsync("Thana/GetAllActive");
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
            return thanas.ToList();
        }

        public static List<ProductLine> ProductLines()
        {
            IEnumerable<ProductLine> productLines = null;
            using (var client = new HttpClientDemo())
            {
                //client.BaseAddress = new Uri(BaseUrl.url + "ProductLine/GetAll");
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
                }
            }
            return productLines.ToList();
        }

        public static List<OrderReject> OrderRejectStatus()
        {
            IEnumerable<OrderReject> orderRejects = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Order/GetOrderRejectStatus");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<OrderReject>>();
                    readTask.Wait();
                    orderRejects = readTask.Result;
                }
                else
                {
                    orderRejects = new List<OrderReject>();
                }
            }

            return orderRejects.ToList();
        }

        public static List<OrderState> OrderStates()
        {
            IEnumerable<OrderState> orderStates = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("Order/GetOrderStates");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<OrderState>>();
                    readTask.Wait();
                    orderStates = readTask.Result;
                }
                else
                {
                    orderStates = new List<OrderState>();
                }
            }
            return orderStates.ToList();
        }

        public static List<ProductUnit> ProductUnits(long id)
        {
            IEnumerable<ProductUnit> productUnits = null;
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
            return productUnits.ToList();
        }

        public static List<MeghnaDepartment> MeghnaDepartments()
        {
            IEnumerable<MeghnaDepartment> departments = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MeghnaDepartment/GetAllActive");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<MeghnaDepartment>>();
                    readTask.Wait();
                    departments = readTask.Result;
                }
                else
                {
                    departments = new List<MeghnaDepartment>();
                }
            }
            return departments.ToList();
        }

        public static List<MeghnaDesignation> MeghnaDesignations()
        {
            IEnumerable<MeghnaDesignation> designations = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("MeghnaDesignation/GetAllActive");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<MeghnaDesignation>>();
                    readTask.Wait();
                    designations = readTask.Result;
                }
                else
                {
                    designations = new List<MeghnaDesignation>();
                }
            }
            return designations.ToList();
        }

        public static List<CorporateDepartment> CorporateDepartments()
        {
            IEnumerable<CorporateDepartment> departments = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("CorporateDepartment/GetAllActive");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CorporateDepartment>>();
                    readTask.Wait();
                    departments = readTask.Result;
                }
                else
                {
                    departments = new List<CorporateDepartment>();
                }
            }
            return departments.ToList();
        }

        public static List<CorporateDesignation> CorporateDesignations()
        {
            IEnumerable<CorporateDesignation> designations = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("CorporateDesignation/GetAllActive");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CorporateDesignation>>();
                    readTask.Wait();
                    designations = readTask.Result;
                }
                else
                {
                    designations = new List<CorporateDesignation>();
                }
            }
            return designations.ToList();
        }

        public static List<UserType> UserTypes()
        {
            IEnumerable<UserType> userTypes = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("UserType/GetAll");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<UserType>>();
                    readTask.Wait();
                    userTypes = readTask.Result;
                }
                else
                {
                    userTypes = new List<UserType>();
                }
            }
            return userTypes.ToList();
        }
        public static List<DeliveryMan> ActiveDeliveryMen()
        {
            IEnumerable<DeliveryMan> deliveryMen = null;
            using (var client = new HttpClientDemo())
            {

                var responseTask = client.GetAsync("DeliveryMan/GetAllActive");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<DeliveryMan>>();
                    readTask.Wait();
                    deliveryMen = readTask.Result;
                }
                else
                {
                    deliveryMen = new List<DeliveryMan>();
                }
            }
            return deliveryMen.ToList();
        }
        public static List<DeliveryMan> DeliveryMenByMasterDepotId(long masterDepotId)
        {
            IEnumerable<DeliveryMan> deliveryMen = null;
            using (var client = new HttpClientDemo())
            {
                var responseTask = client.GetAsync("DeliveryMan/GetAllDeliveryManByMasterDepot?masterDepotId="+masterDepotId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<DeliveryMan>>();
                    readTask.Wait();
                    deliveryMen = readTask.Result;
                }
                else
                {
                    deliveryMen = new List<DeliveryMan>();
                }
            }
            return deliveryMen.ToList();
        }
    }
}