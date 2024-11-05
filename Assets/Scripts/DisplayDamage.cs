using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayDamage : MonoBehaviour
{
    [SerializeField] Canvas damageCanvas;
    [SerializeField] List<GameObject> damageImages;
    [SerializeField] float impactTime = 5f; //Reset to 0.3f after debuging//


    void Start()
    {
        foreach(var image in damageImages)
        {
            image.SetActive (false);
        }
    }
    public void ShowDamageImpact()
    {
        Debug.Log("Blood Splatter is triggered");
        StartCoroutine(ShowSplatter());
    }

    IEnumerator ShowSplatter()
    {
        int randomIndex = Random.Range(0, damageImages.Count);
        GameObject selectedImage = damageImages[randomIndex];
        selectedImage.SetActive(true);
        yield return new WaitForSeconds(impactTime);
        selectedImage.SetActive(false);
    }

}
