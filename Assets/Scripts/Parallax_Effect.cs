using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float startpos;
    public GameObject cam;
    public float parallaxMultiplier;

    void Start()
    {
        startpos = transform.position.x;

        if (cam == null)
        {
            cam = Camera.main.gameObject;
        }
    }

    void LateUpdate()
    {
        float dist = (cam.transform.position.x * parallaxMultiplier);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
    }
}