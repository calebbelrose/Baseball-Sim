using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomFace : MonoBehaviour
{
	public Image freckles, ear, face, eyeShape, eyeColour, hair, mouth, nose;
	public List<Sprite> frecklesSprites, earSprites, faceSprites, eyeShapeSprites, eyeColourSprites, hairSprites, mouthSprites, noseSprites;

	void Start ()
	{
		NewFace ();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown (0))
			NewFace ();
	}

	void NewFace ()
	{
		int skin = (int)Random.Range (0, 4);
		int rand;

		freckles.sprite = frecklesSprites [(int)Random.Range (0, frecklesSprites.Count)];
		rand = (int)Random.Range (0, 3);
		ear.sprite = earSprites [rand * 4 + skin];
		rand = (int)Random.Range (0, 3);
		face.sprite = faceSprites [rand * 4 + skin];
		eyeShape.sprite = eyeShapeSprites [(int)Random.Range (0, eyeShapeSprites.Count)];
		eyeColour.sprite = eyeColourSprites [(int)Random.Range (0, eyeColourSprites.Count)];
		hair.sprite = hairSprites [(int)Random.Range (0, hairSprites.Count)];
		mouth.sprite = mouthSprites [(int)Random.Range (0, mouthSprites.Count)];
		nose.sprite = noseSprites [(int)Random.Range (0, noseSprites.Count)];
	}
}
