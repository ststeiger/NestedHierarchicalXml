
using System;
using System.Collections.Generic;
using System.Text;


namespace NestedHierarchicalXml
{


    class SQL
    {


        public static System.Data.Common.DbDataAdapter GetDataAdapter(string strSQL)
        {
            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.DataSource = System.Environment.MachineName;
            csb.InitialCatalog = "SwissRe_Test_V3";
            csb.IntegratedSecurity = true;
            if (!csb.IntegratedSecurity)
            {
                csb.UserID = "ApertureWebServices";
                csb.Password = "";
            } // End if (!csb.IntegratedSecurity)

            return new System.Data.SqlClient.SqlDataAdapter(strSQL, csb.ConnectionString);
        } // End Function GetDataAdapter


        public static System.Data.DataSet GetDataSet(string strSQL)
        {
            System.Data.DataSet ds = new System.Data.DataSet();

            using (System.Data.Common.DbDataAdapter da = GetDataAdapter(strSQL))
            {
                da.Fill(ds);
            } // End Using da

            return ds;
        }


    }


}
