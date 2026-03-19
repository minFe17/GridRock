using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BackGroundController : MonoBehaviour
{
    private BackGroundMoveController[] _childControllers;
    public void StopBackGroundMove()
    {
        if( _childControllers == null )
            _childControllers = GetComponentsInChildren<BackGroundMoveController>();
        foreach(BackGroundMoveController child in _childControllers)
        {
            child.IsMove= false;
        }
    }
    public void StartBackGroundMove()
    {
        if (_childControllers == null)
            _childControllers = GetComponentsInChildren<BackGroundMoveController>();
        foreach (BackGroundMoveController child in _childControllers)
        {
            child.IsMove = true;
        }
    }
}
