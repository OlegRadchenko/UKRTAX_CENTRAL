CREATE SCHEMA [store]
GO

EXEC sys.sp_addextendedproperty 
	@name=N'Description', 
	@value=N'Сегмент сховища для безпосереднього зберігання даних', 
	@level0type=N'SCHEMA',
	@level0name=N'store'