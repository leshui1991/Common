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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KeLi.Common.Converter.Converter
{
    /// <summary>
    /// A data conllection converter.
    /// </summary>
    public static class CollectionConverter
    {
        /// <summary>
        /// Converts the data table to the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(DataTable dt)
        {
            List<T> results;

            if (dt == null || dt.Rows.Count == 0)
                results = new List<T>();
            else
            {
                results = new List<T>();

                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var tab = Activator.CreateInstance<T>();
                    var properties = tab.GetType().GetProperties();

                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        var index = j;

                        foreach (var property in properties.Where(w => w.Name.Equals(dt.Columns[index].ColumnName)))
                        {
                            property.SetValue(tab, dt.Rows[i][j] != DBNull.Value ? dt.Rows[i][j] : null, null);
                            break;
                        }
                    }

                    results.Add(tab);
                }
            }

            return results;
        }

        /// <summary>
        /// Converts the data table to the ilist.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList ToList(DataTable dt, Type type)
        {
            IList results;
            var master = typeof(List<>);
            var types = master.MakeGenericType(type);

            if (dt == null || dt.Rows.Count == 0)
                results = Activator.CreateInstance(types) as IList;
            else if (type == null)
                results = Activator.CreateInstance(types) as IList;
            else
            {
                results = Activator.CreateInstance(types) as IList;

                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                var constructor = type.GetConstructors(flags).OrderBy(c => c.GetParameters().Length).FirstOrDefault();

                if (constructor == null)
                    return results;

                var parameters = constructor.GetParameters();
                var values = new object[parameters.Length];

                foreach (DataRow dr in dt.Rows)
                {
                    var index = 0;

                    foreach (var item in parameters)
                    {
                        object val = null;

                        if (dr[item.Name] != null && dr[item.Name] != DBNull.Value)
                            val = Convert.ChangeType(dr[item.Name], item.ParameterType);

                        values[index++] = val;
                    }

                    results?.Add(constructor.Invoke(values));
                }
            }

            return results;
        }

        /// <summary>
        /// Converts the sql data reader to the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(SqlDataReader reader)
        {
            List<T> results;

            if (reader == null || !reader.HasRows)
                results = new List<T>();
            else
            {
                results = new List<T>();

                var properties = typeof(T).GetProperties();

                while (reader.Read())
                {
                    var t = Activator.CreateInstance<T>();

                    foreach (var property in properties)
                        property.SetValue(t, reader[property.Name] is DBNull ? null : reader[property.Name]);

                    results.Add(t);
                }

                reader.Close();
            }

            return results;
        }

        /// <summary>
        /// Converts the sql data reader to the ilist.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList ToList(SqlDataReader reader, Type type)
        {
            IList results;
            var masterType = typeof(List<>);
            var listType = masterType.MakeGenericType(type);

            if (reader == null || !reader.HasRows)
                results = Activator.CreateInstance(listType) as IList;
            else if (type == null)
                results = Activator.CreateInstance(listType) as IList;
            else
            {
                results = Activator.CreateInstance(listType) as IList;

                var constructor = type
                    .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .OrderBy(c => c.GetParameters().Length)
                    .First();
                var parameters = constructor.GetParameters();
                var values = new object[parameters.Length];

                while (reader.Read())
                {
                    var index = 0;

                    foreach (var item in parameters)
                    {
                        var val = reader[item.Name];

                        if (val != DBNull.Value)
                            val = Convert.ChangeType(val, item.ParameterType);

                        values[index++] = val;
                    }

                    results?.Add(constructor.Invoke(values));
                }

                reader.Close();
            }

            return results;
        }

        /// <summary>
        /// Converts the list to the data table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DataTable ToTable<T>(List<T> ts)
        {
            var results = new DataTable();
            var props = ts[0].GetType().GetProperties();

            foreach (var prop in props)
                results.Columns.Add(prop.Name, prop.PropertyType);

            foreach (var t in ts)
            {
                var temps = new ArrayList();

                foreach (var prop in props)
                    temps.Add(prop.GetValue(t, null));

                results.LoadDataRow(temps.ToArray(), true);
            }

            return results;
        }

        /// <summary>
        /// Converts the sql data reader to the data table.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DataTable ToTable(SqlDataReader reader)
        {
            DataTable results;

            if (reader == null || !reader.HasRows)
                results = new DataTable();
            else
            {
                results = new DataTable();

                // Adds the data table's columns.
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var column = new DataColumn
                    {
                        DataType = reader.GetFieldType(i),
                        ColumnName = reader.GetName(i)
                    };

                    results.Columns.Add(column);
                }

                // Adds the data table's content.
                while (reader.Read())
                {
                    var row = results.NewRow();

                    for (var i = 0; i < reader.FieldCount; i++)
                        row[i] = reader[i];

                    results.Rows.Add(row);
                }

                reader.Close();
            }

            return results;
        }

        /// <summary>
        /// Converts the type t object to the type s object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static S ToAnyType<T, S>(this T t)
        {
            if (t == null)
                return Activator.CreateInstance<S>();

            var result = Activator.CreateInstance<S>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(t);
                var info = typeof(S).GetProperty(property.Name);

                if (value == null || info == null)
                    continue;

                var type = info.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var val = Convert.ChangeType(value, type.GetGenericArguments()[0]);

                    info.SetValue(result, val);
                }
                else
                {
                    var val = Convert.ChangeType(value, info.PropertyType);

                    info.SetValue(result, val);
                }
            }

            return result;
        }

        /// <summary>
        /// Convert the object to the sql db type object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SqlDbType ToDbType(object obj)
        {
            SqlDbType result;

            if (obj == null)
                result = SqlDbType.NChar;
            else
            {
                result = SqlDbType.NChar;

                var type = obj.GetType();

                switch (type.Name)
                {
                    case "Boolean":
                        result = SqlDbType.Bit;
                        break;

                    case "Byte":
                        result = SqlDbType.TinyInt;
                        break;

                    case "Int16":
                        result = SqlDbType.SmallInt;
                        break;

                    case "Int32":
                        result = SqlDbType.SmallInt;
                        break;

                    case "Single":
                        result = SqlDbType.Real;
                        break;

                    case "Double":
                        result = SqlDbType.Float;
                        break;

                    case "String":
                        result = SqlDbType.NChar;
                        break;

                    case "Guid":
                        result = SqlDbType.UniqueIdentifier;
                        break;

                    case "XmlReader":
                        result = SqlDbType.Xml;
                        break;

                    case "Decimal":
                        result = SqlDbType.Money;
                        break;

                    case "DateTime":
                        result = SqlDbType.DateTime2;
                        break;

                    case "Byte[]":
                        result = SqlDbType.Binary;
                        break;

                    case "Object":
                        result = SqlDbType.Variant;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the name value collection to the idictionary.
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static IDictionary<string, string[]> ToDictionary(this NameValueCollection pairs)
        {
            return pairs.AllKeys.ToDictionary(k => k, pairs.GetValues);
        }

        /// <summary>
        /// Converts the name value collection to the ilookup.
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static ILookup<string, string[]> ToLookup(this NameValueCollection pairs)
        {
            return pairs.AllKeys.ToLookup(t => t, pairs.GetValues);
        }

        /// <summary>
        /// Converts the idictionary to the name value collection.
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static NameValueCollection ToNameValueCollection(this IDictionary<string, string[]> pairs)
        {
            var result = new NameValueCollection();

            foreach (var pair in pairs)
                foreach (var val in pair.Value)
                    result.Add(pair.Key, val);

            return result;
        }

        /// <summary>
        /// Converts the ilookup to the name value collection.
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static NameValueCollection ToNameValueCollection(this ILookup<string, string[]> pairs)
        {
            var result = new NameValueCollection();

            foreach (var pair in pairs)
                foreach (var item in pair.SelectMany(s => s))
                    result.Add(pair.Key, item);

            return result;
        }

        /// <summary>
        /// Converts the name value collection to the pair string.
        /// </summary>
        /// <param name="pairs"></param>
        public static string ToNvcString(this NameValueCollection pairs)
        {
            return string.Join(Environment.NewLine, pairs.AllKeys.SelectMany(pairs.GetValues, (k, v) => k + ": " + v));
        }

        /// <summary>
        /// Converts the pair string to the name value collection.
        /// </summary>
        /// <param name="pairs"></param>
        public static NameValueCollection ToNvc(string pairs)
        {
            var kvs = Regex.Split(pairs, "\r\n", RegexOptions.IgnoreCase);
            var results = new NameValueCollection();

            foreach (var kv in kvs)
            {
                var index = kv.IndexOf(":", StringComparison.Ordinal);

                results.Add(kv.Substring(0, index), kv.Substring(index + 1));
            }

            return results;
        }
    }
}