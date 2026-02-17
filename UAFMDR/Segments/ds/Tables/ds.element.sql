CREATE TABLE [ds].[element]
(
	[Id] INT IDENTITY(1,1) NOT NULL ,
	[DataSet] INT NOT NULL,
	[Name] NVARCHAR(128) NOT NULL,
	[Structure] XML,
	[_Status] TINYINT NOT NULL DEFAULT 1,
	[CHANGE_SESSION] [int] NOT NULL DEFAULT mca.DefaultSession(),
	[SysStart] DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
	[SysEnd] DATETIME2 (7) GENERATED ALWAYS AS ROW END NOT NULL, 
    CONSTRAINT [FK_element_dataset] FOREIGN KEY ([DataSet]) REFERENCES [ds].[dataset]([Id]), 
    CONSTRAINT [FK_element_session] FOREIGN KEY ([CHANGE_SESSION]) REFERENCES [mca].[session] ([ID]), 
    CONSTRAINT [FK_element_status] FOREIGN KEY ([_Status]) REFERENCES [mca].[status]([Id]), 
    PRIMARY KEY ([Id]), 
	PERIOD FOR SYSTEM_TIME ([SysStart], [SysEnd])
)
WITH (SYSTEM_VERSIONING = ON(HISTORY_TABLE=[ds].[element_HISTORY], DATA_CONSISTENCY_CHECK=ON))

GO

CREATE UNIQUE INDEX [IX_element_Name] ON [ds].[element] ([Name], [DataSet])

GO

CREATE INDEX [IX_element_Status] ON [ds].[element] ([_Status])
