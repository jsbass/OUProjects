//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Parking
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Parking()
        {
            this.ParkingTypes = new HashSet<ParkingType>();
        }
    
        public string GeoData { get; set; }
        public string ParkingCode { get; set; }
        public int SpaceCount { get; set; }
        public int HandicapCount { get; set; }
        public int fkLocationId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParkingType> ParkingTypes { get; set; }
        public virtual Location Location { get; set; }
    }
}
