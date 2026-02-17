using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString SelectDataForDataset(string TableName, DateTime From, DateTime To)
    {
        // Put your code here
        return new SqlString (string.Empty);
    }
}
