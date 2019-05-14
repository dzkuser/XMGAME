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
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnString { get; set; } = ConfigurationManager.ConnectionStrings["MySqlStr"].ConnectionString;

        protected IDbConnection GetConnection()
        {
            return new SqlConnection(ConnString);
        }

        

    }
}
