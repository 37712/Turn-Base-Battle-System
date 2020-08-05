using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

//this script goes attached to the buttons
public class test2 : MonoBehaviour
{
    public GameObject myobj;
    public int myfire = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(foo());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator foo ()
    {

        yield return new WaitUntil(() => (myobj.GetComponentInParent<test>().fire == myfire));

        Debug.Log("foo has finished");
    }
}
