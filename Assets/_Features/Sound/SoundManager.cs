using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Pooling Settings")]
    public int sfxPoolSize = 20; // Max number of simultaneous SFX

    // AudioSource for BGM (Only one needed)
    private AudioSource bgmSource;

    // Pool of AudioSources for SFX (Reused)
    private List<AudioSource> sfxSources;

    // [Core] Throttling: Records the last played time of each clip to prevent overlap noise
    private Dictionary<AudioClip, float> lastPlayedTimes = new Dictionary<AudioClip, float>();

    // Default throttle time: prevents the same clip from playing again within 0.05s
    private const float DEFAULT_THROTTLE = 0.05f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeAudioSources();
    }

    void InitializeAudioSources()
    {
        // 1. Create BGM Source
        GameObject bgmObj = new GameObject("BGM_Source");
        bgmObj.transform.SetParent(transform);
        bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.loop = true;

        // 2. Create SFX Source Pool
        GameObject sfxContainer = new GameObject("SFX_Pool_Container");
        sfxContainer.transform.SetParent(transform);
        sfxSources = new List<AudioSource>();

        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sfxObj = new GameObject($"SFX_Source_{i}");
            sfxObj.transform.SetParent(sfxContainer.transform);
            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.playOnAwake = false; // Disable play on awake
            sfxSources.Add(source);
        }
    }

    // --- [BGM Functions] ---
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        // Ignore if the same clip is already playing
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume * masterVolume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // --- [SFX Functions - Includes Throttling & Pitch Variation] ---
    public void PlaySFX(AudioClip clip, float pitchVariation = 0f)
    {
        if (clip == null) return;

        // 1. [Throttling] Prevent sound overlap
        // If less than 0.05s passed since the last play of this clip, skip it.
        if (lastPlayedTimes.ContainsKey(clip))
        {
            float lastTime = lastPlayedTimes[clip];
            if (Time.time - lastTime < DEFAULT_THROTTLE)
            {
                return; // Request ignored (too fast)
            }
        }
        // Update last played time
        lastPlayedTimes[clip] = Time.time;

        // 2. Find an available AudioSource
        AudioSource source = GetFreeSFXSource();
        if (source == null) return; // Pool is full (too many sounds playing)

        // 3. Configure and play
        source.clip = clip;
        source.volume = sfxVolume * masterVolume;

        // Randomize pitch: fluctuate around 1.0
        if (pitchVariation > 0)
        {
            source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        }
        else
        {
            source.pitch = 1f;
        }

        source.Play();
    }

    // Find and return an idle source from the pool
    private AudioSource GetFreeSFXSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        // If all sources are busy? 
        // 1. Do not play (Performance recommended)
        // 2. Stop the oldest sound and play (Complex implementation)
        // Here we choose option 1.
        return null;
    }

    // (Optional) Called from Settings UI
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        bgmSource.volume = bgmVolume * masterVolume;
    }
}