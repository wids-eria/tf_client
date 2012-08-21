using UnityEngine;
using System.Collections;

/// <summary>
/// Minimap.
/// </summary>
public class Minimap :GUIObject
{
	/// <summary>
	/// Enable other script to use static variable of this script
	/// </summary>
	/// <value>
	/// The use.
	/// </value>
	public static Minimap use {get; private set;}
	
	/// <summary>
	/// The minimap background rect.
	/// </summary>
	private Rect m_minimapBackgroundRect = new Rect();

	/// <summary>
	/// The texture.
	/// </summary>
	public Texture2D texture;
	
	/// <summary>
	/// Gets the height.
	/// </summary>
	/// <value>
	/// The height.
	/// </value>
	public int height {
		get {
			return (texture==null)?maxTextureWidth:Mathf.Max(texture.height, (int)(maxTextureWidth*((float)TerrainManager.worldRegion.height/(float)TerrainManager.worldRegion.width)));
		}
	}
	
	/// <summary>
	/// Gets the width.
	/// </summary>
	/// <value>
	/// The width.
	/// </value>
	public int width {
		get {
			return (texture==null)?maxTextureWidth:Mathf.Max(texture.width, maxTextureWidth);
		}
	}
	
	/// <summary>
	/// Gets or sets the scale factor.
	/// </summary>
	/// <value>
	/// The scale factor.
	/// </value>
	public float scaleFactor { get; private set; }
		
	/// <summary>
	/// The maximum width of the texture.
	/// </summary>
	public int maxTextureWidth = 150;
	
	/// <summary>
	/// The minimap frustum material.
	/// </summary>
	public Material frustumOverlayMaterial{get{return GameGUIManager.use.iconTexture.frustumOverlay;}}
	/// <summary>
	/// The color of the minimap frustum.
	/// </summary>
	public Color frustumOverlayColor = Color.magenta;
	
	/// <summary>
	/// Specifies whether or not the minimap control is active.
	/// </summary>
	public bool isActive { get { return InputManager.use.currentInputMode != PlayerAction.InputMode.PaintResourceTiles; } }
	
