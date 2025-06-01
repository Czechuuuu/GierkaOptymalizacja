using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;
    public float seekerSpeed = 4f;
    private PathfindingGrid grid;
    private List<Node> path;
    private int targetIndex;
    private LineRenderer lineRenderer;
    private Rigidbody seekerRigidbody;
    static public bool drawPath = false; // Nowy warunek kontrolujący rysowanie ścieżki

    void Awake()
    {
        grid = GetComponent<PathfindingGrid>();
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            gameObject.AddComponent<LineRenderer>();
            lineRenderer = GetComponent<LineRenderer>();
        }
        seekerRigidbody = seeker.GetComponent<Rigidbody>();
        if (seekerRigidbody == null)
        {
            seekerRigidbody = seeker.gameObject.AddComponent<Rigidbody>();
        }
        seekerRigidbody.isKinematic = true;
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
        if (path != null)
        {
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> newPath = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            newPath.Add(currentNode);
            currentNode = currentNode.parent;
        }
        newPath.Reverse();

        path = newPath;
    }

    IEnumerator FollowPath()
    {
        if (path == null || path.Count == 0)
            yield break;

        targetIndex = 0;
        Vector3 currentWaypoint = path[targetIndex].worldPosition;

        if (drawPath)
        {
            lineRenderer.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                lineRenderer.SetPosition(i, path[i].worldPosition);
            }
        }
        else
        {
            lineRenderer.positionCount = 0; // Wyłączenie rysowania ścieżki
        }

        while (true)
        {
            Vector3 seekerPos = new Vector3(seeker.position.x, 0, seeker.position.z);
            Vector3 waypointPos = new Vector3(currentWaypoint.x, 0, currentWaypoint.z);
            if (Vector3.Distance(seekerPos, waypointPos) < 0.1f)
            {
                targetIndex++;
                if (targetIndex >= path.Count)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex].worldPosition;
            }

            Vector3 direction = (currentWaypoint - seeker.position).normalized;
            Vector3 newPosition = seeker.position + direction * seekerSpeed * Time.deltaTime;

            // Check if the new position is on a walkable node before moving
            Node newPositionNode = grid.NodeFromWorldPoint(newPosition);
            if (newPositionNode != null && newPositionNode.walkable)
            {
                seekerRigidbody.MovePosition(newPosition);
            }

            yield return null;
        }
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
