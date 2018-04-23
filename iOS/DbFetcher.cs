using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(AltiusDashboard.iOS.DbFetcher))]
namespace AltiusDashboard.iOS
{
	public class DbFetcher : IDbDataFetcher
	{
        string CONN_STRING = @"Server=127.0.0.1\" + SettingVariable.Instance + ";Database="
                       + SettingVariable.Database + ";User Id=" + SettingVariable.Username + ";Password=" + SettingVariable.Password + ";Pooling=true";
        string errorMessage = "";

        public List<ItemReport> getItemReport(string query)
        {
            List<ItemReport> data = null;

            SqlConnection connection = new SqlConnection(CONN_STRING);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        data = new List<ItemReport>();
                        while (reader.Read())
                        {
                            ItemReport itemReport = new ItemReport();
                            itemReport.Id = reader[0].ToString();
                            itemReport.Title = reader[1].ToString();
                            data.Add(itemReport);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        errorMessage = e.Message;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    errorMessage = e.Message;
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                errorMessage = e.Message;
            }
            finally
            {
                connection.Dispose();
            }

            return data;
        }

        public List<MainReport> getMainReport(string query)
        {
            List<MainReport> data = null;
            SqlConnection connection = new SqlConnection(CONN_STRING);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        data = new List<MainReport>();
                        while (reader.Read())
                        {
                            MainReport mainReport = new MainReport();
                            mainReport.Dept = reader[0].ToString();

                            data.Add(mainReport);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        errorMessage = e.Message;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    errorMessage = e.Message;
                }
                finally
                {
                    command.Dispose();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                errorMessage = e.Message;
            }
            finally
            {
                connection.Dispose();
            }

            return data;
        }

        public Reportlist getQueryReport(string query)
        {
            Reportlist reportlist = new Reportlist();
            SqlConnection connection = new SqlConnection(CONN_STRING);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            reportlist.Query1 = reader[0].ToString();
                            reportlist.QueryClass = reader[1].ToString();
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        errorMessage = e.Message;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    errorMessage = e.Message;
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                errorMessage = e.Message;
            }
            finally
            {
                connection.Dispose();
            }

            return reportlist;
        }

        public List<ChoiceItem> getChoiceData(string query)
        {
            List<ChoiceItem> list = null;
            SqlConnection connection = new SqlConnection(CONN_STRING);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        list = new List<ChoiceItem>();
                        int fieldCount = reader.FieldCount;
                        while (reader.Read())
                        {
                            ChoiceItem item = new ChoiceItem();

                            string kode = "";
                            if (!reader.IsDBNull(0))
                            {
                                kode = reader[0].ToString();
                            }
                            item.Kode = kode;


                            string name = "";
                            if (fieldCount > 1)
                            {
                                string second = reader[1].ToString();
                                if (second != null && second.Length > 0 && second != kode)
                                {
                                    name = second + "-" + kode;
                                }
                                else name = kode;
                            }
                            else
                            {
                                name = kode;
                            }
                            item.Value = name;


                            list.Add(item);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        errorMessage = e.Message;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    errorMessage = e.Message;
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                errorMessage = e.Message;
            }
            finally
            {
                connection.Dispose();
            }

            return list;
        }

        public List<User> getUser(string query)
        {
            List<User> data = null;
            SqlConnection connection = new SqlConnection(CONN_STRING);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        data = new List<User>();
                        while (reader.Read())
                        {
                            User user = new User();
                            user.Kode = reader[0].ToString();
                            user.Nama = reader[1].ToString();
                            user.Username = reader[2].ToString();

                            data.Add(user);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        errorMessage = e.Message;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    errorMessage = e.Message;
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                errorMessage = e.Message;
            }
            finally
            {
                connection.Dispose();
            }

            return data;
        }

