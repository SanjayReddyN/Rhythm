using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Runtime.InteropServices;

public class FMODManager : MonoBehaviour
{
    public static FMODManager Instance { get; private set; }

    [SerializeField] private EventReference musicEventRef;
    private EventInstance musicInstance;
    private GCHandle timelineHandle;

    [System.Serializable]
    public class TimelineInfo
    {
        public int currentBeat = 0;
        public int currentBar = 0;
        public float tempo = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    private TimelineInfo timelineInfo;
    public event Action OnBeat;
    public event Action OnBarStart;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeMusic();
    }

    private void InitializeMusic()
    {
        try
        {
            if (musicEventRef.IsNull)
            {
                Debug.LogError("FMODManager: Music event reference is null!");
                return;
            }

            // Create timeline info
            timelineInfo = new TimelineInfo();
            timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);

            // Create and set up music instance
            musicInstance = RuntimeManager.CreateInstance(musicEventRef);
            musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

            // Set up callback
            musicInstance.setCallback(BeatEventCallback,
                EVENT_CALLBACK_TYPE.TIMELINE_BEAT |
                EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        }
        catch (Exception e)
        {
            Debug.LogError($"FMODManager: Error during initialization: {e.Message}");
        }
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    private static FMOD.RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        try
        {
            EventInstance instance = new EventInstance(instancePtr);

            // Get the timeline info from user data
            instance.getUserData(out IntPtr timelineInfoPtr);

            if (timelineInfoPtr != IntPtr.Zero)
            {
                GCHandle handle = GCHandle.FromIntPtr(timelineInfoPtr);
                TimelineInfo timelineInfo = (TimelineInfo)handle.Target;

                switch (type)
                {
                    case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                        var beatProps = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(
                            parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));

                        timelineInfo.currentBeat = beatProps.beat;
                        timelineInfo.currentBar = beatProps.bar;
                        timelineInfo.tempo = beatProps.tempo;

                        // Dispatch on main thread
                        if (Instance != null)
                        {
                            Instance.DispatchBeatEvent(beatProps.beat);
                        }
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"FMODManager: Error in beat callback: {e.Message}");
        }
        return FMOD.RESULT.OK;
    }

    private void DispatchBeatEvent(int beat)
    {
        // Must be called on main thread
        if (beat == 0)
        {
            OnBarStart?.Invoke();
        }
        OnBeat?.Invoke();
    }

    public void StartMusic()
    {
        try
        {
            if (!musicInstance.isValid())
            {
                Debug.LogError("FMODManager: Invalid music instance!");
                return;
            }

            musicInstance.start();
        }
        catch (Exception e)
        {
            Debug.LogError($"FMODManager: Error starting music: {e.Message}");
        }
    }

    public void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // Specify FMOD.Studio.STOP_MODE
    }

    private void OnDestroy()
    {
        if (musicInstance.isValid())
        {
            musicInstance.setUserData(IntPtr.Zero);
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }

        if (timelineHandle.IsAllocated)
        {
            timelineHandle.Free();
        }
    }

    // Debug controls
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartMusic();
        }

        if (Input.GetKeyDown(KeyCode.N)) // Press N to stop music
        {
            Debug.Log("Manual music stop requested");
            StopMusic();
        }

        // Debug current playback state
        if (musicInstance.isValid())
        {
            PLAYBACK_STATE state;
            musicInstance.getPlaybackState(out state);
            if (Input.GetKeyDown(KeyCode.P)) // Press P to check status
            {
                Debug.Log($"Current playback state: {state}");
            }
        }
    }
}