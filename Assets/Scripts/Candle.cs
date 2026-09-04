using UnityEngine;

public class Candle : MonoBehaviour
{
    [SerializeField] private float flameAmount = 25f;
    [SerializeField] private bool isOneTimeUse = true;

    private bool isConsumed;
    
    /*private void OnTriggerEnter(Collider other)
    {
        if (isConsumed) return;

        if (other.TryGetComponent<FlameManager>(out var flameManager))
        {
            flameManager.AddFlame(flameAmount); //possível mudança de método** Cada vela possui valor próprio

            if (isOneTimeUse)
            {
                isConsumed = true;
                //apagar a chama da vela visualmente ou desativar o collider
            }
        }
    }*/
}
