using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectOnTime : MonoBehaviour
{
    public float m_Destroy_Time = 3f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyObjectOnTimeFn());
    }

    // Update is called once per frame
    IEnumerator DestroyObjectOnTimeFn()
    {
        yield return new WaitForSeconds(m_Destroy_Time);
        GameObject.Destroy(gameObject);
    }
}
