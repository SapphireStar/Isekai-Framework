using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBinding
{
    object Target { get; }
    object DataContext { get; set; }
}
