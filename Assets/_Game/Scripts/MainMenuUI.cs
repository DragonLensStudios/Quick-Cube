using System;
using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string playerUsername;
    [SerializeField] private int playerCoins;
    [SerializeField] private string leaderboardKey = "highscores";
    [SerializeField] private int displayCount = int.MaxValue;
    [SerializeField] private string memberID;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text validationText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private GameObject highscorePanelPrefab;
    [SerializeField] private GameObject highscoresScrollContent;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private bool showTutorial = true;
    private void Awake()
    {
        playerCoins = PlayerPrefs.GetInt("playerCoins");

        GetName();

        coinsText.text = $"Coins: {playerCoins}";
    }

    public void StartGameOrShowTutorial()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            validationText.gameObject.SetActive(true);
            validationText.text = "Please enter a username";
        }
        else
        {
            if (showTutorial)
            {
                tutorialPanel.SetActive(true);
            }
            else
            {
                validationText.gameObject.SetActive(false);
                SceneManager.LoadScene("MainEndlessLevel");
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainEndlessLevel");
    }

    public bool GetName()
    {
        bool foundUser = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                LootLockerSDKManager.GetPlayerName((nameResponse =>
                {
                    if (nameResponse.success)
                    {
                        if (!string.IsNullOrWhiteSpace(nameResponse.name))
                        {
                            playerUsername = nameResponse.name;
                            var splitName = playerUsername.Split('#');
                            nameInput.text = splitName[0];
                            foundUser = true;
                        }
                    }
                    else
                    {
                        Debug.Log($"Player name not found: {response.Error}");
                    }
                }));
            }
            else
            {
                Debug.LogError($"Could start guest session: {response.Error}");
            }
        });
        return foundUser;
    }

    public void SetName(string name)
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                var splitName = name.Split('#');
                name = $"{splitName[0]}#{response.player_id}";
                LootLockerSDKManager.SetPlayerName(name, (response) =>
                {
                    if (response.success)
                    {
                        Debug.Log($"Player name set to {response.name}");
                    }
                    else
                    {
                        Debug.LogError($"Could not set player name to {name}: {response.Error}");
                    }
                });
            }
            else
            {
                Debug.LogError($"Could start guest session: {response.Error}");
            }
        });
    }

    public void StartSessionAndLoadScores()
    {
        /* Start guest session without an identifier.
         * LootLocker will create an identifier for the user and store it in PlayerPrefs.
         * If you want to create a new player when testing, you can use PlayerPrefs.DeleteKey("LootLockerGuestPlayerID");
         */
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                memberID = response.player_id.ToString();
                ClearHighscores();
                SpawnHighscores();
            }
            else
            {
                Debug.LogError($"Error {response.Error}");
            }
        });
    }

    public void ClearHighscores()
    {
        for (int i = highscoresScrollContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(highscoresScrollContent.transform.GetChild(i).gameObject);
        }
    }

    public void SpawnHighscores()
    {
        LootLockerSDKManager.GetMemberRank(leaderboardKey, memberID, (memberResponse) =>
        {
            if (memberResponse.success)
            {
                if (memberResponse.rank > 0)
                {
                    int playerRank = memberResponse.rank;
                    /*
                     * Set "after" to 5 below and 4 above the rank for the current player.
                     * "after" means where to start fetch the leaderboard entries.
                     */
                    int after = playerRank < 6 ? 0 : playerRank - 5;

                    LootLockerSDKManager.GetScoreList(leaderboardKey, displayCount, after, (scoreResponse) =>
                    {
                        if (scoreResponse.success)
                        {
                            for (int i = 0; i < scoreResponse.items.Length; i++)
                            {
                                string leaderboardText = "";
                                LootLockerLeaderboardMember currentEntry = scoreResponse.items[i];
                                var hs = Instantiate(highscorePanelPrefab, highscoresScrollContent.transform);
                                var scoreText = hs.GetComponentInChildren<TMP_Text>();
                                /*
                                 * Highlight the player with rich text
                                 */
                                if (currentEntry.rank == playerRank)
                                {
                                    leaderboardText += "<color=yellow>";
                                }

                                leaderboardText +=
                                    $"{currentEntry.rank}: {currentEntry.player.name}\n{currentEntry.metadata}";
                                /*
                                * End highlighting the player
                                */
                                if (currentEntry.rank == playerRank)
                                {
                                    leaderboardText += "</color>";
                                }

                                scoreText.text = leaderboardText;
                            }
                        }
                        else
                        {
                            Debug.LogError($"Could not update centered scores: {scoreResponse.Error}");
                        }
                    });
                }
                else
                {
                    LootLockerSDKManager.GetScoreList(leaderboardKey, displayCount, 0, (scoreResponse) =>
                    {
                        if (scoreResponse.success)
                        {
                            for (int i = 0; i < scoreResponse.items.Length; i++)
                            {
                                string leaderboardText = "";
                                LootLockerLeaderboardMember currentEntry = scoreResponse.items[i];
                                var hs = Instantiate(highscorePanelPrefab, highscoresScrollContent.transform);
                                var scoreText = hs.GetComponentInChildren<TMP_Text>();
                                leaderboardText +=
                                    $"{currentEntry.rank}: {currentEntry.player.name}\n{currentEntry.metadata}";
                                scoreText.text = leaderboardText;
                            }
                        }
                        else
                        {
                            Debug.LogError($"Could not update centered scores: {scoreResponse.Error}");
                        }
                    });
                }
            }
            else
            {
                Debug.LogError($"Could not get member rank: {memberResponse.Error}");
            }
        });
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}

