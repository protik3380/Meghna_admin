using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EFreshAdmin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
             name: "dashboard",
             url: "dashboard",
             defaults: new
             {
                 controller = "Home",
                 action = "Index"
             }
            );
            routes.MapRoute(
             name: "district",
             url: "setup/district",
             defaults: new
             {
                 controller = "District",
                 action = "Views"
             }
            );
            routes.MapRoute(
             name: "thana",
             url: "setup/thana",
             defaults: new
             {
                 controller = "Thana",
                 action = "Views"
             }
            );
            routes.MapRoute(
             name: "brand",
             url: "setup/brand",
             defaults: new
             {
                 controller = "Brand",
                 action = "Index"
             }
            );
            routes.MapRoute(
             name: "category",
             url: "setup/category",
             defaults: new
             {
                 controller = "Category",
                 action = "Index"
             }
            );
            routes.MapRoute(
            name: "mgi-department",
            url: "setup/mgi-department",
            defaults: new
            {
                controller = "MeghnaDepartment",
                action = "Index"
            }
           );
           routes.MapRoute(
           name: "mgi-designation",
           url: "setup/mgi-designation",
           defaults: new
           {
               controller = "MeghnaDesignation",
               action = "Index"
           }
           );
           routes.MapRoute(
           name: "corporate-department",
           url: "setup/corporate-department",
           defaults: new
           {
               controller = "CorporateDepartment",
               action = "Index"
           }
           );
           routes.MapRoute(
           name: "corporate-designation",
           url: "setup/corporate-designation",
           defaults: new
           {
               controller = "CorporateDesignation",
               action = "Index"
           }
           );
           routes.MapRoute(
           name: "coupon",
           url: "setup/coupon",
           defaults: new
           {
               controller = "Coupon",
               action = "Index"
           }
           );
            routes.MapRoute(
               name: "faq",
               url: "setup/faq",
               defaults: new
               {
                   controller = "FAQ",
                   action = "Index"
               }
               );
            routes.MapRoute(
               name: "distributor",
               url: "distributor/setup",
               defaults: new
               {
                   controller = "Distributor",
                   action = "Index"
               }
               );
            routes.MapRoute(
               name: "delivery-man",
               url: "delivery-man/setup",
               defaults: new
               {
                   controller = "DeliveryMan",
                   action = "Index"
               }
               );
            routes.MapRoute(
              name: "search-by-master-depot",
              url: "delivery-man/search-by-master-depot",
              defaults: new
              {
                  controller = "DeliveryMan",
                  action = "SearchByMasterDepot"
              }
              );
            routes.MapRoute(
              name: "master-depot",
              url: "master-depot/setup",
              defaults: new
              {
                  controller = "MasterDepot",
                  action = "Index"
              }
              );
            routes.MapRoute(
              name: "link-master-depot-with-thana",
              url: "master-depot/link-master-depot-with-thana",
              defaults: new
              {
                  controller = "MasterDepot",
                  action = "LinkMasterDepotWithThana"
              }
              );
            routes.MapRoute(
              name: "link-master-depot-and-thana-list",
              url: "master-depot/linked-master-depot-list",
              defaults: new
              {
                  controller = "MasterDepot",
                  action = "Views"
              }
              );
            routes.MapRoute(
             name: "product-list",
             url: "product/setup",
             defaults: new
             {
                 controller = "Product",
                 action = "Index"
             }
             );
           
            routes.MapRoute(
             name: "product-add-details",
             url: "product/add-details",
             defaults: new
             {
                 controller = "Product",
                 action = "AddDetails"
             }
             );
            routes.MapRoute(
             name: "product-unit-details",
             url: "product/unit-details-list",
             defaults: new
             {
                 controller = "Product",
                 action = "ViewProductUnit"
             }
             );
            routes.MapRoute(
             name: "product-discount",
             url: "product/discount",
             defaults: new
             {
                 controller = "Product",
                 action = "ViewProductDiscounts"
             }
             );
            routes.MapRoute(
             name: "contract-list",
             url: "contract/list",
             defaults: new
             {
                controller = "Contract",
                action = "Views"
             }
             );
            routes.MapRoute(
             name: "add-contract",
             url: "contract/add",
             defaults: new
             {
                 controller = "Contract",
                 action = "Create"
             }
             );
            routes.MapRoute(
             name: "corporate-user-list",
             url: "contract/corporate/user/list",
             defaults: new
             {
                controller = "Contract",
                action = "CorporateUser"
             }
             );
            routes.MapRoute(
             name: "add-corporate-user",
             url: "contract/user/add",
             defaults: new
             {
                controller = "Contract",
                action = "AddUser"
             }
             );
            routes.MapRoute(
             name: "add-bulk-corporate-user",
             url: "contract/user/bulk-add",
             defaults: new
             {
                controller = "Contract",
                action = "AddBulkUser"
             }
             );
            routes.MapRoute(
             name: "mgi-user",
             url: "mgi/user/list",
             defaults: new
             {
                controller = "MeghnaUser",
                action = "Index"
             }
             );
            routes.MapRoute(
             name: "mgi-add-user",
             url: "mgi/user/add",
             defaults: new
             {
                controller = "MeghnaUser",
                action = "AddUser"
             }
             );
            routes.MapRoute(
             name: "mgi-add-bulk-user",
             url: "mgi/user/bulk/add",
             defaults: new
             {
                controller = "MeghnaUser",
                action = "AddMeghnaUser"
             }
             );
            routes.MapRoute(
             name: "report-analysis",
             url: "report/analysis",
             defaults: new
             {
                controller = "Report",
                action = "Analysis"
             }
             );
           
            routes.MapRoute(
             name: "report-sales-by-product",
             url: "report/sales-by-product",
             defaults: new
             {
                controller = "Report",
                action = "SalesByProduct"
             }
             );
            routes.MapRoute(
             name: "report-total-orders",
             url: "report/total-orders",
             defaults: new
             {
                controller = "Report",
                action = "TotalOrders"
             }
             );
            routes.MapRoute(
             name: "report-sales-by-location",
             url: "report/sales-by-location",
             defaults: new
             {
                controller = "Report",
                action = "SalesByLocation"
             }
             );
            routes.MapRoute(
             name: "report-order-by-status",
             url: "report/order-by-status",
             defaults: new
             {
                controller = "Report",
                action = "OrdersByStatus"
             }
             );
            routes.MapRoute(
             name: "report-order-rate-over-time",
             url: "report/order-rate-over-time",
             defaults: new
             {
                controller = "Report",
                action = "OrderRateOverTime"
             }
             );
            routes.MapRoute(
             name: "report-order-growth-rate",
             url: "report/order-growth-rate",
             defaults: new
             {
                controller = "Report",
                action = "OrderGrowthRate"
             }
             );
            routes.MapRoute(
             name: "pending-orders",
             url: "orders/pending",
             defaults: new
             {
                controller = "MasterDepot",
                action = "PendingOrders"
             }
             );
            routes.MapRoute(
             name: "confirmed-orders",
             url: "orders/confirmed",
             defaults: new
             {
                controller = "MasterDepot",
                action = "ConfirmedOrders"
             }
             );
            routes.MapRoute(
             name: "on-processing-orders",
             url: "orders/on-processing",
             defaults: new
             {
                controller = "MasterDepot",
                action = "OnProcessingOrders"
             }
             );
            routes.MapRoute(
             name: "shipped-orders",
             url: "orders/shipped",
             defaults: new
             {
                controller = "MasterDepot",
                action = "ShippedOrders"
             }
             );
            routes.MapRoute(
             name: "delivered-orders",
             url: "orders/delivered",
             defaults: new
             {
                controller = "MasterDepot",
                action = "DeliveredOrders"
             }
             );
            routes.MapRoute(
             name: "received-orders",
             url: "orders/received",
             defaults: new
             {
                controller = "MasterDepot",
                action = "ReceivedOrders"
             }
             );
            routes.MapRoute(
             name: "rejected-orders",
             url: "orders/rejected",
             defaults: new
             {
                controller = "MasterDepot",
                action = "RejectedOrders"
             }
             );
            routes.MapRoute(
             name: "assign-deliveryman",
             url: "delivery-man/assign",
             defaults: new
             {
                controller = "MasterDepot",
                action = "AssignDeliveryMan"
             }
             );
            routes.MapRoute(
             name: "view-assign-orders",
             url: "delivery-man/assigned-orders",
             defaults: new
             {
                controller = "MasterDepot",
                action = "ViewAssignedOrders"
             }
             );
            routes.MapRoute(
             name: "delivery-man-list",
             url: "delivery-man/list",
             defaults: new
             {
                controller = "DeliveryMan",
                action = "Index"
             }
             );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
