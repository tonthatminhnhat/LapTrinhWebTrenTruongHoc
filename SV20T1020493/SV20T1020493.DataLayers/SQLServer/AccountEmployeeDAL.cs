using Dapper;
using SV20T1020493.DomainModels;
using System.Data;


namespace SV20T1020493.DataLayers.SQLServer
{
    public class AccountEmployeeDAL : _BaseDAL, ICommonDAL<AccountEmployee>
    {
        public AccountEmployeeDAL(string connectionString) : base(connectionString)
        {
        }

        public void SetEmployeeIDInContext(int employeeID, string employeeName)
        {
            using (var connection = OpenConnection())
            {
                Console.WriteLine("Có hoạt động 2: "+ employeeID);

                // Thực hiện truy vấn để đặt CONTEXT_INFO
                var sql = $@"update Session set EmployeeID ={employeeID}, EmployeeName= N'{employeeName}' where SessionID=1";
               /* var sql = $@"
                        DECLARE @IntValue INT = {employeeID};
                     DECLARE @IntVarbinary VARBINARY(4) = CAST( @IntValue AS VARBINARY(4));
                     SET CONTEXT_INFO @IntVarbinary;";*/
                connection.Execute(sql);
                connection.Close();
            }
        }
        public void DeleteOldData()
        {
            using (var connection = OpenConnection())
            {
                // Xây dựng câu lệnh SQL để xóa các hàng dữ liệu có cột TimeChange lớn hơn 1 ngày
                var sql = @"DELETE FROM TrashBin WHERE TimeChange < DATEADD(DAY, -1, GETDATE())";
                connection.Execute(sql);
                connection.Close();
            }   
        }
        /*  select* from TrashBin where TimeChange<DATEADD(HOUR, -1, GETDATE());*/
        public int Add(AccountEmployee data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Employees where Phone = @Phone)
                                select -1
                            else
                                begin
                                    insert into Employees(FullName,Email,Password,RoleNames)
                                    values(@FullName,@Email,@Password,@RoleNames);

                                    select @@identity;
                                end";
                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Password ?? "",
                    IsWorking = data.RoleNames ?? ""
                };
                id = connection.ExecuteScalar<int>(sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Employees 
                            where (@searchValue = N'') or (FullName like @searchValue)";
                var parameters = new { searchValue = searchValue ?? "" };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Orders where EmployeeID = @EmployeeID
                        delete from Employees where EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public AccountEmployee? Get(int id)
        {
            AccountEmployee? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select EmployeeID,FullName, Email,Password,RoleNames from Employees where EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
                };
                data = connection.QueryFirstOrDefault<AccountEmployee>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where EmployeeID = @EmployeeID)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    EmployeeID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return result;
        }

        public IList<AccountEmployee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<AccountEmployee> list = new List<AccountEmployee>();
            if (!string.IsNullOrEmpty(searchValue))

                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                            (
	                            select	EmployeeID,FullName, Email,Password,RoleNames
                                   , row_number() over (order by FullName) as RowNumber
	                            from	Employees 
	                            where	(@searchValue = N'') or (FullName like @searchValue)
                            )
                            select * from cte
                           where  (@pageSize = 0) 
                           or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                          order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? ""
                };
                list = connection.Query<AccountEmployee>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();

            }
            return list;
        }

        public bool Update(AccountEmployee data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Employees where EmployeeID <> @EmployeeID and Phone = @Phone)
                                begin
                                    update Employees 
                                    set FullName = @FullName,
                                        Email = @Email,    
                                        Password=@Password,
                                        RoleNames=@RoleNames
                                    where EmployeeID = @EmployeeID end";
                var parameters = new
                {
                    EmployeeID = data.EmployeeID,
                    FullName = data.FullName ?? "",
                    Password = data.Password,
                    RoleNames = data.RoleNames ?? "",
                    Email = data.Email ?? ""
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

    }
}
