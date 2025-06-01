using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public PathfindingGrid grid;
    public GameObject coinPrefab;
    public GameObject finalCoinPrefab;
    public CoinCollector collector;

    private void Start()
    {
        if (collector == null)
        {
            collector = FindObjectOfType<CoinCollector>();
        }

        if (collector != null)
        {
            collector.spawner = this;
        }

        SpawnNextCoin();
    }

    public void SpawnNextCoin()
    {
        SpawnCoin(coinPrefab, "Coin");
    }

    public void SpawnFinalCoin()
    {
        SpawnCoin(finalCoinPrefab, "FinalCoin");
    }

    private void SpawnCoin(GameObject prefab, string tag)
    {
        int x, y;
        Node node;
        do
        {
            x = Random.Range(0, grid.gridSizeX);
            y = Random.Range(0, grid.gridSizeY);
            node = grid.GetNode(x, y);
        }
        while (node == null || !node.walkable || !NoCoinAtPosition(node.worldPosition));

        GameObject coin = Instantiate(prefab, node.worldPosition, Quaternion.identity);
        coin.tag = tag; // Ustaw tag monety
        Debug.Log("Spawned coin: " + coin.name + " with tag: " + coin.tag);
    }

    private bool NoCoinAtPosition(Vector3 position)
    {
        return Physics.OverlapSphere(position, grid.nodeRadius, LayerMask.GetMask("Coin")).Length == 0;
    }
}
