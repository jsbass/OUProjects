//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class CalendarGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CalendarGroup()
        {
            this.CalendarEvents = new HashSet<CalendarEvent>();
        }
    
        public int GroupId { get; set; }
        public string Name { get; set; }
        public Nullable<int> GroupIdFromSource { get; set; }
        public int fkSourceId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CalendarEvent> CalendarEvents { get; set; }
        public virtual CalendarSource CalendarSource { get; set; }
    }
}
