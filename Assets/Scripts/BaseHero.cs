using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero : BaseUnit
{
    public enum CharacterClass
    {
        Hero,       // reserved for main player character
        Swordsman,  // templar, knight, paladin, valkyrie
        Scoundrel,  // rogue, thief, scout
        hunter,     // archer
        Alchemist,  // mage, elementalist, spell caster
        Healer      // priest
    }
    // this makes the CharacterClass work
    public CharacterClass HeroClass;
}
