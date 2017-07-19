using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour
{
	// Changes to the specified scene
	public void ChangeToScene (int sceneToChangeTo)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneToChangeTo);
	}
}
