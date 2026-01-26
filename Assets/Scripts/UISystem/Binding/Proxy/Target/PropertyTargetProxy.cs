using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class PropertyTargetProxy : ValueTargetProxyBase
{
    protected readonly PropertyInfo propertyInfo;
    public PropertyTargetProxy(object target, PropertyInfo propertyInfo) : base(target)
    {
        this.propertyInfo = propertyInfo;
    }

    public override Type Type
    {
        get { return this.propertyInfo.PropertyType; }
    }

    public override TypeCode TypeCode
    {
        get { return Type.GetTypeCode(this.propertyInfo.PropertyType); }
    }

    public override BindingMode DefaultMode { get { return BindingMode.TwoWay; } }
    public override object GetValue()
    {
        var target = this.Target;
        if(target == null)
        {
            return null;
        }

        return propertyInfo.GetValue(target);
    }

    public override T GetValue<T>()
    {
        var target = this.Target;
        if (target == null)
        {
            return default(T);
        }

        return (T)propertyInfo.GetValue(target);
    }

    public override void SetValue(object value)
    {
        var target = this.Target;
        if (target == null)
        {
            return;
        }

        propertyInfo.SetValue(target, value);
    }

    public override void SetValue<T>(T value)
    {
        var target = this.Target;
        if (target == null)
        {
            return;
        }

        propertyInfo.SetValue(target, value);
    }

    // 如果要支持双向通信，那么需要对target（UI View中的对象）的更改进行监听
    protected override void DoSubscribeForValueChange(object target)
    {
        if(target is INotifyPropertyChanged)
        {
            var targetNotify = target as INotifyPropertyChanged;
            targetNotify.PropertyChanged += OnPropertyChanged;
        }
    }

    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var name = e.PropertyName;
        Debug.Log("Title changed");
        if(string.IsNullOrEmpty(name) || name.Equals(this.propertyInfo.Name))
        {
            var target = this.Target;
            if (target == null)
                return;

            this.RaiseValueChanged();
        }
    }
}
