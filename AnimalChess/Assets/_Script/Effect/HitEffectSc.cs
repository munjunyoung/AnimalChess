using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectSc : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(ActiveOff(0.5f));
    }

    IEnumerator ActiveOff(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}
