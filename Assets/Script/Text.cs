using System.Collections;
using UnityEngine;
using UnityEngine.UI; // For regular UI Text
using TMPro;
// using TMPro; // Uncomment this if you're using TextMeshPro

public class Text : MonoBehaviour
{
    public TMP_Text text1; // Reference to the first UI Text element
    public TMP_Text text2; // Reference to the second UI Text element
    public TMP_Text text3; // Reference to the third UI Text element
 

    // Alternatively, if you're using TextMeshPro, you would use TMP_Text:
    // public text1, text2, text3;

    private void Start()
    {
        // Start the sequence of text displays
        StartCoroutine(DisplayTextSequence());
    }

    private IEnumerator DisplayTextSequence()
    {
        yield return new WaitForSeconds(2f);
        // Show Text1 for 3 seconds
        text1.gameObject.SetActive(true); // Make text1 visible
        yield return new WaitForSeconds(5f); // Wait for 3 seconds
        text1.gameObject.SetActive(false); // Hide text1


        yield return new WaitForSeconds(1f);
        // Show Text2 for 3 seconds
        text2.gameObject.SetActive(true); // Make text2 visible
        yield return new WaitForSeconds(5f); // Wait for 3 seconds
        text2.gameObject.SetActive(false); // Hide text2


        yield return new WaitForSeconds(1f);
        // Show Text3 for 3 seconds
        text3.gameObject.SetActive(true); // Make text3 visible
        yield return new WaitForSeconds(5f); // Wait for 3 seconds
        text3.gameObject.SetActive(false); // Hide text3
    }
}
