using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (SoundManager.Instance == null) return;

        if (masterSlider != null) masterSlider.value = SoundManager.Instance.masterVolume;
        if (bgmSlider != null) bgmSlider.value = SoundManager.Instance.bgmVolume;
        if (sfxSlider != null) sfxSlider.value = SoundManager.Instance.sfxVolume;

        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);

        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
    }
}