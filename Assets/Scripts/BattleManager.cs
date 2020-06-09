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
    public GameObject WONPanel;
    public GameObject LOSTPanel;
    
    // camera selection
    public GameObject MainCamera;
    public bool cameraFollowing = false;

    //contains array of unit model with parent object having the stats of the unit
    public UnitLinkList HeroPartyList;
    public UnitLinkList EnemyPartyList;
    //public int HeroIndex = 0;
    //public int EnemyIndex = 0;

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
                cameraFollowing = false;
                break;

            case BattleState.SelectTarget:

                /*
                bool x = Input.GetButtonDown("LeftArrow");
                bool y = Input.GetKeyDown(KeyCode.LeftArrow);
                bool w = Input.GetKey(KeyCode.LeftArrow);
                float z = Input.GetAxis("horizontal");
                */

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log(  "Attacker " +
                                HeroPartyList.GetCurr().GetComponentInParent<BaseUnit>().name +
                                " Defender " +
                                EnemyPartyList.GetCurr().GetComponentInParent<BaseUnit>().name  );
                    
                    Attack(HeroPartyList.GetCurr(), EnemyPartyList.GetCurr());
                    
                    // if unit is dead
                    if(EnemyPartyList.GetCurr().GetComponentInParent<BaseUnit>().CurrentHealthPoints < 1)
                    {
                        EnemyPartyList.Remove();
                    }
                    
                    // next units turn
                    HeroPartyList.GetNext();
                    state = BattleState.NextUnit;
                }
                else if(Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Debug.Log("left");
                    // get next unit
                    EnemyPartyList.GetNext();
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Debug.Log("right");
                    // get previous unit
                    EnemyPartyList.GetPrev();
                }

                // move camera to enemy unit
                cameraFollowing = true;

                // get camera target
                MainCamera.GetComponent<CameraFollow>().CameraTarget = EnemyPartyList.GetCurr();
                
                break;

            // selects the next hero unit that is going to move
            case BattleState.NextUnit:

                //Debug.Log("NEXT UNIT STATE");

                // checks to see if one of the parties is dead and moves state to WON or LOST state
                if(isPartyDead())
                {
                    break;
                }

                // if all hero units have had their turn
                if(HeroPartyList.isBackToHead())
                {
                    state = BattleState.EnemyTurn;
                }
                else
                {
                    state = BattleState.HeroTurn;
                }

                break;

            case BattleState.EnemyTurn:
                // run enemy attack code

                Debug.Log("Enemy attacks");
                

                // do nothing for now
                state = BattleState.HeroTurn;

                break;

            case BattleState.WON:
                ActionPanel.SetActive(false);
                WONPanel.SetActive(true);
                if(cameraFollowing)
                {
                    cameraFollowing = false;
                }
                break;

            case BattleState.LOST:
                ActionPanel.SetActive(false);
                LOSTPanel.SetActive(true);
                if(cameraFollowing)
                {
                    cameraFollowing = false;
                }
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

    // check if either party is dead
    // moves the state to won or lost
    public bool isPartyDead()
    {
        // check if hero party is dead
        if(HeroPartyList.size == 0)
        {
            state = BattleState.LOST;
            return true;
        }
            
        // check if Enemy party is dead
        if(EnemyPartyList.size == 0)
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
        int EnemyPartySize = 3; //Random.Range(1,Enemy.transform.childCount + 1);

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
            UnitStatsInit(unit.transform.parent.gameObject, i);
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
            UnitStatsInit(unit.transform.parent.gameObject, i);
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
    void UnitStatsInit(GameObject unit, int i)
    {
        BaseUnit UnitStats = unit.gameObject.GetComponent<BaseUnit>();

        UnitStats.UnitName = "Unit " + (i+1); //unit.gameObject.GetComponent<BaseUnit>().name;
        UnitStats.Level = 1;

        UnitStats.BaseHealthPoints = Random.Range(5,10);
        UnitStats.BaseMagicPoitns = Random.Range(5,10);
        UnitStats.BaseSkillPoints = Random.Range(5,10);

        UnitStats.CurrentHealthPoints = UnitStats.BaseHealthPoints;
        UnitStats.CurrentMagicPoitns = UnitStats.BaseMagicPoitns;
        UnitStats.CurrentSkillPoints = UnitStats.BaseSkillPoints;

        UnitStats.Strength = Random.Range(5,15);
        UnitStats.Endurance = Random.Range(5,10);
        UnitStats.Dexterity = Random.Range(5,15);
        UnitStats.Intelligence = Random.Range(5,15);
        UnitStats.Agility = Random.Range(5,15);

        // no need to set these
        //UnitStats.Element = BaseUnit.ElementType.Fire;
    }
}