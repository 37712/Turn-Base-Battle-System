﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState {HeroTurn, SelectTarget, NextUnit, EnemyTurn, WON, LOST};

public class BattleManager : MonoBehaviour
{
    public BattleState state;

    public GameObject Hero;
    public GameObject Enemy;

    // panel control
    public GameObject ActionPanel;
    
    // camera selection
    public GameObject MainCamera;
    public bool cameraFollowing = false;

    //contains array of unit model with parent object having the stats of the unit
    public UnitLinkList HeroPartyList;
    public UnitLinkList EnemyPartyList;
    public int HeroIndex = 0;
    public int EnemyIndex = 0;

    public bool EnemySurpriseAttack;

    // runs before start
    void Awake()
    {
        // initialise scenario for battle
        BattleSetup();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(EnemySurpriseAttack)
        {
            state = BattleState.EnemyTurn;
            ActionPanel.SetActive(false);
            Debug.Log("Enemy move first");
        }
        else
        {
            state = BattleState.HeroTurn;
            ActionPanel.SetActive(true);
            Debug.Log("Heros move first");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case BattleState.HeroTurn:
                // chose attack, magic spell, or skill
                // for this test we only code for attack
                // make hero actionpanel visible
                ActionPanel.SetActive(true);


                Debug.Log("state is hero turn");

                break;

            case BattleState.SelectTarget:

                if(isPartyDead()) // checks to see if one of the parties is dead
                {
                    break;
                }

                /*
                bool x = Input.GetButtonDown("LeftArrow");
                bool y = Input.GetKeyDown(KeyCode.LeftArrow);
                bool w = Input.GetKey(KeyCode.LeftArrow);
                float z = Input.GetAxis("horizontal");
                */

                // move camera to first enemy unit
                if(!cameraFollowing)
                {
                    // camera the first target to follow
                    MainCamera.GetComponent<CameraFollow>().CameraTarget = EnemyPartyList.GetCurr();
                    cameraFollowing = true;
                }

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    Attack(HeroPartyList.GetCurr(), EnemyPartyList.GetCurr());
                    
                    //state = BattleState.NextUnit;
                }
                else if(Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Debug.Log("left");
                    // get next unit
                    if(EnemyIndex < EnemyPartyList.size - 1)
                    {
                        EnemyIndex++;
                    }
                    else
                    {
                        EnemyIndex = 0;
                    }
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Debug.Log("right");

                    // get previous unit
                    if(EnemyIndex == 0) 
                    {
                        EnemyIndex = EnemyPartyList.size - 1;
                    }
                    else
                    {
                        EnemyIndex--;
                    }
                }

                // give new target for camera to follow
                MainCamera.GetComponent<CameraFollow>().CameraTarget = EnemyPartyList.GetCurr();
                
                break;

            // selects the next hero unit that is going to move
            case BattleState.NextUnit:
                if(HeroIndex == HeroPartyList.size)
                {
                    // all hero units have had their turn
                    // move state to enemy unit turn
                    state = BattleState.EnemyTurn;
                    HeroPartyList.CurrToHead(); // return curr to head
                    HeroIndex = 0;
                }
                else
                {
                    // move hero indext to move next hero unit
                    HeroPartyList.GetNext();
                    HeroIndex++;
                }
                break;

            case BattleState.EnemyTurn:
                // run enemy attack code
                Debug.Log("ENEMY TURN");
                break;

            case BattleState.WON:
                // run enemy attack code
                Debug.Log("Player has WON");
                break;

            case BattleState.LOST:
                // run enemy attack code
                Debug.Log("Player has LOST");
                break;
        }
    }

    // this method calculates damage from attacking unit to defending target
    // also checks if target is dead and kills the object
    public void Attack(GameObject AttackingUnit, GameObject DefendingUnit)
    {
        int Defense = DefendingUnit.GetComponentInParent<BaseUnit>().Endurance;
        int Attack = AttackingUnit.GetComponentInParent<BaseUnit>().Strength;

        // attack calculation
        Attack = Attack - Defense;

        // attack can not be negative and must be atleast 1
        if(Attack < 1) Attack = 1;

        Debug.Log("Attack = " + Attack);

        DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints -= Attack;

        // if defending unit health is less than 1 then unit is dead
        if(DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints < 1)
        {
            DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints = 0;
            DefendingUnit.SetActive(false);

            // remove unit from party list so that it is not selectable anymore
            HeroPartyList.Remove();
        }
    }

    /************************* BUTTON SECTION - START ****************************/

    // this is for button only
    public void AttackButton()
    {
        Debug.Log("attack button pressed");
        // disable action panel
        ActionPanel.SetActive(false);

        //SelectionPanel.SetActive(true);
        state = BattleState.SelectTarget;
    }

    /************************* BUTTON SECTION - END ****************************/

    // depricated, needs fix
    // check if either party is dead
    // moves the state to won or lost
    public bool isPartyDead()
    {
        // check if hero party is dead
        bool isHeroPartyDead = true;
        foreach(GameObject unit in HeroPartyList)
            if(unit.GetComponent<BaseHero>().CurrentHealthPoints > 0)
            {
                isHeroPartyDead = false;
                break;
            }
        if(isHeroPartyDead)
        {
            state = BattleState.LOST;
            return true;
        }
            

        // check if Enemy party is dead
        bool isEnemyPartyDead = true;
        foreach(GameObject unit in EnemyPartyList)
            if(unit.GetComponent<BaseEnemy>().CurrentHealthPoints > 0)
            {
                isEnemyPartyDead = false;
                break;
            }
        if(isEnemyPartyDead)
        {
            state = BattleState.WON;
            return true;
        }

        return false;
    }

    // need to run at awake to set up the battle and for other scripts to be able to find what they need
    void BattleSetup()
    {
        // party size for each party
        int HeroPartySize = Random.Range(1,Hero.transform.childCount + 1);
        int EnemyPartySize = Random.Range(1,Enemy.transform.childCount + 1);

        HeroPartyList = new UnitLinkList();
        EnemyPartyList = new UnitLinkList();

        // initializing Hero units
        // unit.transform.childCount wil give you the number of child
        for(int i = 0; i < HeroPartySize; i++)
        {
            // spawn character
            GameObject unit = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            unit.transform.SetParent(Hero.transform.GetChild(i), false);
            Renderer UnitColor = unit.GetComponent<Renderer>();
            UnitColor.material.SetColor("_Color", Color.cyan);

            // add unit script to object and add to list
            //BaseHero HeroScript = unit.gameObject.AddComponent<BaseHero>();// add to child
            BaseHero HeroScript = unit.transform.parent.gameObject.AddComponent<BaseHero>();// add to parent
            HeroPartyList.Add(unit);

            // initialize unit with random stats for testing
            UnitStatsInit(unit.transform.parent.gameObject);
        }

        // initializing Enemy untis
        for(int i = 0; i < EnemyPartySize; i++)
        {
            // spawn enemy
            GameObject unit = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            unit.transform.SetParent(Enemy.transform.GetChild(i), false);
            Renderer UnitColor = unit.GetComponent<Renderer>();
            UnitColor.material.SetColor("_Color", Color.red);

            // add unit script to object and add to list
            //BaseEnemy EnemyScript = unit.gameObject.AddComponent<BaseEnemy>();// add to child
            BaseEnemy EnemyScript = unit.transform.parent.gameObject.AddComponent<BaseEnemy>();// add to parent
            EnemyPartyList.Add(unit);

            // initialize enemy stat units for test
            UnitStatsInit(unit.transform.parent.gameObject);
        }

        // other way to get children
        /*Transform[] allChildren = Enemy.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            //child.gameObject.SetActive(false);
            GameObject unit = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            unit.transform.SetParent(child, false);
            Renderer UnitColor = unit.GetComponent<Renderer>();
            UnitColor.material.SetColor("_Color", Color.red);
        }*/
    }

    // this method is for testing purposes only
    // initialize units stats with random variables
    void UnitStatsInit(GameObject unit)
    {
        BaseUnit UnitStats = unit.gameObject.GetComponent<BaseUnit>();

        UnitStats.UnitName = unit.gameObject.GetComponent<BaseUnit>().name;
        UnitStats.Level = 1;

        UnitStats.BaseHealthPoints = Random.Range(5,15);
        UnitStats.BaseMagicPoitns = Random.Range(5,10);
        UnitStats.BaseSkillPoints = Random.Range(5,10);

        UnitStats.CurrentHealthPoints = UnitStats.BaseHealthPoints;
        UnitStats.CurrentMagicPoitns = UnitStats.BaseMagicPoitns;
        UnitStats.CurrentSkillPoints = UnitStats.BaseSkillPoints;

        UnitStats.Strength = Random.Range(5,15);
        UnitStats.Endurance = Random.Range(5,15);
        UnitStats.Dexterity = Random.Range(5,15);
        UnitStats.Intelligence = Random.Range(5,15);
        UnitStats.Agility = Random.Range(5,15);

        // no need to set these
        //UnitStats.Element = BaseUnit.ElementType.Fire;
    }
}