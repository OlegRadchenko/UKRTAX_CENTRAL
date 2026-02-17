-- налаштування зберігання даних
CREATE SCHEMA [ds]
GO

EXEC sys.sp_addextendedproperty 
	@name=N'Description', 
	@value=N'налаштування зберігання даних', 
	@level0type=N'SCHEMA',
	@level0name=N'ds'