using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Przyciski : MonoBehaviour
{
    public CoinCollector coinCollector;

    public void ZmienScene()
    {
        SceneManager.LoadScene(1);
        coinCollector.UpdateCoinsToCollect();
    }
    public void Wyjdz()
    {
        Application.Quit(); 
    }

    public void PokazywanieSciezki()
    {
        if (Pathfinding.drawPath)
        {
            Pathfinding.drawPath = false;
        }
        else
        {
            Pathfinding.drawPath = true;
        }
    }

}
