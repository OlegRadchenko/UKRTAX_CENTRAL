-- Канали внесення змін до сховища
CREATE TABLE [mca].[channel]
(
	[ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Name] varchar(128) NOT NULL,
	[Active] bit NOT NULL DEFAULT 1,	
	[SysStart] DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
	[SysEnd] DATETIME2 (7) GENERATED ALWAYS AS ROW END NOT NULL,
	PERIOD FOR SYSTEM_TIME ([SysStart], [SysEnd])
)
WITH (SYSTEM_VERSIONING = ON(HISTORY_TABLE=[mca].[channel_HISTORY], DATA_CONSISTENCY_CHECK=ON))
GO

EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Канали внесення змін до сховища' , @level0type=N'SCHEMA',@level0name=N'mca', @level1type=N'TABLE',@level1name=N'channel'
GO

CREATE UNIQUE NONCLUSTERED INDEX [channel_name] ON [mca].[channel]
(
	[Name] ASC,
	[Active] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO