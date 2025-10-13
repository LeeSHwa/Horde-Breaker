using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("Map Settings")]
    public Rect mapBounds = new Rect(-25f, -15f, 50f, 30f);

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); 
        }
        else
        {
            Instance = this; 
            DontDestroyOnLoad(this.gameObject); 
        }
    }
}