        public ReportModel getReportData(string query)
        {
            ReportModel reportModel = null;
            int columnCount = 0;
            SqlConnection connection = new SqlConnection(CONN_STRING);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        columnCount = reader.FieldCount;
                        reportModel = new ReportModel();
                        bool isHeader = true;
                        while (reader.Read())
                        {
                            Model model = new Model();

                            for (int i = 0; i < columnCount; i++)
                            {
                                string dataType = reader.GetDataTypeName(i).ToLower();
                                string columnName = reader.GetName(i).ToUpper();

                                if (isHeader)
                                {
                                    Column column = new Column();
                                    column.ColumnName = columnName;
                                    column.DataType = dataType;
                                    reportModel.Headers.Add(column);

                                    if (dataType == "decimal" || dataType == "int" || dataType == "money")
                                    {
                                        reportModel.Sum.Add(new Summary(0));
                                    }
                                    else
                                    {
                                        reportModel.Sum.Add(new Summary(-999999));
                                    }
                                }

                                string dataValue = "";

                                if (!reader.IsDBNull(i))
                                {
                                    switch (dataType)
                                    {
                                        case "varchar":
                                            dataValue = Encoding.UTF8.GetString(reader.GetSqlString(i).GetNonUnicodeBytes());
                                            break;
                                        case "decimal":
                                            dataValue = "" + (double)reader.GetDecimal(i);
                                            reportModel.Sum[i].Value += (double)reader.GetDecimal(i);
                                            break;
                                        case "int":
                                            dataValue = "" + reader.GetInt32(i);
                                            reportModel.Sum[i].Value += (double)reader.GetInt32(i);
                                            break;
                                        case "money":
                                            dataValue = "" + string.Format(CultureInfo.InvariantCulture, "{0:N0}", (double)reader.GetDecimal(i));
                                            reportModel.Sum[i].Value += (double)reader.GetDecimal(i);
                                            break;
                                        case "datetime":
                                            dataValue = "" + reader.GetDateTime(i).ToString("yyyy-MM-dd");
                                            break;
                                        case "boolean":
                                            dataValue = "" + reader.GetBoolean(i);
                                            break;
                                        case "text":
                                            dataValue = Encoding.UTF8.GetString(reader.GetSqlString(i).GetNonUnicodeBytes());
                                            break;
                                    }
                                }

                                MethodInfo methodInfo = model.GetType().GetMethod("setmodel" + (i + 1));
                                methodInfo.Invoke(model, new object[] { dataValue });
                            }

                            isHeader = false;
                            reportModel.Data.Add(model);
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        errorMessage = e.Message;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    errorMessage = e.Message;
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                errorMessage = e.Message;
            }
            finally
            {
                connection.Dispose();
            }


            return reportModel;
        }

        public List<Branch> getBranch(string query)
        {
            List<Branch> data = null;
            SqlConnection connection = new SqlConnection(CONN_STRING);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        data = new List<Branch>();
                        while (reader.Read())
                        {
                            Branch branch = new Branch();
                            branch.KodeCabang = reader[0].ToString();
                            branch.NamaCabang = reader[1].ToString();
                            branch.KodeStaff = reader[2].ToString();

                            data.Add(branch);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        errorMessage = e.Message;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    errorMessage = e.Message;
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                errorMessage = e.Message;
            }
            finally
            {
                connection.Dispose();
            }

            return data;
        }

        public List<DashboardQuery> getDashboardQueries(string query)
        {
            List<DashboardQuery> data = null;
            SqlConnection connection = new SqlConnection(CONN_STRING);
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        data = new List<DashboardQuery>();
                        while (reader.Read())
                        {
                            DashboardQuery dashboardQuery = new DashboardQuery();
                            dashboardQuery.Id = reader[0].ToString();
                            dashboardQuery.Title = reader[1].ToString();
                            dashboardQuery.Reportlist = reader[2].ToString();
                            dashboardQuery.Judul = reader[3].ToString();
                            dashboardQuery.Query1 = reader[4].ToString();

                            data.Add(dashboardQuery);

                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        errorMessage = e.Message;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    errorMessage = e.Message;
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                errorMessage = e.Message;
            }
            finally
            {
                connection.Dispose();
            }

            return data;
        }

        public void reloadConnectionString()
        {
            this.CONN_STRING = @"Server=127.0.0.1\" + SettingVariable.Instance + ";Database="
                + SettingVariable.Database + ";User Id=" + SettingVariable.Username + ";Password=" + SettingVariable.Password + ";Pooling=false";
        }

        public List<DashboardModel> getDashboard(string query)
        {
            List<DashboardModel> data = null;
            SqlConnection connection = new SqlConnection(CONN_STRING);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        data = new List<DashboardModel>();
                        while (reader.Read())
                        {
                            DashboardModel dashboardQuery = new DashboardModel(reader[0].ToString(), Double.Parse(reader[1].ToString()));
                            data.Add(dashboardQuery);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        errorMessage = e.Message;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    errorMessage = e.Message;
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                errorMessage = e.Message;
            }
            finally
            {
                connection.Dispose();
            }

            return data;
        }

        public string getErrorMessage()
        {
            return errorMessage;
        }
	}
}
