using UnityEngine;

/// <summary> 
/// Plays randomised footstep and landing sounds. 
/// Called from PlayerAnimatorController, which receives the Animation Events. 
/// </summary> 
[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private AudioClip landingClip;

    [Header("Tuning")]
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;

    private AudioSource _source;

    // Remembering the last clip avoids playing the same sound twice in a row 
    private int _lastIndex = -1;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlayRandomFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0)
        {
            return;
        }

        int index = PickIndex();

        // Slight pitch variation makes repeated sounds feel less artificial 
        _source.pitch = Random.Range(minPitch, maxPitch);
        _source.PlayOneShot(footstepClips[index], volume);
    }

    public void PlayLanding()
    {
        if (landingClip == null)
        {
            return;
        }

        _source.pitch = 1f;
        _source.PlayOneShot(landingClip, volume);
    }

    private int PickIndex()
    {
        if (footstepClips.Length == 1)
        {
            return 0;
        }

        int index;

        // Keep drawing until a different clip from the previous one is found 
        do
        {
            index = Random.Range(0, footstepClips.Length);
        }
        while (index == _lastIndex);

        _lastIndex = index;
        return index;
    }
}