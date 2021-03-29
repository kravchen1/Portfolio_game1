using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject cube_for_figure;
    public GameObject explosion_cube;

    public Button Button_Play;

    private List<GameObject> explosions = new List<GameObject>();

    public GameObject[] platforms;
    public Material light_material;
    public Material platform_material;

    public Material[] cube_material;

    public Material gold_cube;

    public GameObject press_hand_first;

    public GameObject restart_button;
    public GameObject end_game_logo;
    public GameObject[] canvasStartPage;
    public GameObject[] canvasButtonsControllet;

    public Text text_score;
    public Text gold_cube_score;
    private int global_score = 0;

    private List<GameObject> AllCubes = new List<GameObject>();
    private List<Material> AllCubes_material = new List<Material>();
    private GameObject[] DownCubes;
    private Position[] check_new_position_DownCubes;
    private int[] unlock_cube;

    private bool Cubes_down, Create_figure = false;

    public Button Button_RotateRight, Button_RotateLeft;//, Button_RotateUp, Button_RotateDown;
    private bool click_rotate_right, click_rotate_left;//, click_rotate_down, click_rotate_up;

    public Button Button_MoveRight, Button_MoveLeft, Button_MoveUp, Button_MoveDown;
    private bool click_move_right, click_move_left, click_move_down, click_move_up;

    public Button Button_Pause;
    private bool pause_on = false;



    private bool b_end_game;
    private bool cubes_fall = false;
    private int y_fall = 0;
    private float max_Y = 0;


    private Figure figure;
    //private Figure next_figure;
    //private bool first_cube = true;

    private void Start()
    {
        //PlayerPrefs.SetInt("gold_cube",900);
        text_score.text = "score: 0" + "\nBest score: " + PlayerPrefs.GetInt("BestScore");
        gold_cube_score.text = ": " + PlayerPrefs.GetInt("gold_cube");
        if (!PlayerPrefs.HasKey("cube 0"))
        {
            PlayerPrefs.SetInt("cube 0", 1);
            PlayerPrefs.SetInt("cube 1", 1);
            PlayerPrefs.SetInt("cube 2", 1);
        }


        unlock_cube = new int[cube_material.Length];


        for (int i = 0; i < cube_material.Length; i++)
        {
            string number_cube = "cube " + i;
            if (PlayerPrefs.GetInt(number_cube) == 1)
            {
                unlock_cube[i] = 1;
            }
            else
            {
                unlock_cube[i] = 0;
            }
        }

        initialized_buttons();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                Physics.gravity = new Vector3(0, -5f, 0);
            }
            else
            {
                Physics.gravity = new Vector3(0, -0.1f, 0);
            }
        }
        else
        {
            //Physics.gravity = new Vector3(0, -0.1f, 0);
        }
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Physics.gravity = new Vector3(0, -5f, 0);
        }
        if (Input.GetKeyUp(KeyCode.F1))
        {
            Physics.gravity = new Vector3(0, -0.1f, 0);
        }
        
        create_new_figure();

        down_cubes();

        on_kinematic_all_cubes();

        processing_buttons();

    }

    private void create_new_figure()
    {
        if(!Cubes_down && Create_figure && !b_end_game)
        {
            figure = initialized_figure();
            #region spawn_next_figure
            /*
            if (first_cube)
            {
                figure = initialized_figure();
                next_figure = initialized_figure();
                first_cube = false;
            }
            else
            {
                figure = next_figure;
                next_figure = initialized_figure();
            } 
            switch(next_figure.type_figure)
            {
                case 1:
                    if(next_figure.horizontal)
                    {
                        if(next_figure.count_cubes == 3)
                        {

                        }
                        else
                        {

                        }
                    }
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            */
            #endregion


            DownCubes = new GameObject[figure.count_cubes];
            check_new_position_DownCubes = new Position[figure.count_cubes];

            int number_material = UnityEngine.Random.Range(0, cube_material.Length);
            while(unlock_cube[number_material] != 1)
            {
                number_material = UnityEngine.Random.Range(0, cube_material.Length);
            }
            Material now_material = cube_material[number_material];
            int r = UnityEngine.Random.Range(0, 100);
            if(r<5)
            {
                now_material = gold_cube;
            }

            int begin = AllCubes.Count;
            int end = AllCubes.Count + figure.count_cubes;
            int j = 0;
            for (int i = begin; i < end; i++)
            {
                j = i - begin;
                AllCubes.Add(Instantiate(cube_for_figure, figure.figures[j].GetVector(), Quaternion.identity) as GameObject);
                AllCubes_material.Add(now_material);
                DownCubes[j] = AllCubes[i];
            }

            lights_platform(figure.count_cubes);
            Round_Y();
            Invoke("Go_Down", 1.3f);
        }
    }
    private Figure initialized_figure()
    {
        Create_figure = false;

        int random_figure = 3;
        int count_cubes = 37;
        /*
        int r = UnityEngine.Random.Range(0, 3);
        if (r == 0)
            count_cubes = 3;
        else
            count_cubes = 5;
        */
        random_figure = UnityEngine.Random.Range(1, 4);
        switch(random_figure)
        {
            case 1:
                int r = UnityEngine.Random.Range(0, 2);
                if(r == 0)
                    count_cubes = 3;
                else
                    count_cubes = 5;
                break;
            case 2:
                count_cubes = 3;
                break;
            case 3:
                 count_cubes = 4;
                break;
        }
        
        

        return(new Figure(count_cubes, cube_for_figure, random_figure));
    }
    private void Go_Down()
    {
        Cubes_down = true;

    }
    private void down_cubes()
    {
        if (Cubes_down && DownCubes != null)
        {
            if (PlayerPrefs.GetInt("press_hand_first") == 0)
                press_hand_first.SetActive(true);
            //print("y: " + DownCubes[0].transform.position.y);
            // print(DownCubes[0].GetComponent<Rigidbody>().velocity);
            bool end_down = false;
            for (int i = 0; (i < DownCubes.Length && Cubes_down && !end_down); i++)
            {
                //print(DownCubes[i].GetComponent<Rigidbody>().velocity.y);
                if (DownCubes[i].GetComponent<Rigidbody>().velocity.y > -0.1)
                {
                    end_down = true;
                }
            }
            if(end_down)
            {
                end_down_cubes();
            }
            check_end_game();
            //Down_Figure.isKinematic = true;
            //Down_Figure.isKinematic = false;
        }
    }
    private void end_down_cubes()
    {
        if (PlayerPrefs.GetInt("press_hand_first") == 0)
            press_hand_first.SetActive(false);
        PlayerPrefs.SetInt("press_hand_first", 1);


        for (int i = 0;i<DownCubes.Length;i++)
        {
            DownCubes[i].GetComponent<Rigidbody>().isKinematic = true;
        }

        complete_combination();
        Cubes_down = false;
        Create_figure = true;
        search_max_y();

        global_score += DownCubes.Length;
        if (global_score > PlayerPrefs.GetInt("BestScore"))
            PlayerPrefs.SetInt("BestScore", global_score);
        text_score.text = "score: " + global_score + "\nBest score: " + PlayerPrefs.GetInt("BestScore");
        
    }
    private void lights_platform(int count_cubes)
    {
        if (PlayerPrefs.GetInt("light_platform") == 0)
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i].GetComponent<MeshRenderer>().material = platform_material;
            }
            for (int i = 0; i < platforms.Length; i++)
            {
                for (int j = 0; j < DownCubes.Length; j++)
                {
                    if (platforms[i].transform.position.x == Math.Round(DownCubes[j].transform.position.x) && platforms[i].transform.position.z == Math.Round(DownCubes[j].transform.position.z))
                    {
                        platforms[i].GetComponent<MeshRenderer>().material = light_material;
                    }
                }
            }

            for (int i = 0; i < AllCubes.Count; i++)
            {
                AllCubes[i].GetComponent<MeshRenderer>().material = AllCubes_material[i];
            }

            for (int i = 0; i < AllCubes.Count - count_cubes; i++)
            {
                for (int j = 0; j < DownCubes.Length; j++)
                {
                    if (AllCubes[i].transform.position.x == Math.Round(DownCubes[j].transform.position.x) && AllCubes[i].transform.position.z == Math.Round(DownCubes[j].transform.position.z))
                    {
                        AllCubes[i].GetComponent<MeshRenderer>().material = light_material;
                    }
                }
            }
        }
    }
    private void initialized_buttons()
    {
        Button_Play.onClick.AddListener(() =>
        {
            if (PlayerPrefs.GetString("music") == "Yes")
                Button_Play.GetComponent<AudioSource>().Play();
            Create_figure = true;
            foreach (GameObject obj in canvasStartPage)
            {
                obj.SetActive(false);
            }
            for (int i = 0; i < canvasButtonsControllet.Length; i++)
            {
                canvasButtonsControllet[i].SetActive(true);
            }
        });

        Button_Pause.onClick.AddListener(() =>
        {
            if (!pause_on)
            {
                pause_on = true;
                Time.timeScale = 0;
                Button_Pause.GetComponent<Image>().color = new Color(80 / 255f, 80 / 255f, 80/255f);
            }
            else
            {
                pause_on = false;
                Time.timeScale = 1;
                Button_Pause.GetComponent<Image>().color = new Color(201/255f, 255/255f, 250/255f);
            }
        });

        Button_RotateRight.onClick.AddListener(() =>
        {
            if (PlayerPrefs.GetString("music") == "Yes")
                Button_RotateRight.GetComponent<AudioSource>().Play();
            click_rotate_right = true;
        });
        Button_RotateLeft.onClick.AddListener(() =>
        {
            if (PlayerPrefs.GetString("music") == "Yes")
                Button_RotateLeft.GetComponent<AudioSource>().Play();
            click_rotate_left = true;
        });

        Button_MoveRight.onClick.AddListener(() =>
        {
            if (PlayerPrefs.GetString("music") == "Yes")
                Button_RotateLeft.GetComponent<AudioSource>().Play();
            click_move_right = true;


        });
        Button_MoveLeft.onClick.AddListener(() =>
        {
            if (PlayerPrefs.GetString("music") == "Yes")
                Button_MoveLeft.GetComponent<AudioSource>().Play();
            click_move_left = true;
        });
        Button_MoveDown.onClick.AddListener(() =>
        {
            if (PlayerPrefs.GetString("music") == "Yes")
                Button_MoveDown.GetComponent<AudioSource>().Play();
            click_move_down = true;
        });
        Button_MoveUp.onClick.AddListener(() =>
        {
            if (PlayerPrefs.GetString("music") == "Yes")
                Button_MoveUp.GetComponent<AudioSource>().Play();
            click_move_up = true;
        });

        if (!PlayerPrefs.HasKey("speed_rotate"))
            PlayerPrefs.SetInt("speed_rotate", 2);
        if (!PlayerPrefs.HasKey("music"))
        {
            PlayerPrefs.SetString("music", "Yes");
        }

    }
    private void processing_buttons()
    {
        if (Cubes_down)
        {
            if (click_rotate_right)
            {
                if (check_rotate_right(DownCubes.Length))
                {
                    rotate_right();
                }
                click_rotate_right = false;
            }

            if (click_rotate_left)
            {
                if (check_rotate_left(DownCubes.Length))
                {
                    rotate_left();
                }
                click_rotate_left = false;
            }

            /*
             
            if (click_rotate_down && check_rotate_down())
            {
                for (int i = DownCubes.Length - 1; i > 0; i--)
                {
                    DownCubes[i].transform.SetPositionAndRotation(
                        new Vector3(DownCubes[i].transform.position.x,
                                    DownCubes[i].transform.position.z + DownCubes[0].transform.position.y - DownCubes[0].transform.position.z,
                                    DownCubes[0].transform.position.y + DownCubes[0].transform.position.z - DownCubes[i].transform.position.y
                        ), Quaternion.identity);
                }
                lights_platform();
                click_rotate_down = false;
            }

            if (click_rotate_up && check_rotate_up())
            {
                for (int i = DownCubes.Length - 1; i > 0; i--)
                {
                    DownCubes[i].transform.SetPositionAndRotation(
                        new Vector3(DownCubes[i].transform.position.x,
                                    DownCubes[0].transform.position.z + DownCubes[0].transform.position.y - DownCubes[i].transform.position.z,
                                    DownCubes[i].transform.position.y + DownCubes[0].transform.position.z - DownCubes[0].transform.position.y
                        ), Quaternion.identity);
                }
                lights_platform();
                click_rotate_up = false;
            }

            */
            if (click_move_right)
            {
                if (check_move_right(DownCubes.Length))
                {
                    for (int i = DownCubes.Length - 1; i >= 0; i--)
                    {
                        DownCubes[i].transform.SetPositionAndRotation(
                            new Vector3(DownCubes[i].transform.position.x + 1,
                                        DownCubes[i].transform.position.y,
                                        DownCubes[i].transform.position.z), Quaternion.identity);
                    }
                    figure.centr_x++;
                    lights_platform(DownCubes.Length);
                }
                click_move_right = false;
            }

            if (click_move_left)
            {
                if (check_move_left(DownCubes.Length))
                {
                    for (int i = DownCubes.Length - 1; i >= 0; i--)
                    {
                        DownCubes[i].transform.SetPositionAndRotation(
                            new Vector3(DownCubes[i].transform.position.x - 1,
                                        DownCubes[i].transform.position.y,
                                        DownCubes[i].transform.position.z), Quaternion.identity);
                    }
                    figure.centr_x--;
                    lights_platform(DownCubes.Length);
                }
                click_move_left = false;
            }

            if (click_move_down)
            {
                if (check_move_down(DownCubes.Length))
                {
                    for (int i = DownCubes.Length - 1; i >= 0; i--)
                    {
                        DownCubes[i].transform.SetPositionAndRotation(
                            new Vector3(DownCubes[i].transform.position.x,
                                        DownCubes[i].transform.position.y,
                                        DownCubes[i].transform.position.z - 1), Quaternion.identity);
                    }
                    figure.centr_z--;
                    lights_platform(DownCubes.Length);
                }
                click_move_down = false;
            }

            if (click_move_up)
            {
                if (check_move_up(DownCubes.Length))
                {
                    for (int i = DownCubes.Length - 1; i >= 0; i--)
                    {
                        DownCubes[i].transform.SetPositionAndRotation(
                            new Vector3(DownCubes[i].transform.position.x,
                                        DownCubes[i].transform.position.y,
                                        DownCubes[i].transform.position.z + 1), Quaternion.identity);
                    }
                    figure.centr_z++;
                    lights_platform(DownCubes.Length);
                }
                click_move_up = false;
            }
        }
    }
    private void search_max_y()
    {
        for(int i = 0;i<AllCubes.Count;i++)
        {
            if (AllCubes[i].transform.position.y > max_Y)
            {
                max_Y = AllCubes[i].transform.position.y;
            }
        }
    }

    private bool check_move_right(int count_cubes)
    {
        bool check = true;

        
        if (DownCubes != null)
        {
            for (int i = DownCubes.Length - 1; i >= 0; i--)
            {
                check_new_position_DownCubes[i].SetVector(
                    new Vector3(DownCubes[i].transform.position.x + 1,
                                DownCubes[i].transform.position.y,
                                DownCubes[i].transform.position.z));
            }

            for (int i = 0; i < check_new_position_DownCubes.Length && check; i++)
            {
                if (check_new_position_DownCubes[i].x <= -3 || check_new_position_DownCubes[i].x > 3 || check_new_position_DownCubes[i].z < -3 || check_new_position_DownCubes[i].z >= 3 || check_new_position_DownCubes[i].y <= 0)
                {
                    check = false;
                    continue;
                }
                    for (int j = AllCubes.Count - 1 - count_cubes; j >= 0 && check; j--)
                    {
                            if (Math.Round(check_new_position_DownCubes[i].y) == Math.Round(AllCubes[j].transform.position.y) && check_new_position_DownCubes[i].x == AllCubes[j].transform.position.x && check_new_position_DownCubes[i].z == AllCubes[j].transform.position.z)
                            {
                                check = false;
                            }
                    }
            }
        }
        else
        {
            check = false;
        }
        //print(1);
        return check;
    }
    private bool check_move_left(int count_cubes)
    {
        bool check = true;

        //check_new_position_DownCubes
        if (DownCubes != null)
        {
            for (int i = DownCubes.Length - 1; i >= 0; i--)
            {
                check_new_position_DownCubes[i].SetVector(
                    new Vector3(DownCubes[i].transform.position.x - 1,
                                DownCubes[i].transform.position.y,
                                DownCubes[i].transform.position.z));
            }
            for (int i = 0; i < check_new_position_DownCubes.Length && check; i++)
            {
                if (check_new_position_DownCubes[i].x <= -3 || check_new_position_DownCubes[i].x > 3 || check_new_position_DownCubes[i].z < -3 || check_new_position_DownCubes[i].z >= 3 || check_new_position_DownCubes[i].y <= 0)
                {
                    check = false;
                    continue;
                }
                for (int j = AllCubes.Count - 1 - count_cubes; j >= 0 && check; j--)
                {
                        if (Math.Round(check_new_position_DownCubes[i].y) == Math.Round(AllCubes[j].transform.position.y) && check_new_position_DownCubes[i].x == AllCubes[j].transform.position.x && check_new_position_DownCubes[i].z == AllCubes[j].transform.position.z)
                        {
                            check = false;
                        }
                }
            }
        }
        else
        {
            check = false;
        }

        return check;
    }
    private bool check_move_down(int count_cubes)
    {
        bool check = true;

        //check_new_position_DownCubes
        if (DownCubes != null)
        {
            for (int i = DownCubes.Length - 1; i >= 0; i--)
            {
                check_new_position_DownCubes[i].SetVector(
                    new Vector3(DownCubes[i].transform.position.x,
                                DownCubes[i].transform.position.y,
                                DownCubes[i].transform.position.z-1));
            }
            for (int i = 0; i < check_new_position_DownCubes.Length && check; i++)
            {
                if (check_new_position_DownCubes[i].x <= -3 || check_new_position_DownCubes[i].x > 3 || check_new_position_DownCubes[i].z < -3 || check_new_position_DownCubes[i].z >= 3 || check_new_position_DownCubes[i].y <= 0)
                {
                    check = false;
                    continue;
                }
                for (int j = AllCubes.Count - 1 - count_cubes; j >= 0 && check; j--)
                {
                        if (Math.Round(check_new_position_DownCubes[i].y) == Math.Round(AllCubes[j].transform.position.y) && check_new_position_DownCubes[i].x == AllCubes[j].transform.position.x && check_new_position_DownCubes[i].z == AllCubes[j].transform.position.z)
                        {
                            check = false;
                        }
                }
            }
        }
        else
        {
            check = false;
        }

        return check;
    }
    private bool check_move_up(int count_cubes)
    {
        bool check = true;

        //check_new_position_DownCubes
        if (DownCubes != null)
        {
            for (int i = DownCubes.Length - 1; i >= 0; i--)
            {
                check_new_position_DownCubes[i].SetVector(
                    new Vector3(DownCubes[i].transform.position.x,
                                DownCubes[i].transform.position.y,
                                DownCubes[i].transform.position.z + 1));
            }
            for (int i = 0; i < check_new_position_DownCubes.Length && check; i++)
            {
                if (check_new_position_DownCubes[i].x <= -3 || check_new_position_DownCubes[i].x > 3 || check_new_position_DownCubes[i].z < -3 || check_new_position_DownCubes[i].z >= 3 || check_new_position_DownCubes[i].y <= 0)
                {
                    check = false;
                    continue;
                }
                for (int j = AllCubes.Count - 1 - count_cubes; j >= 0 && check; j--)
                {
                        if (Math.Round(check_new_position_DownCubes[i].y) == Math.Round(AllCubes[j].transform.position.y) && check_new_position_DownCubes[i].x == AllCubes[j].transform.position.x && check_new_position_DownCubes[i].z == AllCubes[j].transform.position.z)
                        {
                            check = false;
                        }
                }
            }
        }
        else
        {
            check = false;
        }

        return check;
    }

    private bool check_rotate_right(int count_cubes)
    {
        bool check = true;
        float for_zero_x_1, for_zero_x_2, for_zero_z_1, for_zero_z_2, _new_x, _new_z;
        if (DownCubes != null)
        {
            for (int i = DownCubes.Length - 1; i >= 0; i--)
            {
                for_zero_x_1 = DownCubes[i].transform.position.x - figure.centr_x;
                for_zero_z_1 = DownCubes[i].transform.position.z - figure.centr_z;

                for_zero_x_2 = (0 - (for_zero_z_1 * (-1)));
                for_zero_z_2 = (for_zero_x_1 * (-1));

                _new_x = for_zero_x_2 + figure.centr_x;
                _new_z = for_zero_z_2 + figure.centr_z;

                check_new_position_DownCubes[i].SetVector(
                            new Vector3(_new_x,
                            DownCubes[i].transform.position.y,
                            _new_z
                            ));
            }
            for (int i = 0; i < check_new_position_DownCubes.Length && check; i++)
            {
                if (check_new_position_DownCubes[i].x <= -3 || check_new_position_DownCubes[i].x > 3 || check_new_position_DownCubes[i].z < -3 || check_new_position_DownCubes[i].z >= 3 || check_new_position_DownCubes[i].y <= 0)
                {
                    check = false;
                    continue;
                }
                for (int j = AllCubes.Count - 1 - count_cubes; j >= 0 && check; j--)
                {
                        if (Math.Round(check_new_position_DownCubes[i].y) == Math.Round(AllCubes[j].transform.position.y) && check_new_position_DownCubes[i].x == AllCubes[j].transform.position.x && check_new_position_DownCubes[i].z == AllCubes[j].transform.position.z)
                        {
                            check = false;
                        }
                }
            }
        }
        else
        {
            check = false;
        }

        return check;
    }
    private bool check_rotate_left(int count_cubes)
    {
        bool check = true;

        float for_zero_x_1, for_zero_x_2, for_zero_z_1, for_zero_z_2, _new_x, _new_z;
        if (DownCubes != null)
        {
            for (int i = DownCubes.Length - 1; i >= 0; i--)
            {
                for_zero_x_1 = DownCubes[i].transform.position.x - figure.centr_x;
                for_zero_z_1 = DownCubes[i].transform.position.z - figure.centr_z;

                for_zero_x_2 = (0 - for_zero_z_1);
                for_zero_z_2 = for_zero_x_1;

                _new_x = for_zero_x_2 + figure.centr_x;
                _new_z = for_zero_z_2 + figure.centr_z;

                check_new_position_DownCubes[i].SetVector(
                            new Vector3(_new_x,
                            DownCubes[i].transform.position.y,
                            _new_z
                            ));
            }
            for (int i = 0; i < check_new_position_DownCubes.Length && check; i++)
            {
                if (check_new_position_DownCubes[i].x <= -3 || check_new_position_DownCubes[i].x > 3 || check_new_position_DownCubes[i].z < -3 || check_new_position_DownCubes[i].z >= 3 || check_new_position_DownCubes[i].y <= 0)
                {
                    check = false;
                    continue;
                }
                for (int j = AllCubes.Count - 1 - count_cubes; j >= 0 && check; j--)
                {
                        if (Math.Round(check_new_position_DownCubes[i].y) == Math.Round(AllCubes[j].transform.position.y) && check_new_position_DownCubes[i].x == AllCubes[j].transform.position.x && check_new_position_DownCubes[i].z == AllCubes[j].transform.position.z)
                        {
                            check = false;
                        }
                }
            }
        }
        else
        {
            check = false;
        }

        return check;
    }

    private void rotate_right()
    {
        float _new_x;
        float _new_z;

        float for_zero_x_1;
        float for_zero_z_1;

        float for_zero_x_2;
        float for_zero_z_2;
        for (int i = 0; i < DownCubes.Length; i++)
        {
            //print(i + " / (" + DownCubes[i].transform.position.x + ";" + DownCubes[i].transform.position.z + ")");
            for_zero_x_1 = DownCubes[i].transform.position.x - figure.centr_x;
            for_zero_z_1 = DownCubes[i].transform.position.z - figure.centr_z;
            //print(i + " / (" + for_zero_x_1 + ";" + for_zero_z_1 + ")");

            for_zero_x_2 = (0 - (for_zero_z_1 * (-1)));
            for_zero_z_2 = (for_zero_x_1 * (-1));
            //print(i + " / (" + for_zero_x_2 + ";" + for_zero_z_2 + ")");

            _new_x = for_zero_x_2 + figure.centr_x;
            _new_z = for_zero_z_2 + figure.centr_z;
            //print(i + " / (" + _new_x + ";" + _new_z + ")");

            DownCubes[i].transform.SetPositionAndRotation(
                        new Vector3(_new_x,
                        DownCubes[i].transform.position.y,
                        _new_z
                        ), Quaternion.identity);
        }
        lights_platform(DownCubes.Length);
    }
    private void rotate_left()
    {
        float _new_x;
        float _new_z;

        float for_zero_x_1;
        float for_zero_z_1;

        float for_zero_x_2;
        float for_zero_z_2;
        for (int i = 0; i < DownCubes.Length; i++)
        {
            //print(i + " / (" + DownCubes[i].transform.position.x + ";" + DownCubes[i].transform.position.z + ")");
            for_zero_x_1 = DownCubes[i].transform.position.x - figure.centr_x;
            for_zero_z_1 = DownCubes[i].transform.position.z - figure.centr_z;
            //print(i + " / (" + for_zero_x_1 + ";" + for_zero_z_1 + ")");

            for_zero_x_2 = (0 - (for_zero_z_1));
            for_zero_z_2 = (for_zero_x_1);
            //print(i + " / (" + for_zero_x_2 + ";" + for_zero_z_2 + ")");

            _new_x = for_zero_x_2 + figure.centr_x;
            _new_z = for_zero_z_2 + figure.centr_z;
            //print(i + " / (" + _new_x + ";" + _new_z + ")");

            DownCubes[i].transform.SetPositionAndRotation(
                        new Vector3(_new_x,
                        DownCubes[i].transform.position.y,
                        _new_z
                        ), Quaternion.identity);
        }
        lights_platform(DownCubes.Length);
    }

    /*
    private bool check_rotate_down()
    {
        bool check = true;

        //check_new_position_DownCubes
        if (DownCubes != null)
        {
            for (int i = DownCubes.Length - 1; i >= 0; i--)
            {
                check_new_position_DownCubes[i].SetVector(
                    new Vector3(DownCubes[i].transform.position.x,
                                DownCubes[i].transform.position.z + DownCubes[0].transform.position.y - DownCubes[0].transform.position.z,
                                DownCubes[0].transform.position.y + DownCubes[0].transform.position.z - DownCubes[i].transform.position.y));
            }
            for (int i = 0; i < check_new_position_DownCubes.Length && check; i++)
            {
                if (check_new_position_DownCubes[i].x <= -3 || check_new_position_DownCubes[i].x >= 3 || check_new_position_DownCubes[i].z <= -3 || check_new_position_DownCubes[i].z >= 3 || check_new_position_DownCubes[i].y <= 0)
                {
                    check = false;
                    continue;
                }
                for (int j = AllCubes.Count - 1; j >= 0 && check; j--)
                {
                    if (AllCubes[j].GetComponent<Rigidbody>().velocity.y >= -0.01)
                    {
                        if (Math.Round(check_new_position_DownCubes[i].y) == Math.Round(AllCubes[j].transform.position.y) && check_new_position_DownCubes[i].x == AllCubes[j].transform.position.x && check_new_position_DownCubes[i].z == AllCubes[j].transform.position.z)
                        {
                            check = false;
                        }
                    }
                }
            }
        }
        else
        {
            check = false;
        }

        return check;
    }
    private bool check_rotate_up()
    {
        bool check = true;

        //check_new_position_DownCubes
        if (DownCubes != null)
        {
            for (int i = DownCubes.Length - 1; i >= 0; i--)
            {
                check_new_position_DownCubes[i].SetVector(
                    new Vector3(DownCubes[i].transform.position.x,
                                DownCubes[0].transform.position.z + DownCubes[0].transform.position.y - DownCubes[i].transform.position.z,
                                DownCubes[i].transform.position.y + DownCubes[0].transform.position.z - DownCubes[0].transform.position.y));
            }
            for (int i = 0; i < check_new_position_DownCubes.Length && check; i++)
            {
                if (check_new_position_DownCubes[i].x <= -3 || check_new_position_DownCubes[i].x >= 3 || check_new_position_DownCubes[i].z <= -3 || check_new_position_DownCubes[i].z >= 3 || check_new_position_DownCubes[i].y <= 0)
                {
                    check = false;
                    continue;
                }
                for (int j = AllCubes.Count - 1; j >= 0 && check; j--)
                {
                    if (AllCubes[j].GetComponent<Rigidbody>().velocity.y >= -0.01)
                    {
                        if (Math.Round(check_new_position_DownCubes[i].y) == Math.Round(AllCubes[j].transform.position.y) && check_new_position_DownCubes[i].x == AllCubes[j].transform.position.x && check_new_position_DownCubes[i].z == AllCubes[j].transform.position.z)
                        {
                            check = false;
                        }
                    }
                }
            }
        }
        else
        {
            check = false;
        }

        return check;
    }
    */
    private void check_end_game()
    {
        for(int i = 0;i<AllCubes.Count - DownCubes.Length;i++)
        {
            if (AllCubes[i].transform.position.y > 12.5)
            {
                b_end_game = true;
            }
        }
        if(b_end_game)
        {
            end_game();
        }
    }
    private void end_game()
    {
        restart_button.SetActive(true);
        end_game_logo.SetActive(true);
        for (int i = 0; i < canvasButtonsControllet.Length; i++)
        {
            canvasButtonsControllet[i].SetActive(false);
        }
    }
    private void complete_combination()
    {
       // print(1);
        int[] count_coubes_equally_y = new int[10];
        for(int i=0;i<10;i++)
        {
            count_coubes_equally_y[i] = 0;
        }
        for (int i = 0;i<AllCubes.Count;i++)
        {

            if (Math.Round(AllCubes[i].transform.position.y) == 1)
                count_coubes_equally_y[0]++;
            else if (Math.Round(AllCubes[i].transform.position.y) == 2)
                count_coubes_equally_y[1]++;
            else if (Math.Round(AllCubes[i].transform.position.y) == 3)
                count_coubes_equally_y[2]++;
            else if (Math.Round(AllCubes[i].transform.position.y) == 4)
                count_coubes_equally_y[3]++;
            else if (Math.Round(AllCubes[i].transform.position.y) == 5)
                count_coubes_equally_y[4]++;
            else if (Math.Round(AllCubes[i].transform.position.y) == 6)
                count_coubes_equally_y[5]++;
            else if (Math.Round(AllCubes[i].transform.position.y) == 7)
                count_coubes_equally_y[6]++;
            else if (Math.Round(AllCubes[i].transform.position.y) == 8)
                count_coubes_equally_y[7]++;
            else if (Math.Round(AllCubes[i].transform.position.y) == 9)
                count_coubes_equally_y[8]++;
            else if (Math.Round(AllCubes[i].transform.position.y) == 10)
                count_coubes_equally_y[9]++;
        }

        int score = 0;
        for(int i = 0;i<10;i++)
        {
            if (count_coubes_equally_y[i] == platforms.Length)
            {
               // print(11);
                for (int j = 0; j < AllCubes.Count; j++)
                {
                    if (Math.Round(AllCubes[j].transform.position.y) == (i+1))
                    {
                        if (AllCubes[j].GetComponent<MeshRenderer>().material.name == "Gold_cube (Instance)")
                        {
                            PlayerPrefs.SetInt("gold_cube", PlayerPrefs.GetInt("gold_cube") + 1);
                            gold_cube_score.text = ": " + PlayerPrefs.GetInt("gold_cube");
                        }
                        explosions.Add(Instantiate(explosion_cube, AllCubes[j].transform.position, Quaternion.identity));
                        Destroy(AllCubes[j]);
                        AllCubes.RemoveAt(j);
                        AllCubes_material.RemoveAt(j);

                        j--;
                        score++;
                        y_fall = i + 1;
                    }
                }
            }
        }

        if(score>0)
        {
            Invoke("destroy_explosions", 1f);
            if (PlayerPrefs.GetString("music") == "Yes")
                GetComponent<AudioSource>().Play();
            /*for (int i = 0; i < DownCubes.Length; i++)
            {
                DownCubes[i].GetComponent<Rigidbody>().isKinematic = true;
                DownCubes[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            for (int i = 0; i < DownCubes.Length; i++)
            {
                DownCubes[i].GetComponent<Rigidbody>().isKinematic = false;
                DownCubes[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            }*/
            for (int i = 0; i < AllCubes.Count; i++)
            {
                AllCubes[i].GetComponent<Rigidbody>().isKinematic = false;
                //AllCubes[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            
            Invoke("cubes_fall_true", 1.3f);


            global_score += score;
            if (global_score > PlayerPrefs.GetInt("BestScore"))
            {
                PlayerPrefs.SetInt("BestScore", global_score);
            }
            text_score.text = "score: " + global_score + "\nBest score: " + PlayerPrefs.GetInt("BestScore");

            //присвоение очков
        }
    }
    private void cubes_fall_true()
    {
        cubes_fall = true;
    }
    private void on_kinematic_all_cubes()
    {
        bool end_down = false;
        if (cubes_fall)
        {
            for (int i = 0; (i < AllCubes.Count - DownCubes.Length) && !end_down; i++)
            {
                //print(DownCubes[i].GetComponent<Rigidbody>().velocity.y);
                if(AllCubes[i].transform.position.y > y_fall)
                    if (AllCubes[i].GetComponent<Rigidbody>().velocity.y > -0.1)
                    {
                        end_down = true;
                    }
            }
            if (end_down)
            {
                for (int i = 0; i < AllCubes.Count - DownCubes.Length; i++)
                {
                    AllCubes[i].GetComponent<Rigidbody>().isKinematic = true;
                }
                cubes_fall = false;
            }
        }
    }
    private void destroy_explosions()
    {
        for(int i=0;i < explosions.Count;i++)
        {
            Destroy(explosions[i]);
            explosions.RemoveAt(i);
            i--;
        }
    }
    private void Round_Y()
    {
            for (int i = 0; i<AllCubes.Count - DownCubes.Length;i++)
            {
                AllCubes[i].transform.SetPositionAndRotation(new Vector3(AllCubes[i].transform.position.x, Convert.ToInt32(Math.Round(AllCubes[i].transform.position.y)), AllCubes[i].transform.position.z), Quaternion.identity);
            }
    }

    private void pause()
    {
        for (int i = 0; i < AllCubes.Count; i++)
        {
            AllCubes[i].GetComponent<Rigidbody>().isKinematic = pause_on;
        }
    }
}

