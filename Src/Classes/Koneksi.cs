using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace SimplifikasiFID.Classes
{
    public class Koneksi
    {
        public static SqlConnection GetKoneksi()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["EntitiesConnection"].ToString();
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            else
            {
                conn.Open();
            }

            return conn;
        }

        public static DataTable GetDataTable(string SQL)
        {
            try
            {

                DataTable dtTable = new DataTable();
                SqlConnection cnn = new SqlConnection();
                cnn.ConnectionString = ConfigurationManager.ConnectionStrings["EntitiesConnection"].ToString();

                if (cnn.State == System.Data.ConnectionState.Open)
                {
                    cnn.Close();
                }
                else
                {
                    cnn.Open();
                }


                SqlCommand sCommand = new SqlCommand(SQL, cnn);
                sCommand.CommandTimeout = 300;
                SqlDataReader dtReader = sCommand.ExecuteReader();
                dtTable.Load(dtReader);
                cnn.Close();

                return dtTable;
            }
            catch (Exception ex)
            {
                throw new Exception("Error : " + ex.ToString());
            }
        }

        public static Boolean execQuery(string SQL)
        {

            SqlConnection cnn = new SqlConnection();
            cnn.ConnectionString = ConfigurationManager.ConnectionStrings["EntitiesConnection"].ToString();

            if (cnn.State == System.Data.ConnectionState.Open)
            {
                cnn.Close();
            }
            else
            {
                cnn.Open();
            }

            try
            {

                SqlCommand sCommand = new SqlCommand(SQL, cnn);
                sCommand.ExecuteNonQuery();
                cnn.Close();
                return true;

            }
            catch (Exception ex)
            {

                cnn.Close();
                return false;

            }

        }

        public static string getScalarValue(string SQL)
        {

            SqlConnection cnn = new SqlConnection();
            cnn.ConnectionString = ConfigurationManager.ConnectionStrings["EntitiesConnection"].ToString();

            if (cnn.State == System.Data.ConnectionState.Open)
            {
                cnn.Close();
            }
            else
            {
                cnn.Open();
            }

            try
            {

                SqlCommand sCommand = new SqlCommand(SQL, cnn);
                string sVAL = sCommand.ExecuteScalar().ToString();
                return sVAL;

            }
            catch (Exception ex)
            {

                return null;

            }

        }
    }
}