using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    public static MusicController MusicInstance;
    public AudioSource backMusic;
    public AudioSource[] Sounds;
    private void Awake()
    {
        backMusic = GetComponent<AudioSource>();
        if (MusicInstance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            MusicInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void SoundControll()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            var temp = PlayerPrefs.GetInt("Sound");
            if (temp != 0) //off
            {
                for(int i = 0;i<Sounds.Length;i++)
                    Sounds[i].mute = true;
            }
            else
            {
                for (int i = 0; i < Sounds.Length; i++)
                    Sounds[i].mute = false;
            }
        }
    }
    public void MusicControll()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            var temp = PlayerPrefs.GetInt("Music");
            if (temp != 0) //off
            {
                backMusic.mute = true;
            }
            else
            {
                backMusic.mute = false;
            }
        }
    }
    public void PlayClickSound()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
                Sounds[3].Play();
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Main")
        {
            backMusic.volume = 0.15f;
        }
        else
        {
            backMusic.volume = 0.5f;
        }
    }
}
