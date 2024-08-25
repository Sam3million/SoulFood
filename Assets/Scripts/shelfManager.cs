using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shelfManager : MonoBehaviour
{
    public shelf[] shelfArray;
    public GameObject[] itemsArray;
    public int itemsPerShelf = 4;
    
    // Start is called before the first frame update
    void Start()
    {


        foreach (shelf shelf in shelfArray)
        {
            for (int i = 0; i < itemsPerShelf; i++)
            {
                GameObject item = itemsArray[Random.Range(0, itemsArray.Length)];
                shelf.shelfItem(item);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
