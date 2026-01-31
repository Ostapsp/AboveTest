using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentManager : MonoBehaviour {
    private int step = 0;
    public Image[] baners;
    private int banerCount;
    private int banerIndex;
    void Start() {
        banerCount = baners.Length;
        StartCoroutine(CycleCoroutine());
    }

    IEnumerator CycleCoroutine() {
        Debug.Log("Начало корутины. banerCount = " + banerCount);

        while (true) {
            yield return new WaitForSeconds(2f); // Задержка на 2 секунды
            Debug.Log("Две секунды прошли nn = " + step + " banerIndex = " + banerIndex);
            baners[banerIndex].enabled = false;
            step++;
            banerIndex = step % banerCount;
            baners[banerIndex].enabled = true;
        }
    }

    //   void Update()
    //{

    //}
}
