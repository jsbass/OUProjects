//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Badges.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserRequirement
    {
        public string UserId { get; set; }
        public int RequirementId { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public bool IsCompleted { get; set; }
        public string StatusText { get; set; }
        public Nullable<decimal> PercentCompleted { get; set; }
    
        public virtual Requirement Requirement { get; set; }
        public virtual User User { get; set; }
    }
}
