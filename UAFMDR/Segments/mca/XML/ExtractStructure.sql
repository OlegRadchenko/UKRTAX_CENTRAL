/*
	Видобування до XML структури бази даних
	- Зауваження: таблиці, ім'я яких починаються з 'w', виключаються
*/
SELECT
	d.CATALOG_NAME AS '@name',
    GETDATE() AS '@exctacted',
	ORIGINAL_LOGIN() AS '@user',
	(
		SELECT
			t.[TABLE_NAME] as '@name',
			(
				SELECT 
					  [COLUMN_NAME] as "NAME"
					  ,[ORDINAL_POSITION]
					  ,[IS_NULLABLE]
					  ,[DATA_TYPE]
					  ,[CHARACTER_MAXIMUM_LENGTH]
					  ,[CHARACTER_OCTET_LENGTH]
					  ,[NUMERIC_PRECISION]
					  ,[NUMERIC_PRECISION_RADIX]
					  ,[NUMERIC_SCALE]
					  ,[DATETIME_PRECISION]
					  ,[CHARACTER_SET_CATALOG]
					  ,[CHARACTER_SET_SCHEMA]
					  ,[CHARACTER_SET_NAME]
					  ,[COLLATION_NAME]
				FROM [INFORMATION_SCHEMA].[COLUMNS] c
				WHERE 
					c.[TABLE_CATALOG] = t.[TABLE_CATALOG] 
					AND c.[TABLE_NAME] = t.[TABLE_NAME]
				ORDER BY c.[ORDINAL_POSITION]
				FOR XML RAW('column'), ROOT('columns'), TYPE	
			),
			(
				SELECT k.[COLUMN_NAME], k.[ORDINAL_POSITION]
				FROM [INFORMATION_SCHEMA].[KEY_COLUMN_USAGE] k
				WHERE 
					k.[TABLE_CATALOG] = t.[TABLE_CATALOG] 
					AND k.[TABLE_NAME] = t.[TABLE_NAME] 
					AND [CONSTRAINT_SCHEMA] = 'dbo'
					AND [CONSTRAINT_NAME] = 'PK_' + [TABLE_NAME]
				ORDER BY k.[ORDINAL_POSITION]
				FOR XML RAW('field'), ROOT('key'), TYPE	
			)
        FROM [INFORMATION_SCHEMA].[TABLES] AS t
        WHERE 
			t.[TABLE_CATALOG] = d.[CATALOG_NAME] 
			AND [TABLE_SCHEMA] = 'dbo' 
			AND t.[TABLE_TYPE] = 'BASE TABLE'
			AND t.TABLE_NAME not like 'w%' -- виключити спеціальні таблиці
        ORDER BY t.[TABLE_NAME]
		FOR XML PATH('table'), TYPE
	)
FROM [INFORMATION_SCHEMA].[SCHEMATA] as d
WHERE d.[SCHEMA_NAME] = 'dbo' 
FOR XML PATH('database'), TYPE;