﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using EFreshAdmin.Models.EntityModels;
using EFreshStoreCore.Model.Context;

namespace EFreshStore.Models.Context
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class FreshContext : DbContext
    {
        public FreshContext(): base("name=FreshContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CorporateContract> CorporateContracts { get; set; }
        public virtual DbSet<CorporateUser> CorporateUsers { get; set; }
        public virtual DbSet<Distributor> Distributors { get; set; }
        public virtual DbSet<DistributorProductLine> DistributorProductLines { get; set; }
        public virtual DbSet<MasterDepot> MasterDepots { get; set; }
        public virtual DbSet<MasterDepotProductLine> MasterDepotProductLines { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductLine> ProductLines { get; set; }
        public virtual DbSet<ProductLineDetail> ProductLineDetails { get; set; }
        public virtual DbSet<ProductUnit> ProductUnits { get; set; }
        public virtual DbSet<ProductUnitPrice> ProductUnitPrices { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Thana> Thanas { get; set; }
        public virtual DbSet<MeghnaUser> MeghnaUsers { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<ThanaWiseMasterDepot> ThanaWiseMasterDepots { get; set; }
    }
}
