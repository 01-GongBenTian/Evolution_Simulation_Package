using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature Ability", menuName = "Scriptable/Creature/Creature Ability")]
public class Ability : ScriptableObject
{
    public string AbilityName;
    public string AbilityDescription;
    public List<AbilityEffect> Effects = new List<AbilityEffect>();
    
    public Ability Upgrade;
}
