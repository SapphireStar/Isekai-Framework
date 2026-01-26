using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class PropertyNodeProxy : NotifiableSourceProxyBase, IModifiable, IObtainable, INotifiable
{
    protected PropertyInfo propertyInfo;

    public PropertyNodeProxy(object source, PropertyInfo propertyInfo) : base(source)
    {
        this.propertyInfo = propertyInfo;
        if (this.source == null)
            return;
        if(this.source is INotifyPropertyChanged)
        {
            var sourceNotify = this.source as INotifyPropertyChanged;
            sourceNotify.PropertyChanged += OnPropertyChanged;
        }
    }
    public override Type Type
    {
        get
        {
            return propertyInfo.PropertyType;
        }
    }

    protected virtual void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var name = e.PropertyName;
        
        if (string.IsNullOrEmpty(name) || name.Equals(propertyInfo.Name))
        {
            this.RaiseValueChanged();
        }
            
    }

    // GetValue和SetValue是供Binding数据绑定使用的，如果是外部对ViewModel进行修改，那么修改直接在ViewModel内部进行
    public object GetValue()
    {
        return propertyInfo.GetValue(source);
    }

    public T GetValue<T>()
    {
        return (T) GetValue();
    }

    public void SetValue(object value)
    {
        propertyInfo.SetValue(source, value);
    }

    public void SetValue<T>(T value)
    {
        propertyInfo.SetValue(source, value);
    }
}
