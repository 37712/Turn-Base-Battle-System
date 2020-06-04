using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// depricate, decided not to use, might be usefull in the future
// this will be a simple double circular linklist
// this link list has special methods to work with this specific project
public class UnitLinkList
{
    private Node head; // start of the linklist, we do need it do not erase
    private Node curr; // where we are currently pointing
    public int size;

    private class Node
    {
        public GameObject data; // this is where we put the basehero or base enemy
        //public int index; // depricated need fix on remcurr
        public Node next; // get character to the right
        public Node prev; // get character to the left

        //constructor for the Node class
        public Node()
        {
            data = null;
            //index = 0;
            next = null;
            prev = null;
        }
    }

    //Special Note to Remember:
    //DO NOT FUCK AROUND WITH THE HEAD USE A POINTER
    //DONT FORGET TO INCREASE OR DECREASE SIZE

    // add node to the end of the list
    public bool Add(GameObject data)
    {
        Node tmp = new Node();
        tmp.data = data;
        //tmp.index = ++size;

        if(head == null) // size = 0
        {
            head = curr = tmp;
            head.next = head;
            head.prev = head;
        }
        else // size at least 1
        {
            Node ptr = head.next;
            while(ptr.next != head) // while we are not back at the head
            {
                ptr = ptr.next; // move ptr to next node
            }
            ptr.next = tmp;
            tmp.prev = ptr;
            tmp.next = head;
        }

        size++; // dont forget to increase size

        return true;
    }

    // NOT TESTED, it is finished and expected to work
    // remove curr node from list
    // move current to prev and remove node
    public void Remove()
    {
        if(size == 1) // if size 1
        {
            curr = head = null;
            size--; // dont forget to decrease size
        }
        else if(size > 1) // if greater than size 1
        {
            Node ptr = curr.prev; // goes to the node prev to curr
            curr = curr.next;
            ptr.next = curr;
            curr.prev = ptr;

            if(head.prev == ptr && head.next == curr) // if head node was removed from link
            {
                head = curr;
            }
            
            size--; // dont forget to decrease size
        }
    }

    // reset curr pointer back to head
    public void CurrToHead()
    {
        curr = head;
    }

    // get current node in list, return GameObject
    public GameObject GetCurr()
    {
        return curr.data;
    }
    // move curr to next node in list and get GameObject
    public GameObject GetNext()
    {
        curr = curr.next;
        return curr.data;
    }
    // move curr to previous node in list and get GameObject
    public GameObject GetPrev()
    {
        curr = curr.prev;
        return curr.data;
    }
}
