-- Сесія за замовчуванням. Зараз dummy
CREATE FUNCTION [mca].[DefaultSession]
( )
RETURNS int AS 
BEGIN
	DECLARE @Result int;
	SELECT @Result = ID FROM mca.session;
	RETURN @Result;
END;
