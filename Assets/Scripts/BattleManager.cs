using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum BattleState {TurnStart, NextUnit, ActionSelect, TargetSelect, EnemyTurn, BattlePhase, TurnEnd, WON, LOST}; // states taken out: NextUnit

public class BattleManager : MonoBehaviour
{
    public BattleState state;

    public GameObject Hero; // current hero selected
    public GameObject Enemy; // current enemy selected

    // panel control
    public GameObject ActionPanel;
    public GameObject WONPanel;
    public GameObject LOSTPanel;
    
    // camera selection
    public GameObject MainCamera;
    public bool cameraFollowing = false;

    //contains liked list of units with parent object having the stats of the unit
    public UnitLinkList HeroPartyList;
    public UnitLinkList EnemyPartyList;

    public int fireCounter = 0; // used to set off the attacks in order
    public int partyindex = 1; // used to know if we have iterated though the Hero/Enemy party
    public int turnCounter = 1; // counte how many turns have passed
    public bool EnemySurpriseAttack; // not yet implemented

    // runs before start
    void Awake()
    {
        // initialise scenario for battle
        BattleSetup();
    }

    // Start is called before the first frame update
    void Start()
    {
        // set battle state
        state = BattleState.TurnStart;

        // set panels
        ActionPanel.SetActive(true);
        
        // depricated, if enemy suprise attack just give +5 to agility to all enemy units
        // or just give enemy a free attack turn
        /*
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
        */
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {   
            // this is where the game makes decitions
            case BattleState.TurnStart:

                Debug.Log("##############Turn counter = " + turnCounter + "################");
                turnCounter++;
                state = BattleState.NextUnit;
                
                break;

            // this is where the game makes decitions
            case BattleState.NextUnit:
                
                // set current unit
                Hero = HeroPartyList.GetCurr();

                if(partyindex > HeroPartyList.size)
                {
                    state = BattleState.EnemyTurn;
                    partyindex = 1; // reset party index to 1
                }
                else
                {
                    state = BattleState.ActionSelect;
                    partyindex++;
                }
                
                break;

            // this is where the player can choose to attack, magic spell, skill, item, run away, etc
            case BattleState.ActionSelect:

                // set panels
                ActionPanel.SetActive(true); // action panel can change the state to target select
                cameraFollowing = false;

                break;

            // this is where the player selects the enemy unit to attack
            case BattleState.TargetSelect:

                //bool x = Input.GetButtonDown("LeftArrow");
                //bool y = Input.GetKeyDown(KeyCode.LeftArrow);
                //bool w = Input.GetKey(KeyCode.LeftArrow);
                //float z = Input.GetAxis("horizontal");
                
                // if space is pressed
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    Hero = HeroPartyList.GetCurr();
                    Enemy = EnemyPartyList.GetCurr();

                    // attach method called
                    StartCoroutine(Attack(Hero, Enemy));
                    
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

            // run enemy attack code
            case BattleState.EnemyTurn:
            
                Enemy = EnemyPartyList.GetCurr();

                // attach method called
                StartCoroutine(Attack(Enemy, HeroPartyList.GetRandom()));

                if(partyindex == EnemyPartyList.size)
                {
                    partyindex = 1; // reset party index to 1
                    state = BattleState.BattlePhase;
                }
                else
                    partyindex++;

                EnemyPartyList.GetNext(); // move to next enemy unit
                
                break;

            // This is where the battle take place
            case BattleState.BattlePhase:

                if(fireCounter <= 20)
                    fireCounter++;
                else
                {
                    state = BattleState.TurnEnd;
                    fireCounter = 0;
                }

                break;

            case BattleState.TurnEnd:

                // if heros are dead
                if(HeroPartyList.size == 0) state = BattleState.LOST;
                // if enemys are dead
                else if(EnemyPartyList.size == 0) state = BattleState.WON;
                // both parties are still standing
                else
                    state = BattleState.TurnStart;

                break;

            case BattleState.WON:

                Debug.Log("Player has won");
                ActionPanel.SetActive(false);
                WONPanel.SetActive(true);
                cameraFollowing = false;
                
                break;

            case BattleState.LOST:

                Debug.Log("Player has lost");
                ActionPanel.SetActive(false);
                LOSTPanel.SetActive(true);
                cameraFollowing = false;

                break;
        }
    }

