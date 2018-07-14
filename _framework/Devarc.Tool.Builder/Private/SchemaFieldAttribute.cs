using System;

[System.AttributeUsage(System.AttributeTargets.Field)]
class SchemaFieldAttribute : System.Attribute
{
    public int Length = 0;
}
