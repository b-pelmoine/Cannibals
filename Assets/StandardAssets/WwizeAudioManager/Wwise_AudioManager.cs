using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Script for Audio Management via Wwise function */

namespace EquilibreGames
{
        public class Wwise_AudioManager : Singleton<Wwise_AudioManager> {

#if WWISE
       [SerializeField]
        List<string> bankNames = new List<string>{"WwiseUnitySB"};


        uint bankID = 0;

        //Loading the soundBank 
        public void Awake()
        {
            foreach (string i in bankNames )
                AkSoundEngine.LoadBank(i, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID);
        }
	
	

        //Functions for playing/pausing/resuming some events


        public static void PlayEvent(string eventName, GameObject gameObject)
        {
            AkSoundEngine.PostEvent(eventName,gameObject);
        }


        //The fadeout parameter is the time (in seconds) for sound to stop 
        public static void StopEvent(string eventName, GameObject gameObject, int fadeout)
        {
            uint eventID;
            eventID = AkSoundEngine.GetIDFromString(eventName);
            AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Stop, gameObject, fadeout * 1000, AkCurveInterpolation.AkCurveInterpolation_Sine);
        }


        public static void PauseEvent(string eventName, GameObject gameObject, int fadeout)
        {
            uint eventID;
            eventID = AkSoundEngine.GetIDFromString(eventName);
            AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Pause, gameObject, fadeout * 1000, AkCurveInterpolation.AkCurveInterpolation_Sine);
        }


        public static void ResumeEvent(string eventName, GameObject gameObject, int fadeout)
        {
            uint eventID;
            eventID = AkSoundEngine.GetIDFromString(eventName);
            AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Resume, gameObject, fadeout * 1000, AkCurveInterpolation.AkCurveInterpolation_Sine);
        }



        public static void SetRTPCValue(string rtpcName, float value)
        {
            AkSoundEngine.SetRTPCValue(rtpcName, value);
        } 
#endif
  }
}
