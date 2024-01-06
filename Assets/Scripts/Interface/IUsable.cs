using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IUsable
{
    UnityEvent OnUse { get; }

    void Use(Transform actor);
}
