using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class shelf : MonoBehaviour
{

    public float offset = 0.35f;
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
            Instantiate(item, new Vector3(transform.position.x, transform.position.y + 0.95f, transform.position.z) + ((o) * transform.forward) + (-0.2f * transform.right) + (offset * transform.forward), Quaternion.identity, transform);
            o += .5f;
        }
    }

    public void shelfItem(GameObject item)
    {
        itemArray.Add(item);
        updateShelf();
    }

}
