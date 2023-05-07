using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] private int damageAmount;

    public int DamageAmount
    {
        get => damageAmount;
        set => damageAmount = value;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Lives -= damageAmount;
                player.MoveSpeed = player.BaseSpeed;
                player.CoinMultiplierMod = 1;
                PlayerController.MultiplierChanged(player.CoinMultiplierMod);
                PlayerController.LivesChanged(player.Lives);

            }
            Destroy(gameObject);
        }
    }
}
