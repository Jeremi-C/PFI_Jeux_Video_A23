using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyScript : MonoBehaviour
{
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y + (89 * Time.deltaTime), 0);
        transform.position = new Vector3(transform.position.x, Mathf.Sin(time)/3, transform.position.z);
    }
}
