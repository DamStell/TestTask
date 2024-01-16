using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    public GameManager gameManager;
    public PlayerController playerController;

    public Button leader1Button;
    public Button leader2Button;
    public Button leader3Button;


    void Start()
    {
        leader1Button.onClick.AddListener(() => ChangeLeader(0));
        leader2Button.onClick.AddListener(() => ChangeLeader(1));
        leader3Button.onClick.AddListener(() => ChangeLeader(2));

    }

    void ChangeLeader(int leaderIndex)
    {
        if (gameManager != null && playerController != null && leaderIndex >= 0 && leaderIndex < gameManager.characters.Count)
        {
            Transform newLeader = gameManager.characters[leaderIndex].transform;
            playerController.ChangeLeader(newLeader);
           // gameManager.characters[leaderIndex].ChangeLeader(newLeader);
            playerController.stats = gameManager.characters[leaderIndex].stats;

            foreach (Character character in gameManager.characters)
            {
                character.ChangeLeader(newLeader);
            }
        }
    }
}
