USE [UAFMDR]
GO

DECLARE @source xml

SELECT @source = '<database application="UKRTAX" source="UKRTAX"/>'

EXECUTE [dbo].[ukrtax_update_relations] 
   @ApplicationStructure
GO
