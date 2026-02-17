using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml.Linq;
using UAFMDR.UKRTAX;

public partial class StoredProcedures
{
    [SqlProcedure(Name = "ukrtax_export_data")]
    /// <summary>Створення оперативної бази Field-Map UKRTAX з центрального сховища</summary>
    /// <param name="target">Призначення даних</param>
    /// <param name="condition">Визначення набору даних для експорту</param>
    public static void ukrtax_export_data (SqlXml task)
    {
        XDocument Task = XDocument.Load(task.CreateReader());
        string server = (string)Task.Element("DataBase")?.Attribute("server");
        string DBName = (string)Task.Element("DataBase")?.Attribute("name");
        SqlConnection SourceConnection;
        if (server == "local")
        {
            SourceConnection = new("context connection = true");
        }
        else
        {
            SourceConnection = new(server);
        }
        SourceConnection.Open();
        SourceConnection.ChangeDatabase(DBName);
        UKRTAXHelper.ExportData(SourceConnection, Task.Root.Element("Conditions"));
    }
}
