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
    
    public partial class Menu_Order
    {
        public int idMenu_Order { get; set; }
        public int idMenu_Item { get; set; }
        public int idOrder { get; set; }
        public Nullable<int> idComp { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }
    
        public virtual Comp Comp { get; set; }
        public virtual Menu_Item Menu_Item { get; set; }
        public virtual Order Order { get; set; }
    }
}
