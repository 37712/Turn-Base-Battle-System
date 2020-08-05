using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum BattleState {TurnStart, NextUnit, ActionSelect, TargetSelect, EnemyTurn, BattlePhase, TurnEnd, WON, LOST, BattleEnd}; // states taken out: NextUnit

public class BattleManager : MonoBehaviour
{
    public BattleState state;

    public GameObject Hero; // parent of hero tree
    public GameObject Enemy; // parent of enemy tree
    public GameObject currHero; // current hero selected
    public GameObject currEnemy; // current enemy selected

    // panel control
    public GameObject ActionPanel;
    public GameObject WONPanel;
    public GameObject LOSTPanel;
    
    // camera selection
    public GameObject MainCamera;
    public bool cameraFollowing = false;

    public int fireCounter = 0; // used to set off the attacks in order
    public int HeroPartySize; // not to be reduced when hero is killed
    public int EnemyPartySize; // not to be reduced when enemy is killed
    public int PartyIndex = 1;
    public int turnCounter = 1; // counte how many turns have passed
    public int TargetIndex = 1;
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
        
        // depricated, if enemy suprise attack just give enemy a free attack turn
        // or just give +5 to agility to all enemy units
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
            // this is whare each start of a turn gets set up
            case BattleState.TurnStart:

                Debug.Log("##############Turn counter = " + turnCounter + "################");
                
                state = BattleState.NextUnit;
                
                break;

            // this is where the game makes decisions
            case BattleState.NextUnit:
                
                // select first or next unit to move
                currHero = GameObject.Find("Hero Position " + PartyIndex);
                
                // if all party members have had their turn
                if(PartyIndex > HeroPartySize)
                {
                    PartyIndex = 1; // reset party index to 1
                    state = BattleState.EnemyTurn;
                }
                else if (isDead(currHero)) // if unit is dead
                {
                    PartyIndex++;
                }
                else
                {
                    state = BattleState.ActionSelect;   
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

                    // attach method called
                    StartCoroutine(Attack(currHero, currEnemy));
                    
                    // next units turn
                    PartyIndex++;
                    state = BattleState.NextUnit;
                }
                else if(Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Debug.Log("left");
                    // get next unit
                    GetNext();
                    currEnemy = GameObject.Find("Enemy Position " + TargetIndex);
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Debug.Log("right");
                    // get previous unit
                    GetPrev();
                    currEnemy = GameObject.Find("Enemy Position " + TargetIndex);
                }

                // move camera to enemy unit
                cameraFollowing = true;

                // get camera target
                if(isDead(currEnemy))
                {
                    Debug.Log("getting new target for camera");
                    GetNext();
                    currEnemy = GameObject.Find("Enemy Position " + TargetIndex);
                }
                MainCamera.GetComponent<CameraFollow>().CameraTarget = currEnemy;
                
                break;

            // run enemy attack code
            case BattleState.EnemyTurn:
            
                currEnemy = GameObject.Find("Enemy Position " + PartyIndex);
                currHero = GameObject.Find("Hero Position " + Random.Range(1, HeroPartySize+1));
                
                // if unit is not dead then it can attack
                if(!isDead(currEnemy)) StartCoroutine(Attack(currEnemy, currHero));

                // have all party members had their turn
                if(PartyIndex == EnemyPartySize)
                {
                    PartyIndex = 1; // reset party index to 1
                    state = BattleState.BattlePhase;
                    currEnemy = GameObject.Find("Enemy Position " + TargetIndex);
                }
                else
                    PartyIndex++;
                
                break;

            // This is where the battle takes place
            case BattleState.BattlePhase:
                
                if(fireCounter <= 20) // need to fix this in the future
                    fireCounter++;
                else
                {
                    fireCounter = 0;
                    state = BattleState.TurnEnd;
                }

                break;

            case BattleState.TurnEnd:

                // if heros are dead
                if(isPartyDead(Hero)) state = BattleState.LOST;
                // if enemys are dead
                else if(isPartyDead(Enemy)) state = BattleState.WON;
                // both parties are still standing
                else
                {
                    state = BattleState.TurnStart;
                    turnCounter++;
                }

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

            case BattleState.BattleEnd:

                // there is nothing to put here

