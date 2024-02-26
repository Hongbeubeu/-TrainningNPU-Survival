using System.Collections.Generic;
using System;
using System.Reflection;
using Npu.Core;

public class Binder<T>
{
    private Type type;
    private string path;

    private List<DataNode> nodes;

    public Binder(Type type, string path)
    {
        this.type = type;
        this.path = path;

        ParsePath();
    }

    public void Bind(object target, T value, bool asString = false)
    {
        var t = target;
        for (var i = 0; i < nodes.Count - 1; i++)
        {
            var n = nodes[i];
            t = n.GetValue(t);
        }

        var node = nodes[nodes.Count - 1];
        node.SetValue(t, value, asString);
    }

    public SecuredDouble GetValue(object target)
    {
        var t = target;
        for (var i = 0; i < nodes.Count - 1; i++)
        {
            var n = nodes[i];
            t = n.GetValue(t);
        }

        var node = nodes[nodes.Count - 1];
        return (SecuredDouble)node.GetValue(t);
    }

    private void ParsePath()
    {
        nodes = new List<DataNode>();
        var s = path.Split('.');
        var node = type;
        for (var i = 0; i < s.Length; i++)
        {
            var si = s[i];
            var dataNode = new DataNode(node, si);
            nodes.Add(dataNode);
            node = dataNode.NodeType();
        }

        if (nodes.Count == 0)
        {
            throw new SystemException("Path is invalid");
        }

        if (!nodes[nodes.Count - 1].NodeType().IsAssignableFrom(typeof(T)))
        {
            //			throw new SystemException ("Path is invalid");
        }
    }
}

public class DataNode
{
    private Kind type;
    private FieldInfo fieldInfo;
    private PropertyInfo propertyInfo;

    public DataNode(System.Type type, string name)
    {
        fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (fieldInfo != null)
        {
            this.type = Kind.FIELD;
        }
        else
        {
            propertyInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            this.type = Kind.PROPERTY;
            if (propertyInfo == null)
            {
                throw new SystemException(string.Format("Cannot find field or property named \"{0}\" of type {1}", name, type));
            }
        }
    }

    public Type NodeType()
    {
        return type == Kind.FIELD ? fieldInfo.FieldType : propertyInfo.PropertyType;
    }

    public void SetValue(object target, object value, bool asString)
    {
        if (type == Kind.FIELD)
            fieldInfo.SetValue(target, asString ? value.ToString() : value);
        else
            propertyInfo.SetValue(target, asString ? value.ToString() : value, null);
    }

    public object GetValue(object target)
    {
        return type == Kind.FIELD ? fieldInfo.GetValue(target) : propertyInfo.GetValue(target, null);
    }

    public override string ToString()
    {
        return string.Format("[DataNode: kind = {0}, type = {1}]", type, NodeType());
    }

    public enum Kind
    {
        FIELD,
        PROPERTY
    }
}