    // maybe we can giev it a pointer to the node itself instead of a game object
    // this method calculates damage from attacking unit to defending target
    // also checks if target is dead and kills the unit
    public IEnumerator Attack(GameObject AttackingUnit, GameObject DefendingUnit)
    {
        // The yield return line is the point at which execution will pause and be resumed the following frame.
        // makes the corutine wait until it is its turn to execute
        //yield return new WaitUntil(() => (AttackingUnit.GetComponentInParent<BaseUnit>().fire == fireCounter));
        while(AttackingUnit.GetComponentInParent<BaseUnit>().fire != fireCounter) yield return null;

        // if attacking unit and defending unit are still alive, procede with attack
        if(AttackingUnit.activeSelf && DefendingUnit.activeSelf)
        {
            int Defense = DefendingUnit.GetComponentInParent<BaseUnit>().Endurance;
            int Attack = AttackingUnit.GetComponentInParent<BaseUnit>().Strength;

            // damage calculation
            int damage = Attack - Defense;

            // damage can not be negative and must be atleast 1
            if(damage < 1) damage = 1;

            DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints -= damage;

            Debug.Log(  "Attacker " +
                        AttackingUnit.GetComponentInParent<BaseUnit>().name +
                        ", Defender " +
                        DefendingUnit.GetComponentInParent<BaseUnit>().name +
                        " fire " + fireCounter + " atk = " + damage );
            
            // if defending unit health is less than 1, then unit is dead
            // kill off the unit
            if(DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints < 1)
            {
                // remove dead unit from linked list
                if(DefendingUnit.GetComponentInParent<BaseHero>() != null) // if hero
                {
                    Debug.Log("removing hero");
                    HeroPartyList.Remove(DefendingUnit);
                }
                else // if enemy
                {
                    Debug.Log("removing enemy");
                    EnemyPartyList.Remove(DefendingUnit);
                }

                DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints = 0;
                DefendingUnit.SetActive(false);

                Debug.Log(DefendingUnit.GetComponentInParent<BaseUnit>().name + " has died**");
            }
        }
        // attacking or defending unit are dead
        else
        {
            // if attacking unit is dead, do nothing
            if(!AttackingUnit.activeSelf)
            {
                Debug.Log("attack from " + AttackingUnit.GetComponentInParent<BaseUnit>().name + " nullified, because "
                         + AttackingUnit.GetComponentInParent<BaseUnit>().name + " is dead**");
            }
            // if defending unit is dead, find new target
            else if(!DefendingUnit.activeSelf)
            {
                Debug.Log("attack from " + AttackingUnit.GetComponentInParent<BaseUnit>().name + " nullified, because "
                         + DefendingUnit.GetComponentInParent<BaseUnit>().name + " is dead**");
                
                // if there are still units to attack
                if(HeroPartyList.size > 0 && EnemyPartyList.size > 0)
                {
                    // need to select new unit to target for this attack
                    Debug.Log("Finding new target");

                    // if defending unit is hero
                    if(DefendingUnit.GetComponentInParent<BaseHero>() != null)
                        DefendingUnit = HeroPartyList.GetRandom();
                    else // if enemy
                        DefendingUnit = EnemyPartyList.GetRandom();

                    int Defense = DefendingUnit.GetComponentInParent<BaseUnit>().Endurance;
                    int Attack = AttackingUnit.GetComponentInParent<BaseUnit>().Strength;

                    // damage calculation
                    int damage = Attack - Defense;

                    // damage can not be negative and must be atleast 1
                    if(damage < 1) damage = 1;

                    DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints -= damage;

                    Debug.Log(  "Attacker " +
                                AttackingUnit.GetComponentInParent<BaseUnit>().name +
                                ", Defender " +
                                DefendingUnit.GetComponentInParent<BaseUnit>().name +
                                " fire " + fireCounter + " atk = " + damage );
                    
                    // if defending unit health is less than 1, then unit is dead
                    // kill off the unit
                    if(DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints < 1)
                    {
                        // remove dead unit from linked list
                        if(DefendingUnit.GetComponentInParent<BaseHero>() != null) // if hero
                            HeroPartyList.Remove(DefendingUnit);
                        else // if enemy
                            EnemyPartyList.Remove(DefendingUnit);

                        DefendingUnit.GetComponentInParent<BaseUnit>().CurrentHealthPoints = 0;
                        DefendingUnit.SetActive(false);

                        Debug.Log(DefendingUnit.GetComponentInParent<BaseUnit>().name + " has died**");
                    }
                }
                else
                    Debug.Log("There are no more units to attack, party has been defeated");
            }
        }
    }

    // needs to run at awake to set up the battle at start so that other scripts are able to find what they need
    void BattleSetup()
    {
        // party size for each party
        int HeroPartySize = 3; //Random.Range(1,Hero.transform.childCount + 1);
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

        // sort units byt agility to decide turn order
        GameObject[] arr = new GameObject[HeroPartyList.size + EnemyPartyList.size];

        // pupulate arr with GameObject units
        //Debug.Log("0" + HeroPartyList.GetCurr().GetComponentInParent<BaseUnit>().name);
        arr[0] = HeroPartyList.GetCurr();
        HeroPartyList.GetNext();
        int ii = 1;
        while(!HeroPartyList.isHead())
        {
            //Debug.Log(ii + HeroPartyList.GetCurr().GetComponentInParent<BaseUnit>().name);
            arr[ii++] = HeroPartyList.GetCurr();
            HeroPartyList.GetNext();
        }

        //Debug.Log(ii + EnemyPartyList.GetCurr().GetComponentInParent<BaseUnit>().name);
        arr[ii++] = EnemyPartyList.GetCurr();
        EnemyPartyList.GetNext();
        while(!EnemyPartyList.isHead())
        {
            //Debug.Log(ii + EnemyPartyList.GetCurr().GetComponentInParent<BaseUnit>().name);
            arr[ii++] = EnemyPartyList.GetCurr();
            EnemyPartyList.GetNext();
        }

        // sort arr with insertion sort in desending order
        int n = arr.Length;
        for (ii = 1; ii < n; ++ii)
        {
            GameObject key = arr[ii];
            int j = ii - 1;
            while (j >= 0 && arr[j].GetComponentInParent<BaseUnit>().Agility < key.GetComponentInParent<BaseUnit>().Agility)
            {
                arr[j + 1] = arr[j];
                j = j - 1;
            }
            arr[j + 1] = key;
        }

        // assigne fire order number to each unit
        for(ii = 0; ii < n; ii++)
        {
            arr[ii].GetComponentInParent<BaseUnit>().fire = ii + 1;
            //Debug.Log(arr[ii].GetComponentInParent<BaseUnit>().name + " fire = " + arr[ii].GetComponentInParent<BaseUnit>().fire);
        }

        // print turn order list
        Debug.Log("Printing turn order list START ***");
        for (int i = 0; i < n; ++i) 
            Debug.Log(arr[i].GetComponentInParent<BaseUnit>().name); 
        Debug.Log("Printing turn order list END ***");
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