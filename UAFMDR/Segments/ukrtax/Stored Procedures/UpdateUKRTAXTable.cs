using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

public partial class Triggers
{
    /// <summary>
    /// Оновлення даних UKRTAX
    /// </summary>
    /// <returns>кількість оновлених записів</returns>
    public static void UpdateUKRTAXTable ()
    {
        var tc = SqlContext.TriggerContext;
        string xml = tc.EventData.ToString();
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        string tableName = doc.SelectSingleNode("/EVENT_INSTANCE/ObjectName")?.InnerText;
        string schemaName = doc.SelectSingleNode("/EVENT_INSTANCE/SchemaName")?.InnerText;
        // Replace with your own code
        SqlContext.Pipe.Send("Trigger FIRED");
    }
}

