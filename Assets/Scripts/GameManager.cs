using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger instance;
  public Player player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
           
        }
      
    }
}
