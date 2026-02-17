using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml.Linq;
using UAFMDR.UKRTAX;

public partial class StoredProcedures
{

    [SqlProcedure(Name = "ukrtax_import_fm_database")]
    /// <summary>Імпорт даних із оперативної бази Field-Map UKRTAX</summary>
    /// <param name="source"> Джерело даних </param>
    /// <param name="forced"> Логічний - ознака примусового оновлення  </param>
    public static void ukrtax_import_fm_database (SqlXml source, SqlBoolean forced)
    {
        XDocument SourceDataBase = XDocument.Load(source.CreateReader());
        string server = (string)SourceDataBase.Root.Attribute("server");
        string DBName = (string)SourceDataBase.Root.Attribute("name");
        SqlConnection SourceConnection;
        if (server == "local") {
            SourceConnection = new("context connection = true");
        }
        else
        {
            SourceConnection = new(server);
        }
        SourceConnection.Open();
        SourceConnection.ChangeDatabase(DBName);
        UKRTAXHelper.ImportData(SourceConnection, forced);
    }
}
