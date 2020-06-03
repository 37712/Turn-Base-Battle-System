using System.Collections;
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
    public GameObject[] HeroPartyList;
    public GameObject[] EnemyPartyList;
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

                //Debug.Log("state is hero turn");
                //state++; // moves to next state
                break;

            case BattleState.SelectTarget:

                /*
                bool x = Input.GetButtonDown("LeftArrow");
                bool y = Input.GetKeyDown(KeyCode.LeftArrow);
                bool w = Input.GetKey(KeyCode.LeftArrow);
                float z = Input.GetAxis("horizontal");
                */

                // move camera to first enemy unit
                if(!cameraFollowing) cameraFollowing = true;
                
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Attack");
                    Attack(HeroPartyList[HeroIndex], EnemyPartyList[EnemyIndex]);
                    //state = BattleState.NextUnit;
                }
                else if(Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Debug.Log("left");
                    if(EnemyIndex < EnemyPartyList.Length - 1) EnemyIndex++;
                    else EnemyIndex = 0;
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Debug.Log("right");
                    if(EnemyIndex == 0) EnemyIndex = EnemyPartyList.Length - 1;
                    else EnemyIndex--; 
                }
                MainCamera.GetComponent<CameraFollow>().CameraTarget = EnemyPartyList[EnemyIndex];

                break;

            case BattleState.NextUnit:
                if(HeroIndex == HeroPartyList.Length)
                {
                    // all hero units have had their turn
                    // move state to enemy unit turn
                    state = BattleState.EnemyTurn;
                    HeroIndex = 0;
                }
                else
                {
                    // move hero indext to move next hero unit
                    HeroIndex++;
                }
                break;

            case BattleState.EnemyTurn:
                // run enemy attack code
                Debug.Log("ENEMY TURN");
                break;
        }
    }


    

    // this method calculates damage and applies damage to target
    // also checks if target is dead and kills the object
    public void Attack(GameObject AttackingUnit, GameObject DefendingUnit)
    {
        int Defense = DefendingUnit.GetComponentInParent<BaseUnit>().Endurance;
        int Attack = AttackingUnit.GetComponentInParent<BaseUnit>().Strength;

        // attack calculation
        Attack = Attack - Defense;

        // attack can not be negative and must be atleast 1
        if(Attack < 1) Attack = 1;

        DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints -= Attack;

        // if defending unit health is less than 1 then unit is dead
        if(DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints < 1)
        {
            DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints = 0;
            DefendingUnit.SetActive(false);
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

    // depricated, needs update
    // check if either party is dead
    // moves the state to won or lost
    bool PartyDead()
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
            return true;

        // check if Enemy party is dead
        bool isEnemyPartyDead = true;
        foreach(GameObject unit in EnemyPartyList)
            if(unit.GetComponent<BaseEnemy>().CurrentHealthPoints > 0)
            {
                isEnemyPartyDead = false;
                break;
            }
        if(isEnemyPartyDead)
            return true;

        return false;
    }

    // need to run at awake to set up the battle and for other scripts to be able to find what they need
    void BattleSetup()
    {
        // party size for each party
        int HeroPartySize = Random.Range(1,Hero.transform.childCount + 1);
        int EnemyPartySize = Random.Range(1,Enemy.transform.childCount + 1);

        HeroPartyList = new GameObject[HeroPartySize];
        EnemyPartyList = new GameObject[EnemyPartySize];

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
            HeroPartyList[i] = unit;

            // initialize unit with random stats for testing
            UnitStatsInit(unit.transform.parent.gameObject);
        }

        // initializing Enemy untis
        //int enemyCount = Random.Range(1,Enemy.transform.childCount + 1); // range is from 1 to 3
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
            EnemyPartyList[i] = unit;

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