using SV21T1020581.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DataLayers
{
    public interface iCommonDAL<T> where T : class
    {
        //SearchValue rong lay toan bo du lieu
        List<T> List(int page = 1, int pageSize = 0, string searchValue ="");

        int Count(String SearchValue = "");

        //Bo sung du lieu vao trong CSDL, Ham tra ve ID cua du lieu duoc bo sung
        int Add(T data);

        //Cap nhap du lieu
        bool Update(T data);

        bool Delete(int id);

        //Lay mot dong du lieu dua vao id, tra ve null neu du lieu can lay khong ton tai
        T? Get(int id);

        //Kiem tra dong du lieu co khoa id hien co dang lien ket du lieu o bang khac hay khong
        bool InUsed(int id);
    }
}
