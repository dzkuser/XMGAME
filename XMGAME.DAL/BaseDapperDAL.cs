using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.DAL
{
    public class BaseDapperDAL
    {

        protected string ConnString { get; set; } = ConfigurationManager.ConnectionStrings["MySqlStr"].ConnectionString;

        protected IDbConnection GetConnection()
        {
            return new SqlConnection(ConnString);
        }

        

    }
}
