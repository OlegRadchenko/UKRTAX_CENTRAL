-- Виконаваці, які уповноважені на зміну даних у сховищі
CREATE TABLE [mca].[doer]
(
	[ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Alias] VARCHAR(30) NULL,
	[Active] bit NOT NULL,
	[DoerData] XML,
	[SysStart] DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
	[SysEnd] DATETIME2 (7) GENERATED ALWAYS AS ROW END NOT NULL, 
	PERIOD FOR SYSTEM_TIME ([SysStart], [SysEnd])
)
WITH (SYSTEM_VERSIONING = ON(HISTORY_TABLE=[mca].[doer_HISTORY], DATA_CONSISTENCY_CHECK=ON))
GO

EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Виконавці' , @level0type=N'SCHEMA',@level0name=N'mca', @level1type=N'TABLE',@level1name=N'doer'
GO

CREATE NONCLUSTERED INDEX [DoerActive] ON [mca].[doer] ([Active])
GO	

ALTER TABLE [mca].[doer] ADD CONSTRAINT [DF_doer_active]  DEFAULT ((1)) FOR [Active]
GO

CREATE NONCLUSTERED INDEX [DoerAlias] ON [mca].[doer]	([Alias])
GO	 