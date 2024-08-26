using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class shelf : MonoBehaviour
{

    public float xOffset = 0.55f;

    public float yOffset = 0.95f;

    public float zOffset = -.2f;

    public float itemDistance = 0.5f;

    public List<GameObject> itemArray;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void updateShelf()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        float o = 0;
        foreach (GameObject item in itemArray)
        {
            // show transform root
            // Instantiate(item, transform.position, transform.rotation, transform); 
            Instantiate(item, new Vector3(transform.position.x, transform.position.y, transform.position.z) + ((o) * transform.forward) + (zOffset * transform.right) + (xOffset * transform.forward) + (yOffset * transform.up), Quaternion.identity, transform);
            o += itemDistance;
        }
    }

    public void shelfItem(GameObject item)
    {
        itemArray.Add(item);
        updateShelf();
    }

}
