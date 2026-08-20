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
            FadeOutObjects(primaryFade);
            FadeOutObjects(secondaryFade);
            FadeOutObjects(tertiaryFade);
            StartCoroutine(FadePrimaryObjects());
        }
        else if(!hasFadeCompleted && tutorialStateManager.GetIsFirstTutorial()) //do a different fade sequence if it's the first tutorial
        {
            FadeOutObjects(firstTutorialFade);
            StartCoroutine(FadeFirstTutorialObjects());
        }
    }

    private void FadeOutObjects(GameObject[] gameObjects)
    {
        for(int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].GetComponent<CanvasRenderer>()) //there is no need to change the alpha if the gameobject doesn't have a canvas renderer component
            {
                gameObjects[i].GetComponent<CanvasRenderer>().SetAlpha(0f);
            }

            if (gameObjects[i].transform.childCount > 0)
            {
                FadeOutObjects(PopulateChildArray(gameObjects[i])); //recursive calls until every child has been run through the method
            }
            else
            {
                continue;
            }
        }
    }

    private void CrossFadeAlphaObjects(GameObject[] gameObjects)
    {
        for(int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].GetComponent<CanvasRenderer>()) //cross fade alpha method comes from graphic class but requires canvas renderer component present
            {
                gameObjects[i].GetComponent<Graphic>().CrossFadeAlpha(1f, fadeInDuration, false);
            }

            if (gameObjects[i].transform.childCount > 0)
            {
                CrossFadeAlphaObjects(PopulateChildArray(gameObjects[i])); //recursive calls until every child has been run through the method
            }
            else
            {
                continue;
            }
        }
    }

    private GameObject[] PopulateChildArray(GameObject parent)
    {
        GameObject[] tempArray = new GameObject[parent.transform.childCount];

        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = parent.transform.GetChild(i).GameObject();
        }

        return tempArray;
    }

    private IEnumerator FadePrimaryObjects() //this functionally calls every applicable gameobject and their children in the array like the above method with near identical code, just tweens the alpha back to full over time instead of setting it to zero
    {
        //a different coroutine is used for each array to create a delay between each set of gameobjects fading in

        yield return new WaitForSeconds(4f); //wait for camera to pan around

        //give the player control back after panning is completed
        playerMovement.EnableActions(0);
        playerMovement.EnableActions(1);
        playerMovement.EnableActions(2);

        CrossFadeAlphaObjects(primaryFade);

        yield return new WaitForSeconds(1.8f);

        pauseButton.interactable = true;
        playerMovement.AssignPauseButton();

        StartCoroutine(FadeSecondaryObjects());
    }

    private IEnumerator FadeSecondaryObjects()
    {
        CrossFadeAlphaObjects(secondaryFade);

        yield return new WaitForSeconds(1.8f);

        StartCoroutine(FadeTertiaryObjects());
    }

    private IEnumerator FadeTertiaryObjects()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        if (!tutorialButtons.GetHasFadedOut()) //don't change the control scheme alphas if the player has already made them fade through making input
        {
            CrossFadeAlphaObjects(tertiaryFade);
        }
        tutorialButtons.UpdateGlyphs();
#endif
        hasFadeCompleted = true;
        yield return null;
    }

    private IEnumerator FadeFirstTutorialObjects()
    {
        
        yield return new WaitForSeconds(4f); //wait for camera to pan around

        CrossFadeAlphaObjects(firstTutorialFade);

        yield return null;
    }

    public bool GetHasFadeCompleted()
    {
        return hasFadeCompleted;
    }
}
