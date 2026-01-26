using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifiable
{
    void SetValue(object value);
    void SetValue<T>(T value);
}
