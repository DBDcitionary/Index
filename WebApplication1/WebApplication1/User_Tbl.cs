//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_Tbl
    {
        public int User_ID { get; set; }
        public string UserName { get; set; }
        public Nullable<int> User_Role_ID { get; set; }
        public byte[] Password { get; set; }
    
        public virtual User_Role_Tbl User_Role_Tbl { get; set; }
    }
}