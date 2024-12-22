using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DataLayers
{
    public interface iAnalysProductDAL<T> where T : class
    {
        /// <summary>
        /// Lấy những mặt hàng có số lượng tiêu thụ cao nhất
        /// </summary>
        /// <returns></returns>
        List<T> Featured();
        /// <summary>
        /// Lấy những mặt hàng mới nhất
        /// </summary>
        /// <returns></returns>
        List<T> New();
        /// <summary>
        /// Lấy những mặt hàng có đánh giá cao nhất. Dựa trên số lần được mua
        /// </summary>
        /// <returns></returns>
        List<T> HighRate();
    }
}
