using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;

public class TactalHandler : MonoBehaviour
{

    // Haptic feedback
    public GameObject TactalPrefab;
    public TactSource HeadTactSource;
    public Tactal HeadBand;
    public BhapticsManager hapticPlayer;
    public bool TactalConnected;


	public bool VibrationOn;
    public float ActivationAngle;

    public Timer StopVibrationTimer;

    void OnEnable()
    {
		EventManager.EventStartVibration += StartVibration;
    }
    void OnDisable()
    {
		EventManager.EventStartVibration -= StartVibration;
    }
    void Start()
    {
        InitTactal();
        StopVibrationTimer = gameObject.AddComponent<Timer>();
        StopVibrationTimer.Duration = 1;
        StopVibrationTimer.AddTimerFinishedEventListener(StopVibration);
    }

    // Update is called once per frame
    void Update()
    {
        TactalConnected = TactalIsConnected();
        if (VibrationOn)
        {
            HeadBand.Play(ActivationAngle);
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            //HeadTactSource.Play();
            HeadBand.Play(-10);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            //HeadTactSource.Play();
            HeadBand.Play(10);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            //HeadTactSource.Play();
            HeadBand.Play(50);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            //HeadTactSource.Play();
            HeadBand.Play(-50);
        }
    }

    public void StartVibration(GameObject newTarget)
    {
        ActivationAngle = Vector3.SignedAngle(Camera.main.transform.forward, new Vector3(newTarget.transform.position.x, 0, newTarget.transform.position.z), Vector3.up);
        StopVibrationTimer.Run();
        VibrationOn = true;
    }
    public void StopVibration()
    {
        VibrationOn = false;
        StopVibrationTimer.Duration = 1;
    }

    public void InitTactal()
    {
        hapticPlayer = FindObjectOfType<BhapticsManager>();

        // Instantiate HapticSource object from HapticMotorPrefab
        GameObject TactalObj = Instantiate(TactalPrefab, GameObject.Find("GameControll").transform);
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