	/// <summary>
	/// Gets the URL for the minimap.
	/// </summary>
	/// <value>
	/// The URL for the minimap.
	/// </value>
	public static string url {
		get {
			return string.Format(
				"{0}/worlds/{1}/images/world.png",
				WebRequests.urlServer,
				UserData.worldNumber
			);
		} 
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Minimap"/> class.
	/// </summary>
	public Minimap(){
		use= this;
		m_minimapBackgroundRect.width = width+Styles.roundDarkBox.padding.left+Styles.roundDarkBox.padding.right;
		m_minimapBackgroundRect.height = height+Styles.roundDarkBox.padding.top+Styles.roundDarkBox.padding.bottom;
		m_minimapBackgroundRect.y = Screen.height -85f -m_minimapBackgroundRect.height-Styles.roundDarkBox.margin.bottom-Styles.roundDarkBox.margin.top;
		m_minimapBackgroundRect.x = Styles.roundDarkBox.margin.left;
	}
	
	/// <summary>
	/// Download this instance.
	/// </summary>
	public IEnumerator Download()
	{
		// wait for world to load
		while (!TerrainManager.isWorldLoaded) {
			yield return 0;
		}
		
		// compute scale factor
		scaleFactor = maxTextureWidth / (float)TerrainManager.worldRegion.width;
		
		// download map graphic
		WWW www = new WWW(WWWX.AppendParametersToUrl(url, WebRequests.authenticatedParameters));
		yield return www;
		if (!string.IsNullOrEmpty(www.error)) {
			GameGUIManager.use.SetErrorMessage("Error downloading minimap.");
			yield break;
		}
		if (www.texture != null) {
//			// create a new texture with the target size
//			texture = new Texture2D(
//				maxTextureWidth,
//				(int)(maxTextureWidth*((float)TerrainManager.worldRegion.height/(float)TerrainManager.worldRegion.width))
//			);
//			// copy pixels
//			texture.render
			texture = www.texture;
		}
		else {
			GameGUIManager.use.SetErrorMessage("Error downloading minimap.");
		}
	}
	
	/// <summary>
	/// Cache for the texture rect.
	/// </summary>
	private Rect m_texRect;
	
	/// <summary>
	/// Draw the specified parent.
	/// </summary>
	/// <param name='parent'>
	/// Parent rect.
	/// </param>
	public override void Draw()
	{	
		GUILayout.BeginArea(m_minimapBackgroundRect, Styles.roundDarkBox); {
			// fade out if disabled
			Color col = GUI.color;
			if (!isActive) {
				GUI.color = new Color(col.r, col.g, col.b, GameGUIManager.use.inactiveGUIFade);
			}
			
			// determine jump-to position based on mouse position in texture
			Vector3 jumpTo = new Vector3(
					Event.current.mousePosition.x-m_texRect.x,
					0f,
					m_texRect.height-Event.current.mousePosition.y+m_texRect.y
					)/scaleFactor;
			
			// create button
			if (GUILayout.Button(
					new GUIContent("", string.Format("Jump to world location ({0}, {1}).", (int)jumpTo.x, (int)jumpTo.z)),
					Styles.empty,
					GUILayout.Width(width),
					GUILayout.Height(height)
				) &&
				isActive
			) {
				CameraRig.use.JumpTo(jumpTo);
			}
			
			// cache texture rect
			if (Event.current.type == EventType.Repaint) {
				m_texRect = GUILayoutUtility.GetLastRect();
			}
			
			// draw the texture itself
			if (texture != null) {
				GUI.DrawTexture(m_texRect, texture);
				GUI.DrawTexture(m_texRect, DataMapController.GetProjector<ProjectorMapList>("DataMaps").activeMap.texture);
			}
			
			// draw frustum over the top of texture
			GL.PushMatrix();
			frustumOverlayMaterial.SetPass(0);
			GL.LoadPixelMatrix();
			GL.Color(frustumOverlayColor);
			GL.Begin(GL.LINES);
			Vector3 c = new Vector3( // upper-left corner of texture
				m_minimapBackgroundRect.x+m_texRect.x,
				Screen.height-m_minimapBackgroundRect.y-m_texRect.y-m_texRect.height,
				0f
			);
			GL.Vertex(CamCoordToGLVert(CameraRig.use.coordDnLeft)*scaleFactor+c);
			GL.Vertex(CamCoordToGLVert(CameraRig.use.coordDnRight)*scaleFactor+c); // bottom
			GL.Vertex(CamCoordToGLVert(CameraRig.use.coordDnRight)*scaleFactor+c);
			GL.Vertex(CamCoordToGLVert(CameraRig.use.coordUpRight)*scaleFactor+c); // right
			GL.Vertex(CamCoordToGLVert(CameraRig.use.coordUpRight)*scaleFactor+c);
			GL.Vertex(CamCoordToGLVert(CameraRig.use.coordUpLeft)*scaleFactor+c); // top
			GL.Vertex(CamCoordToGLVert(CameraRig.use.coordUpLeft)*scaleFactor+c);
			GL.Vertex(CamCoordToGLVert(CameraRig.use.coordDnLeft)*scaleFactor+c); // left
			GL.End();
			GL.PopMatrix();
			
			// restore gui color
			GUI.color = col;
		} GUILayout.EndArea();
	}
	
	/// <summary>
	/// Convert a coordinate to a GL vertex for drawing frustum.
	/// </summary>
	/// <returns>
	/// The coordinate to GL vert.
	/// </returns>
	/// <param name='v'>
	/// V.
	/// </param>
	Vector3 CamCoordToGLVert(Vector3 v)
	{
		return new Vector3(v.x, v.z, 0f);
	}
	
	/// <summary>
	/// Determines whether this instance is mouse over.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsMouseOver(){
		if(visible && m_minimapBackgroundRect.Contains (GUIHelpers.MouseToGUIPosition (Input.mousePosition))){
			return true;
		}
		else
			return false;
	}
}