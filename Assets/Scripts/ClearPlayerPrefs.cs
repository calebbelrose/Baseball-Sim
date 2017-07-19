public class ClearPlayerPrefs : UnityEngine.MonoBehaviour
{
	void Start ()
	{
		Clear ();
	}

	// Clears save
	public static void Clear()
	{
		UnityEngine.PlayerPrefs.DeleteAll ();

		if (System.IO.Directory.Exists ("Save"))
			System.IO.Directory.Delete ("Save", true);

		System.IO.Directory.CreateDirectory ("Save");
		System.IO.File.Create (@"Save\SimulatedGames.txt").Close ();
	}
}
