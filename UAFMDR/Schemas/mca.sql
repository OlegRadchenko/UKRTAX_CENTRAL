-- managment, control & audit
CREATE SCHEMA [mca]
GO 

EXEC sys.sp_addextendedproperty 
	@name=N'Description', 
	@value=N'Засоби для загального управління та аудиту', 
	@level0type=N'SCHEMA',
	@level0name=N'mca'
GO