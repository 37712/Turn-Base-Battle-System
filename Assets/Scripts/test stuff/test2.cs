using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

//this script goes attached to the buttons
public class test2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Do this when the selectable UI object is selected.
	public void OnSelect (BaseEventData eventData) 
	{
		Debug.Log (this.gameObject.name + " was selected");
	}

    public void BTPressed()
    {
        print("button has been pressed");
    }
}
