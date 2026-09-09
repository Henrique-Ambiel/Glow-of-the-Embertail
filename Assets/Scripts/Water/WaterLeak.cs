using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private float timeoDrop;

    float timer = 0.0f;
    
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeoDrop)
        {
            Instantiate(dropPrefab, transform.position, transform.rotation);
            timer = 0.0f;
        }
    }
}
