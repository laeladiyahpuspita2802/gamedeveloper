using UnityEngine;

public class PlayerCollectPoin : MonoBehaviour
{
    public CollectPoin collectPoin;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Poin"))
        {
            collectPoin.TambahPoin(1);
            Destroy(other.gameObject);
        }
    }
}
