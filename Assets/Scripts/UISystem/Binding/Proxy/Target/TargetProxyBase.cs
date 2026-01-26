using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetProxyBase : ITargetProxy
{
    protected readonly WeakReference target;
    protected TypeCode typeCode = TypeCode.Empty;
    protected readonly string targetName;
    
    public TargetProxyBase(object target)
    {
        if(target!=null)
        {
            this.target = new WeakReference(target, false);
        }
    }
    public abstract Type Type { get; }

    public virtual TypeCode TypeCode
    {
        get
        {
            if (typeCode == TypeCode.Empty)
            {
                typeCode = Type.GetTypeCode(Type);
            }
            return typeCode;
        }
    }

    public object Target
    {
        get
        {
            var target = this.target != null ? this.target.Target : null;
            return IsAlive(target) ? target : null;
        }
    }

    private bool IsAlive(object target)
    {
        try
        {
            if (target == null)
                return false;
            if(target is UnityEngine.Object)
            {
                // 通过尝试获取target的name来判断是否其已经在unity中被销毁，但在c#中还没有被销毁，获取name失败则抛出异常
                var name = ((UnityEngine.Object)target).name;
                return true;
            }
            return target != null;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public virtual BindingMode DefaultMode {  get { return BindingMode.OneWay; } }
}

public abstract class ValueTargetProxyBase : TargetProxyBase, IModifiable, IObtainable, INotifiable
{
    private bool subscribed = false;

    protected EventHandler valueChanged;
    protected ValueTargetProxyBase(object target) : base(target)
    {
    }

    public event EventHandler ValueChanged
    {
        add
        {
            this.valueChanged += value;
            if(this.valueChanged != null && !this.subscribed)
            {
                this.Subscribe();
            }
        }
        remove
        {
            this.valueChanged -= value;
            if(this.valueChanged == null && this.subscribed)
            {
                this.Unsubscribe();
            }
        }
    }

    protected void Subscribe()
    {
        try
        {
            if (subscribed)
                return;

            var target = this.Target;
            if (target == null)
                return;

            this.subscribed = true;
            this.DoSubscribeForValueChange(target);
        }
        catch (Exception e)
        {
            Debug.LogFormat("{0} Subscribe Exception:{1}", this.targetName, e);
        }
    }

    protected virtual void DoSubscribeForValueChange(object target)
    {

    }

    protected void Unsubscribe()
    {
        try
        {
            if (!subscribed)
                return;

            var target = this.Target;
            if (target == null)
                return;

            this.subscribed = false;
            this.DoUnsubscribeForValueChange(target);
        }
        catch (Exception e)
        {
            Debug.LogFormat("{0} Unsubscribe Exception:{1}", this.targetName, e);
        }
    }

    protected virtual void DoUnsubscribeForValueChange(object target)
    {

    }

    public abstract object GetValue();

    public abstract T GetValue<T>();

    public abstract void SetValue(object value);

    public abstract void SetValue<T>(T value);

    protected void RaiseValueChanged()
    {
        var handler = this.valueChanged;
        if(handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}

public abstract class EventTargetProxyBase : TargetProxyBase, IModifiable
{
    protected EventTargetProxyBase(object target) : base(target)
    {
    }

    public abstract void SetValue(object value);

    public abstract void SetValue<T>(T value);
}