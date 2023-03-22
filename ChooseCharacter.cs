using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCharacter : MonoBehaviour
{ 
    private GameObject[] characterList;
    public GameObject SelectionUI, mcamera;
    TerrainSingleMovementcc playermov;
    public float runs;
    public float walk;
    private int characterIndex;

    [SerializeField]
    Dropdown dp;

    public int genderSelect { get; private set;}


    private void Start()
    {
        characterList = new GameObject[transform.childCount];

        for(int i=0;i<transform.childCount;i++){
            characterList[i]= transform.GetChild(i).gameObject; 
        }

        foreach (GameObject item in characterList)
        {
            item.SetActive(false);
        }

        //Oncharacterchange();
    }

    void Update()
    {
        dp.RefreshShownValue();

        //Debug.Log(dp.value);
        Oncharacterchange();
    }

    public void Oncharacterchange()
    {
        //Debug.Log("character changed");
        //Debug.Log(dp.value);

        switch (dp.value) {
            case 0:
                  chooseMale();
                 break;
            case 1:
                chooseFemale();
                 break;
            case 2:
                   chooseMale2();
                 break;
            default:
                chooseMale();
                break;
        }

    }

    public void chooseMale()
    {

        characterList[0].SetActive(true);
        characterList[1].SetActive(false);
        characterList[2].SetActive(false);
        playermov = GetComponentInChildren<TerrainSingleMovementcc>();
        playermov.Runspeed = 0f;
        playermov.Walkspeed = 0f;
        characterIndex = 0;
        genderSelect = dp.value;
    }
    public void chooseFemale()
    {
        characterList[0].SetActive(false);
        characterList[1].SetActive(true);
        characterList[2].SetActive(false);
        playermov = GetComponentInChildren<TerrainSingleMovementcc>();
        playermov.Runspeed = 0f;
        playermov.Walkspeed = 0f;
        characterIndex = 1;
        genderSelect = dp.value;
    }

    public void chooseMale2()
    {
        Debug.Log("male2called");
        characterList[0].SetActive(false);
        characterList[1].SetActive(false);
        characterList[2].SetActive(true);
        playermov = GetComponentInChildren<TerrainSingleMovementcc>();
        playermov.Runspeed = 0f;
        playermov.Walkspeed = 0f;
        characterIndex = 2;
        genderSelect = dp.value;
    }

    public void confirmChoice(){
        SelectionUI.SetActive(false);
        mcamera.SetActive(false);
        playermov.Runspeed = runs;
        playermov.Walkspeed = walk;

    }

    public void initiateSpawn()
    {
        characterList[0].SetActive(false);
        characterList[1].SetActive(false);
        SelectionUI.SetActive(false);
        mcamera.SetActive(false);
        playermov.Runspeed = 1f;
        playermov.Walkspeed = 1f;
        //gameManager.SpawnPlayer(characterIndex);
        
    }
}
