using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

[StructLayout(LayoutKind.Explicit, Size = 32)]
struct BeitongAndroidModeInputReport : IInputStateTypeInfo
{
    public FourCC format => new FourCC('H', 'I', 'D');

    [FieldOffset(0)] public byte reportId;
    
    [InputControl(name = "leftTrigger", format = "BIT",layout = "Button", bit = 0)]
    [InputControl(name = "rightTrigger", format = "BIT",layout = "Button", bit = 1)]
    [InputControl(name = "select",layout = "Button", bit = 2)]
    [InputControl(name = "start",layout = "Button", bit = 3)]
    [InputControl(name = "home",layout = "Button", bit = 4)]
    [InputControl(name = "leftStickPress",layout = "Button", bit = 5)]
    [InputControl(name = "rightStickPress",layout = "Button", bit = 6)]
    [FieldOffset(4)] public byte buttons;

    [InputControl(name = "buttonSouth",layout = "Button", displayName = "A", bit = 0)]
    [InputControl(name = "buttonEast",layout = "Button", displayName = "B", bit = 1)]
    [InputControl(name = "buttonWest",layout = "Button", displayName = "X", bit = 3)]
    [InputControl(name = "buttonNorth",layout = "Button", displayName = "Y", bit = 4)]
    [InputControl(name = "leftShoulder",layout = "Button", bit = 6)]
    [InputControl(name = "rightShoulder",layout = "Button", bit = 7)]
    [FieldOffset(3)] public byte buttons1;
    
    
    [InputControl(name = "dpad", format = "BYTE", layout = "Dpad", sizeInBits = 8, defaultState = 8)]
    [InputControl(name = "dpad/up", format = "BIT", layout = "DiscreteButton", parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7", bit = 0, sizeInBits = 4)]
    [InputControl(name = "dpad/right", format = "BIT", layout = "DiscreteButton", parameters = "minValue=1,maxValue=3", bit = 0, sizeInBits = 4)]
    [InputControl(name = "dpad/down", format = "BIT", layout = "DiscreteButton", parameters = "minValue=3,maxValue=5", bit = 0, sizeInBits = 4)]
    [InputControl(name = "dpad/left", format = "BIT", layout = "DiscreteButton", parameters = "minValue=5, maxValue=7", bit = 0, sizeInBits = 4)]
    [FieldOffset(2)] public byte buttons2;
    
    
    [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
    [InputControl(name = "leftStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "leftStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(5)] public byte leftStickX;
    [FieldOffset(6)] public byte leftStickY;
    
    [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
    [InputControl(name = "rightStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "rightStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(7)] public byte rightStickX;
    [FieldOffset(8)] public byte rightStickY;
    
}


// Using InputControlLayoutAttribute, we tell the system about the state
// struct we created, which includes where to find all the InputControl
// attributes that we placed on there. This is how the Input System knows-
// what controls to create and how to configure them.
#if UNITY_EDITOR
[InitializeOnLoad] // Make sure static constructor is called during startup.
#endif
[InputControlLayout(stateType = typeof(BeitongAndroidModeInputReport))]
public class BeitongAndroidGamePad : Gamepad
{
    static BeitongAndroidGamePad()
    {
        InputSystem.RegisterLayout<BeitongAndroidGamePad>(
            matches: new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x20BC) 
                .WithCapability("productId", 0x5053)
                .WithCapability("usagePage", 0x1)
            ); 
    }

    // In the Player, to trigger the calling of the static constructor,
    // create an empty method annotated with RuntimeInitializeOnLoadMethod.
    [RuntimeInitializeOnLoadMethod]
    static void Init() {}
}
