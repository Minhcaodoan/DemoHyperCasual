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
    [Header("List random BGM play")]
    [SerializeField] private List<AudioClip> randomBGMList = new List<AudioClip>();

    // Các kênh phát (Loa)
    private AudioSource sfxAudioSource;
    private AudioSource bgmAudioSource;
    // Từ điển giúp tìm kiếm âm thanh cực nhanh bằng tên
    private Dictionary<string, AudioClip> soundDictionary;
    private bool isPlayingRandomBGM = false;

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
        bgmAudioSource.loop = false;

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
    private void Update()
    {
        // Nếu đang bật chế độ nhạc ngẫu nhiên, và bài nhạc hiện tại đã phát xong (!isPlaying) -> Tự động đổi bài mới
        if (isPlayingRandomBGM && !bgmAudioSource.isPlaying && randomBGMList.Count > 0)
        {
            PlayNextRandomBGM();
        }
    }
    public void StartRandomBGM()
    {
        if (randomBGMList.Count == 0)
        {
            Debug.LogWarning("Chưa có bản nhạc ngẫu nhiên nào trong danh sách!");
            return;
        }

        isPlayingRandomBGM = true;
        PlayNextRandomBGM();
    }
    private void PlayNextRandomBGM()
    {
        if (randomBGMList.Count == 0) return;

        // Chọn ngẫu nhiên một index trong danh sách
        int randomIndex = Random.Range(0, randomBGMList.Count);
        AudioClip clip = randomBGMList[randomIndex];

        bgmAudioSource.Stop();
        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }
    public void StopRandomBGM()
    {
        isPlayingRandomBGM = false;
        bgmAudioSource.Stop();
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
        StopRandomBGM(); // Tắt chế độ ngẫu nhiên nếu phát nhạc đơn lẻ (như nhạc Menu)
        if (soundDictionary.TryGetValue(musicName, out AudioClip clip) && clip != null)
        {
            if (bgmAudioSource.isPlaying && bgmAudioSource.clip == clip) return;

            bgmAudioSource.Stop();
            bgmAudioSource.clip = clip;
            bgmAudioSource.loop = true; // Nhạc menu thì cho lặp lại
            bgmAudioSource.Play();
        }
    }
}