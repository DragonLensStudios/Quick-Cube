using UnityEngine;

public class MultiplierPowerupController : MonoBehaviour
{
    [SerializeField] private double coinMulti = 1;
    [SerializeField] private float speedMod = 5f;

    public double CoinMulti
    {
        get => coinMulti;
        set => coinMulti = value;
    }
    
    public float SpeedMod
    {
        get => speedMod;
        set => speedMod = value;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CoinMultiplierMod += coinMulti;
                PlayerController.MultiplierChanged(player.CoinMultiplierMod);
                player.MoveSpeed += speedMod;
                Destroy(gameObject);
            }
        }
    }
}