struct CubePos
{
    public int x, y, z;
    public GameObject cube;
    public CubePos(int x,int y, int z, GameObject cube)
    {
        this.cube = cube;
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 GetVector()
    {
        return new Vector3(x, y, z);
    }
    public void SetVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}
struct Position
{
    public float x, y, z;
    public Position(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 GetVector()
    {
        return new Vector3(x, y, z);
    }
    public void SetVector(Vector3 pos)
    {
        //x = Convert.ToInt32(pos.x);
        //y = Convert.ToInt32(pos.y);
        //z = Convert.ToInt32(pos.z);
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }
}

struct Figure
{
    public CubePos[] figures;
    public int count_cubes;
    public float centr_x;
    public float centr_z;
    public int type_figure;
    public bool horizontal;
    public Figure(int i, GameObject cube, int type_figure)
    {
        this.type_figure = type_figure;
        figures = new CubePos[i];
        count_cubes = i;
        centr_x = 0;
        centr_z = 0;
        horizontal = true;

        switch (type_figure)
        {
            case 1:
                {
                    switch (count_cubes)
                    {
                        case 3:
                            {
                                int r = UnityEngine.Random.Range(0, 2);
                                if (r == 0)
                                {
                                    for (int j = 0; j < i; j++)
                                    {
                                        figures[j].SetVector(new Vector3(0, 13, j - 1));
                                        figures[j].cube = cube;
                                    }
                                    centr_x = 0f;
                                    centr_z = 0f;
                                }
                                else
                                {
                                    int k = 0;
                                    for (int j = i - 1; j >= 0; j--)
                                    {
                                        figures[k].SetVector(new Vector3(0, j+13, 0));
                                        figures[k].cube = cube;
                                        k++;
                                    }
                                    centr_x = 0.5f;
                                    centr_z = 0.5f;
                                    horizontal = false;
                                }
                                break;
                            }
                        case 5:
                            {
                                int k = 0;
                                horizontal = false;
                                for (int j = i - 1; j >= 0; j--)
                                    {
                                        figures[k].SetVector(new Vector3(0, j+13, 0));
                                        figures[k].cube = cube;
                                    k++;
                                    }
                                    centr_x = 0.5f;
                                    centr_z = 0.5f;
                                break;
                            }
                    }
                    break;
                }
            case 2:
                {
                    switch (count_cubes)
                    {
                        case 3:
                            {
                                for (int j = 0; j < (i - 1); j++)
                                {
                                    figures[j].SetVector(new Vector3(0, 13, j));
                                    figures[j].cube = cube;
                                }
                                figures[i - 1].SetVector(new Vector3(1, 13, (i - 2)));
                                figures[i - 1].cube = cube;
                                centr_x = 0.5f;
                                centr_z = 0.5f;
                                break;
                            }
                        case 4:
                            {
                                for (int j = 0; j < (i - 1); j++)
                                {
                                    figures[j].SetVector(new Vector3(0, 13, j - 1));
                                    figures[j].cube = cube;
                                }
                                figures[i - 1].SetVector(new Vector3(1, 13, (i - 3 - 1)));
                                figures[i - 1].cube = cube;
                                centr_x = 0f;
                                centr_z = 0f;
                                break;
                            }
                    }
                    break;
                }
            case 3:
                {
                    switch (i)
                    {
                        case 4:
                            figures[0].SetVector(new Vector3(0, 13, 0));
                            figures[1].SetVector(new Vector3(1, 13, 1));
                            figures[2].SetVector(new Vector3(1, 13, 0));
                            figures[3].SetVector(new Vector3(0, 13, 1));
                            centr_x = 0.5f;
                            centr_z = 0.5f;
                            break;
                        case 8:
                            figures[0].SetVector(new Vector3(0, 13, -1));
                            figures[1].SetVector(new Vector3(0, 13, 0));
                            figures[2].SetVector(new Vector3(0, 13, 1));
                            figures[3].SetVector(new Vector3(0, 13, 2));
                            figures[4].SetVector(new Vector3(1, 13, -1));
                            figures[5].SetVector(new Vector3(1, 13, 0));
                            figures[6].SetVector(new Vector3(1, 13, 1));
                            figures[7].SetVector(new Vector3(1, 13, 2));
                            centr_x = 0.5f;
                            centr_z = 0.5f;
                            break;
                        case 9:
                            figures[0].SetVector(new Vector3(-2, 13, -2));
                            figures[1].SetVector(new Vector3(-2, 13, -1));
                            figures[2].SetVector(new Vector3(-2, 13, 0));
                            figures[3].SetVector(new Vector3(-1, 13, -2));
                            figures[4].SetVector(new Vector3(-1, 13, -1));
                            figures[5].SetVector(new Vector3(-1, 13, 0));
                            figures[6].SetVector(new Vector3(0, 13, -2));
                            figures[7].SetVector(new Vector3(0, 13, -1));
                            figures[8].SetVector(new Vector3(0, 13, 0));
                            centr_x = 0f;
                            centr_z = 0f;
                            break;
                        case 37:
                            figures[0].SetVector(new Vector3(-2, 13, -2));
                            figures[1].SetVector(new Vector3(-2, 13, -1));
                            figures[2].SetVector(new Vector3(-2, 13, 0));
                            figures[3].SetVector(new Vector3(-2, 13, 1));
                            figures[4].SetVector(new Vector3(-2, 13, 2));
                            figures[5].SetVector(new Vector3(-2, 13, -3));
                            figures[6].SetVector(new Vector3(-1, 13, -3));
                            figures[7].SetVector(new Vector3(-1, 13, -2));
                            figures[8].SetVector(new Vector3(-1, 13, -1));
                            figures[9].SetVector(new Vector3(-1, 13, 0));
                            figures[10].SetVector(new Vector3(-1, 13, 1));
                            figures[11].SetVector(new Vector3(-1, 13, 2));
                            figures[12].SetVector(new Vector3(0, 13, -3));
                            figures[13].SetVector(new Vector3(0, 13, -2));
                            figures[14].SetVector(new Vector3(0, 13, -1));
                            figures[15].SetVector(new Vector3(0, 13, 0));
                            figures[16].SetVector(new Vector3(0, 13, 1));
                            figures[17].SetVector(new Vector3(0, 13, 2));
                            figures[18].SetVector(new Vector3(1, 13, -3));
                            figures[19].SetVector(new Vector3(1, 13, -2));
                            figures[20].SetVector(new Vector3(1, 13, -1));
                            figures[21].SetVector(new Vector3(1, 13, 0));
                            figures[22].SetVector(new Vector3(1, 13, 1));
                            figures[23].SetVector(new Vector3(1, 13, 2));
                            figures[24].SetVector(new Vector3(2, 13, -3));
                            figures[25].SetVector(new Vector3(2, 13, -2));
                            figures[26].SetVector(new Vector3(2, 13, -1));
                            figures[27].SetVector(new Vector3(2, 13, 0));
                            figures[28].SetVector(new Vector3(2, 13, 1));
                            figures[29].SetVector(new Vector3(2, 13, 2));
                            figures[30].SetVector(new Vector3(3, 13, -3));
                            figures[31].SetVector(new Vector3(3, 13, -2));
                            figures[32].SetVector(new Vector3(3, 13, -1));
                            figures[33].SetVector(new Vector3(3, 13, 0));
                            figures[34].SetVector(new Vector3(3, 13, 1));
                            figures[35].SetVector(new Vector3(3, 13, 2));
                            figures[36].SetVector(new Vector3(0, 14, 0));
                            centr_x = 0f;
                            centr_z = 0f;
                            break;
                    }
                    for (int j = 0; j < i; j++)
                    {
                        figures[j].cube = cube;
                    }
                    break;
                }
        }
    }
}
