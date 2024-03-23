using SV20T1020493.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.DataLayers
{
    public interface IHistoryDAL
    {
        IList<History> List(int page = 1, int pageSize = 0,
       string work ="", DateTime? fromTime = null, DateTime? toTime = null,
       string searchValue = "",string tableName="");

        int Count(string work = "", DateTime? fromTime = null, DateTime? toTime = null,
       string searchValue = "", string tableName = "");

        bool Updaterestore(string sql);
        bool Deleterestore(string sql);
        History? GetHistory(int id);
    }
}
