using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseUnit
{
    // used for unit vulnerability and resistance
    // maybe it is better to use list
    public enum ElementType
    {
        Fire, Water,
        Earth, Wind,
        Light, shadow

        // OR
        /*
        Fire,   // defeats Ice, bad against water
        Ice,    // defeats wind, bad against fire
        Wind,   // defeats Earth, bad agaisnt Ice
        Earth,  // defeats thunder, bad agaisnt wind
        Thunder,// defeats water, bad against Earth
        Water,  // defeats fire, bad against thunder
        Light,  // defeats shadow
        shadow  // defeats light
        */
    }
    // this makes the ElementType work
    public ElementType Element;

    // item drop
}
