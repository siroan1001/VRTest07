using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    //BGM = 0,
    SE_GAKO = 0,
    SE_WAIT,
    SE_ROLL,
    SE_PUSH,
    SE_PAYOUT,
    SOUND_TYPE_MAX,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private Dictionary<SoundType, AudioSource> audioSources = new Dictionary<SoundType, AudioSource>();
    private string[] str = 
        {
            "Audio/gako", "Audio/wait", "Audio/roll", "Audio/push", "Audio/payout",
        };

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

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < (int)SoundType.SOUND_TYPE_MAX; i++)
        {
            LoadAudioSource((SoundType)i, str[i]);
        }
    }

    public void LoadAudioSource(SoundType type, string path)
    {
        //Debug.Log(path);
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSources[type] = audioSource;
            //Debug.Log("読み込み成功");
        }
        else
        {

            //Debug.Log("読み込み失敗");
        }
    }

    public void PlaySound(SoundType type, float volume = 1.0f, bool loop = false, int playCount = 1)
    {
        if (audioSources.ContainsKey(type))
        {
            StartCoroutine(PlaySoundCoroutine(type, volume, loop, playCount));
        }
    }

    private IEnumerator PlaySoundCoroutine(SoundType type, float volume, bool loop, int playCount)
    {
        AudioSource audioSource = audioSources[type];
        audioSource.volume = volume;
        audioSource.loop = loop;

        for (int i = 0; i < playCount; i++)
        {
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
            if (loop) break; // ループ指定の場合、1回再生して終了
        }

        // 最後に再生を停止
        audioSource.Stop();
    }

    public void StopSound(SoundType type)
    {
        if (audioSources.ContainsKey(type))
        {
            audioSources[type].Stop();
        }
    }
}
