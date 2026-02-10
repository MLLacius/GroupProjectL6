using UnityEngine;
//Leyton script
public class FirstTutorialTriggerController : MonoBehaviour
{
    //this script serves to provide a connection between colliders on tutorial segments and the tutorial sequence

    [SerializeField] private int identifier;

    TutorialStateManager tutorialStateManager;

    private void Awake()
    {
        tutorialStateManager = GameObject.Find("Game Master").GetComponent<TutorialStateManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (identifier)
        {
            case 0:
                if(other.CompareTag("Player"))
                {
                    StartCoroutine(tutorialStateManager.ExplainLeftRight());
                }
            break;

            case 1:
                if(other.CompareTag("Player"))
                {
                    StartCoroutine(tutorialStateManager.ExplainJump());
                }
            break;

            case 2:
                if(other.CompareTag("Player"))
                {
                    StartCoroutine(tutorialStateManager.ExplainDash());
                }
            break;

            default:
                Debug.LogError("Triggered an out of bounds tutorial identifier");
            break;
        }
    }
}
