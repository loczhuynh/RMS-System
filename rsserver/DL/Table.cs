//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RMS.Server.ServiceModel.Service.DL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Table
    {
        public Table()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int idTable { get; set; }
        public string Location { get; set; }
        public int idServer { get; set; }
        public string Request { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}