using UnityEngine;

public class LifePowerupController : MonoBehaviour
{
    [SerializeField] private int lifeValue = 1;
    [SerializeField] private float speedMod = 3f;

    public int LifeValue
    {
        get => lifeValue;
        set => lifeValue = value;
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
                player.Lives += lifeValue;
                PlayerController.LivesChanged(player.Lives);
                player.MoveSpeed += speedMod;
                Destroy(gameObject);
            }
        }
    }
}