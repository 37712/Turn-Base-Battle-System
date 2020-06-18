using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPanelScript : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // this is for button only
    public void AttackButton()
    {
        // disable action panel
        GameObject.Find("Action Panel").SetActive(false);

        // move game state to select target state
        this.GetComponent<BattleManager>().state = BattleState.TargetSelect;

        // pre-select specific button
        //GameObject.Find("Enemy Pos 1")
    }
}
