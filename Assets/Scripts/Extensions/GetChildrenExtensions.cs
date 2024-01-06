using System;
using UnityEngine;

public static class GetChildrenExtensions
{
    public static Transform GetChild(this Transform parent, string name)
    {
        Transform[] children = new Transform[parent.childCount];

        for (int i = 0; i < parent.childCount; i++)
        {
            children[i] = parent.GetChild(i);

            if (children[i] != null && children[i].name == name)
                return children[i];
        }
        
        return null;
    }
}
