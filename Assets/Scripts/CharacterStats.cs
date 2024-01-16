using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public float speed;
    public float agility; 
    public float maxStamina;
    public float staminaRegenerationRate; // Stamina regeneration per second
    public float staminaConsumptionRate; //  Stamina consumption per second
}