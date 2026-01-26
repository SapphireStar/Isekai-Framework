using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISourceProxy
{
    Type Type {  get; }

    TypeCode TypeCode { get; }

    object Source { get; }
}
