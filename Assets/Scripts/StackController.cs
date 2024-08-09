using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackController : MonoBehaviour
{
    [SerializeField] private StackPartController[] stackPartControlls = null;

    public void ShatterAllPart () {
        if(transform.parent != null) {
            transform.parent = null;
            FindObjectOfType<Ball>().IncreaseBrokenStacks();
        }
            
        foreach(StackPartController i in stackPartControlls){
            i.Shatter();
        }
        StartCoroutine(RemoveParts());
    }

    IEnumerator RemoveParts () {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
