using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace UAFMDR.UKRTAX
{
    /// <summary>Службові процедури для роботи з додатком UKRTAX</summary>
    class UKRTAXHelper
    {
        public const string APPLICATION = @"UKRTAX";

        /// <summary>Видобування опису структури (у форматі XML) з бази, яка створена додатком UKRTAX</summary>
        public static XDocument GetStructure(string origin, SqlConnection db)
		{
			string command = EXTRACT_SQL.Replace("{%origin%}", origin);
            XDocument result;
            try
			{
                if (db.State != ConnectionState.Open) db.Open();
                SqlCommand cmd = new(command, db);
                XmlReader reader = cmd.ExecuteXmlReader();
                result = XDocument.Load(reader);
            }
            catch (Exception ex) {
				throw new Exception(ex.Message, ex);
            }
            return result;
		}

        /// <summary>Видобування даних з бази, яка створена додатком UKRTAX</summary>
		public static int ImportData(SqlConnection source, SqlBoolean forced)
		{
			int result = 0;
			return result;
		}

        /// <summary>Створення оперативної бази</summary>
		public static int ExportData(SqlConnection target, XElement conditions)
        {
            int result = 0;
            return result;
        }

        public static int UpdateRelations(XElement dataset, int application, SqlConnection db)
		{
			int result = 0;
			if (dataset.Parent.Name == "dataset")
			{
                object answer = null;
				SqlCommand cmd;
				// отримати ідентифікатор батьківського набору даних
                if (db.State != ConnectionState.Open) db.Open();
                cmd = new(EXTRACT_DATASETS_ID
                    .Replace("{%application%}", application.ToString())
                    .Replace("{%dataset%}", dataset.Parent.Attribute("name").Value),
                    db);
                answer = cmd.ExecuteScalar();
                int ParentDataSetID = 0;
                if (answer != null && answer != DBNull.Value)
                    ParentDataSetID = Convert.ToInt32(answer);
                // отримати ідентифікатор набору даних
                if (db.State != ConnectionState.Open) db.Open();
                cmd = new(EXTRACT_DATASETS_ID
                    .Replace("{%application%}", application.ToString())
                    .Replace("{%dataset%}", dataset.Attribute("name").Value),
                    db);
                answer = cmd.ExecuteScalar();
                int DataSetID = 0;
                if (answer != null && answer != DBNull.Value)
                    DataSetID = Convert.ToInt32(answer);
				result++;
            }

            return result;
		}

		/// <summary>Побудова запиту для видобування набору даних</summary>
		/// <returns></returns>
		public static string BuildQuery(string dataset, SqlConnection db)
		{
			// видобування структури набору даних
			if (db.State != ConnectionState.Open) db.Open();
			string command = UAFMDRHelper.BuildExtractDS_SQL(APPLICATION, dataset);
			SqlCommand cmd = new(command, db);
			object answer = cmd.ExecuteScalar();
			if (answer == null || answer == DBNull.Value) 
				throw new Exception("Dataset not exist!");
			XDocument structure;
            structure = XDocument.Parse(answer.ToString());
            structure.Root.Attribute("name").Value = dataset;
            if (structure.Root.Attribute("name").Value != dataset)
                throw new Exception("Structure is invalid!");

            string result = @"SELECT"+Environment.NewLine;
			// видобування ключових полей
			Dictionary<string, int> keys = new();
			foreach (XElement key in structure.Root.Element("key").Nodes())
			{
				keys.Add(key.Attribute(
					"COLUMN_NAME").ToString(),
                    (int)key.Attribute("ORDINAL_POSITION")
                );
			}
            // видобування усіх полей
            //Dictionary<string, XElement> columns = new();
            foreach (var item in structure.Root.Element("columns").Nodes())
            {
                
            }

            return result;
		}

		#region SQL

		private const string UPDATE_LINK = @"

";

        private const string EXTRACT_DATASETS_ID = @"
SELECT Id
FROM ds.dataset
WHERE {%application%}=2 AND Name='{%dataset%}' AND _Status>0";

        private const string EXTRACT_SQL = @"
/*
	Видобування до XML структури бази даних
	- Зауваження: таблиці, ім'я яких починаються з 'w', виключаються
*/
SELECT
	(
		SELECT CASE WHEN COUNT(TABLE_NAME) = 3 THEN 'UKRTAX' ELSE 'UNKNOWN' END
		  FROM {%origin%}.INFORMATION_SCHEMA.TABLES
		  WHERE TABLE_SCHEMA = 'dbo' 
			AND TABLE_TYPE = 'BASE TABLE'  
			AND TABLE_NAME IN ('SYSTAB_JOURNAL', 'NDI', 'MNGUNIT')	
	) as '@application',
	(
		SELECT
			t.TABLE_NAME as '@name',
			(
				SELECT 
					  COLUMN_NAME
					  ,ORDINAL_POSITION
					  ,IS_NULLABLE
					  ,DATA_TYPE
					  ,CHARACTER_MAXIMUM_LENGTH
					  ,CHARACTER_OCTET_LENGTH
					  ,NUMERIC_PRECISION
					  ,NUMERIC_PRECISION_RADIX
					  ,NUMERIC_SCALE
					  ,DATETIME_PRECISION
					  ,CHARACTER_SET_CATALOG
					  ,CHARACTER_SET_SCHEMA
					  ,CHARACTER_SET_NAME
					  ,COLLATION_NAME
				FROM {%origin%}.INFORMATION_SCHEMA.COLUMNS c
				WHERE 
					c.TABLE_CATALOG = t.TABLE_CATALOG 
					AND c.TABLE_NAME = t.TABLE_NAME
				ORDER BY c.ORDINAL_POSITION
				FOR XML RAW('column'), ROOT('columns'), TYPE	
			),
			(
				SELECT k.COLUMN_NAME, k.ORDINAL_POSITION
				FROM {%origin%}INFORMATION_SCHEMA.KEY_COLUMN_USAGE k
				WHERE 
					k.TABLE_CATALOG = t.TABLE_CATALOG 
					AND k.TABLE_NAME = t.TABLE_NAME 
					AND CONSTRAINT_SCHEMA = 'dbo'
					AND CONSTRAINT_NAME = 'PK_' + TABLE_NAME
				ORDER BY k.ORDINAL_POSITION
				FOR XML RAW('field'), ROOT('key'), TYPE	
			)
        FROM {%origin%}.INFORMATION_SCHEMA.TABLES AS t
        WHERE 
			t.TABLE_CATALOG = d.CATALOG_NAME 
			AND TABLE_SCHEMA = 'dbo' 
			AND t.TABLE_TYPE = 'BASE TABLE'
			AND t.TABLE_NAME not like 'w%' -- виключити спеціальні таблиці
        ORDER BY t.TABLE_NAME
		FOR XML PATH('dataset'), TYPE
	)
FROM {%origin%}.INFORMATION_SCHEMA.SCHEMATA as d
	WHERE d.SCHEMA_NAME = 'dbo' 
FOR XML PATH('database'), TYPE;
	";

        #endregion SQL

    }
}
