using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class  EventManager 
{


    #region Testing
    // For Testing
    //public delegate void GOEventAction(GameObject GO);
    //public static event GOEventAction GOEvent;


    //public static void CallGOEvent(GameObject GO)
    //{
    //    GOEvent?.Invoke(GO);
    //}
    #endregion


    #region Events without Input
    public delegate void EventAction();
    public static event EventAction TriggerLeftEvent; // NO listener yet
    public static event EventAction TriggerRightEvent;// NO listener yet    
    public static event EventAction TouchLeftEvent; // NO listener yet
    public static event EventAction TouchRightEvent;// NO listener yet

    public static void CallTriggerLeftEvent()
    {
        TriggerLeftEvent?.Invoke();
    }
    public static void CallTriggerRightEvent()   
    {
        if (TriggerRightEvent!=null)
        {
            TriggerRightEvent.Invoke();
        }
        else
        {
            Debug.LogWarning("No Listener for RightTriggerEvent");
        }
    }
    public static void CallTouchLeftEvent()
    {
        if (TouchLeftEvent != null)
        {
            TouchLeftEvent.Invoke();
        }
        else
        {
            Debug.LogWarning("No Listener for RightTouchEvent");
        }
    }
    public static void CallTouchRightEvent()
    {
        if (TouchRightEvent != null)
        {
            TouchRightEvent.Invoke();
        }
        else
        {
            Debug.LogWarning("No Listener for RightTouchEvent");
        }
    }

    #endregion


    #region Events with Float Input
    public delegate void FloatEventAction(float inp);
    //public static event FloatEventAction EventFloat;
    public static event FloatEventAction DefineSpaceEvent;

    //public static void CallFloatEvent(float Float)
    //{
    //    EventFloat?.Invoke(Float);
    //}    
    public static void CallDefineSpaceEvent(float Float)
    {
        //DefineSpaceEvent?.Invoke(Float);
        if (DefineSpaceEvent != null)
        {
            DefineSpaceEvent.Invoke(Float);
        }
        else
        {
            Debug.LogWarning("No Listener");
        }
    }

    #endregion


    #region Events with GameObject Input
    public delegate void GOEventAction(GameObject GO);
    public static event GOEventAction ColliderInteractionEvent;
    public static event GOEventAction StartVibrationEvent;



    public static void CallColliderInteractionEvent(GameObject GO)
    {
        //ColliderInteractionEvent?.Invoke(GO);
        if (ColliderInteractionEvent != null)
        {
            ColliderInteractionEvent.Invoke(GO);
        }
        else
        {
            Debug.LogWarning("No Listener for Call ColliderInteraction");
        }
    }
    public static void CallStartVibrationEvent(GameObject GO)
    {
        if (StartVibrationEvent != null)
        {
            StartVibrationEvent.Invoke(GO);
        }
        else
        {
            Debug.LogWarning("No Listener");
        }
    }

    #endregion
}
