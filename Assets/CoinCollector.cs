using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CoinCollector : MonoBehaviour
{
    public CoinSpawner spawner;
    public InputField coinsToCollectInput;

    private int collectedCoins = 0;
    private int coinsToCollect = 0;

    private void Start()
    {
        if (spawner == null)
        {
            spawner = FindObjectOfType<CoinSpawner>();
        }
        coinsToCollectInput.onEndEdit.AddListener(delegate { UpdateCoinsToCollect(); });
    }

    public void UpdateCoinsToCollect()
    {
        if (int.TryParse(coinsToCollectInput.text, out coinsToCollect) && coinsToCollect > 0)
        {
            Debug.Log("Coins to collect set to: " + coinsToCollect);
        }
        else
        {
            Debug.LogError("Invalid number of coins input. Please enter a positive integer.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name + " with tag: " + other.gameObject.tag);

        if (other.gameObject.CompareTag("Coin"))
        {
            CollectCoin(other.gameObject);
        }
        else if (other.gameObject.CompareTag("FinalCoin"))
        {
            CollectFinalCoin(other.gameObject);
        }
    }

    private void CollectCoin(GameObject coin)
    {
        Destroy(coin);
        collectedCoins++;
        Debug.Log("Collected coin: " + collectedCoins);

        if (collectedCoins < coinsToCollect)
        {
            spawner.SpawnNextCoin();
        }
        else if (collectedCoins == coinsToCollect | collectedCoins == coinsToCollect)
        {
            spawner.SpawnFinalCoin();
        }
    }

    private void CollectFinalCoin(GameObject coin)
    {
        Destroy(coin);
        Debug.Log("Collected final coin");
        FinalCoinCollected();
    }

    public void FinalCoinCollected()
    {
        Debug.Log("Loading Menu scene");
        SceneManager.LoadScene("Menu");
    }
}
