using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
public static class EventManager 
{
    #region Events without Input
    public delegate void EventAction();
	//public static event EventAction TriggerLeftEvent; // NO listener yet
	//public static event EventAction TriggerRightEvent;// NO listener yet    
	//public static event EventAction TouchLeftEvent; // NO listener yet
	//public static event EventAction TouchRightEvent;// NO listener yet
	//public static event EventAction TriggerEvent;// NO listener yet    
	[SyncEvent]
	public static event EventAction EventDefineNewTarget;
	[SyncEvent]
	public static event EventAction EventStartSearching;

    public static void CallStartSearchingEvent()
    {
        if (EventStartSearching != null)
        {
            EventStartSearching.Invoke();
        }
        else
        {
           // Feedback.AddTextToSide("No Listener for StartSeachringEvent", false);
        }
    }      
    public static void CallDefineNewTargetEvent()
    {
        if (EventDefineNewTarget != null)
        {
            EventDefineNewTarget.Invoke();
        }
        else
        {
           // Feedback.AddTextToSide("No Listener for DefineNewTargetEvent", false);
        }
    }     
   
    #endregion


    #region Events with Float Input
    public delegate void FloatEventAction(float inp);
	//public static event FloatEventAction EventFloat;
	//public static event FloatEventAction DefineSpaceEvent;
	[SyncEvent]
	public static event FloatEventAction EventCue;



    public static void CallCueEvent(float Float)
    {
        if (EventCue != null)
        {
            EventCue.Invoke(Float);
        }
        else
        {
         //   Feedback.AddTextToSide("No Listener",false);
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
	[SyncEvent]
	public static event GOEventAction EventTargetShot;
	[SyncEvent]
	public static event GOEventAction EventStartVibration;



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
        if (EventTargetShot != null)
        {
            EventTargetShot.Invoke(GO);
        }
        else
        {
        //    Feedback.AddTextToSide("No Listener for Call TargetShotEvent",false);
        }
    }
    public static void CallStartVibrationEvent(GameObject GO)
    {
        if (EventStartVibration != null)
        {
            EventStartVibration.Invoke(GO);
        }
        else
        {
            //Feedback.AddTextToSide("No Listener", false);
        }
    }

    #endregion
}
