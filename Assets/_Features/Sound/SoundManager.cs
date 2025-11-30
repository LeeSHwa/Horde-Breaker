using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class BGMTrack
{
    [Tooltip("Enter the EXACT Scene Name here (e.g., 'LobbyScene', 'SampleScene')")]
    public string sceneName;
    public AudioClip clip;
    // Removed playDuration as it's rarely used for scene BGM
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Volume Settings")]
    // These will be loaded from PlayerPrefs in Awake
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("BGM Configuration")]
    public List<BGMTrack> bgmPlaylist;
    public float crossfadeDuration = 1.0f;

    [Header("Settings")]
    public int sfxPoolSize = 20;

    // Audio Sources
    private AudioSource bgmSource;
    private AudioSource uiSource; // Added back uiSource definition
    private List<AudioSource> sfxSources;

    // SFX Throttling (Prevents spamming the same sound)
    private Dictionary<AudioClip, float> lastPlayedTimes = new Dictionary<AudioClip, float>();
    private const float DEFAULT_THROTTLE = 0.05f;

    // Runtime variables
    private bool isCrossfading = false;

    [Header("Event BGM Settings")]
    public AudioClip levelUpBGM;

    void Awake()
    {
        // [CORE CHANGE] No DontDestroyOnLoad.
        // A new instance is created for every scene.
        Instance = this;

        // [SETTINGS SHARED] Load volume settings immediately
        LoadVolumeSettings();

        InitializeAudioSources();
    }

    void Start()
    {
        // Play BGM for the current scene immediately
        PlaySceneBGM(SceneManager.GetActiveScene().name);
    }

    // Initialize AudioSources (Called once per scene)
    void InitializeAudioSources()
    {
        // 1. Create BGM Source
        GameObject bgmObj = new GameObject("BGM_Source");
        bgmObj.transform.SetParent(transform);
        bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        // 2. Create UI/Event Source
        GameObject uiObj = new GameObject("UI_Source");
        uiObj.transform.SetParent(transform);
        uiSource = uiObj.AddComponent<AudioSource>();
        uiSource.loop = false;
        uiSource.ignoreListenerPause = true;

        // 3. Create SFX Pool
        GameObject sfxContainer = new GameObject("SFX_Pool_Container");
        sfxContainer.transform.SetParent(transform);
        sfxSources = new List<AudioSource>();

        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sfxObj = new GameObject($"SFX_Source_{i}");
            sfxObj.transform.SetParent(sfxContainer.transform);
            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxSources.Add(source);
        }

        // Apply loaded volumes to the new sources
        UpdateSourceVolumes();
    }

    // Logic to find and play BGM by Scene Name
    private void PlaySceneBGM(string currentSceneName)
    {
        if (bgmPlaylist == null) return;

        BGMTrack targetTrack = bgmPlaylist.Find(track => track.sceneName == currentSceneName);

        if (targetTrack != null)
        {
            // Just play immediately (fresh start)
            bgmSource.clip = targetTrack.clip;
            bgmSource.volume = bgmVolume * masterVolume;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"[SoundManager] No BGM found for scene: {currentSceneName}");
        }
    }

    // --- SFX Logic ---
    public void PlaySFX(AudioClip clip, float pitchVariation = 0f)
    {
        if (clip == null) return;

        // Throttle check
        if (lastPlayedTimes.ContainsKey(clip))
        {
            if (Time.time - lastPlayedTimes[clip] < DEFAULT_THROTTLE) return;
        }
        lastPlayedTimes[clip] = Time.time;

        AudioSource source = GetFreeSFXSource();
        if (source == null) return;

        source.clip = clip;
        source.volume = sfxVolume * masterVolume;

        // Pitch variation
        if (pitchVariation > 0)
            source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        else
            source.pitch = 1f;

        source.Play();
    }

    public void PlayUISound(AudioClip clip)
    {
        // UI sounds usually don't need pitch variation
        PlaySFX(clip, 0f);
    }

    private AudioSource GetFreeSFXSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying) return source;
        }
        return sfxSources[0]; // Reuse first if full
    }

    // --- Level Up Logic ---
    public void PlayLevelUpBGM()
    {
        if (levelUpBGM == null) return;
        uiSource.volume = bgmVolume * masterVolume;
        uiSource.clip = levelUpBGM;
        uiSource.Play();
    }

    public void StopLevelUpBGM()
    {
        if (uiSource.isPlaying) uiSource.Stop();
    }

    // --- Shared Settings Logic (PlayerPrefs) ---

    private void LoadVolumeSettings()
    {
        // Load saved values or default to 1.0 / 0.5
        masterVolume = PlayerPrefs.GetFloat("Vol_Master", 1f);
        bgmVolume = PlayerPrefs.GetFloat("Vol_BGM", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("Vol_SFX", 1f);
    }

    private void UpdateSourceVolumes()
    {
        // Update BGM source immediately
        if (bgmSource != null)
            bgmSource.volume = bgmVolume * masterVolume;

        if (uiSource != null)
            uiSource.volume = bgmVolume * masterVolume;
    }

    // Call these from your Settings UI Sliders
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        PlayerPrefs.SetFloat("Vol_Master", volume); // Save
        UpdateSourceVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        PlayerPrefs.SetFloat("Vol_BGM", volume); // Save
        UpdateSourceVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("Vol_SFX", volume); // Save
        // SFX volume is applied when PlaySFX is called, so no immediate update needed
    }
}