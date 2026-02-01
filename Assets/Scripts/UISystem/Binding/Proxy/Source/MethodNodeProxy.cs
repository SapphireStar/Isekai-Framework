using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class MethodNodeProxy : SourceProxyBase, IObtainable
{
    MethodInfo methodInfo;
    public MethodNodeProxy(object source, MethodInfo methodInfo) : base(source)
    {
        this.methodInfo = methodInfo;
    }

    public override Type Type { get { return typeof(MethodInfo); } }

    public object GetValue()
    {
        return Delegate.CreateDelegate(typeof(Action), this.source, this.methodInfo);
    }

    public T GetValue<T>()
    {
        return (T)default;
    }
}
