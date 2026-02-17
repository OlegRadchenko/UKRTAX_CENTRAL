CREATE TABLE [ds].[application]
(
	[Id] INT IDENTITY(1,1) NOT NULL ,
	[Name] NVARCHAR(128) NOT NULL,
	[Structure] XML,
	[_Status] TINYINT NOT NULL DEFAULT 1,
	[CHANGE_SESSION] [int] NOT NULL DEFAULT mca.DefaultSession(),
	[SysStart] DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
	[SysEnd] DATETIME2 (7) GENERATED ALWAYS AS ROW END NOT NULL, 
    CONSTRAINT [FK_application_session] FOREIGN KEY ([CHANGE_SESSION]) REFERENCES [mca].[session]([ID]), 
    CONSTRAINT [FK_application_status] FOREIGN KEY ([_Status]) REFERENCES [mca].[status]([Id]), 
    PRIMARY KEY ([Id]),
	PERIOD FOR SYSTEM_TIME ([SysStart], [SysEnd])
)
WITH (SYSTEM_VERSIONING = ON(HISTORY_TABLE=[ds].[application_HISTORY], DATA_CONSISTENCY_CHECK=ON))

GO

CREATE UNIQUE INDEX [IX_application_Name] ON [ds].[application] ([Name])

GO

CREATE INDEX [IX_application_Status] ON [ds].[application] ([_Status])
