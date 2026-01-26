using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObtainable
{
    object GetValue();
    T GetValue<T>();
}
