CREATE FUNCTION [mca].[GetDefault]
(
	@name varchar(32)
)
RETURNS varchar(MAX)
AS
BEGIN
	DECLARE @result varchar(MAX);
	SELECT TOP 1 @result = Value FROM mca.defaults WHERE Name=@name
	RETURN @result
END
