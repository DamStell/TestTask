using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CharacterStats defaultStats; // Default statistics to use as a reference
    public List<Character> characters; 
    public PlayerController playerController;

    void Start()
    {
  
        foreach (Character character in characters)
        {
            GenerateRandomStats(character);
        }

        if (characters.Count > 0 && playerController != null)
        {
            playerController.stats = characters[0].stats;
        }
    }

    void GenerateRandomStats(Character character)
    {
        if (character != null)
        {
            // Kopiujemy domyœlne statystyki
            CharacterStats randomStats = new CharacterStats();
            randomStats.speed = Random.Range(defaultStats.speed * 0.2f, defaultStats.speed * 1.8f); // Speed +/- 80%
            randomStats.agility = Random.Range(defaultStats.agility * 0.2f, defaultStats.agility * 1.8f); // agility +/- 80%
            randomStats.maxStamina = Random.Range(defaultStats.maxStamina * 0.2f, defaultStats.maxStamina * 1.8f); // maxStamina +/- 80%
            randomStats.staminaRegenerationRate = defaultStats.staminaRegenerationRate;
            randomStats.staminaConsumptionRate = defaultStats.staminaConsumptionRate;

            // Ustawiamy statystyki postaci
            character.stats = randomStats;

            Debug.Log($"Stats for {character.name}: Speed: {randomStats.speed}, Agility: {randomStats.agility}, Max Stamina: {randomStats.maxStamina}");
        }
        else
        {
            Debug.LogError("Character reference is null.");
        }
    }


}
