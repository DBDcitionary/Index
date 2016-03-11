using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class ServerInfor
    {
        public string ServerName { get; set; }
        public string InstanceName { get; set; }
        public string IsClustered { get; set; }
        public string Version { get; set; }

        public List<string> fullServerName { get; set; }
    }

    public class dbInfor
    {
        public string name { get; set; }
        public List<string> dbnames { get; set; }
    }
}