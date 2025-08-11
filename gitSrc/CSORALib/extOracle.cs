using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.ManagedDataAccess.Client
{
    public static class extOracle
    {
        public static async Task<DataTable> xQueryAsync(this OracleCommand cmd, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
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
        public static async Task<DataTable> xQueryAsync(string connectionstring, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            using (OracleConnection conn = new OracleConnection(connectionstring))
            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                await conn.OpenAsync();
                return await cmd.xQueryAsync(sql, timeoutSeconds, paramValues);
            }
        }
        public static async Task<int> xNonQueryAsync(this OracleCommand cmd, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            if (cmd.Connection.State != ConnectionState.Open)
            {
                await cmd.Connection.OpenAsync();
            }
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
            return await cmd.ExecuteNonQueryAsync();
        }
        public static async Task<int> xNonQueryAsync(string connectionstring, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            using (OracleConnection conn = new OracleConnection(connectionstring))
            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                await conn.OpenAsync();
                return await cmd.xNonQueryAsync(sql, timeoutSeconds, paramValues);
            }
        }
        public static async Task<int> xCountingAsync(this OracleCommand cmd, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
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
            object result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result); // 建議使用 Convert，以避免 unboxing 失敗
        }
        public static async Task<int> xCountingAsync(string connectionstring, string sql, int timeoutSeconds = -1, params (string key, object value)[] paramValues)
        {
            using (var conn = new OracleConnection(connectionstring))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                return await cmd.xCountingAsync(sql, timeoutSeconds, paramValues);
            }
        }
    }
}
