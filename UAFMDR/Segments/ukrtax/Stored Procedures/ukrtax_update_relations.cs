using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml.Linq;
using UAFMDR;
using UAFMDR.UKRTAX;

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void ukrtax_update_relations (SqlXml ApplicationStructure)
    {
        /* відкрити з'єднаня з БД */
        SqlConnection db = new("context connection = true");
        db.Open();
        // get Application Id
        SqlCommand cmd = new(EXTRACT_APP_ID.Replace("{%application%}", UKRTAXHelper.APPLICATION), db);
        object result = cmd.ExecuteScalar();
        int ApplicationID = 0;
        if (result != null && result != DBNull.Value)
            ApplicationID = Convert.ToInt32(result);

        XDocument structure = XDocument.Load(ApplicationStructure.CreateReader());
        foreach (XElement dataset in structure.Descendants("dataset"))
        {
            UKRTAXHelper.UpdateRelations(dataset, ApplicationID, db);    
        }

    }

    #region SQL

    private const string EXTRACT_APP_ID = @"
    SELECT Id
    FROM ds.application
    WHERE Name='{%application%}' AND _Status>0
    ";

    #endregion SQL
}
