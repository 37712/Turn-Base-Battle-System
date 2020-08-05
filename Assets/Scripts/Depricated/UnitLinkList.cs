using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this will be a simple double circular linklist
// this link list has special methods to work with this specific project
public class UnitLinkList
{
    private Node head; // start of the linklist, do not mess around with the head
    private Node curr; // where we are currently pointing
    public int size;

    public class Node
    {
        public GameObject data; // this is where we put the basehero or base enemy
        //public int index; // depricated need fix on remcurr
        public Node next; // get character to the right
        public Node prev; // get character to the left

        //constructor for the Node class
        public Node()
        {
            data = null;
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
            Node ptr = head; // creates pointer

            while(ptr.next != head) // while we are not back at the head
            {
                ptr = ptr.next; // move ptr to next node
            }
            
            // connecct last node to new node
            ptr.next = tmp;
            tmp.prev = ptr;

            // connect new node to head
            tmp.next = head;
            head.prev = tmp;
        }

        size++; // dont forget to increase size

        return true;
    }

    // not working, if curr node is the one that gets removed.
    // remove node with specified data
    public void Remove(GameObject data)
    {
        Debug.Log("attempting to remove " + data.GetComponentInParent<BaseUnit>().name);
        Node tmp = curr;

        for(int i = 1; i < size; i++)
        {
            Debug.Log("tmp link: " + tmp.data.GetComponentInParent<BaseUnit>().name);
            if(data == tmp.data){Debug.Log("found"); break;}
            else tmp = tmp.next;
        }
        if(data == tmp.data)
        {
            Debug.Log("removing");

            if(size == 1) // if only 1 node
            {
                tmp = head = null;
                size--; // dont forget to decrease size
            }
            else if(size > 1) // if more than 1 node
            {
                // move pointers to delete middle node
                Node ptr = tmp.prev;
                tmp = tmp.next;
                
                // make new next and prev
                ptr.next = tmp;
                tmp.prev = ptr;

                // if head node was removed from link
                if(head.prev == ptr && head.next == tmp)
                {
                    head = tmp;
                }
                
                size--; // dont forget to decrease size
            }
        }
        else Debug.Log("failed to remove, data was not found on linked list");
    }

    // remove curr node from list
    // move current to prev and remove node
    public void RemoveCurr()
    {
        if(size == 1) // if only 1 node
        {
            curr = head = null;
            size--; // dont forget to decrease size
        }
        else if(size > 1) // if more than 1 node
        {
            // move pointers to delete middle node
            Node ptr = curr.prev;
            curr = curr.next;
            
            // make new next and prev
            ptr.next = curr;
            curr.prev = ptr;

            // if head node was removed from link
            if(head.prev == ptr && head.next == curr)
            {
                head = curr;
            }
            
            size--; // dont forget to decrease size
        }
    }

    // BASIC METHODS

    // get current node in list, return GameObject
    public GameObject GetCurr()
    {
        
        return (curr == null) ? null : curr.data;
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

    // OTHER SPECIAL METHODS

    // return true if curr is pointing to head
    // usefeull to know if you have gone through one full rotation of the link
    public bool isHead()
    {
        return (curr == head) ? true : false;
    }

    // return true if curr is NOT pointing to head
    // usefeull to know if you are still traversing the link
    public bool isNotHead()
    {
        return (curr != head) ? true : false;
    }
    
    // return true if next node is head node
    // usefeull to know if you have gone though all nodes in link and next link is head
    public bool isHeadNext()
    {
        return (curr.next == head) ? true : false;
    }

    // go back to start of circle
    public void ReturnToHead()
    {
        curr = head;
    }

    // move curr to random unit and get GameObject
    public GameObject GetRandom()
    {
        Node tmp = head; // pointer
        for(int i = 0; i < Random.Range(0,size); i++) tmp = tmp.next;
        return tmp.data;
    }

}
