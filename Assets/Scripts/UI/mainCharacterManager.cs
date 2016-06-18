﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[ExecuteInEditMode]
public class mainCharacterManager : MonoBehaviour {

    public GameObject bgAccess;
    public GameObject bgCharacterS, bgCharacterCustom;
    public GameObject startGBtn;
    public GameObject[] prefabs;
    public const int nbPlayers = 4;
    public List<playerSelectionUI> selectPlayerList;

    [System.Serializable]
    public class playerSelectionUI
    {
        public playerSelectionUI()
        {
            confirm = false;
            selectionIndex = 1;
        }
      
        public Text name;
        public const float speedMax = 100, healthMax = 150;
        public GameObject containerArrow, readyBtn, speedShow, healthShow;
        public Transform worldPosition;
        public GameObject playerLayOut, playerObject;

        private bool confirm;
        public bool isConfirm { get { return confirm; } }
        public void setConfirm(bool state) { confirm = state; }
       

        public float speedRatio { get { return robotManager.Robots[selectionIndex].Speed / speedMax;  } }
        public float healthRatio { get { return robotManager.Robots[selectionIndex].Health / healthMax; } }

        private int selectionIndex;
        public int SelectionIndex { get { return selectionIndex; } }
        public void setIndex(int i) { selectionIndex = i; }


    }


    // Use this for initialization
    void Awake() {
        prefabs = Resources.LoadAll<GameObject>("Prefab");
        robotManager robot = new robotManager();
    }

    // Update is called once per frame
    void Update() {

        foreach (playerSelectionUI pUI in selectPlayerList)
        {
            if (Input.GetMouseButtonDown(0) && !pUI.containerArrow.activeInHierarchy && pUI.playerLayOut.activeInHierarchy)
            {
                pUI.containerArrow.SetActive(true);
                pUI.readyBtn.SetActive(false);
            }
        }

    }

    public void onStartCharacterSelection()
    {
        if (bgAccess.activeInHierarchy)
        {
            bgCharacterS.SetActive(true);
            bgAccess.SetActive(false);

            foreach (playerSelectionUI pUI in selectPlayerList)
            {
                pUI.readyBtn.SetActive(false);
                if (!pUI.containerArrow.activeInHierarchy)
                    pUI.containerArrow.SetActive(true);

                assignRobot(pUI);
            }
            startGBtn.SetActive(false);



        }

        else if (bgCharacterS.activeInHierarchy)
        {
            bgCharacterS.SetActive(false);
            bgAccess.SetActive(true);

            foreach (playerSelectionUI pUI in selectPlayerList)
            {
                Destroy(pUI.playerObject);
            }
        }


    }

    public void onStartCharacterCustomisation()
    {
        if (bgAccess.activeInHierarchy)
        {
            bgAccess.SetActive(false);
            bgCharacterCustom.SetActive(true);
        }
    }


    public int returnpUIbyName(Button btn)
    {
        int id = int.Parse(btn.name.Substring(btn.name.Length - 1, 1));
        return id - 1;
    }

    public void onNext(Button btn)
    {
        playerSelectionUI pUI = selectPlayerList[returnpUIbyName(btn)];
        pUI.setIndex(pUI.SelectionIndex + 1);
        if (pUI.SelectionIndex > prefabs.Length)
            pUI.setIndex(1);

        assignRobot(pUI);

    }

    void assignRobot( playerSelectionUI pUI)
    {
        if (pUI.playerObject != null)
            Destroy(pUI.playerObject);

        Player robot = robotManager.Robots[pUI.SelectionIndex];

       pUI.playerObject = Instantiate(prefabs[pUI.SelectionIndex - 1], pUI.worldPosition.position, Quaternion.identity) as GameObject;
        iTween.ValueTo(pUI.speedShow, iTween.Hash("from", 0, "to", pUI.speedRatio, "time", 0.3f, "onupdate", "onScale","easetype","easeInCubic"));
        iTween.ValueTo(pUI.healthShow, iTween.Hash("from", 0, "to", pUI.healthRatio, "time", 0.3f, "onupdate", "onScale", "easetype", "easeInCubic"));
        pUI.name.text = robot.Name;

    }


   

    public void onPrev(Button btn)
    {

        playerSelectionUI pUI = selectPlayerList[returnpUIbyName(btn)];
        pUI.setIndex(pUI.SelectionIndex - 1);
        if (pUI.SelectionIndex < 1)
            pUI.setIndex(prefabs.Length);

        assignRobot(pUI);


    }

    public void onConfirm(Button btn)
    {
        if(returnpUIbyName(btn) < selectPlayerList.Count)
        {
            onSelect(returnpUIbyName(btn));
            startGBtn.SetActive(arePlayersReady());
        }
           
    }

    bool arePlayersReady()
    {
        bool isReady = true;
        foreach (playerSelectionUI pUI in selectPlayerList)
        {
            if (pUI.isConfirm == false)
            {
                isReady = false;
                break;
            }
        }
        return isReady;
    }

    void onSelect(int id)
    {
        selectPlayerList[id].setConfirm(true);
        selectPlayerList[id].readyBtn.SetActive(true);
        selectPlayerList[id].containerArrow.SetActive(false);
    }

    public void onSave()
    {

    }

   



  }


