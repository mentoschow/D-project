using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MessageData
{
    public readonly bool valueBool;
    public readonly int valueInt;
    public readonly float valueFloat;
    public readonly string valueString;
    public readonly System.Object valueObject;

    public MessageData(bool value)
    {
        valueBool = value;
    }

    public MessageData(int value)
    {
        valueInt = value;
    }

    public MessageData(float value)
    {
        valueFloat = value;
    }

    public MessageData(string value)
    {
        valueString = value;
    }

    public MessageData(System.Object value)
    {
        valueObject = value;
    }
}
