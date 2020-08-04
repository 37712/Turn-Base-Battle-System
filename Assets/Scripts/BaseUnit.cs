using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseUnit : MonoBehaviour
{
    public string UnitName;

    //public bool isHero;
    
    public int fire; // turn order number

    public int Level; // experience level

    // base stats for battle
    public int BaseHealthPoints;    // HP
    public int BaseMagicPoitns;     // MP
    public int BaseSkillPoints;     // SP

    public int CurrentHealthPoints; // HP
    public int CurrentMagicPoitns;  // MP
    public int CurrentSkillPoints;  // SP

    // base stats for level up and battle
    public int Strength;    // STR, same as attack
    public int Endurance;   // END, same as defense
    public int Dexterity;   // DEX, used for skill points
    public int Intelligence;// INT, used for magic
    public int Agility;     // AGI, similar to speed
}