using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour {

	public Image fillImage; // a reference for progress bar/health bar
	public Image thumbnailImage;

	public Sprite[] fillSprites; // stores red, yellow, and green bars

	void Start()
    {
		SetProgress(1.0f);
	}

	private Sprite SpriteForProgress(float progress)
    {
		if (progress >= 0.5f)
			return fillSprites[0];
		if (progress >= 0.25f)
			return fillSprites[1];
		return fillSprites[2];
	}

	public void SetThumbnail(Sprite image, Color color)
    {
		thumbnailImage.sprite = image;
		thumbnailImage.color = color;
	}

	public void SetProgress(float progress)
    {
		fillImage.fillAmount = progress;
		fillImage.sprite = SpriteForProgress(progress);
	}

	public void EnableLifeBar(bool enabled)
    {
		foreach(Transform tr in transform)
        {
			tr.gameObject.SetActive(enabled);
		}
	}
}
