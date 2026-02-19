using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//Leyton script; luke helped fix a bug with first tutorial

public class GameplayUIFading : MonoBehaviour
{
    [SerializeField] private GameObject[] primaryFade;
    [SerializeField] private GameObject[] secondaryFade;
    [SerializeField] private GameObject[] tertiaryFade;
    [SerializeField] private GameObject[] firstTutorialFade;
    [SerializeField] private Button pauseButton;

    [SerializeField] private TutorialStateManager tutorialStateManager;
    [SerializeField] private TutorialButtons tutorialButtons;
    [SerializeField] private PlayerMovement playerMovement;

    private bool hasFadeCompleted = false;
    private float fadeInDuration = 1.5f;

    private void Start()
    {
        hasFadeCompleted = false;
    }

    public void StartFadeSequence() //called through the editor
    {
        pauseButton.interactable = false;

        if (!hasFadeCompleted && !tutorialStateManager.GetIsFirstTutorial()) //stop this fading sequence from happening in the first tutorial
        {
            //disable player controls during the panning cutscene
            playerMovement.DisableActions(0);
            playerMovement.DisableActions(1);
            playerMovement.DisableActions(2);
            FadeOutObjects();
            StartCoroutine(FadePrimaryObjects());
        }
        else if(!hasFadeCompleted && tutorialStateManager.GetIsFirstTutorial()) //do a different fade sequence if it's the first tutorial
        {
            FadeOutFirstTutorialObjects();
            StartCoroutine(FadeFirstTutorialObjects());
        }
    }

    private void FadeOutObjects() //this method goes through all the gameobjects in the array and their children to set alphas to zero if a canvas renderer component is present, amount of nested loops used depends on how many children the respective gameobjects have and if those have children
    {
        for (int i = 0; i < primaryFade.Length; i++)
        {
            if (primaryFade[i].GetComponent<CanvasRenderer>()) //there is no need to change the alpha if the gameobject doesn't have a canvas renderer component
            {
                primaryFade[i].GetComponent<CanvasRenderer>().SetAlpha(0f);
            }

            if (primaryFade[i].transform.childCount > 0)
            {
                GameObject[] tempArray = new GameObject[primaryFade[i].transform.childCount];

                for (int j = 0; j < tempArray.Length; j++)
                {
                    if (primaryFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>()) //there is no need to change the alpha if the gameobject doesn't have a canvas renderer component
                    {
                        primaryFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>().SetAlpha(0f);
                    }

                    if (primaryFade[i].transform.GetChild(j).transform.childCount > 0)
                    {
                        GameObject[] tempArray2 = new GameObject[primaryFade[i].transform.GetChild(j).transform.childCount];

                        for (int k = 0; k < tempArray2.Length; k++)
                        {
                            if (primaryFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<CanvasRenderer>()) //there is no need to change the alpha if the gameobject doesn't have a canvas renderer component
                            {
                                primaryFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<CanvasRenderer>().SetAlpha(0f);
                            }
                        }
                    }
                }
            }
        }

        //below code in this method just does the same thing as above but for other gameobject arrays, less nested loops depending on presence of children of children as well

        for (int i = 0; i < secondaryFade.Length; i++)
        {
            if (secondaryFade[i].GetComponent<CanvasRenderer>())
            {
                secondaryFade[i].GetComponent<CanvasRenderer>().SetAlpha(0f);
            }

            if (secondaryFade[i].transform.childCount > 0)
            {
                GameObject[] tempArray = new GameObject[secondaryFade[i].transform.childCount];

                for (int j = 0; j < tempArray.Length; j++)
                {
                    if (secondaryFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>())
                    {
                        secondaryFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>().SetAlpha(0f);
                    }
                }
            }
        }

        for (int i = 0; i < tertiaryFade.Length; i++)
        {
            if (tertiaryFade[i].GetComponent<CanvasRenderer>())
            {
                tertiaryFade[i].GetComponent<CanvasRenderer>().SetAlpha(0f);
            }

            if (tertiaryFade[i].transform.childCount > 0)
            {
                GameObject[] tempArray = new GameObject[tertiaryFade[i].transform.childCount];

                for (int j = 0; j < tempArray.Length; j++)
                {
                    if (tertiaryFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>())
                    {
                        tertiaryFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>().SetAlpha(0f);
                    }

                    if (tertiaryFade[i].transform.GetChild(j).transform.childCount > 0)
                    {
                        GameObject[] tempArray2 = new GameObject[tertiaryFade[i].transform.GetChild(j).transform.childCount];

                        for (int k = 0; k < tempArray2.Length; k++)
                        {
                            if (tertiaryFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<CanvasRenderer>())
                            {
                                tertiaryFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<CanvasRenderer>().SetAlpha(0f);
                            }
                        }
                    }
                }
            }
        }
    }

