using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FootstepAudio : MonoBehaviour
{
    [SerializeField]
    private EventReference footstepPath;
    void PlayFootstep()
    {
        RuntimeManager.PlayOneShotAttached("event:/Player/Footstep", gameObject);
    }
}
