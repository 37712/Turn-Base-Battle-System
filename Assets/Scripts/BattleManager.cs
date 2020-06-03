using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState {HeroTurn, SelectTarget, EnemyTurn, WON, LOST};

public class BattleManager : MonoBehaviour
{
    public BattleState state;

    public GameObject Hero;
    public GameObject Enemy;

    // panel control
    public GameObject ActionPanel;
    //public GameObject SelectionPanel;

    //contains array of unit object and stats
    public GameObject[] HeroPartyList;
    public GameObject[] EnemyPartyList;

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

        // this panel always starts as false
        //SelectionPanel.SetActive(false);
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

                // move camera to first enemy unit

            /*
            bool x = Input.GetButtonDown("LeftArrow");
            bool y = Input.GetKeyDown(KeyCode.LeftArrow);
            bool w = Input.GetKey(KeyCode.LeftArrow);
            float z = Input.GetAxis("horizontal");
            */
                if(Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Debug.Log("left");
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Debug.Log("right");
                }
                
               
                //state++; // moves to next state
                break;

            case BattleState.EnemyTurn:
                // run enemy attack code
                break;
        }
    }

    public void AttackButton()
    {
        Debug.Log("attack button pressed");
        // disable action panel
        ActionPanel.SetActive(false);

        //SelectionPanel.SetActive(true);
        state = BattleState.SelectTarget;
    }

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