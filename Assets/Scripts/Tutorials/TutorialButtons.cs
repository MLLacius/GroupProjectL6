using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//Leyton script
public class TutorialButtons : MonoBehaviour
{
    [SerializeField] private GameObject WASDGlyphHolder;
    [SerializeField] private GameObject[] WASDGlyphs = new GameObject[3];
    [SerializeField] private GameObject arrowKeysGlyphHolder;
    [SerializeField] private GameObject[] arrowKeysGlyphs = new GameObject[3];

    private float[] backgroundAlphas = new float[3];
    private float[] characterAlphas = new float[3];

    [SerializeField] private float fadeDuration;
    [SerializeField] private float fadeOutDuration;
    private bool hasFadedOut;

    [SerializeField] private TutorialStateManager tutorialStateManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameplayUIFading gameplayUIFading;

#if !UNITY_ANDROID || UNITY_EDITOR
    private void OnEnable()
    {
        hasFadedOut = false;     
        if (!tutorialStateManager.GetIsFirstTutorial())
        {
            playerMovement.AssignTutorialEvents();
            UpdateGlyphs();
        }
    }

    public void UpdateGlyphs()
    {
            //change the displayed glyphs on screen based on the control scheme
            switch (PlayerPrefs.GetInt("ControlSchemeKey"))
            {
                case 0:
                    WASDGlyphHolder.SetActive(true);
                    for (int i = 0; i < WASDGlyphs.Length; i++)
                    {
                        WASDGlyphs[i].SetActive(true);
                    }

                    arrowKeysGlyphHolder.SetActive(false);
                    for (int i = 0; i < arrowKeysGlyphs.Length; i++)
                    {
                        arrowKeysGlyphs[i].SetActive(false);
                    }
                    break;

                case 1:
                    arrowKeysGlyphHolder.SetActive(true);
                    for (int i = 0; i < arrowKeysGlyphs.Length; i++)
                    {
                        arrowKeysGlyphs[i].SetActive(true);
                    }

                    WASDGlyphHolder.SetActive(false);
                    for (int i = 0; i < WASDGlyphs.Length; i++)
                    {
                        WASDGlyphs[i].SetActive(false);
                    }
                    break;

                default:
                    WASDGlyphHolder.SetActive(true);
                    for (int i = 0; i < WASDGlyphs.Length; i++)
                    {
                        WASDGlyphs[i].SetActive(true);
                    }

                    arrowKeysGlyphHolder.SetActive(false);
                    for (int i = 0; i < arrowKeysGlyphs.Length; i++)
                    {
                        arrowKeysGlyphs[i].SetActive(false);
                    }
                    break;
            }
            UpdateGlyphAlphas();
        if (gameplayUIFading.GetHasFadeCompleted() && !hasFadedOut && this.isActiveAndEnabled)
            {
                StartCoroutine(GlyphFade());
            }
    }

    public void StartFadeOut()
    {
        //this method is here to access the coroutine through the editor
        if (!hasFadedOut)
        {
            StartCoroutine(GlyphFadeOut());
        }
    }

    private void UpdateGlyphAlphas()
    {
        //assign alpha values of glyphs (this probably isn't needed but it'll make it a bit easier to change fade values in future should they need to be changed)
        for (int i = 0; i < backgroundAlphas.Length; i++)
        {
            if (WASDGlyphHolder.activeSelf)
            {
                backgroundAlphas[i] = WASDGlyphs[i].GetComponent<Image>().canvasRenderer.GetAlpha();
            }
            if (arrowKeysGlyphHolder.activeSelf)
            {
                backgroundAlphas[i] = arrowKeysGlyphs[i].GetComponent<Image>().canvasRenderer.GetAlpha();
            }
        }

        for (int i = 0; i < characterAlphas.Length; i++)
        {
            if (WASDGlyphHolder.activeSelf)
            {
                characterAlphas[i] = WASDGlyphs[i].GetComponentInChildren<TextMeshProUGUI>().canvasRenderer.GetAlpha();
            }
            if (arrowKeysGlyphHolder.activeSelf)
            {
                characterAlphas[i] = arrowKeysGlyphs[i].GetComponentInChildren<TextMeshProUGUI>().canvasRenderer.GetAlpha();
            }
        }
    }

    private IEnumerator GlyphFade()
    {
        //fade glyphs in and out by decreasing and increasing alpha to create a flash effect
        while (WASDGlyphHolder.activeSelf || arrowKeysGlyphHolder.activeSelf)
        {
            if (!hasFadedOut)
            {
                //fade out by decreasing alpha
                for (int i = 0; i < backgroundAlphas.Length && i < characterAlphas.Length && backgroundAlphas[i] == 1 && characterAlphas[i] == 1; i++)
                {
                    WASDGlyphs[i].GetComponent<Image>().CrossFadeAlpha(0.5f, fadeDuration, false);
                    WASDGlyphs[i].GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(0.8f, fadeDuration, false);
                    arrowKeysGlyphs[i].GetComponent<Image>().CrossFadeAlpha(0.5f, fadeDuration, false);
                    arrowKeysGlyphs[i].GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(0.8f, fadeDuration, false);
                }
            }
            else
            {
                StartCoroutine(GlyphFadeOut());
                break;
            }

            yield return new WaitForSeconds(0.7f); //wait for a short while before fading back in

            UpdateGlyphAlphas();

            if (!hasFadedOut)
            {
                for (int i = 0; i < backgroundAlphas.Length && i < characterAlphas.Length && backgroundAlphas[i] >= 0.5 && characterAlphas[i] >= 0.8; i++)
                {
                    //fade in by increasing alpha
                    WASDGlyphs[i].GetComponent<Image>().CrossFadeAlpha(1f, fadeDuration, false);
                    WASDGlyphs[i].GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(1f, fadeDuration, false);
                    arrowKeysGlyphs[i].GetComponent<Image>().CrossFadeAlpha(1f, fadeDuration, false);
                    arrowKeysGlyphs[i].GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(1f, fadeDuration, false);
                }
            }
            else
            {
                StartCoroutine(GlyphFadeOut());
                break;
            }

            yield return new WaitForSeconds(0.7f); //wait for a short while before fading back out

            UpdateGlyphAlphas();
        }
        yield break;
    }

    public IEnumerator GlyphFadeOut()
    {
        //fully fade out glyphs
        for (int i = 0; i < backgroundAlphas.Length && i < characterAlphas.Length; i++)
        {
            WASDGlyphs[i].GetComponent<Image>().CrossFadeAlpha(0f, fadeOutDuration, false);
            WASDGlyphs[i].GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(0f, fadeOutDuration, false);
            arrowKeysGlyphs[i].GetComponent<Image>().CrossFadeAlpha(0f, fadeOutDuration, false);
            arrowKeysGlyphs[i].GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(0f, fadeOutDuration, false);
        }

        hasFadedOut = true; //prevent fade values being accessed again by any coroutines

        yield return null;
    }

    public bool GetHasFadedOut()
    {
        return hasFadedOut;
    }
#endif
}
