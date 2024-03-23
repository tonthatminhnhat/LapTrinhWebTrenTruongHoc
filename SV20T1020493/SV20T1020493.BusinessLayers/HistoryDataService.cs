using SV20T1020493.DataLayers;
using SV20T1020493.DataLayers.SQLServer;
using SV20T1020493.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.BusinessLayers
{
    public static class HistoryDataService
    {
        private static readonly IHistoryDAL historyDB;

        static HistoryDataService()
        {
            historyDB = new HistoryDAL(Configuration.ConnectionString);
        }
        public static List<History> ListHistory(out int rowCount, int page = 1, int pageSize = 0, string work = "",
            DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "", string tableName = "")
        {

            rowCount = historyDB.Count(work, fromTime, toTime, searchValue, tableName);
            Console.WriteLine("Lisst có hoạt động: " + rowCount + "-" + page + "-" + pageSize + "-" +
                    work + "-" + fromTime + "-" + toTime + "-" + searchValue + "-" + tableName);
            return historyDB.List(page, pageSize, work, fromTime, toTime, searchValue, tableName).ToList();
        }
        public static History? GetHistory(int TrashBinId)
        {
            return historyDB.GetHistory(TrashBinId);
        }
        public static bool Deleterestore(string sql)
        {
            return historyDB.Deleterestore(sql);
        }
        public static bool Updaterestore(string sql)
        {
            return historyDB.Updaterestore(sql);
        }


    }
}
