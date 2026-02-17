-- Сесії внесення змін до сховища
CREATE TABLE [mca].[session]
(
	[ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Start] DATETIME2 (7) DEFAULT SYSUTCDATETIME ( ) NOT NULL, 
    [Finish] DATETIME2 NULL, 
    [Doer] INT NOT NULL, 
    [Channel] INT NOT NULL ,
)
GO

EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Сесії внесення змін до сховища' , @level0type=N'SCHEMA',@level0name=N'mca', @level1type=N'TABLE',@level1name=N'session'
GO

ALTER TABLE [mca].[session] ADD  CONSTRAINT [FK_session_channel] FOREIGN KEY([Channel])
REFERENCES [mca].[channel] ([ID])
GO

ALTER TABLE [mca].[session] ADD  CONSTRAINT [FK_session_doer] FOREIGN KEY([Doer])
REFERENCES [mca].[doer] ([ID])
GO


