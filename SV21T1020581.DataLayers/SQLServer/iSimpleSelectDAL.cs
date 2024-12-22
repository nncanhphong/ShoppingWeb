using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DataLayers.SQLServer
{
    public interface iSimpleSelectDAL<T> where T: class
    {
        List<T> List();
    }
}
