using System;
using Ultimate.Core.Runtime.Singleton;
using UnityEngine;

public class InputController : Singleton<InputController>
{
    [SerializeField] private Joystick _joystick;

    public Joystick Joystick => _joystick;

    public override void Init()
    {
    }
}