using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBindingContext
{
    event EventHandler DataContextChanged;

    object Owner { get; }
    
    object DataContext { get; set; }

    void Add(IBinding binding, object key = null);

    void Add(IEnumerable<IBinding> bindings, object key = null);

    void Add(object target, BindingDescription description, object key = null);

    void Add(object target, IEnumerable<BindingDescription> descriptions, object key = null);

    void Clear(object key);

    void Clear();
}