    private void FadeOutFirstTutorialObjects()
    {
        for (int i = 0; i < firstTutorialFade.Length; i++)
        {
            if (firstTutorialFade[i].GetComponent<CanvasRenderer>()) //there is no need to change the alpha if the gameobject doesn't have a canvas renderer component
            {
                firstTutorialFade[i].GetComponent<CanvasRenderer>().SetAlpha(0f);
            }

            if (firstTutorialFade[i].transform.childCount > 0)
            {
                GameObject[] tempArray = new GameObject[firstTutorialFade[i].transform.childCount];

                for (int j = 0; j < tempArray.Length; j++)
                {
                    if (firstTutorialFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>()) //there is no need to change the alpha if the gameobject doesn't have a canvas renderer component
                    {
                        firstTutorialFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>().SetAlpha(0f);
                    }

                    if (firstTutorialFade[i].transform.GetChild(j).transform.childCount > 0)
                    {
                        GameObject[] tempArray2 = new GameObject[firstTutorialFade[i].transform.GetChild(j).transform.childCount];

                        for (int k = 0; k < tempArray2.Length; k++)
                        {
                            if (firstTutorialFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<CanvasRenderer>()) //there is no need to change the alpha if the gameobject doesn't have a canvas renderer component
                            {
                                firstTutorialFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<CanvasRenderer>().SetAlpha(0f);
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator FadePrimaryObjects() //this functionally calls every applicable gameobject and their children in the array like the above method with near identical code, just tweens the alpha back to full over time instead of setting it to zero
    {
        //a different coroutine is used for each array to create a delay between each set of gameobjects fading in

        yield return new WaitForSeconds(4f); //wait for camera to pan around

        //give the player control back after panning is completed
        playerMovement.EnableActions(0);
        playerMovement.EnableActions(1);
        playerMovement.EnableActions(2);

        for(int i = 0; i < primaryFade.Length; i++)
        {
            if (primaryFade[i].gameObject.GetComponent<CanvasRenderer>())
            {
                primaryFade[i].GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
            }

            if (primaryFade[i].transform.childCount > 0)
            {
                GameObject[] tempArray = new GameObject[primaryFade[i].transform.childCount];

                for (int j = 0; j < tempArray.Length; j++)
                {
                    if (primaryFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>())
                    {
                        primaryFade[i].transform.GetChild(j).GameObject().GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
                    }

                    if (primaryFade[i].transform.GetChild(j).transform.childCount > 0)
                    {
                        GameObject[] tempArray2 = new GameObject[primaryFade[i].transform.GetChild(j).transform.childCount];

                        for (int k = 0; k < tempArray2.Length; k++)
                        {
                            if (primaryFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<CanvasRenderer>())
                            {
                                primaryFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
                            }
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(1.8f);

        pauseButton.interactable = true;

        StartCoroutine(FadeSecondaryObjects());
    }

    private IEnumerator FadeSecondaryObjects()
    {
        for (int i = 0; i < secondaryFade.Length; i++)
        {
            if (secondaryFade[i].gameObject.GetComponent<CanvasRenderer>())
            {
                secondaryFade[i].GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
            }

            if (secondaryFade[i].transform.childCount > 0)
            {
                GameObject[] tempArray = new GameObject[secondaryFade[i].transform.childCount];

                for (int j = 0; j < tempArray.Length; j++)
                {
                    if (secondaryFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>())
                    {
                        secondaryFade[i].transform.GetChild(j).GameObject().GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
                    }
                }
            }
        }

        yield return new WaitForSeconds(1.8f);

        StartCoroutine(FadeTertiaryObjects());
    }

    private IEnumerator FadeTertiaryObjects()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        if (!tutorialButtons.GetHasFadedOut()) //don't change the control scheme alphas if the player has already made them fade through making input
        {
            for (int i = 0; i < tertiaryFade.Length; i++)
            {
                if (tertiaryFade[i].gameObject.GetComponent<CanvasRenderer>())
                {
                    tertiaryFade[i].GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
                }

                if (tertiaryFade[i].transform.childCount > 0)
                {
                    GameObject[] tempArray = new GameObject[tertiaryFade[i].transform.childCount];

                    for (int j = 0; j < tempArray.Length; j++)
                    {
                        if (tertiaryFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>())
                        {
                            tertiaryFade[i].transform.GetChild(j).GameObject().GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
                        }

                        if (tertiaryFade[i].transform.GetChild(j).transform.childCount > 0)
                        {
                            GameObject[] tempArray2 = new GameObject[tertiaryFade[i].transform.GetChild(j).transform.childCount];

                            for (int k = 0; k < tempArray2.Length; k++)
                            {
                                if (tertiaryFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<CanvasRenderer>())
                                {
                                    tertiaryFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
                                }
                            }
                        }
                    }
                }
            }
        }
        tutorialButtons.UpdateGlyphs();
#endif
        hasFadeCompleted = true;
        yield return null;
    }

    private IEnumerator FadeFirstTutorialObjects()
    {
        
        yield return new WaitForSeconds(4f); //wait for camera to pan around

        for (int i = 0; i < firstTutorialFade.Length; i++)
        {
            if (firstTutorialFade[i].gameObject.GetComponent<CanvasRenderer>())
            {
                firstTutorialFade[i].GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
            }

            if (firstTutorialFade[i].transform.childCount > 0)
            {
                GameObject[] tempArray = new GameObject[firstTutorialFade[i].transform.childCount];

                for (int j = 0; j < tempArray.Length; j++)
                {
                    if (firstTutorialFade[i].transform.GetChild(j).GameObject().GetComponent<CanvasRenderer>())
                    {
                        firstTutorialFade[i].transform.GetChild(j).GameObject().GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
                    }

                    if (firstTutorialFade[i].transform.GetChild(j).transform.childCount > 0)
                    {
                        GameObject[] tempArray2 = new GameObject[firstTutorialFade[i].transform.GetChild(j).transform.childCount];

                        for (int k = 0; k < tempArray2.Length; k++)
                        {
                            if (firstTutorialFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<CanvasRenderer>())
                            {
                                firstTutorialFade[i].transform.GetChild(j).transform.GetChild(k).GameObject().GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
                            }
                        }
                    }
                }
            }
        }
        yield return null;
    }

    public bool GetHasFadeCompleted()
    {
        return hasFadeCompleted;
    }
}
