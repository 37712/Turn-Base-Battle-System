using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this mothod is only for testing and debug purposes of this project
public class StateUpdator : MonoBehaviour
{
    public GameObject BattleController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = BattleController.GetComponent<BattleManager>().state.ToString("g");
    }
}
