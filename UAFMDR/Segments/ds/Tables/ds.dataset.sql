CREATE TABLE [ds].[dataset]
(
	[Id] INT IDENTITY(1,1) NOT NULL ,
	[Application] INT NOT NULL,
	[Name] NVARCHAR(128) NOT NULL,
	[Structure] XML,
	[_Status] TINYINT NOT NULL DEFAULT 1,
	[CHANGE_SESSION] [int] NOT NULL DEFAULT mca.DefaultSession(),
	[SysStart] DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
	[SysEnd] DATETIME2 (7) GENERATED ALWAYS AS ROW END NOT NULL, 
    CONSTRAINT [FK_dataset_application] FOREIGN KEY ([Application]) REFERENCES [ds].[application]([Id]), 
    CONSTRAINT [FK_dataset_session] FOREIGN KEY ([CHANGE_SESSION]) REFERENCES [mca].[session] ([ID]), 
    CONSTRAINT [FK_dataset_status] FOREIGN KEY ([_Status]) REFERENCES [mca].[status]([Id]), 
    PRIMARY KEY ([Id]), 
	PERIOD FOR SYSTEM_TIME ([SysStart], [SysEnd])
)
WITH (SYSTEM_VERSIONING = ON(HISTORY_TABLE=[ds].[dataset_HISTORY], DATA_CONSISTENCY_CHECK=ON))
GO

CREATE UNIQUE INDEX [IX_dataset_Name] ON [ds].[dataset] ([Name], [Application])
GO

CREATE INDEX [IX_dataset_Status] ON [ds].[dataset] ([_Status])
GO