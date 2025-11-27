using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class to define a BGM track with duration
[System.Serializable]
public class BGMTrack
{
    public AudioClip clip;
    [Tooltip("Duration to play this track in seconds")]
    public float playDuration;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("BGM Playlist Settings")] // Playlist configuration
    public List<BGMTrack> bgmPlaylist;
    [Tooltip("Time for crossfade transition (seconds)")]
    public float crossfadeDuration = 2.0f;
    [Tooltip("If true, the playlist will loop indefinitely. If false, it stops after the last track.")]
    public bool loopPlaylist = true; // [NEW] Loop toggle

    [Header("Pooling Settings")]
    public int sfxPoolSize = 20; // Max number of simultaneous SFX

    // AudioSource for BGM (Only one needed)
    private AudioSource bgmSource;

    // Pool of AudioSources for SFX (Reused)
    private List<AudioSource> sfxSources;

    // Throttling: Records the last played time of each clip to prevent overlap noise
    private Dictionary<AudioClip, float> lastPlayedTimes = new Dictionary<AudioClip, float>();

    // Default throttle time: prevents the same clip from playing again within 0.05s
    private const float DEFAULT_THROTTLE = 0.05f;

    // Runtime variables for BGM logic
    private int currentTrackIndex = 0;
    private float bgmTimer = 0f;
    private bool isCrossfading = false;
    private bool isPausedByTimeScale = false; // [NEW] State tracking for pause logic

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeAudioSources();
    }

    void Start()
    {
        // Start playing the playlist if it's not empty
        if (bgmPlaylist != null && bgmPlaylist.Count > 0)
        {
            PlayTrack(0); // Start the first track
        }
    }

    // Update loop to handle BGM timer synchronized with game time
    void Update()
    {
        // Auto-Pause Logic based on Time.timeScale
        if (Time.timeScale == 0)
        {
            if (!isPausedByTimeScale)
            {
                if (bgmSource.isPlaying) bgmSource.Pause(); // Physically pause the audio
                isPausedByTimeScale = true;
            }
            return; // Stop timer update
        }
        else
        {
            if (isPausedByTimeScale)
            {
                bgmSource.UnPause(); // Resume audio
                isPausedByTimeScale = false;
            }
        }

        // If playlist is empty or crossfading is happening, do nothing
        if (bgmPlaylist == null || bgmPlaylist.Count == 0) return;
        if (isCrossfading) return;

        // BGM Timer Logic
        if (bgmSource.isPlaying)
        {
            bgmTimer += Time.deltaTime;

            // Check if it's time to switch tracks
            if (bgmTimer >= bgmPlaylist[currentTrackIndex].playDuration)
            {
                PlayNextTrack();
            }
        }
    }

    void InitializeAudioSources()
    {
        // 1. Create BGM Source
        GameObject bgmObj = new GameObject("BGM_Source");
        bgmObj.transform.SetParent(transform);
        bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.loop = true; // Loop is true, but we will switch clip manually

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

    // --- BGM Playlist Logic ---

    public void PlayNextTrack()
    {
        if (bgmPlaylist.Count == 0) return;

        // [NEW] Check for end of playlist if loop is disabled
        if (!loopPlaylist && currentTrackIndex >= bgmPlaylist.Count - 1)
        {
            StopBGM(); // Stop playing
            return;
        }

        // Move to next index (Loop back to 0 if end reached)
        currentTrackIndex = (currentTrackIndex + 1) % bgmPlaylist.Count;

        // Start the track with crossfade
        StartCoroutine(CrossfadeCoroutine(bgmPlaylist[currentTrackIndex].clip));
    }

    private void PlayTrack(int index)
    {
        if (index < 0 || index >= bgmPlaylist.Count) return;

        currentTrackIndex = index;
        bgmTimer = 0f;

        // Initial play (no crossfade for the very first start)
        bgmSource.clip = bgmPlaylist[index].clip;
        bgmSource.volume = bgmVolume * masterVolume;
        bgmSource.Play();
    }

    // Crossfade Coroutine for smooth transition
    private IEnumerator CrossfadeCoroutine(AudioClip newClip)
    {
        isCrossfading = true;
        float startVolume = bgmSource.volume;
        float timer = 0f;

        // 1. Fade Out
        while (timer < crossfadeDuration / 2)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / (crossfadeDuration / 2));
            yield return null;
        }
        bgmSource.volume = 0f;

        // 2. Swap Clip
        bgmSource.clip = newClip;
        bgmSource.Play();
        bgmTimer = 0f; // Reset track timer

        // 3. Fade In
        timer = 0f;
        float targetVolume = bgmVolume * masterVolume;
        while (timer < crossfadeDuration / 2)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, timer / (crossfadeDuration / 2));
            yield return null;
        }
        bgmSource.volume = targetVolume;
        isCrossfading = false;
    }

    // External Control: Pause BGM (e.g. Level Up Screen)
    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    // External Control: Resume BGM
    public void ResumeBGM()
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.UnPause();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // --- [SFX Functions] ---
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
        // If all sources are busy
        // Do not play.
        return null;
    }

    // fix error in setting master volume during bgm fading
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        if (!isCrossfading) // Only update immediately if not fading
        {
            bgmSource.volume = bgmVolume * masterVolume;
        }
    }
}