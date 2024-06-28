 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class plantManager : MonoBehaviour
{
    private bool isPlanted;
    private int plantStage;
    private int harvestStage;
    private GameObject plantInstance;
    private GameObject waterDropletInstance;

    private bool plantOk; //did you give it water or not
    private bool plantNeedsSomething;
    private bool plantHarvestReady;

    float time;
    float placeholderLength;

    public GameObject placeholder;
    public GameObject waterDroplet;
    public plantObject[] plantArray;
    public int selectedPlant;

    Vector3 lastPos;
    Vector3 newestPos;
    Vector3 harvestedPlantSpawnPos;
    Quaternion harvestedPlantSpawnRotation;

    //0. cabbage
    //1. tomato


    // Start is called before the first frame update
    void Start()
    {
        placeholderLength = placeholder.GetComponent<Renderer>().bounds.size.y;
        isPlanted = false;
        plantOk = false;
        plantNeedsSomething = false;
    }

    private void Update()
    {
        if (isPlanted)
        {
            if (plantStage > harvestStage)
            {
                plantStage = harvestStage;
            }

            if (time > 0)
            {
                time -= Time.deltaTime;
            }

            if (time <= 0 && plantStage < harvestStage)
            {
                //it doesnt needs smthng
                if (plantOk)
                {
                    Debug.Log("Plant is fine");
                    time = plantArray[selectedPlant].timeBetweenStages;
                    plantStage++;
                    Debug.Log("plant stage: " + plantStage);
                    UpdatePlant();
                    plantOk = false;
                    Setup();
                }
                else
                {
                    if (!plantNeedsSomething)
                    {
                        plantNeedsSomething = true;
                        Debug.Log("Water pls");
                        waterDropletInstance = Instantiate(waterDroplet, new Vector3(placeholder.transform.position.x, placeholder.transform.position.y + 1.7f, 
                            placeholder.transform.position.z), Quaternion.Euler(new Vector3(-90, 0, 0)));

                    }
                }
            }
            else if (time <= 0 && plantStage == harvestStage)
            {
                if (!plantHarvestReady)
                {
                    plantHarvestReady = true;
                    Debug.Log("harvest pls");
                    lastPos = plantInstance.transform.position;
                }
            }

            if(plantHarvestReady == true)
            {
                newestPos = plantInstance.transform.position;
                if(Vector3.Distance(lastPos, newestPos) > 0)
                {
                    Harvest();
                }
            }
        }
    }

    void Plant()
    {
        Setup();
        Debug.Log("plant");
        isPlanted = true;
        plantOk = false;
        UpdatePlant();
    }

    void Harvest()
    {
        plantStage++;
        harvestedPlantSpawnPos = plantInstance.transform.position;
        harvestedPlantSpawnRotation = plantInstance.transform.rotation;

        Destroy(plantInstance);
        plantInstance = Instantiate(plantArray[selectedPlant].plant[plantStage], harvestedPlantSpawnPos, harvestedPlantSpawnRotation);
        
        Debug.Log("harvested");
        isPlanted = false;
        plantInstance = null; 
        plantStage = 0;
    }

    void UpdatePlant()
    {
        Debug.Log("update?");
        Destroy(plantInstance);
        plantInstance = Instantiate(plantArray[selectedPlant].plant[plantStage], new Vector3(placeholder.transform.position.x, placeholder.transform.position.y + placeholderLength/2, placeholder.transform.position.z),
                                       Quaternion.identity);
    }

    private void OnTriggerEnter(Collider collision)//remember this wont work if one of the things interacting doesnt have a rigidbody
    {
        Debug.Log("collision collided");

        if (!isPlanted)
        {
            if (collision.CompareTag("cabbage"))
            {
                Debug.Log("cabbage");
                selectedPlant = 0;
                Plant();
            }

            if (collision.CompareTag("tomato"))
            {
                Debug.Log("tomato");
                selectedPlant = 1;
                Plant();
            }

            if (collision.CompareTag("carrot"))
            {
                Debug.Log("carrot");
                selectedPlant = 2;
                Plant();
            }

            if (collision.CompareTag("turnip"))
            {
                Debug.Log("turnip");
                selectedPlant = 3;
                Plant();
            }
        }

        if (isPlanted)
        {
            if (collision.CompareTag("water") && plantNeedsSomething && plantStage < harvestStage)
            {
                Debug.Log("wotah");
                plantOk = true;
                Destroy(waterDropletInstance);
            }
        }
    }

    private void Setup()
    {
        time = plantArray[selectedPlant].timeBetweenStages;
        harvestStage = plantArray[selectedPlant].plant.Length - 2;//kan yg terakhir itu aset buat pas harvest
        plantHarvestReady = false;
        plantNeedsSomething = false;
    }
}
