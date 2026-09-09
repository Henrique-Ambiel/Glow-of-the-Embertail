using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    void Start()
    {
        GetComponent<Rigidbody>().WakeUp();
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerEnergy playerEnergy = collision.gameObject.GetComponent<PlayerEnergy>();

        if (playerEnergy != null)
        {
            playerEnergy.Consume(playerEnergy.Maximum);
        }

        Destroy(this.gameObject);
    }
}
