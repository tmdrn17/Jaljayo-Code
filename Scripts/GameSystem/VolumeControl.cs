using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// 슬라이더 값으로 오디오 믹서의 배경음·효과음 볼륨을 바꾼다. 슬라이더 최솟값이면 음소거한다.
public class VolumeControl : MonoBehaviour
{
    private const string BgmParameter = "BGM";
    private const string SfxParameter = "SFX";
    private const float MuteVolume = -80f;

    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider audioSlider;

    // 슬라이더 OnValueChanged 연결
    public void BGMControl()
    {
        SetMixerVolume(BgmParameter);
    }

    // 슬라이더 OnValueChanged 연결
    public void SFXControl()
    {
        SetMixerVolume(SfxParameter);
    }

    private void SetMixerVolume(string parameter)
    {
        float sound = audioSlider.value;

        if (sound == audioSlider.minValue)
        {
            masterMixer.SetFloat(parameter, MuteVolume);
        }
        else
        {
            masterMixer.SetFloat(parameter, sound);
        }
    }
}
