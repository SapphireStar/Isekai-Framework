using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SourceProxyBase : ISourceProxy
{
    protected TypeCode typeCode = TypeCode.Empty;
    protected readonly object source;

    public SourceProxyBase(object source)
    {
        this.source = source;
    }

    public abstract Type Type { get; }

    public virtual TypeCode TypeCode
    {
        get
        {
            if(typeCode == TypeCode.Empty)
            {
                typeCode = Type.GetTypeCode(Type);
            }
            return typeCode;
        }
    }

    public virtual object Source { get { return source; } }
}

public abstract class NotifiableSourceProxyBase : SourceProxyBase, INotifiable
{
    protected EventHandler valueChanged;
    protected NotifiableSourceProxyBase(object source) : base(source)
    {
    }

    public event EventHandler ValueChanged
    {
        add
        {
            this.valueChanged += value;
        }
        remove
        {
            this.valueChanged -= value;
        }
    }

    protected virtual void RaiseValueChanged()
    {
        if(this.valueChanged != null)
        {
            valueChanged(this, EventArgs.Empty);
        }
    }


}
