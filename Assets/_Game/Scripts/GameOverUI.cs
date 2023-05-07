using System;
using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject highscorePanelPrefab;
    [SerializeField] private GameObject highscoresScrollContent;
    [SerializeField] private TMP_Text highScoresCurrentText;
    [SerializeField] private string leaderboardKey = "highscores";
    [SerializeField] private int displayCount = int.MaxValue;
    [SerializeField] private string memberID;
    [SerializeField] private PlayerController player;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindObjectOfType<PlayerController>(true);;
        }
    }

    private void OnEnable()
    {
        StartSessionAndUploadScore();
    }

    public void ClearHighscores()
    {
        for (int i = highscoresScrollContent.transform.childCount - 1; i >= 0 ; i--)
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
                if(memberResponse.rank == 0)
                {
                    return;
                }
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
                            if(currentEntry.rank == playerRank)
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
                Debug.LogError($"Could not get member rank: {memberResponse.Error}");
            }
        });
    }
    
    public void StartSessionAndUploadScore()
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
                UploadScore();
                ClearHighscores();
                SpawnHighscores();
            }
            else
            {
                Debug.LogError($"Error {response.Error}");
            }
        });
    }
    
    public void UploadScore()
    {
        if (player != null)
        {
            string distance = "";
            if (player.Score >= 1000)
            {
                var km = player.Score / 1000;
                var m = player.Score % 1000;
                distance = $"{km} Kilometers {m} Meters";
            }
            else
            {
                distance = $"{player.Score} Meters";
            }
            
            LootLockerSDKManager.GetMemberRank(leaderboardKey, memberID, response =>
            {
                if (response.success)
                {
                    if (player != null)
                    {
                        if (response.score > player.Score)
                        {
                            highScoresCurrentText.text = $"No new High Score\nScore: {player.Score}";
                            highScoresCurrentText.color = Color.red;
                        }
                        else
                        {
                            highScoresCurrentText.text = $"New High Score!\nScore: {player.Score}";
                            highScoresCurrentText.color = Color.green;
                        }
                    }
                    else
                    {
                        Debug.LogError("Player is Null when trying to compare scores.");
                    }
                }
                else
                {
                    Debug.LogError("Unable to get Member rank");
                }
            });
            
            LootLockerSDKManager.SubmitScore("", player.Score, leaderboardKey, distance, (response) =>
            {
                if (response.success)
                {
                    Debug.Log($"Submitted Score for ID: {response.member_id}");
                }
                else
                {
                    Debug.LogError($"Error {response.Error}");
                }
            });
        }
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }
}
