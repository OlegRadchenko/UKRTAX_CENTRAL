using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using System.Xml.Linq;

namespace UAFMDR
{
    /// <summary>
    /// Службові засоби для додатку UAFDMR
    /// </summary>
    class UAFMDRHelper
    {

        /// <summary>Оновлення опису структури елемента даних (у форматі XML)</summary>
        /// <returns>кількість оновлених записів</returns>
        public static int UpdateStructure(string application, string dataset, string element, 
            XDocument structure, SqlConnection db, SqlBoolean forced)
        {
            int result = 0; 
            string command = GET_ELSTRUCT_SQL
                .Replace("{%application%}", application)
                .Replace("{%dataset%}", dataset)
                .Replace("{%element%)", element);
            XDocument old = new();
            using (SqlCommand cmd = new(command, db))
            {
                XmlReader reader = cmd.ExecuteXmlReader();
                if (reader != null && reader.Read())
                {
                    old = XDocument.Load(reader);
                }

                if (!forced && !XNode.DeepEquals(old.Root, structure.Root))
                {
                    string updateText = UPDATE_ELEMENT_SQL
                        .Replace("{%application%}", application)
                        .Replace("{%dataset%}", dataset)
                        .Replace("{%element%}", element);
                    SqlCommand update = new(updateText, db);
                    update.Parameters.Add("@structure", SqlDbType.Xml)
                        .Value = new SqlXml(structure.CreateReader());
                    try
                    {
                        using var r = update.ExecuteReader();
                        while (r.Read()) result++;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }
            }

            return result;
        }

        /// <summary>Оновлення опису структури набору даних (у форматі XML)</summary>
        /// <returns>кількість оновлених записів</returns>
        public static int UpdateStructure(string application, string dataset, 
            XDocument structure, SqlConnection db, SqlBoolean forced)
        {
            int result = 0;
            // Зчитування переліку наборів даних, які зберігаються напряму
            Dictionary<string, string> directs = new();
            using (SqlCommand getDefault = new("SELECT mca.GetDefault('DIRECT_DS')"))
            {
                XmlReader reader = getDefault.ExecuteXmlReader();
                if (reader != null && reader.Read())
                {
                    var direct = XDocument.Load(reader);
                    foreach (var item in direct.Root.Elements())
                    {
                        directs.Add(item.Attribute("name").Value, item.Attribute("store").Value);
                    }
                }
            }
            // зчитування власне структури набору даних
            string command = GET_DS_STRUCT_SQL
                .Replace("{%application%}", application)
                .Replace("{%dataset%}", dataset);
            XDocument old = new();
            using (SqlCommand cmd = new(command, db))
            {
                XmlReader reader = cmd.ExecuteXmlReader();
                if (reader != null && reader.Read())
                {
                    old = XDocument.Load(reader);
                }
                if (!forced && !XNode.DeepEquals(old.Root, structure.Root))
                {
                    // оновлення запису в структурі ds.dataset
                    string updateText = UPDATE_DATASET_SQL
                        .Replace("{%application%}", application)
                        .Replace("{%dataset%}", dataset);
                    SqlCommand update = new(updateText, db);
                    update.Parameters.Add("@structure", SqlDbType.Xml)
                        .Value = new SqlXml(structure.CreateReader());
                    try
                    {
                        using var r = update.ExecuteReader();
                        while (r.Read()) result++;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                    // оновлення мапінгу набору даних
                    if (directs.ContainsKey(dataset))
                    {
                        var store = directs[dataset];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Оновлення опису структури додатка (у форматі XML)</summary>
        /// <returns>кількість оновлених записів</returns>
        public static int UpdateStructure(string application, XDocument structure, 
            SqlConnection db, SqlBoolean forced)
        {

            int result = 0;
            string command = GET_APPSTRUCT_SQL.Replace("{%application%}", application);
            XDocument old = new();
            using (SqlCommand cmd = new(command, db))
            {
                XmlReader reader = cmd.ExecuteXmlReader();
                if (reader != null && reader.Read()) {
                    old = XDocument.Load(reader);
                }
                    
                if (!forced && !XNode.DeepEquals(old.Root, structure.Root)) {
                    string updateText = UPDATE_APPLICATION_SQL.Replace("{%application%}", application);
                    SqlCommand update = new(updateText, db);
                    update.Parameters.Add("@structure", SqlDbType.Xml)
                        .Value = new SqlXml(structure.CreateReader());
                    try
                    {
                        using var r = update.ExecuteReader();
                        while (r.Read()) result++;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }           
            }

            return result;
        }

        /// <summary>Побудова рядка зв'язку</summary>
        /// <returns>Рядок вигляду ""%prefix% JOIN %dataset% as %name% ON %condition%"</returns>
        public static string BuildJOIN(string prefix, string dataset, string name, string condition)
        {

            return JOIN_TEMPLATE
                .Replace("%prefix%", prefix)
                .Replace("%dataset%", dataset)
                .Replace("%name%", name)
                .Replace("%condition%", condition);
        }

        /// <summary>Побудова запиту на видобування структури набору даних додатка</summary>
        public static string BuildExtractDS_SQL(string application, string dataset)
        {
            return EXTRACT_DS_SQL
                .Replace("%application%", application)
                .Replace("%dataset%", dataset);
        }

        #region SQL

        private const string EXTRACT_DS_SQL = @"
            SELECT Structure 
            FROM ds.dataset 
            WHERE Application='%application%' AND Name='%dataset%'";

        private const string JOIN_TEMPLATE = @"%prefix% JOIN %dataset% as %name% ON %condition%";
        
        private const string GET_ELSTRUCT_SQL = @"
		    SELECT e.Structure 
		    FROM ds.element e 
            INNER JOIN ds.dataset ds ON e.DataSet = ds.Id
            INNER JOIN ds.application app ON ds.Application = app.Id 
		    WHERE  
                app.Name='{%application%}'
                AND ds.Name='{%dataset%}'
                AND e.Name='{%element%}'
                AND ds.Structure IS NOT NULL           
        ";

        private const string GET_DS_STRUCT_SQL = @"
		    SELECT ds.Structure 
		    FROM ds.dataset ds 
            INNER JOIN ds.application app ON ds.Application = app.Id
		    WHERE 
                app.Name='{%application%}'
                AND ds.Name='{%dataset%}'
                AND ds.Structure IS NOT NULL           
        ";

        private const string GET_APPSTRUCT_SQL = @"
		    SELECT Structure 
		    FROM ds.application 
		    WHERE Name='{%application%}' AND Structure IS NOT NULL           
        ";

        private const string UPDATE_ELEMENT_SQL = @"

            DECLARE @newIds TABLE (ID INT);
            DECLARE @application INT
            DECLARE @dataset INT

			SELECT TOP (1) @application = Id
			FROM ds.application
			WHERE Name = '{%application%}';

			SELECT TOP (1) @dataset = Id
			FROM ds.dataset
			WHERE Application = @application AND Name = '{%dataset%}';
            
			UPDATE ds.element
            SET Structure = @structure
            OUTPUT INSERTED.Id INTO @newIds
            WHERE DataSet = @dataset AND Name = '{%element%}';

            IF @@ROWCOUNT = 0
			BEGIN
                INSERT INTO ds.element (DataSet, Name, Structure) 
                OUTPUT INSERTED.Id INTO @newIds
                VALUES (@dataset, '{%element%}', @structure);
			END

            SELECT * FROM @newIds;
        ";

        private const string UPDATE_DATASET_SQL = @"

            DECLARE @newIds TABLE (ID INT);
            DECLARE @application INT

			SELECT TOP (1) @application = Id
			FROM ds.application
			WHERE Name = '{%application%}';
            
			UPDATE ds.dataset
            SET Structure = @structure
            OUTPUT INSERTED.Id INTO @newIds
            WHERE Name = '{%dataset%}';

            IF @@ROWCOUNT = 0
			BEGIN
                INSERT INTO ds.dataset (Application, Name, Structure) 
                OUTPUT INSERTED.Id INTO @newIds
                VALUES (@application, '{%dataset%}', @structure);
			END

            SELECT * FROM @newIds;
        ";

        private const string UPDATE_APPLICATION_SQL = @"

            DECLARE @newIds TABLE (ID INT);
            
            UPDATE ds.application
            SET Structure = @structure
            OUTPUT INSERTED.Id INTO @newIds
            WHERE Name = '{%application%}';

            IF @@ROWCOUNT = 0
                INSERT INTO ds.application (Name, Structure) 
                OUTPUT INSERTED.Id INTO @newIds
                VALUES ('{%application%}', @structure);

            SELECT * FROM @newIds;
        ";

        #endregion SQL

    }

}
