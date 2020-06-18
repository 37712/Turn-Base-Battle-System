using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class test : MonoBehaviour
{
    public GameObject myButton;
    public GameObject cube;

    public GameObject PanelA;
    public GameObject PanelB;

    public GameObject FirstButton;
    public GameObject SelectionButton;

    int index = 1;
    int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        cube.SetActive(false);

        PanelA.SetActive(true);
        PanelB.SetActive(false);

        // clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        // start of game first button to be selected
        EventSystem.current.SetSelectedGameObject(FirstButton);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
        if(EventSystem.current.currentSelectedGameObject == myButton)
        {
            cube.SetActive(true);
        }
        else
        {
            cube.SetActive(false);
        }
        
        /*if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            counter++;
            print("key up registered, counter = " + counter);
        }*/

        //Invoke(some method, 3.0f);
        //Invokerepeating(some method, 0f, 3.0f);
    }

    bool doo(int fire)
    {
        //return (fire == counter)? true : false;
        if(fire == counter)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // coroutine
    IEnumerator foo(int fire)
    {
        // works as intended
        yield return new WaitUntil(() => (fire == counter) );

        // works as intended
        /*while(fire != counter)
        {
            yield return new WaitForSeconds(0.1f);
        }*/
        print("animation running " + fire);

        // other tests
        //yield return new WaitForSeconds(0.1f);
        //Debug.Log("start");
        //yield return null;
        //Debug.Log("end");
    }

    public void testbuttonCoroutine()
    {
        Debug.Log("coroutine started " + index);
        StartCoroutine(foo(index));
        index++;
    }

    public void testbutton()
    {
        print("A button pressed");

        PanelA.SetActive(false);
        PanelB.SetActive(true);

        // clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        // set button to be selected
        EventSystem.current.SetSelectedGameObject(SelectionButton);
    }

    //IsHighlighted()
}
