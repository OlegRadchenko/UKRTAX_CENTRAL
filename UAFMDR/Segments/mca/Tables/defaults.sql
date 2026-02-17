CREATE TABLE [mca].[defaults]
(
	[Name] VARCHAR(32) NOT NULL PRIMARY KEY,
	[Value] VARCHAR(MAX) NULL,
	[_Status] TINYINT NOT NULL REFERENCES mca.status(Id) DEFAULT 1,
	[CHANGE_SESSION] [int] NOT NULL REFERENCES mca.session(ID) DEFAULT mca.DefaultSession(),
	[SysStart] DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
	[SysEnd] DATETIME2 (7) GENERATED ALWAYS AS ROW END NOT NULL,
	PERIOD FOR SYSTEM_TIME ([SysStart], [SysEnd])
)
WITH (SYSTEM_VERSIONING = ON(HISTORY_TABLE=[mca].[defaults_HISTORY], DATA_CONSISTENCY_CHECK=ON))
GO

EXEC sys.sp_addextendedproperty 
	@name=N'Description', 
	@value=N'Значення по замовчуванню' , 
	@level0type=N'SCHEMA',
	@level0name=N'mca', 
	@level1type=N'TABLE',
	@level1name=N'defaults'
GO
