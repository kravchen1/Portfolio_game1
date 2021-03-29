using UnityEngine;
using UnityEngine.UI;
public class ButtonBuy : MonoBehaviour
{
    public GameObject explosion_cube;
    public Material[] material_cubes;
    public Material closed_cube;
    public GameObject[] view_cubes;

    public GameObject music_effect_for_get_skin;
    public Material light_cube;
    public Text gold_cube_score;

    private int random;
    private int for_invoke;
    private Color lerped_color;

    private bool go;
    private bool flick;
    private bool end_anim;

    private int[] flick_cubes;
    void Start()
    {
        flick_cubes = new int[material_cubes.Length];
        start_init();
    }

    void Update()
    {
        if(go)
        {
            for (int i = 0; i < material_cubes.Length; i++)
            {
                if (flick_cubes[i] == 0)
                {
                    lerped_color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 0.5f));
                    view_cubes[i].GetComponent<MeshRenderer>().material.color = lerped_color;
                }
            }
            flick_material();

            if(check_end_animation() && !end_anim)
            {
                if (PlayerPrefs.GetString("music") == "Yes")
                    music_effect_for_get_skin.GetComponent<AudioSource>().Play();
                PlayerPrefs.SetInt("gold_cube", PlayerPrefs.GetInt("gold_cube") - 10);
                gold_cube_score.text = ": " + PlayerPrefs.GetInt("gold_cube");
                Instantiate(explosion_cube, view_cubes[random].transform.position, Quaternion.identity);
                end_anim = true;
                flick = false;
                go = false;
                view_cubes[random].GetComponent<MeshRenderer>().material = material_cubes[random];
                string number_cube = "cube " + random;
                PlayerPrefs.SetInt(number_cube, 1);
                start_init();
            }
        }
    }

    private void start_init()
    {
        flick = true;
        end_anim = false;
        for (int i = 0; i < material_cubes.Length; i++)
        {
            string number_cube = "cube " + i;
            if (PlayerPrefs.GetInt(number_cube) == 1 || PlayerPrefs.GetInt(number_cube) == 2)
            {
                flick_cubes[i] = 1;
            }
            else
            {
                flick_cubes[i] = 0;
            }
        }
    }
    public void click()
    {
        if (PlayerPrefs.GetString("music") == "Yes")
            GetComponent<AudioSource>().Play();
        if (PlayerPrefs.GetInt("gold_cube") >= 10)
        {
            if (!go && check_close_material())
            {
                gold_cube_score.text = ": " + PlayerPrefs.GetInt("gold_cube");
                random = UnityEngine.Random.Range(3, material_cubes.Length);// material_cubes.Length);
                while (flick_cubes[random] == 1)
                {
                    random = UnityEngine.Random.Range(3, material_cubes.Length);
                }
                print("random: " + random);
                go = true;
            }
        }
    }
    private void flick_material()
    {
        if(flick)
        {
            flick = false;
            for_invoke = UnityEngine.Random.Range(3, material_cubes.Length);
            while (for_invoke!=random && flick_cubes[for_invoke] == 1)
            {
                for_invoke = UnityEngine.Random.Range(3, material_cubes.Length);
            }
            if (for_invoke != random)
                Invoke("close_material", 0.1f);
            else
            {
                flick = true;
            }
        }
    }
    private bool check_close_material()
    {
        bool check = false;
        for(int i = 0; i< flick_cubes.Length && !check;i++)
        {
            if (flick_cubes[i] == 0)
                check = true;
        }
        return check;
    }
    private void close_material()
    {
        if (go)
        {
            if (PlayerPrefs.GetString("music") == "Yes")
                view_cubes[for_invoke].GetComponent<AudioSource>().Play();
            flick = true;
            flick_cubes[for_invoke] = 1;
            view_cubes[for_invoke].GetComponent<MeshRenderer>().material = closed_cube;
        }
    }
    private bool check_end_animation()
    {
        bool check = true;
        for(int i = 0;i< material_cubes.Length; i++)
        {
            if (i != random)
            {
                if (flick_cubes[i] == 0)
                {
                    check = false;
                }
            }
        }
        return check;
    }
}
