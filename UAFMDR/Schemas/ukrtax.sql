CREATE SCHEMA [ukrtax]
GO

EXEC sys.sp_addextendedproperty 
	@name=N'Description', 
	@value=N'Для додатку Field-Map UKRTAX', 
	@level0type=N'SCHEMA',
	@level0name=N'ukrtax'
GO