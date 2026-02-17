CREATE TABLE [ukrtax].[JOURNAL]
(
	[DataSetRow] INT REFERENCES store.DataSetRow(Id) NOT NULL,
    [ID]                      INT             NOT NULL,
    [ADD_DATE]                DATETIME        NOT NULL,
    [ADD_USER]                VARCHAR (8)     NOT NULL,
    [FMGUID]                  CHAR (38)       NOT NULL,
    [MESSAGETYPE]             VARCHAR (20)    NOT NULL,
    [MESSAGEKEYWORDS]         VARCHAR (50)    NULL,
    [FIELDMAPRESTARTREQUIRED] INT             NULL,
    [STATUS]                  VARCHAR (15)    NOT NULL,
    [MISCATTRIB]              INT             NULL,
    [TEXTMESSAGE]             NTEXT           NULL,
    [DATA]                    VARBINARY (MAX) NULL,
    [PARAMETERS]              NTEXT           NULL,
    [MAILINGLIST]             NTEXT           NULL,
    [IPADDRESS]               VARCHAR (40)    NULL,
	[_Status] TINYINT NOT NULL REFERENCES mca.status(Id) DEFAULT 1,
	[CHANGE_SESSION] [int] NOT NULL REFERENCES mca.session(ID) DEFAULT mca.DefaultSession(),
	[SysStart] DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
	[SysEnd] DATETIME2 (7) GENERATED ALWAYS AS ROW END NOT NULL,
    PRIMARY KEY ([DataSetRow]), 
	PERIOD FOR SYSTEM_TIME ([SysStart], [SysEnd])
)
WITH (SYSTEM_VERSIONING = ON(HISTORY_TABLE=[ukrtax].[JOURNAL_HISTORY], DATA_CONSISTENCY_CHECK=ON))
GO

CREATE UNIQUE INDEX [IX_JOURNAL_Keys] ON [ukrtax].[JOURNAL] ([ID])
GO
