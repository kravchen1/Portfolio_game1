using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasButton : MonoBehaviour
{
    public Sprite musinOn, musicOff;
    public GameObject[] buttons_settings;
    public Text[] texts_settings;
    public GameObject[] buttons_hide;


    public Sprite Speed_rotate_x1, Speed_rotate_x2, Speed_rotate_x3, Light_platform_on, Light_platform_off;

    public Camera main_camera;

    private bool open_setting = false;
    private void Start()
    {
        if (PlayerPrefs.GetFloat("zoom_camera") == 0)
            PlayerPrefs.SetFloat("zoom_camera", 60);
        if (main_camera != null)
        {
            main_camera.fieldOfView = PlayerPrefs.GetFloat("zoom_camera");
        }
        if (gameObject.name == "Music")
        {
            if (PlayerPrefs.GetString("music") == "No")
            {
                GetComponent<Image>().sprite = musicOff;
                GetComponent<AudioSource>().Stop();
            }
        }
        
        if(gameObject.name == "speed_rotate")
        {
            if (PlayerPrefs.GetInt("speed_rotate") == 1)
                GetComponent<Image>().sprite = Speed_rotate_x1;
            else if(PlayerPrefs.GetInt("speed_rotate") == 2)
                GetComponent<Image>().sprite = Speed_rotate_x2;
            else
                GetComponent<Image>().sprite = Speed_rotate_x3;
        }
        if (gameObject.name == "light_platform")
        {
            if (PlayerPrefs.GetInt("light_platform") == 0)
                GetComponent<Image>().sprite = Light_platform_on;
            else
                GetComponent<Image>().sprite = Light_platform_off;
        }
    }

    public void RestartGame()
    {
        if (PlayerPrefs.GetString("music") == "Yes")
            GetComponent<AudioSource>().Play();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void LoadShop()
    {
        if (PlayerPrefs.GetString("music") == "Yes")
            GetComponent<AudioSource>().Play();
        SceneManager.LoadScene("Shop");
    }
    public void CloseShop()
    {
        if (PlayerPrefs.GetString("music") == "Yes")
            GetComponent<AudioSource>().Play();
        SceneManager.LoadScene("Main");
    }
    public void MusicWork()
    {
        //сейчас музыка выключена и её нужно включить
        if(PlayerPrefs.GetString("music") == "No")
        {
            GetComponent<AudioSource>().Play();
            PlayerPrefs.SetString("music","Yes");
            GetComponent<Image>().sprite = musinOn;
            GameObject.FindGameObjectWithTag("music").GetComponent<DontDestroyMusic>().SoundControl(true);
        }
        else
        {
            PlayerPrefs.SetString("music", "No");
            GetComponent<Image>().sprite = musicOff;
            GameObject.FindGameObjectWithTag("music").GetComponent<DontDestroyMusic>().SoundControl(false);
        }
    }

    public void plus_zoom_camera()
    {
        if (PlayerPrefs.GetString("music") == "Yes")
            GetComponent<AudioSource>().Play();
        if (main_camera.fieldOfView > 45)
        {
            main_camera.fieldOfView--;
            PlayerPrefs.SetFloat("zoom_camera", main_camera.fieldOfView);
        }
    }
    public void minus_zoom_camera()
    {
        if (PlayerPrefs.GetString("music") == "Yes")
            GetComponent<AudioSource>().Play();
        if (main_camera.fieldOfView < 75)
        {
            main_camera.fieldOfView++;
            PlayerPrefs.SetFloat("zoom_camera", main_camera.fieldOfView);
        }
    }
    public void Settings()
    {
        if (PlayerPrefs.GetString("music") == "Yes")
            GetComponent<AudioSource>().Play();
        if (!open_setting)
        {
            open_setting = true;
            foreach (GameObject obj in buttons_settings)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in buttons_hide)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            open_setting = false;
            foreach (GameObject obj in buttons_settings)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in buttons_hide)
            {
                obj.SetActive(true);
            }
        }
    }
    public void click_button_speed_rotate()
    {
        if (PlayerPrefs.GetString("music") == "Yes")
            GetComponent<AudioSource>().Play();
        int r = PlayerPrefs.GetInt("speed_rotate");
        switch(r)
        {
            case 1:
                r++;
                GetComponent<Image>().sprite = Speed_rotate_x2;
                break;
            case 2:
                r++;
                GetComponent<Image>().sprite = Speed_rotate_x3;
                break;
            case 3:
                r = 1;
                GetComponent<Image>().sprite = Speed_rotate_x1;
                break;
        }
        PlayerPrefs.SetInt("speed_rotate", r);
    }
    public void click_button_light_platform()
    {
        if (PlayerPrefs.GetString("music") == "Yes")
            GetComponent<AudioSource>().Play();
        if (PlayerPrefs.GetInt("light_platform") == 0)
        {
            PlayerPrefs.SetInt("light_platform", 1);
            GetComponent<Image>().sprite = Light_platform_off;
        }
        else
        {
            PlayerPrefs.SetInt("light_platform", 0);
            GetComponent<Image>().sprite = Light_platform_on;
        }
    }
}
