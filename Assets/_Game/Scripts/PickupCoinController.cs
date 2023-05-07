using UnityEngine;

public class PickupCoinController : MonoBehaviour
{
    [SerializeField] private int coinValue;
    [SerializeField] private float speedMod = 1f;

    public int CoinValue
    {
        get => coinValue;
        set => coinValue = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Coins += (int)(coinValue * player.CoinMultiplierMod);
                PlayerController.CoinsChanged(player.Coins);
                player.MoveSpeed += speedMod;
                Destroy(gameObject);
            }
        }
    }
}