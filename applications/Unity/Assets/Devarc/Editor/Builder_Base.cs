﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devarc
{
    public abstract class Builder_Base
    {
        protected string NameSpace = "Devarc";
        protected SCHEMA_TYPE dataFileType = SCHEMA_TYPE.SHEET;

        protected BaseSchemaReader _createReader()
        {
            switch (dataFileType)
            {
                case SCHEMA_TYPE.EXCEL:
                    return new ExcelReader();
                case SCHEMA_TYPE.SHEET:
                    return new XmlSheetReader();
                case SCHEMA_TYPE.SCHEMA:
#if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
                    return null;
#else
                    return new SchemaReader();
#endif
                default:
                    return null;
            }
        }

        protected string GetClassName(string _path)
        {
            int startIndex = _path[0] == '!' ? 1 : 0;
            int endIndex = _path.IndexOf('@');
            if (endIndex >= 0)
                return _path.Substring(startIndex, endIndex - startIndex);
            else
                return _path.Substring(startIndex, _path.Length - startIndex);
        }

        protected string GetClassNameEx(string _path)
        {
            string value = System.IO.Path.GetFileNameWithoutExtension(_path);
            int startIndex = value[0] == '!' ? 1 : 0;
            int endIndex = value.IndexOf('@');
            if (endIndex >= 0)
                return value.Substring(startIndex, endIndex - startIndex);
            else
                return value.Substring(startIndex, value.Length - startIndex);
        }
    }
}
