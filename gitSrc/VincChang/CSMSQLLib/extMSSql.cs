using System.Data;
using Microsoft.Data.SqlClient;

namespace Microsoft.Data.SqlClient
{
    public static class extMSSql
    {
        public static async Task<DataTable> xQueryAsync(this SqlCommand cmd, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            if (cmd.Connection.State != ConnectionState.Open)
            {
                await cmd.Connection.OpenAsync();
            }
            cmd.CommandText = sql;
            if (timeoutSeconds > 30)
            {
                cmd.CommandTimeout = timeoutSeconds;
            }
            if (paramValues != null)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(paramValues);
            }
            DataTable dt = new DataTable();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                dt.Load(reader);
            }
            return dt;
        }
        public static async Task<DataTable> xQueryAsync(string connstring, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            using (SqlConnection conn = new SqlConnection(connstring))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                return await xQueryAsync(cmd, sql, timeoutSeconds, paramValues);
            }
        }
        public static async Task<DataSet> xQueryDataSetAsync(this SqlCommand cmd, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            if (cmd.Connection.State != ConnectionState.Open)
            {
                await cmd.Connection.OpenAsync();
            }

            cmd.CommandText = sql;
            if (timeoutSeconds > 30)
            {
                cmd.CommandTimeout = timeoutSeconds;
            }
            if (paramValues != null)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(paramValues);
            }
            var ds = new DataSet();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                int tableIndex = 0;

                do
                {
                    var table = new DataTable($"Table{tableIndex++}");
                    table.Load(reader); // ← 這裡正確：DataTable 有 Load(IDataReader)
                    ds.Tables.Add(table);
                } while (await reader.NextResultAsync()); // 支援多個結果集
            }

            return ds;
        }
        public static async Task<DataSet> xQueryDataSetAsync(string connstring, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            using (SqlConnection conn = new SqlConnection(connstring))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                return await cmd.xQueryDataSetAsync(sql, timeoutSeconds, paramValues);
            }
        }
        public static async Task<int> xNonQueryAsync(this SqlCommand cmd, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            if (cmd.Connection.State != ConnectionState.Open)
            {
                await cmd.Connection.OpenAsync();
            }
            cmd.CommandText = sql;
            if (paramValues != null)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(paramValues);
            }
            if (timeoutSeconds > 30)
            {
                cmd.CommandTimeout = timeoutSeconds;
            }
            return await cmd.ExecuteNonQueryAsync();
        }
        public static async Task<int> xNonQueryAsync(string connstring, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            using (SqlConnection conn = new SqlConnection(connstring))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                return await cmd.xNonQueryAsync(sql, timeoutSeconds, paramValues);
            }
        }
        public static async Task<int> xNonQueryWithTxnAsync(this SqlCommand cmd, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            int cnt = 0;
            using (SqlTransaction txn = cmd.Connection.BeginTransaction())
            {
                cmd.CommandText = sql;
                if (paramValues != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(paramValues);
                }
                try
                {
                    if (timeoutSeconds > 30)
                    {
                        cmd.CommandTimeout = timeoutSeconds;
                    }
                    cmd.Transaction = txn;
                    cnt = await cmd.ExecuteNonQueryAsync();
                    await txn.CommitAsync();
                }
                catch (Exception ex)
                {
                    await txn.RollbackAsync();
                    throw;
                }
            }
            return cnt;
        }
        public static async Task<int> xNonQueryWithTxnAsync(string connectionstring, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            int cnt = 0;
            using (SqlConnection conn = new SqlConnection(connectionstring))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                return await cmd.xNonQueryWithTxnAsync(sql, timeoutSeconds, paramValues);
            }
        }
        public static async Task<int> xCountingAsync(string connstring, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            using (SqlConnection conn = new SqlConnection(connstring))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                if (timeoutSeconds > 30)
                {
                    cmd.CommandTimeout = timeoutSeconds;
                }
                cmd.CommandText = sql;
                if (paramValues != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(paramValues);
                }
                object obj = await cmd.ExecuteScalarAsync();
                return (int)obj;
            }
        }
        public static SqlDataAdapter xNewAdapter(this SqlConnection conn, string sql)
        {
            return new SqlDataAdapter(sql, conn);
        }
    }
}
