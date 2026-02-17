USE [UAFMDR]
GO

DECLARE @Task xml
DECLARE @forced int

SELECT @forced = 1
SELECT @Task = '<database application="UKRTAX" source="irpin.UKRTAX"/>'

EXECUTE [dbo].[ukrtax_update_app_structure] 
   @Task
  ,@forced
GO

