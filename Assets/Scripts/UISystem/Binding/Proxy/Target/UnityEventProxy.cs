using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public abstract class UnityEventProxyBase<T> : EventTargetProxyBase
{
    protected Delegate handler; // delegate binding

    protected T unityEvent;

    public UnityEventProxyBase(object target, T unityEvent) : base(target)
    {
        if (unityEvent == null)
            throw new ArgumentNullException("unityEvent");

        this.unityEvent = unityEvent;
        this.BindEvent();
    }

    public override Type Type => throw new NotImplementedException();

    protected abstract void BindEvent();

    protected abstract void UnbindEvent();

    protected abstract bool IsValid(Delegate handler);

    protected abstract bool IsValid(MethodInfo methodInfo);

    public override void SetValue(object value)
    {
        var target = this.target;
        if (target == null)
            return;

        if(this.handler != null)
            this.handler = null;

        if (value == null)
            return;

        // Bind Delegate
        if(value is Delegate handler)
        {
            if (!IsValid(handler))
                throw new ArgumentException("Bind delegate fialed. The parameter types do not match");
            this.handler = handler;
            return;
        }
    }

    public override void SetValue<TValue>(TValue value)
    {
        this.SetValue((object)value);   
    }
}

public class UnityEventProxy : UnityEventProxyBase<UnityEvent>
{
    public UnityEventProxy(object target, UnityEvent unityEvent) : base(target, unityEvent)
    {
    }

    public override Type Type { get { return typeof(UnityEvent); } }

    protected override void BindEvent()
    {
        this.unityEvent.AddListener(OnEvent);
    }

    protected override void UnbindEvent()
    {
        this.unityEvent.RemoveListener(OnEvent);
    }

    protected override bool IsValid(Delegate handler)
    {
        if (handler is UnityAction || handler is Action)
            return true;
        MethodInfo info = handler.Method;

        if (!info.ReturnType.Equals(typeof(void)))
            return false;

        var parameterTypes = info.GetParameters();
        if (parameterTypes.Length != 0)
            return false;
        return true;
    }

    protected override bool IsValid(MethodInfo methodInfo)
    {
        if (!methodInfo.ReturnType.Equals(typeof(void)))
            return false;

        var parameterTypes = methodInfo.GetParameters();
        if (parameterTypes.Length != 0)
            return false;
        return true;
    }

    protected virtual void OnEvent()
    {
        try
        {
            if(this.handler!=null)
            {
                if(this.handler is Action action)
                {
                    action();
                }
                else if(this.handler is UnityAction unityAction)
                {
                    unityAction();
                }
                else
                {
                    this.handler.DynamicInvoke();
                }
                return;
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
}