/*==============================================================================
Copyright (c) 2017-2019 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/
using System.Timers;
using UnityEngine;
using Vuforia;

public class PlaneManager : MonoBehaviour
{
    public enum PlaneMode
    {
        GROUND,
    }

    #region PUBLIC_MEMBERS
    public static PlaneMode CurrentPlaneMode = PlaneMode.GROUND;
    public static bool GroundPlaneHitReceived { get; private set; }
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    [SerializeField] PlaneFinderBehaviour planeFinder = null;

    //[Header("Plane, Mid-Air, & Placement Augmentations")]
    //[SerializeField] GameObject planeAugmentation = null;


    const string UnsupportedDeviceTitle = "Unsupported Device";
    const string UnsupportedDeviceBody =
        "This device has failed to start the Positional Device Tracker. " +
        "Please check the list of supported Ground Plane devices on our site: " +
        "\n\nhttps://library.vuforia.com/articles/Solution/ground-plane-supported-devices.html";

    StateManager stateManager;
    SmartTerrain smartTerrain;
    PositionalDeviceTracker positionalDeviceTracker;
    ContentPositioningBehaviour contentPositioningBehaviour;
    TouchHandler touchHandler;


    AnchorBehaviour planeAnchor, midAirAnchor, placementAnchor;
    int automaticHitTestFrameCount;
    static TrackableBehaviour.Status StatusCached = TrackableBehaviour.Status.NO_POSE;
    static TrackableBehaviour.StatusInfo StatusInfoCached = TrackableBehaviour.StatusInfo.UNKNOWN;

    // More Strict: Property returns true when Status is Tracked and StatusInfo is Normal.
    public static bool TrackingStatusIsTrackedAndNormal
    {
        get
        {
            return
                (StatusCached == TrackableBehaviour.Status.TRACKED ||
                 StatusCached == TrackableBehaviour.Status.EXTENDED_TRACKED) &&
                StatusInfoCached == TrackableBehaviour.StatusInfo.NORMAL;
        }
    }

    // Less Strict: Property returns true when Status is Tracked/Normal or Limited/Unknown.
    public static bool TrackingStatusIsTrackedOrLimited
    {
        get
        {
            return
                ((StatusCached == TrackableBehaviour.Status.TRACKED ||
                 StatusCached == TrackableBehaviour.Status.EXTENDED_TRACKED) &&
                 StatusInfoCached == TrackableBehaviour.StatusInfo.NORMAL) ||
                (StatusCached == TrackableBehaviour.Status.LIMITED &&
                 StatusInfoCached == TrackableBehaviour.StatusInfo.UNKNOWN);
        }
    }

    bool SurfaceIndicatorVisibilityConditionsMet
    {
        // The Surface Indicator should only be visible if the following conditions
        // are true:
        // 1. Tracking Status is Tracked or Limited (sufficient for Hit Test Anchors
        // 2. Ground Plane Hit was received for this frame
        // 3. The Plane Mode is equal to GROUND or PLACEMENT(see #4)
        // 4. If the Plane Mode is equal to PLACEMENT and *there's no active touches
        get
        {
            return
                 (TrackingStatusIsTrackedOrLimited &&
                 GroundPlaneHitReceived &&
                 (CurrentPlaneMode == PlaneMode.GROUND
                     && Input.touchCount == 0));
                    
        }
    }

    Timer timer;
    bool timerFinished;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        //VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);

        this.planeFinder.HitTestMode = HitTestMode.AUTOMATIC;


        this.touchHandler = FindObjectOfType<TouchHandler>();


        //this.planeAnchor = this.planeAugmentation.GetComponentInParent<AnchorBehaviour>();
        //this.midAirAnchor = this.midAirAugmentation.GetComponentInParent<AnchorBehaviour>();
        //this.placementAnchor = this.placementAugmentation.GetComponentInParent<AnchorBehaviour>();

        //UtilityHelper.EnableRendererColliderCanvas(this.planeAugmentation, false);
        //UtilityHelper.EnableRendererColliderCanvas(this.midAirAugmentation, false);
        //UtilityHelper.EnableRendererColliderCanvas(this.placementAugmentation, false);

        // Setup a timer to restart the DeviceTracker if tracking does not receive
        // status change from StatusInfo.RELOCALIZATION after 10 seconds.
        this.timer = new Timer(10000);
        this.timer.Elapsed += TimerFinished;
        this.timer.AutoReset = false;
    }

    void Update()
    {
        // The timer runs on a separate thread and we need to ResetTrackers on the main thread.
        if (this.timerFinished)
        {
            ResetTrackers();
            this.timerFinished = false;
        }
    }


    void LateUpdate()
    {
        // The AutomaticHitTestFrameCount is assigned the Time.frameCount in the
        // HandleAutomaticHitTest() callback method. When the LateUpdate() method
        // is then called later in the same frame, it sets GroundPlaneHitReceived
        // to true if the frame number matches. For any code that needs to check
        // the current frame value of GroundPlaneHitReceived, it should do so
        // in a LateUpdate() method.
        GroundPlaneHitReceived = (this.automaticHitTestFrameCount == Time.frameCount);
        Debug.Log(GroundPlaneHitReceived+"------------------------------------");
        // Surface Indicator visibility conditions rely upon GroundPlaneHitReceived,
        // so we will move this method into LateUpdate() to ensure that it is called
        // after GroundPlaneHitReceived has been updated in Update().

        SetSurfaceIndicatorVisible(SurfaceIndicatorVisibilityConditionsMet);
    }
    
    #region MY Custom Editing

    public static bool SetDisplayOffForScreenRectle
    {
        get
        {
            return (StatusCached == TrackableBehaviour.Status.NO_POSE &&
            (StatusInfoCached == TrackableBehaviour.StatusInfo.RELOCALIZING ||
            StatusInfoCached == TrackableBehaviour.StatusInfo.INITIALIZING || 
            StatusInfoCached == TrackableBehaviour.StatusInfo.UNKNOWN)) 
            ||
                (StatusCached == TrackableBehaviour.Status.LIMITED &&
                (StatusInfoCached == TrackableBehaviour.StatusInfo.INITIALIZING ||
                 StatusInfoCached == TrackableBehaviour.StatusInfo.RELOCALIZING ||
                 StatusInfoCached == TrackableBehaviour.StatusInfo.UNKNOWN));

        }
    }

    #endregion

    void OnDestroy()
    {
        Debug.Log("OnDestroy() called.");

        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
        //VuforiaARController.Instance.UnregisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.UnregisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    #endregion // MONOBEHAVIOUR_METHODS


    #region GROUNDPLANE_CALLBACKS

    public void HandleAutomaticHitTest(HitTestResult result)
    {
        this.automaticHitTestFrameCount = Time.frameCount;

    }

    public void HandleInteractiveHitTest(HitTestResult result)
    {
        if (result == null)
        {
            Debug.LogError("Invalid hit test result!");
            return;
        }


    }

    

    #endregion // GROUNDPLANE_CALLBACKS


    #region PUBLIC_BUTTON_METHODS

    public void SetGroundMode(bool active)
    {
        if (active)
        {
            SetMode(PlaneMode.GROUND);
        }
    }




    /// <summary>
    /// This method resets the augmentations and scene elements.
    /// It is called by the UI Reset Button and also by OnVuforiaPaused() callback.
    /// </summary>
    //public void ResetScene()
    //{
    //    Debug.Log("ResetScene() called.");

    //    // reset augmentations
    //    this.planeAugmentation.transform.position = Vector3.zero;
    //    this.planeAugmentation.transform.localEulerAngles = Vector3.zero;
    //    UtilityHelper.EnableRendererColliderCanvas(this.planeAugmentation, false);

        
    //    this.touchHandler.enableRotation = false;
    //}

    /// <summary>
    /// This method stops and restarts the PositionalDeviceTracker.
    /// It is called by the UI Reset Button and when RELOCALIZATION status has
    /// not changed for 10 seconds.
    /// </summary>
    public void ResetTrackers()
    {
        Debug.Log("ResetTrackers() called.");

        this.smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
        this.positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

        // Stop and restart trackers
        this.smartTerrain.Stop(); // stop SmartTerrain tracker before PositionalDeviceTracker
        this.positionalDeviceTracker.Reset();
        this.smartTerrain.Start(); // start SmartTerrain tracker after PositionalDeviceTracker
    }

    #endregion // PUBLIC_BUTTON_METHODS


    #region PRIVATE_METHODS

    /// <summary>
    /// This private method is called by the UI Button handler methods.
    /// </summary>
    /// <param name="mode">PlaneMode</param>
    void SetMode(PlaneMode mode)
    {
        CurrentPlaneMode = mode;

        this.planeFinder.enabled = (mode == PlaneMode.GROUND);

        this.touchHandler.enableRotation = (mode == PlaneMode.GROUND);

    }

    /// <summary>
    /// This method can be used to set the Ground Plane surface indicator visibility.
    /// This sample will display it when the Status=TRACKED and StatusInfo=Normal.
    /// </summary>
    /// <param name="isVisible">bool</param>
    void SetSurfaceIndicatorVisible(bool isVisible)
    {

    }

    /// <summary>
    /// This is a C# delegate method for the Timer:
    /// ElapsedEventHandler(object sender, ElapsedEventArgs e)
    /// </summary>
    /// <param name="source">System.Object</param>
    /// <param name="e">ElapsedEventArgs</param>
    void TimerFinished(System.Object source, ElapsedEventArgs e)
    {
        this.timerFinished = true;
    }
    #endregion // PRIVATE_METHODS


    #region VUFORIA_CALLBACKS

    void OnVuforiaStarted()
    {
        Debug.Log("OnVuforiaStarted() called.");

        stateManager = TrackerManager.Instance.GetStateManager();

        // Check trackers to see if started and start if necessary
        this.positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        this.smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (this.positionalDeviceTracker != null && this.smartTerrain != null)
        {
            if (!this.positionalDeviceTracker.IsActive)
                this.positionalDeviceTracker.Start();
            if (this.positionalDeviceTracker.IsActive && !this.smartTerrain.IsActive)
                this.smartTerrain.Start();
        }
        else
        {
            if (this.positionalDeviceTracker == null)
                Debug.Log("PositionalDeviceTracker returned null. GroundPlane not supported on this device.");
            if (this.smartTerrain == null)
                Debug.Log("SmartTerrain returned null. GroundPlane not supported on this device.");

            MessageBox.DisplayMessageBox(UnsupportedDeviceTitle, UnsupportedDeviceBody, false, null);
        }
    }

    //void OnVuforiaPaused(bool paused)
    //{
    //    Debug.Log("OnVuforiaPaused(" + paused.ToString() + ") called.");

    //    if (paused)
    //        ResetScene();
    //}

    #endregion // VUFORIA_CALLBACKS


    #region DEVICE_TRACKER_CALLBACKS

    void OnTrackerStarted()
    {
        Debug.Log("PlaneManager.OnTrackerStarted() called.");

        this.positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        this.smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (this.positionalDeviceTracker != null && this.smartTerrain != null)
        {
            if (!this.positionalDeviceTracker.IsActive)
                this.positionalDeviceTracker.Start();

            if (!this.smartTerrain.IsActive)
                this.smartTerrain.Start();

            Debug.Log("PositionalDeviceTracker is Active?: " + this.positionalDeviceTracker.IsActive +
                      "\nSmartTerrain Tracker is Active?: " + this.smartTerrain.IsActive);
        }
    }

    void OnDevicePoseStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
    {
        Debug.Log("PlaneManager.OnDevicePoseStatusChanged(" + status + ", " + statusInfo + ")");

        StatusCached = status;
        StatusInfoCached = statusInfo;

        // If the timer is running and the status is no longer Relocalizing, then stop the timer
        if (statusInfo != TrackableBehaviour.StatusInfo.RELOCALIZING && this.timer.Enabled)
        {
            this.timer.Stop();
        }

        switch (statusInfo)
        {
            case TrackableBehaviour.StatusInfo.NORMAL:
                break;
            case TrackableBehaviour.StatusInfo.UNKNOWN:
                break;
            case TrackableBehaviour.StatusInfo.INITIALIZING:
                break;
            case TrackableBehaviour.StatusInfo.EXCESSIVE_MOTION:
                break;
            case TrackableBehaviour.StatusInfo.INSUFFICIENT_FEATURES:
                break;
            case TrackableBehaviour.StatusInfo.RELOCALIZING:
                // Start a 10 second timer to Reset Device Tracker
                if (!this.timer.Enabled)
                {
                    this.timer.Start();
                }
                break;
            default:
                break;
        }
    }

    #endregion // DEVICE_TRACKER_CALLBACK_METHODS
}