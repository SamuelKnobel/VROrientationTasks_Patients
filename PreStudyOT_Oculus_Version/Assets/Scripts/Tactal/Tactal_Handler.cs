using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;

public class Tactal_Handler : MonoBehaviour
{
    // Haptic feedback
    public GameObject TactalPrefab;
    public TactSource HeadTactSource;
    public Tactal HeadBand;
    public BhapticsManager hapticPlayer;
    public bool TactalConnected;

    void Start()
    {
        InitTactal();
    }

    // Update is called once per frame
    void Update()
    {
        TactalConnected = TactalIsConnected();
    }

    public void InitTactal()
    {
        hapticPlayer = FindObjectOfType<BhapticsManager>();

        // Instantiate HapticSource object from HapticMotorPrefab
        GameObject TactalObj = Instantiate(TactalPrefab);
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
