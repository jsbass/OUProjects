﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Calendar.Models.DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DB_109670_portalEntities : DbContext
    {
        public DB_109670_portalEntities()
            : base("name=DB_109670_portalEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CalendarGroup> CalendarGroups { get; set; }
        public virtual DbSet<CalendarSource> CalendarSources { get; set; }
        public virtual DbSet<CalendarEvent> CalendarEvents { get; set; }
    }
}
