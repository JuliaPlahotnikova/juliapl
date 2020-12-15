using System;
using System.Reflection;
using System.Xml;

namespace ConfigParser
{
    public class XMLParcer : IParce
    {
        public T Parce<T>(string path)
        {
            //T extends Options;
            object result = Activator.CreateInstance(typeof(T));

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo pi in properties)
            {
                DeserializeRecursive(pi, result, FindNode<T>(path, result.GetType()));
            }

            return (T)result;
        }

        private XmlNode FindNode<T>(string path, Type ttype)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(path);

            if (typeof(T) == ttype)
            {
                return doc.DocumentElement;
            }

            PropertyInfo[] properties = ttype.GetProperties();

            XmlNode result = null;

            foreach (PropertyInfo ppi in properties)
            {
                FindNodeRecursive<T>(ppi, doc.DocumentElement, ref result);
            }

            if (result is null)
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }

            return result;
        }
        private void FindNodeRecursive<T>(PropertyInfo pi, XmlNode parentNode, ref XmlNode result)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == pi.Name && pi.PropertyType == typeof(T) && result == null)
                {
                    result = node;

                    if (!pi.PropertyType.IsPrimitive && !(pi.PropertyType == typeof(string)))
                    {
                        Type subt = pi.PropertyType;

                        PropertyInfo[] props = subt.GetProperties();
                        foreach (PropertyInfo ppi in props)
                        {
                            FindNodeRecursive<T>(ppi, node, ref result);
                        }
                    }
                }
            }
        }
        private void DeserializeRecursive(PropertyInfo pi, object parent, XmlNode parentNode)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == pi.Name)
                {
                    if (pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(string))
                    {
                        if (node.InnerText == "0" || node.InnerText == "1")
                        {
                            if (node.InnerText == "1")
                            {
                                pi.SetValue(parent, Convert.ChangeType(true, pi.PropertyType));
                            }
                            else
                            {
                                pi.SetValue(parent, Convert.ChangeType(false, pi.PropertyType));
                            }
                        }
                        else
                        {
                            pi.SetValue(parent, Convert.ChangeType(node.InnerText, pi.PropertyType));
                        }
                    }
                    else if (pi.PropertyType.IsEnum)
                    {
                        pi.SetValue(parent, Enum.Parse(pi.PropertyType, node.InnerText));
                    }
                    else
                    {
                        Type subType = pi.PropertyType;
                        object subObj = Activator.CreateInstance(subType);

                        pi.SetValue(parent, subObj);

                        PropertyInfo[] subPIs = subType.GetProperties();
                        foreach (PropertyInfo spi in subPIs)
                        {
                            DeserializeRecursive(spi, subObj, node);
                        }
                    }
                }
            }
        }
    }
}
