using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public Material[] material_cubes;
    public Material closed_cube;
    public GameObject[] view_cubes;
    public Material lock_cube;

    public GameObject help_click_lock;

    public Text gold_cube_score;

    void Start()
    {
        gold_cube_score.text = ": " + PlayerPrefs.GetInt("gold_cube");
        for (int i = 0; i< material_cubes.Length; i++)
        {
            string number_cube = "cube " + i; 
            if(PlayerPrefs.GetInt(number_cube) == 1)
            {
                view_cubes[i].GetComponent<MeshRenderer>().material = material_cubes[i];
            }
            else if(PlayerPrefs.GetInt(number_cube) == 2)
            {
                view_cubes[i].GetComponent<MeshRenderer>().material = lock_cube;
            }
            else
            {
                view_cubes[i].GetComponent<MeshRenderer>().material = closed_cube;
            }
        }
        if(!PlayerPrefs.HasKey("help_click_lock"))
        {
            help_click_lock.SetActive(true);
            Invoke("help_click_close", 5f);
        }
    }

    private void help_click_close()
    {
        PlayerPrefs.SetInt("help_click_lock", 1);
        help_click_lock.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
