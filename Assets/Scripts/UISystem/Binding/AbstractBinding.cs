using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBinding : IBinding
{
    private WeakReference target;
    private object dataContext;

    public AbstractBinding(object dataContext, object target)
    {
        this.target = new WeakReference(target, false);
        this.dataContext = dataContext;
    }

    public virtual object Target
    {
        get
        {
            var target = this.target!=null?this.target.Target: null;
            return IsAlive(target) ? target : null;
        }
    }

    private bool IsAlive(object target)
    {
        try
        {
            if(target is UnityEngine.Object)
            {
                var name = ((UnityEngine.Object)target).name;
                return true;
            }
            return target != null;
        }
        catch(Exception)
        {
            return false;
        }
    }

    public virtual object DataContext
    {
        get { return this.dataContext; }
        set
        {
            if (this.dataContext == value)
                return;

            this.dataContext = value;
            this.OnDataContextChanged();
        }
    }

    protected abstract void OnDataContextChanged();
}
