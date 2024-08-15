using Unity.VisualScripting;
using UnityEngine;

public class Resource_TileManager : MonoBehaviour
{
    [SerializeField] private GameObject ResourceToken;
    private bool ResourcePulledUp;
    private bool ResourceRemoved;
    void Update()
    {
        if (ResourceRemoved == true)
        {
            ResourceToken.SetActive(false);
        }
        else
        {
            ResourceToken.SetActive(true);
        }
    }
}
