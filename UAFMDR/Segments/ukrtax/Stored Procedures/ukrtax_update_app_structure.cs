using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml.Linq;
using UAFMDR;
using UAFMDR.UKRTAX;

public partial class StoredProcedures
{
    [SqlProcedure(Name = "ukrtax_update_app_structure")]
    /// <summary>Оновлення налаштувань структури сховища для роботи з додатком UKRTAX</summary>
    /// <param name="source">
    /// Джерело даних про структуру. Якщо вказано:
    /// <database application="UKRTAX" source="ім'я бази-джерела">
    /// то структура видобувається запитом з вказаної бази,
    /// інакше - безпосередньо з параметру
    /// </param>
    /// <param name="forced"> Логічний - ознака примусового оновлення 
    /// </param>
    /// <returns>1 якщо структуру змінено, 0 - якщо вона не змінилася, або помилку (через SqlContext)</returns>
    public static void ukrtax_update_app_structure(SqlXml source, SqlBoolean forced)
    {       
        /* відкрити з'єднаня з БД */
        SqlConnection db = new("context connection = true");
        db.Open();        
        
        XDocument structure = XDocument.Load(source.CreateReader());
        if (structure.Root.Attribute("source") != null)
        {
            string origin = (string)structure.Root.Attribute("source");
            structure = UKRTAXHelper.GetStructure(origin, db);
        }

        int updated = UAFMDRHelper.UpdateStructure(UKRTAXHelper.APPLICATION, structure, db, forced);

        if (!forced && (updated > 0)) {
            foreach (XElement dataset in structure.Root.Elements())
            {
                int changed = UAFMDRHelper.UpdateStructure(
                    UKRTAXHelper.APPLICATION,
                    dataset.Attribute("name").Value,
                    new XDocument(dataset), db, forced
                );
                if (!forced && (changed > 0)) {
                    foreach (XElement column in dataset.Element("columns").Elements()) {
                        UAFMDRHelper.UpdateStructure(
                            UKRTAXHelper.APPLICATION,
                            dataset.Attribute("name").Value,
                            column.Attribute("COLUMN_NAME").Value,
                            new XDocument(column), db, forced
                        );
                    }
                }
            }
        }

    }

}
