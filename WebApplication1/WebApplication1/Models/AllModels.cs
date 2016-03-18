using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace WebApplication1.Models
{
    public class model
    {
        public string connectionstring = WebConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        public class server
        {

        }

        public class database
        {

        }

        public class table
        {

        }

        public class column
        {

        }
    }
}