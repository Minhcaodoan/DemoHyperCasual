using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public struct Sound
    {
        public string name;      // Tên định danh (VD: "PlayerJump", "PlayerHit", "UIClick", "ObstacleBoom")
        public AudioClip clip;   // File âm thanh tương ứng
    }

    [Header("Danh sách tất cả âm thanh trong Game")]
    [SerializeField] private List<Sound> soundList = new List<Sound>();

    // Các kênh phát (Loa)
    private AudioSource sfxAudioSource;
    private AudioSource bgmAudioSource;

    // Từ điển giúp tìm kiếm âm thanh cực nhanh bằng tên
    private Dictionary<string, AudioClip> soundDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Tạo loa
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource.loop = true;

        // Chuyển danh sách (List) thành Từ điển (Dictionary) để tối ưu tốc độ tìm kiếm
        soundDictionary = new Dictionary<string, AudioClip>();
        foreach (var sound in soundList)
        {
            if (!soundDictionary.ContainsKey(sound.name))
            {
                soundDictionary.Add(sound.name, sound.clip);
            }
        }
    }

    // --- HÀM PHÁT ÂM THANH TỔNG QUÁT ---
    public void PlaySFX(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            AudioClip clip = soundDictionary[soundName];
            if (clip != null)
            {
                sfxAudioSource.PlayOneShot(clip);
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy âm thanh có tên: " + soundName);
        }
    }

    public void PlayBGM(string musicName)
    {
        if (soundDictionary.ContainsKey(musicName))
        {
            AudioClip clip = soundDictionary[musicName];
            if (clip != null && bgmAudioSource.clip != clip)
            {
                bgmAudioSource.clip = clip;
                bgmAudioSource.Play();
            }
        }
    }
}