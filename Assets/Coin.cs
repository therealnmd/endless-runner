using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            GameManager.instance.coins++;
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.coins++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
