using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    int index = 1;
    int counter = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            counter++;
            print("key up registered, counter = " + counter);
        }

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

    public void testbutton()
    {
        Debug.Log("coroutine started " + index);
        StartCoroutine(foo(index));
        index++;
    }
}
