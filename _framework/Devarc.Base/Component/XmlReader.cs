using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading.Tasks;

namespace Devarc
{
    public class XmlReader : BaseSchemaReader
    {
        enum XML_NODE_TYPE
        {
            NONE,
            TYPE,
            LOCALIZE,
            ENCRYPT,
            DATA,
        }

        public override bool ReadData(string _data)
        {
            try
            {
                byte[] tempData = Encoding.UTF8.GetBytes(_data);
                MemoryStream stream = new MemoryStream(tempData);
                XmlTextReader xmlReader = new XmlTextReader(stream);
                return readData(xmlReader);
            }
            catch (Exception ex)
            {
                Log.OnMessage(LOG_TYPE.ERROR, ex.Message);
                return false;
            }
        }

        public override bool ReadFile(string _filePath)
        {
            try
            {
                XmlTextReader xmlReader = new XmlTextReader(_filePath);
                return readData(xmlReader);
            }
            catch (Exception ex)
            {
                Log.OnMessage(LOG_TYPE.ERROR, ex.Message);
                return false;
            }
        }

        bool readData(XmlTextReader _reader)
        {
            XML_NODE_TYPE tempNodeType = XML_NODE_TYPE.NONE;
            string tableName = string.Empty;
            bool tempReadRoot = false;
            bool success = true;
            PropTable header = new PropTable();

            _reader.WhitespaceHandling = WhitespaceHandling.None;
            while (_reader.Read())
            {
                switch (_reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (tempReadRoot == false)
                        {
                            tempReadRoot = true;
                            tableName = _reader.Name;
                        }

                        if (_reader.Name.ToUpper().StartsWith("DATATYPES"))
                        {
                            tempNodeType = XML_NODE_TYPE.TYPE;
                        }
                        else if (_reader.Name.ToUpper().StartsWith("DATATYPE"))
                        {
                            string _NAME_ = string.Empty;
                            string _TYPE_NAME_ = string.Empty;
                            CLASS_TYPE _CLASS_TYPE_ = CLASS_TYPE.VALUE;
                            bool _KEY_ = false;
                            for (int i = 0; i < _reader.AttributeCount; i++)
                            {
                                if (_reader.MoveToNextAttribute() == false)
                                {
                                    break;
                                }
                                if (string.Equals("NAME", _reader.Name))
                                    _NAME_ = _reader.Value;
                                else if (string.Equals("TYPE_NAME", _reader.Name))
                                    _TYPE_NAME_ = _reader.Value;
                                else if (string.Equals("CLASS_TYPE", _reader.Name))
                                {
                                    switch (_reader.Value)
                                    {
                                        case "CLASS":
                                            _CLASS_TYPE_ = CLASS_TYPE.CLASS;
                                            break;
                                        case "CLASS_LIST":
                                            _CLASS_TYPE_ = CLASS_TYPE.CLASS_LIST;
                                            break;
                                        case "VALUE_LIST":
                                            _CLASS_TYPE_ = CLASS_TYPE.VALUE_LIST;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else if (string.Equals("KEY", _reader.Name))
                                {
                                    bool.TryParse(_reader.Value, out _KEY_);
                                }
                            }
                            header.Attach(_NAME_, _TYPE_NAME_, _CLASS_TYPE_, _KEY_, string.Empty);
                        }
                        else if (_reader.Name.ToUpper().StartsWith("DATAS"))
                        {
                            tempNodeType = XML_NODE_TYPE.DATA;
                        }
                        else if (_reader.Name.ToUpper().StartsWith("DATA"))
                        {
                            header.ClearData();
                            for (int i = 0; i < _reader.AttributeCount; i++)
                            {
                                if (_reader.MoveToNextAttribute() == false)
                                {
                                    break;
                                }
                                header.Set_Data(_reader.Name, _reader.Value);
                            }
                            callback_data(tableName, header);
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (_reader.Name.ToUpper().StartsWith("DATA_TYPE"))
                        {
                            tempNodeType = XML_NODE_TYPE.NONE;
                        }
                        else if (_reader.Name.ToUpper().StartsWith("DATAS"))
                        {
                            tempNodeType = XML_NODE_TYPE.NONE;
                            //onReadComplete();
                        }
                        break;

                    default:
                        break;
                }
            }
            return success;
        }

    }
}
