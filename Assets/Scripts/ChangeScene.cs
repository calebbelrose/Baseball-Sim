using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {

	public void ChangeToScene(int sceneToChangeTo)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToChangeTo);
    }
}
