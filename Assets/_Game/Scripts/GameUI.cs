using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text scoreText, coinsText, livesText, distanceText, coinMultiplierText;
    [SerializeField] private PlayerController player;
    
    public GameObject GameOverPanel
    {
        get => gameOverPanel;
        set => gameOverPanel = value;
    }
    
    public TMP_Text ScoreText
    {
        get => scoreText;
        set => scoreText = value;
    }

    public TMP_Text CoinsText
    {
        get => coinsText;
        set => coinsText = value;
    }
    
    public TMP_Text LivesText
    {
        get => livesText;
        set => livesText = value;
    }


    public TMP_Text DistanceText
    {
        get => distanceText;
        set => distanceText = value;
    }
    
    public TMP_Text CoinMultiplierText
    {
        get => coinMultiplierText;
        set => coinMultiplierText = value;
    }
    
    public PlayerController Player
    {
        get => player;
        set => player = value;
    }

    private void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        PlayerController.onScoreChanged += OnScoreChanged;
        PlayerController.onCoinsChanged += OnCoinsChanged;
        PlayerController.onLivesChanged += OnLivesChanged;
        PlayerController.onMultiplierChanged += OnMultiplierChanged;
        PlayerController.onGameOver += OnGameOver;
        if (player != null)
        {
            PlayerController.ScoreChanged(player.Score);
            PlayerController.CoinsChanged(player.Coins);
            PlayerController.LivesChanged(player.Lives);
            PlayerController.MultiplierChanged(player.CoinMultiplierMod);            
        }

    }
    
    private void OnDisable()
    {
        PlayerController.onScoreChanged -= OnScoreChanged;
        PlayerController.onCoinsChanged -= OnCoinsChanged;
        PlayerController.onLivesChanged -= OnLivesChanged;
        PlayerController.onMultiplierChanged -= OnMultiplierChanged;
        PlayerController.onGameOver -= OnGameOver;
    }
    private void OnLivesChanged(int val)
    {
        livesText.text = $"Lives: {val}";
    }

    private void OnCoinsChanged(int val)
    {
        coinsText.text = $"Coins: {val}";
    }

    private void OnScoreChanged(int val)
    {
        scoreText.text = $"Score: {val}";
        if (val >= 1000)
        {
            var km = val / 1000;
            var m = val % 1000;
            distanceText.text = $"{km} Kilometers {m} Meters";
        }
        else
        {
            distanceText.text = $"{val} Meters";
        }
    }
    
    private void OnMultiplierChanged(double val)
    {
        coinMultiplierText.text = $"Coin Multiplier: {val}";
    }
    
    private void OnGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
