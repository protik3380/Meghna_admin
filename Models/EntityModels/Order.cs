using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EFreshAdmin.Models;
using EFreshAdmin.Models.EntityModels;
using EFreshStore.Models;
using EFreshStore.Models.Context;
using EFreshStoreCore.Model.Context;

public class Order
{
    public Order()
    {
        this.OrderDetails = new HashSet<OrderDetail>();
        this.AssignOrders = new HashSet<AssignOrder>();
    }

    public long Id { get; set; }
    public string OrderNo { get; set; }

    [Required(ErrorMessage = "Please enter customer name")]
    public string CustomerName { get; set; }

    [Required(ErrorMessage = "Please enter mobile no")]
    [RegularExpression(@"^(?:\\+?88|0088)?01[15-9]\\d{8}$", ErrorMessage = "Please enter a valid mobile no")]
    public string MobileNo { get; set; }

    [Required(ErrorMessage = "Please enter email address")]
    [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please enter a valid email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please enter delivery address")]
    public string DeliveryAddress { get; set; }

    [Required(ErrorMessage = "Please select thana")]
    public Nullable<long> ThanaId { get; set; }

    [Required(ErrorMessage = "Please select master depot")]
    public Nullable<long> MasterDepotId { get; set; }
    public Nullable<long> OrderStateId { get; set; }
    public Nullable<long> UserId { get; set; }
    public Nullable<System.DateTime> OrderDate { get; set; }
    public Nullable<long> OrderRejectId { get; set; }
    public string CouponCode { get; set; }
    public Nullable<decimal> CouponDiscount { get; set; }
    public bool IsCheck { get; set; }
    public Nullable<long> OrderStatusChangedBy { get; set; }
    public decimal DeliveryCharge { get; set; }
    public virtual MasterDepot MasterDepot { get; set; }
    public virtual OrderState OrderState { get; set; }
    public virtual Thana Thana { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    public virtual OrderReject OrderReject { get; set; }
    public virtual ICollection<AssignOrder> AssignOrders { get; set; }
}