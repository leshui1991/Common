/*
 * MIT License
 *
 * Copyright(c) 2019 kelicto
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/*
             ,---------------------------------------------------,              ,---------,
        ,----------------------------------------------------------,          ,"        ,"|
      ,"                                                         ,"|        ,"        ,"  |
     +----------------------------------------------------------+  |      ,"        ,"    |
     |  .----------------------------------------------------.  |  |     +---------+      |
     |  | C:\>FILE -INFO                                     |  |  |     | -==----'|      |
     |  |                                                    |  |  |     |         |      |
     |  |                                                    |  |  |/----|`---=    |      |
     |  |              Author: kelicto                       |  |  |     |         |      |
     |  |              Email: kelistudy@163.com              |  |  |     |         |      |
     |  |              Creation Time: 10/30/2019 07:08:41 PM |  |  |     |         |      |
     |  | C:\>_                                              |  |  |     | -==----'|      |
     |  |                                                    |  |  |   ,/|==== ooo |      ;
     |  |                                                    |  |  |  // |(((( [66]|    ,"
     |  `----------------------------------------------------'  |," .;'| |((((     |  ,"
     +----------------------------------------------------------+  ;;  | |         |,"
        /_)_________________________________________________(_/  //'   | +---------+
           ___________________________/___  `,
          /  oooooooooooooooo  .o.  oooo /,   \,"-----------
         / ==ooooooooooooooo==.o.  ooo= //   ,`\--{)B     ,"
        /_==__==========__==_ooo__ooo=_/'   /___________,"
*/

using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace KeLi.Common.Drive.Sql
{
    /// <summary>
    /// Sql assist.
    /// </summary>
    public class SqlAssist
    {
        /// <summary>
        /// The sql connection string.
        /// </summary>
        public static string ConnSql => ConfigurationManager.ConnectionStrings["ConnSql"].ConnectionString;

        /// <summary>
        /// Returns connection the database's result.
        /// </summary>
        /// <returns></returns>
        public bool ConnectSuccess()
        {
            using (var conn = new SqlConnection(ConnSql))
            {
                conn.Open();
                return conn.State == ConnectionState.Open;
            }
        }

        /// <summary>
        /// Returns update num by the single sql and no parameters.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static int UpdateData(string text, CommandType ct = CommandType.Text)
        {
            int result;

            using (var conn = new SqlConnection(ConnSql))
            {
                var cmd = new SqlCommand(text, conn) { CommandType = ct };

                conn.Open();
                result = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Returns update num by the single sql and parameters.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sps"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static int UpdateData(string text, SqlParameter[] sps, CommandType ct = CommandType.Text)
        {
            int result;

            using (var conn = new SqlConnection(ConnSql))
            {
                var cmd = new SqlCommand(text, conn) { CommandType = ct };

                conn.Open();
                cmd.Parameters.AddRange(sps);
                result = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Returns update num by the multi sqls and no parameters.
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static int UpdateMultiple(List<string> texts, CommandType ct = CommandType.Text)
        {
            var result = 0;
            var cmd = new SqlCommand();

            try
            {
                using (var conn = new SqlConnection(ConnSql))
                {
                    conn.Open();
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    foreach (var text in texts)
                    {
                        cmd.CommandText = text;
                        cmd.CommandType = ct;
                        result += cmd.ExecuteNonQuery();
                    }

                    cmd.Transaction.Commit();
                }
            }
            catch
            {
                cmd.Transaction?.Rollback();
            }
            finally
            {
                if (cmd.Transaction != null)
                    cmd.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Returns update num by the multi sqls and parameters.
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="sps"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static int UpdateMultiple(List<string> texts, SqlParameter[][] sps, CommandType ct = CommandType.Text)
        {
            var result = 0;
            var cmd = new SqlCommand();

            try
            {
                using (var conn = new SqlConnection(ConnSql))
                {
                    conn.Open();
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();

                    var index = 0;

                    foreach (var text in texts)
                    {
                        cmd.Parameters.AddRange(sps[index++]);
                        cmd.CommandText = text;
                        cmd.CommandType = ct;
                        result += cmd.ExecuteNonQuery();
                    }

                    cmd.Transaction.Commit();
                }
            }
            catch
            {
                cmd.Transaction?.Rollback();
            }
            finally
            {
                if (cmd.Transaction != null)
                    cmd.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Querys object by the single sql and no parameters.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static object QueryObject(string text, CommandType ct = CommandType.Text)
        {
            object result;

            using (var conn = new SqlConnection(ConnSql))
            {
                var cmd = new SqlCommand(text, conn) { CommandType = ct };

                conn.Open();
                 result = cmd.ExecuteScalar();
                cmd.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Querys object by the single sql and parameters.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sps"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static object QueryObject(string text, SqlParameter[] sps, CommandType ct = CommandType.Text)
        {
            object result;

            using (var conn = new SqlConnection(ConnSql))
            {
                var cmd = new SqlCommand(text, conn) { CommandType = ct };

                conn.Open();
                cmd.Parameters.AddRange(sps);
                 result = cmd.ExecuteScalar();
                cmd.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Querys the data table by the single sql and no parameters.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static DataTable QueryData(string text, CommandType ct = CommandType.Text)
        {
           var results = new DataTable();

            using (var conn = new SqlConnection(ConnSql))
            {
                var cmd = new SqlCommand(text, conn) { CommandType = ct };

                new SqlDataAdapter(cmd).Fill(results);
                cmd.Dispose();
            }

            return results;
        }

        /// <summary>
        /// Querys the data table by the single sql and parameters.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sps"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static DataTable QueryData(string text, SqlParameter[] sps, CommandType ct = CommandType.Text)
        {
            var results = new DataTable();

            using (var conn = new SqlConnection(ConnSql))
            {
                var cmd = new SqlCommand(text, conn) { CommandType = ct };

                cmd.Parameters.AddRange(sps);
                new SqlDataAdapter(cmd).Fill(results);
                cmd.Dispose();
            }

            return results;
        }
    }
}