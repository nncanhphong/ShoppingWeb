using Dapper;
using SV21T1020581.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DataLayers.SQLServer
{
    public class ProvinceDAL : BaseDAL, iSimpleSelectDAL<Province>
    {
        public ProvinceDAL(string connectionString) : base(connectionString)
        {
        }

        public List<Province> List()
        {
            List<Province> data = new List<Province>();
            using (var connection = OpenConnnection())
            {
                var sql = "select * from Provinces";
                var parameter = new
                {

                };
                data = connection.Query<Province>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
    }
}
