using UnityEngine;
using System.Collections;

/// <summary>
/// Loading screen
/// </summary>
public class LoadingScreen : MonoBehaviour
{
	public Texture2D tex;
	public Texture2D tex2;
	public float fadeTime = 2f;
	bool isLoading = false;
	
	void Start()
	{
		StartCoroutine(LoadNextLevel());
		Screen.SetResolution(740, 555, false); // TODO: possible remove
	}
	
	Color c = new Color(1f,1f,1f,0f);
	void OnGUI()
	{
		GUI.color = c;
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), tex);
		if (!isLoading) return;
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		for (int i=0; i<(int)Time.time%5; i++)
		{
			sb.Append(".");
		}
		GUI.Box(new Rect(200f, Screen.height-200f, Screen.width-400f, 22f), sb.ToString());
	}
	
	public IEnumerator LoadNextLevel()
	{
		float t = 0f;
		while (t < fadeTime)
		{
			c.a = t/fadeTime;
			t += Time.deltaTime;
			yield return 0;
		}
		c = Color.white; t = fadeTime;
		yield return new WaitForSeconds(fadeTime);
		while (t > 0f)
		{
			c.a = t/fadeTime;
			t -= Time.deltaTime;
			yield return 0;
		}
		isLoading = true;
		c.a = 0f; t = 0f;
		tex = tex2;
		while (t < fadeTime)
		{
			c.a = t/fadeTime;
			t += Time.deltaTime;
			yield return 0;
		}
		c = Color.white; t = fadeTime;
		yield return 0;
		AsyncOperation async = ApplicationEx.Instance.LoadLevelAdditiveAsync(Application.loadedLevel+1);
		yield return async;
		while (t > 0f)
		{
			c.a = t/fadeTime;
			camera.backgroundColor = new Color(0f, 0f, 0f, t/fadeTime);
			t -= Time.deltaTime;
			yield return 0;
		}
		Destroy(gameObject);
	}
}