                break;
        }
    }

    // moves target index to next enemy unit that is still alive
    // can only be used to point at enemy target
    public void GetNext()
    {
        Debug.Log("get next");
        if(TargetIndex == EnemyPartySize) TargetIndex = 1;
        else TargetIndex++;
        
        if(GameObject.Find("Enemy Position " + TargetIndex).GetComponent<BaseUnit>().CurrentHealthPoints == 0)
            GetNext();
    }

    // moves target index to prev enemy unit that is still alive
    // can only be used to point at enemy target
    public void GetPrev()
    {
        Debug.Log("get prev");
        if(TargetIndex == 1) TargetIndex = EnemyPartySize;
        else TargetIndex--;
        
        if(GameObject.Find("Enemy Position " + TargetIndex).GetComponent<BaseUnit>().CurrentHealthPoints == 0)
            GetPrev();
    }

    // checks to see if unit is dead
    public bool isDead(GameObject Unit)
    {
        return (Unit.GetComponent<BaseUnit>().CurrentHealthPoints == 0) ? true : false;
    }

    // checks to see if party is dead, you must give it either Hero or Enemy parent object in tree
    public bool isPartyDead(GameObject UnitParent)
    {
        for(int i = 0; i < UnitParent.transform.childCount; i++)
        {
            if(UnitParent.transform.GetChild(i).GetComponent<BaseUnit>() != null &&
               UnitParent.transform.GetChild(i).GetComponent<BaseUnit>().CurrentHealthPoints != 0) return false;
        }
        return true;
    }

    // this method calculates damage from attacking unit to defending unit
    // also checks if target is dead and kills the unit
    public IEnumerator Attack(GameObject AttackingUnit, GameObject DefendingUnit)
    {
        // The yield return line is the point at which execution will pause and be resumed the following frame.
        // makes the corutine wait until it is its turn to execute
        //yield return new WaitUntil(() => (AttackingUnit.GetComponentInParent<BaseUnit>().fire == fireCounter));
        while(AttackingUnit.GetComponentInParent<BaseUnit>().fire != fireCounter) yield return null;

        // attacking and defending unit are alive
        if(!isDead(AttackingUnit) && !isDead(DefendingUnit))
        {
            int Defense = DefendingUnit.GetComponent<BaseUnit>().Endurance;
            int Attack = AttackingUnit.GetComponent<BaseUnit>().Strength;

            // damage calculation
            int damage = Attack - Defense;

            // damage can not be negative and must be atleast 1
            if(damage < 1) damage = 1;

            DefendingUnit.GetComponent<BaseUnit>().CurrentHealthPoints -= damage;

            Debug.Log(  "Attacker " +
                        AttackingUnit.GetComponent<BaseUnit>().name +
                        ", Defender " +
                        DefendingUnit.GetComponent<BaseUnit>().name +
                        " fire " + fireCounter + " dmg = " + damage );
            
            // if defending unit health is less than 1, then unit is dead
            // kill off the unit
            if(DefendingUnit.GetComponent<BaseUnit>().CurrentHealthPoints < 1)
            {
                DefendingUnit.GetComponent<BaseUnit>().CurrentHealthPoints = 0;
                DefendingUnit.transform.GetChild(0).gameObject.SetActive(false);

                Debug.Log(DefendingUnit.GetComponent<BaseUnit>().name + " has died**");
            }
        }
        
        else // attacking or defending unit is dead
        {
            // if attacking unit is dead, do nothing because attacker is dead
            if(isDead(AttackingUnit))
            {
                Debug.Log("attack from " + AttackingUnit.GetComponent<BaseUnit>().name + " nullified, because "
                         + AttackingUnit.GetComponent<BaseUnit>().name + " is deadAAA**");
            }
            // if defending unit is dead, find new target
            else
            {
                Debug.Log("attack from " + AttackingUnit.GetComponent<BaseUnit>().name + " nullified, because "
                         + DefendingUnit.GetComponent<BaseUnit>().name + " is deadBBB**");
                
                // if there are still units to attack
                if(!isPartyDead(Hero) && !isPartyDead(Enemy))
                {
                    // need to select new unit to target for this attack
                    Debug.Log("Finding new target");

                    // if defending unit is hero
                    if(DefendingUnit.GetComponent<BaseHero>() != null)
                    {
                        // get defending hero unit
                        int i;
                        for(i = 1; isDead(GameObject.Find("Hero Position " + i)); i++) Debug.Log("i = " + i);
                        DefendingUnit = GameObject.Find("Hero Position " + i);
                    }
                    else // if enemy
                    {
                        // get defending enemy unit
                        int i;
                        for(i = 1; isDead(GameObject.Find("Enemy Position " + i)); i++) Debug.Log("i = " + i);
                        DefendingUnit = GameObject.Find("Enemy Position " + i);
                    }

                    int Defense = DefendingUnit.GetComponent<BaseUnit>().Endurance;
                    int Attack = AttackingUnit.GetComponent<BaseUnit>().Strength;

                    // damage calculation
                    int damage = Attack - Defense;

                    // damage can not be negative and must be atleast 1
                    if(damage < 1) damage = 1;

                    DefendingUnit.GetComponent<BaseUnit>().CurrentHealthPoints -= damage;

                    Debug.Log(  "Attacker " +
                                AttackingUnit.GetComponent<BaseUnit>().name +
                                ", Defender " +
                                DefendingUnit.GetComponent<BaseUnit>().name +
                                " fire " + fireCounter + " dmg = " + damage );
                    
                    // if defending unit health is less than 1, then unit is dead
                    // kill off the unit
                    if(DefendingUnit.GetComponent<BaseUnit>().CurrentHealthPoints < 1)
                    {
                        DefendingUnit.GetComponent<BaseUnit>().CurrentHealthPoints = 0;
                        DefendingUnit.transform.GetChild(0).gameObject.SetActive(false);

                        Debug.Log(DefendingUnit.GetComponent<BaseUnit>().name + " has died**");
                    }
                }
                else // there are no more units to attack
                    Debug.Log("There are no more units to attack, party has been defeated");
            }
        }
    }

    // needs to run at awake to set up the battle before start so that other scripts are able to find what they need
    void BattleSetup()
    {
        // party size for each party
        HeroPartySize = 3; //Random.Range(1,Hero.transform.childCount + 1);
        EnemyPartySize = 3; //Random.Range(1,Enemy.transform.childCount + 1);

        // initializing Hero units
        // unit.transform.childCount will give you the number of child
        for(int i = 0; i < HeroPartySize; i++)
        {
            // spawn character
            GameObject unit = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            unit.transform.SetParent(Hero.transform.GetChild(i), false); // set parent
            Renderer UnitColor = unit.GetComponent<Renderer>();
            UnitColor.material.SetColor("_Color", Color.cyan);

            // add unit script to object and add to list
            //BaseHero HeroScript = unit.gameObject.AddComponent<BaseHero>();// add to child
            BaseHero HeroScript = unit.transform.parent.gameObject.AddComponent<BaseHero>();// add to parent

            // initialize unit with random stats for testing
            UnitStatsInit(unit.transform.parent.gameObject, "Hero" + (i+ 1));
        }

        // initializing Enemy untis
        for(int i = 0; i < EnemyPartySize; i++)
        {
            // spawn enemy
            GameObject unit = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            unit.transform.SetParent(Enemy.transform.GetChild(i), false); // set parent
            Renderer UnitColor = unit.GetComponent<Renderer>();
            UnitColor.material.SetColor("_Color", Color.red);

            // add unit script to object and add to list
            //BaseEnemy EnemyScript = unit.gameObject.AddComponent<BaseEnemy>();// add to child
            BaseEnemy EnemyScript = unit.transform.parent.gameObject.AddComponent<BaseEnemy>();// add to parent

            // initialize unit with random stats for testing
            UnitStatsInit(unit.transform.parent.gameObject, "Enemy" + (i+ 1));
        }

        // sort units by agility to decide turn order
        GameObject[] arr = new GameObject[HeroPartySize + EnemyPartySize];

        // pupulate arr with GameObject units
        arr[0] = GameObject.Find("Hero Position 1");
        arr[1] = GameObject.Find("Hero Position 2");
        arr[2] = GameObject.Find("Hero Position 3");
        arr[3] = GameObject.Find("Enemy Position 1");
        arr[4] = GameObject.Find("Enemy Position 2");
        arr[5] = GameObject.Find("Enemy Position 3");

        // sort arr with insertion sort in desending order
        // unit with highest agility goes first
        int n = arr.Length;
        for (int i = 1; i < n; ++i)
        {
            GameObject key = arr[i];
            int j = i - 1;
            while (j >= 0 && arr[j].GetComponent<BaseUnit>().Agility < key.GetComponent<BaseUnit>().Agility)
            {
                arr[j + 1] = arr[j];
                j = j - 1;
            }
            arr[j + 1] = key;
        }

        // assigne fire order number based on agility to each unit
        for(int i = 0; i < n; i++)
        {
            arr[i].GetComponentInParent<BaseUnit>().fire = i + 1;
            //Debug.Log(arr[i].GetComponentInParent<BaseUnit>().name + " fire = " + arr[i].GetComponentInParent<BaseUnit>().fire);
        }

        // print turn order list
        Debug.Log("Printing turn order list START ***");
        for (int i = 0; i < n; ++i) 
            Debug.Log(arr[i].GetComponentInParent<BaseUnit>().name); 
        Debug.Log("Printing turn order list END ***");
    }

    // this method is for testing purposes only
    // initialize units stats with random variables
    void UnitStatsInit(GameObject unit, string str)
    {
        BaseUnit UnitStats = unit.gameObject.GetComponent<BaseUnit>();

        UnitStats.UnitName = str;
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