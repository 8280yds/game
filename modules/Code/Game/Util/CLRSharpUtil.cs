using CLRSharp;
using System.Collections.Generic;

public class CLRSharpUtil
{
    public static string[] getEnumItemNames(ICLRType clrType)
    {
        List<string> itemNames = new List<string>(clrType.GetFieldNames());
        itemNames.RemoveAt(0);
        return itemNames.ToArray();
    }
    
}
