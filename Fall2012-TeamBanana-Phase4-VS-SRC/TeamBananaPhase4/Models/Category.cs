//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TeamBananaPhase4.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Category
    {
        public Category()
        {
            this.CommDocument = new HashSet<CommDocument>();
        }
    
        public string Type { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<CommDocument> CommDocument { get; set; }
    }
}
