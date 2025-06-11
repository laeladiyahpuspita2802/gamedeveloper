using UnityEngine;
using TMPro;

public class CollectPoin : MonoBehaviour
{
    public int totalPoin = 0;
    public TextMeshProUGUI poinText;

    public void TambahPoin(int jumlah)
    {
        totalPoin += jumlah;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (poinText != null)
            poinText.text = "Poin: " + totalPoin;
    }
}
