using Dapper;
using SV21T1020581.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DataLayers.SQLServer
{
    public class OrderStatusDAL : BaseDAL, iSimpleSelectDAL<OrderStatus>
    {
        public OrderStatusDAL(string connectionString) : base(connectionString)
        {
        }

        public List<OrderStatus> List()
        {
            List<OrderStatus> data = new List<OrderStatus>();
            using (var connection = OpenConnnection())
            {
                var sql = "select * from OrderStatus";
                var parameter = new
                {

                };
                data = connection.Query<OrderStatus>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
    }
}
