using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using System.Web.Configuration;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class NetworkServers
    {
        public string ServerName { get; set; }
    }

    public class NetworkPaths
    {
        public NetworkPaths()
        {
            SqlDataSourceEnumerator Servers = SqlDataSourceEnumerator.Instance;
            DataTable DataSource = Servers.GetDataSources();
            List<NetworkServers> _networkservers = (from d in DataSource.AsEnumerable()
                                                    select new NetworkServers
                                                    {
                                                        ServerName = d["InstanceName"].ToString() == String.Empty ? d["ServerName"].ToString() : d["ServerName"].ToString() + "\\" + d["InstanceName"].ToString()
                                                    }).ToList();            
            ServerPathList = new SelectList(_networkservers, "ServerName", "ServerName");
        }
        //Accesors
        public SelectList ServerPathList{get; set; }
    }
}