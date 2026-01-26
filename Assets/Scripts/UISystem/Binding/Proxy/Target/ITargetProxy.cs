using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetProxy
{
    Type Type { get; }

    TypeCode TypeCode { get; }

    object Target { get; }

    BindingMode DefaultMode { get; }
}
