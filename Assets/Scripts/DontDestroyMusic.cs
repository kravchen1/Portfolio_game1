using UnityEngine;
public class DontDestroyMusic : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.GetString("music") == "Yes")
        {
            GetComponent<AudioSource>().Play();
        }
        else
        {
            GetComponent<AudioSource>().Pause();
        }
    }
    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("music");
        
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void SoundControl(bool soundOff_On)
    {
        if(soundOff_On)
        {
            GetComponent<AudioSource>().Play();
        }
        else
        {
            GetComponent<AudioSource>().Pause();
        }
    }   

}


