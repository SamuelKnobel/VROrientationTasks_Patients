using Bhaptics.Tact;
using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTactal : MonoBehaviour
{

    // Haptic feedback
    public GameObject TactalPrefab;
    public TactSource HeadTactSource;
    public Tactal HeadBand;
    public BhapticsManager hapticPlayer;
    public bool TactalConnected;
   
    // Start is called before the first frame update
    void Start()
    {
        InitTactal();
    }

    // Update is called once per frame
    void Update()
    {
        TactalConnected = TactalIsConnected();


        // TODO: Play tactal  if SpawneNew Target Event is happening-->event has to be sent 
        // with the angel where the object aperars. Additionally a Timer starts to cut the vibration    



    }

    public void InitTactal()
    {
        hapticPlayer = FindObjectOfType<BhapticsManager>();

        // Instantiate HapticSource object from HapticMotorPrefab
        GameObject TactalObj = Instantiate(TactalPrefab, GameObject.Find("Testing").transform);
        HeadTactSource = TactalObj.GetComponent<TactSource>();

        // Set up TactSource parameters
        HeadTactSource.Position = Pos.Head;
        HeadTactSource.FeedbackType = FeedbackType.DotMode;

        // Create the head band object
        HeadBand = new Tactal(ref HeadTactSource);

        // Set default tactal parameters
        HeadBand.maxIntensityPerc = 50;
        HeadBand.mode = VibrationMode.SACCADIC;
    }

    public bool TactalIsConnected()
    {
        return BhapticsManager.HapticPlayer.IsActive(PositionType.Head);
    }

}
