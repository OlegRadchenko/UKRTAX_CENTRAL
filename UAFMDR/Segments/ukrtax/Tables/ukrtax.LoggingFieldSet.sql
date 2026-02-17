CREATE TABLE [ukrtax].[LoggingFieldSet]
(
	[DataSetRow] INT REFERENCES store.DataSetRow(Id) NOT NULL,
    [PROCESSCODE]         SMALLINT        NOT NULL,
    [FIELDSTATUS]         BINARY (24)     NULL,
    [ADD_DATE]            DATETIME        NOT NULL,
    [ADD_USER]            VARCHAR (8)     NOT NULL,
    [EDIT_DATE]           DATETIME        NOT NULL,
    [EDIT_USER]           VARCHAR (8)     NOT NULL,
    [GREDIT_DATE]         DATETIME        NOT NULL,
    [GREDIT_USER]         VARCHAR (8)     NOT NULL,
    [SYNCHRO_DATE]        DATETIME        NULL,
    [GRSYNCHRO_DATE]      DATETIME        NULL,
    [MISCATTRIB]          INT             NULL,
    [PARENTFMGUID]        UNIQUEIDENTIFIER       NULL,
    [FMGUID]              UNIQUEIDENTIFIER       NULL,
	[_Status] TINYINT NOT NULL REFERENCES mca.status(Id) DEFAULT 1,
	[CHANGE_SESSION] [int] NOT NULL REFERENCES mca.session(ID) DEFAULT mca.DefaultSession(),
	[SysStart] DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
	[SysEnd] DATETIME2 (7) GENERATED ALWAYS AS ROW END NOT NULL, 
    CONSTRAINT [PK_LoggingFieldSet] PRIMARY KEY ([DataSetRow]),
	PERIOD FOR SYSTEM_TIME ([SysStart], [SysEnd])
)
WITH (SYSTEM_VERSIONING = ON(HISTORY_TABLE=[ukrtax].[LoggingFieldSet_HISTORY], DATA_CONSISTENCY_CHECK=ON))
