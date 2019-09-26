﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal.Models.DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Link> Links { get; set; }
        public virtual DbSet<LinkCategory> LinkCategories { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<OuMapDumpBuilding> OuMapDumpBuildings { get; set; }
        public virtual DbSet<OuMapDumpParking> OuMapDumpParkings { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<Parking> Parkings { get; set; }
        public virtual DbSet<ParkingType> ParkingTypes { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
    }
}
