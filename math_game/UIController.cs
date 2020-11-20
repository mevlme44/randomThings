using System.Collections;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject helpPanel;
    public Animator OpenPanel;
    public GameObject PausePanel;

    public void EnterHelp()
    {
        helpPanel.SetActive(true);
        StartCoroutine(OpenAnimation());
    }
    public void OutHelp()
    {
        helpPanel.SetActive(false);
    }
    public IEnumerator OpenAnimation()
    {
        OpenPanel.enabled = true;
        yield return new WaitForSeconds(0.4f);
        OpenPanel.enabled = false;
    }
    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape) && !Defines.pause)
        {
            PausePanel.SetActive(true);
            Defines.pause = true;
        }
        else if(Input.GetKeyUp(KeyCode.Escape))
        {
            PausePanel.SetActive(false);
            Defines.pause = false;
        }
    }
}
