using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class  EventManager 
{
    #region Events without Input
    public delegate void EventAction();
    public static event EventAction TriggerLeftEvent; // NO listener yet
    public static event EventAction TriggerRightEvent;// NO listener yet    
    public static event EventAction TouchLeftEvent; // NO listener yet
    public static event EventAction TouchRightEvent;// NO listener yet
    public static event EventAction TriggerEvent;
    public static event EventAction DefineNewTargetEvent;
    public static event EventAction StartSeachringEvent;

    public static void CallStartSearchingEvent()
    {
        if (StartSeachringEvent != null)
        {
            StartSeachringEvent.Invoke();
        }
        else
        {
            //Feedback.AddTextToSide("No Listener for StartSeachringEvent", false);
        }
    }      
    public static void CallDefineNewTargetEvent()
    {
        if (DefineNewTargetEvent != null)
        {
            DefineNewTargetEvent.Invoke();
        }
        else
        {
            //Feedback.AddTextToSide("No Listener for DefineNewTargetEvent", false);
        }
    }
    public static void CallTriggerLeftEvent()
    {
        //TriggerLeftEvent?.Invoke();
        if (TriggerLeftEvent != null)
        {
            TriggerLeftEvent.Invoke();
        }
        else
        {
            //Feedback.AddTextToSide("No Listener for TriggerLeftEvent", false);
        }
    }
    public static void CallTriggerEvent()
    {
        if (TriggerEvent != null)
        {
            TriggerEvent.Invoke();
        }
        else
        {
            //Feedback.AddTextToSide("No Listener for TriggerEvent", false);
        }
    }
    public static void CallTriggerRightEvent()
    {
        if (TriggerRightEvent != null)
        {
            TriggerRightEvent.Invoke();
        }
        else
        {
            //Feedback.AddTextToSide("No Listener for RightTriggerEvent", false);
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
            //Feedback.AddTextToSide("No Listener for LeftTouchEvent", false);
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
            //Feedback.AddTextToSide("No Listener for RightTouchEvent", false);
        }
    }

    #endregion


    #region Events with Float Input
    public delegate void FloatEventAction(float inp);
    //public static event FloatEventAction EventFloat;
    //public static event FloatEventAction DefineSpaceEvent;

    public static event FloatEventAction CueEvent;



    public static void CallCueEvent(float Float)
    {
        if (CueEvent != null)
        {
            CueEvent.Invoke(Float);
        }
        else
        {
            //Feedback.AddTextToSide("No Listener",false);
        }
    }

    //public static void CallFloatEvent(float Float)
    //{
    //    EventFloat?.Invoke(Float);
    //}    
    //public static void CallDefineSpaceEvent(float Float)
    //{
    //    //DefineSpaceEvent?.Invoke(Float);
    //    if (DefineSpaceEvent != null)
    //    {
    //        DefineSpaceEvent.Invoke(Float);
    //    }
    //    else
    //    {
    //        Feedback.AddTextToSide("No Listener", false);
    //    }
    //}

    #endregion


    #region Events with GameObject Input
    public delegate void GOEventAction(GameObject GO);
    //public static event GOEventAction ColliderInteractionEvent;
    public static event GOEventAction TargetShotEvent;
    public static event GOEventAction StartVibrationEvent;



    //public static void CallColliderInteractionEvent(GameObject GO)
    //{
    //    //ColliderInteractionEvent?.Invoke(GO);
    //    if (ColliderInteractionEvent != null)
    //    {
    //        ColliderInteractionEvent.Invoke(GO);
    //    }
    //    else
    //    {
    //        Feedback.AddTextToSide("No Listener for Call ColliderInteraction", false);

    //    }
    //}   

    public static void CallTargetShotEvent(GameObject GO)
    {
        if (TargetShotEvent != null)
        {
            TargetShotEvent.Invoke(GO);
        }
        else
        {
            //Feedback.AddTextToSide("No Listener for Call TargetShotEvent",false);
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
            //Feedback.AddTextToSide("No Listener", false);
        }
    }

    #endregion
}
