using Dapper;
using SV20T1020493.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.DataLayers.SQLServer
{
    public class HistoryDAL : _BaseDAL, IHistoryDAL
    {
        public HistoryDAL(string connectionString) : base(connectionString)
        {
        }

        public int Count(string work = "", DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "", string tableName = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";        
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT(*) FROM TrashBin
                    WHERE (@work = N'' OR Work = @work)
                        AND(@tableName = N'' OR TableName = @tableName)
                        AND(@fromtime IS NULL OR TimeChange >= @fromTime)
                        AND(@toTime IS NULL OR TimeChange <= @toTime)
                        AND(@searchValue = N'' OR EmployeeName LIKE @searchValue)
                       ";
              
                var parameters = new
                {
                    work = work??"",
                    tableName = tableName??"",
                    fromTime = fromTime,
                    toTime = toTime,
                    searchValue = searchValue ?? ""
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public History? GetHistory(int id)
        {
            History? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select *from TrashBin where TrashBinID = @trashBinID";
                var parameters = new { trashBinID = id };
                data = connection.QueryFirstOrDefault<History>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public IList<History> List(int page = 1, int pageSize = 0, string work = "", DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "", string tableName = "")
        {
           
            List<History> list = new List<History>();
            if (!string.IsNullOrEmpty(searchValue))

                searchValue = "%" + searchValue + "%";         
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                            (
	                            select	*, row_number() over (order by TimeChange DESC) as RowNumber
	                            from	TrashBin  
	                            where	
                                     (@work = N'' or Work = @work)
                                  and (@tableName = '' or TableName = @tableName)
                                  and (@fromTime is null or TimeChange >= @fromTime)
                                  and (@toTime is null or TimeChange <= @toTime)
                                  and (@searchValue = N''or EmployeeName like @searchValue)
                              ) 
                            select * from cte
                           where  (@pageSize = 0) 
                           or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                          order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "",
                    work = work??"",
                    tableName = tableName??"",
                    fromTime = fromTime,
                    toTime = toTime
                };
                list = connection.Query<History>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();

            }
            return list;
        }

        bool IHistoryDAL.Deleterestore(string sql)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                int rowsAffected = connection.Execute(sql, commandType: CommandType.Text);
                result = rowsAffected > 0;

                connection.Close();
            }
            return result;
        }

        bool IHistoryDAL.Updaterestore(string sql)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                Console.WriteLine("SQl: " + sql);
                int rowsAffected = connection.Execute(sql, commandType: CommandType.Text);
                result = rowsAffected > 0;

                connection.Close();
            }
            return result;
        }
    }
}
