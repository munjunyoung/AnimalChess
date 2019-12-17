using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOffSc : MonoBehaviour
{
    public float offTime;
    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(ActiveOff(offTime));
    }

    IEnumerator ActiveOff(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}
