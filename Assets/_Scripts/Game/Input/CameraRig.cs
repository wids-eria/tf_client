using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// A component for working with the camera system
/// </summary>
public class CameraRig : MonoBehaviour
{
	/// <summary>
	/// Camera rig's desired position
	/// </summary>
	[HideInInspector]
	public Vector3 desiredPosition;
	/// <summary>
	/// Cache the camera's position in the previous frame for computing velocity
	/// </summary>
	private Vector3 m_previousPosition;
	/// <summary>
	/// Damping when moving to desired position
	/// </summary>
	[SerializeField]
	private float m_damping = 5f;
	/// <summary>
	/// Property to get the camera's current velocity
	/// </summary>
	public Vector3 velocity { get { return m_transform.position - m_previousPosition; } }
	/// <summary>
	/// Simple velocity test to determine whether or not the camera is currently moving
	/// </summary>
	public bool isCameraMoving {
		get {
			return velocity.sqrMagnitude > 0.001f || // TODO: intelligently tie this to input sensitivity
				!Mathf.Approximately(m_mainTransform.localPosition.magnitude, m_desiredCameraDistance); // TODO: maybe not here?
		}
	}
	/// <summary>
	/// how long has the camera been idle?
	/// </summary>
	public float idleTime { get; private set; }
	/// <summary>
	/// Amount that one unit on the input axis will translate the main camera
	/// </summary>
	[SerializeField]
	private float m_camZoomScalar = 10f;
	/// <summary>
	/// Maximum distance the camera can pull out
	/// </summary>
	[SerializeField]
	private float m_maxCamDistance = Mathf.Sqrt(162f);
	/// <summary>
	/// Minimum distance the camera can pull out
	/// </summary>
	[SerializeField]
	private float m_minCamDistance = 5f;
	/// <summary>
	/// Desired distance for main camera
	/// </summary>
	private float m_desiredCameraDistance;
	/// <summary>
	/// Gets the zoom scale factor.
	/// </summary>
	/// <value>
	/// The zoom scale factor.
	/// </value>
	public float zoomScaleFactor { get { return (m_maxCamDistance-m_mainTransform.localPosition.magnitude) / (m_maxCamDistance-m_minCamDistance); } }
	
	/// <summary>
	/// singleton
	/// </summary>
	public static CameraRig use;
	/// <summary>
	/// cache the camera rig's transform
	/// </summary>
	private Transform m_transform;
	/// <summary>
	/// cache the transform of the main camera
	/// </summary>
	private Transform m_mainTransform;	
	
	/// <summary>
	/// Initialize
	/// </summary>
	void Awake ()
	{
		// store singleton
		if (use != null) GameObject.Destroy(use.gameObject);
		use = this;
		
		// listen for its own messages
		MessengerAM.Listen(MessengerAM.listenTypeInput, this);
		
		// cache transforms
		m_transform = transform;
		m_mainTransform = Camera.main.transform;
		
		// set initial positions
		desiredPosition = m_transform.position;
		m_desiredCameraDistance = m_mainTransform.localPosition.magnitude;
		m_previousPosition = m_transform.position;
		
		// add some default idle time to prevent sending message on first frame
		idleTime = 1f;
	}
	
	/// <summary>
	/// Add the SSAO effect if supported
	/// </summary>
	void Start()
	{
		// TODO: perform more complex performance evaluation
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
//			Camera.main.gameObject.AddComponent<SSAOEffect>();
		}
	}
	
	/// <summary>
	/// The amount of idle time to wait before download request.
	/// </summary>
	[SerializeField]
	private float m_amountOfIdleTimeToWaitBeforeDownloadRequest = 0.5f;
		
	/// <summary>
	/// Update camera locations
	/// </summary>
	void LateUpdate()
	{
		// accumulate idle time
		if (!isCameraMoving) {
			if (idleTime-Time.deltaTime < m_amountOfIdleTimeToWaitBeforeDownloadRequest &&
				idleTime >= m_amountOfIdleTimeToWaitBeforeDownloadRequest
			) {
				MessengerAM.Send(new MessageCameraStopped());
			}
			idleTime += Time.deltaTime;
		}
		else {
			idleTime = 0f;
			MessengerAM.Send(new MessageCameraMoved());
		}
		
		// move the rig
		m_previousPosition = m_transform.position;
		m_transform.position = Vector3.Lerp(m_transform.position, desiredPosition, m_damping*Time.deltaTime);
				
		// zoom in and out
		m_mainTransform.localPosition = Vector3.Lerp(m_mainTransform.localPosition, m_mainTransform.localPosition.normalized*m_desiredCameraDistance, m_damping*Time.deltaTime);
	}
	
	/// <summary>
	/// Jump the view to the supplied location
	/// </summary>
	/// <param name="p">
	/// A <see cref="Vector3"/>
	/// </param>
	public void JumpTo(Vector3 p)
	{
		m_previousPosition = m_transform.position;
		desiredPosition = p;
		m_transform.position = p;
	}
	
	/// <summary>
	/// Zoom the main camera in or out as specified
	/// </summary>
	/// <param name="axis">
	/// A <see cref="System.Single"/>
	/// </param>
	public void Zoom(float axis)
	{
		m_desiredCameraDistance = Mathf.Max(m_minCamDistance, Mathf.Min(m_maxCamDistance, m_desiredCameraDistance+m_camZoomScalar*axis));
	}
	
	/// <summary>
	/// Ground plane
	/// </summary>
	private Plane m_groundPlane = new Plane(Vector3.up, Vector3.zero);
	/// <summary>
	/// Intersection point of a ray on the ground plane
	/// </summary>
	private Vector3 m_intersectionPoint;
	
	/// <summary>
	/// Get the currently visible region
	/// </summary>
	/// <returns>
	/// A <see cref="Region"/>
	/// </returns>
	public Region GetVisibleRegion()
	{
		// initialize using point in lower left corner
		m_intersectionPoint = coordDnLeft;
		Region region = new Region(
		                           Mathf.FloorToInt(m_intersectionPoint.x),
		                           Mathf.CeilToInt(m_intersectionPoint.x),
		                           Mathf.CeilToInt(m_intersectionPoint.z),
		                           Mathf.FloorToInt(m_intersectionPoint.z)
		                           );
		// grow the region to include each corner
		m_intersectionPoint = coordUpLeft;
		region.Encapsulate(Mathf.FloorToInt(m_intersectionPoint.x), Mathf.CeilToInt(m_intersectionPoint.z));
		m_intersectionPoint = coordUpRight;
		region.Encapsulate(Mathf.CeilToInt(m_intersectionPoint.x), Mathf.CeilToInt(m_intersectionPoint.z));
		m_intersectionPoint = coordDnRight;
		region.Encapsulate(Mathf.CeilToInt(m_intersectionPoint.x), Mathf.FloorToInt(m_intersectionPoint.z));
		// return the results
		return region;
	}
	
	/// <summary>
	/// Gets the tile coordinate at screenPosition.
	/// </summary>
	/// <returns>
	/// The <see cref="Vector3"/>.
	/// </returns>
	/// <param name='screenPosition'>
	/// Screen position.
	/// </param>
	private Vector3 GetCoordAt(Vector3 screenPosition)
	{
		VectorHelpers.GetIntersectionOnPlane(Camera.main.ScreenPointToRay(screenPosition), m_groundPlane, out m_intersectionPoint);
		m_intersectionPoint.x = Mathf.FloorToInt(m_intersectionPoint.x);
		m_intersectionPoint.z = Mathf.FloorToInt(m_intersectionPoint.z);
		return m_intersectionPoint;
	}
	
	/// <summary>
	/// Gets the tile coordinate in the lower left of the screen.
	/// </summary>
	/// <value>
	/// The tile coordinate in the lower left of the screen.
	/// </value>
	public Vector3 coordDnLeft {
		get {
			return GetCoordAt(Vector3.zero);
		}
	}
	/// <summary>
	/// Gets the tile coordinate in the upper left of the screen.
	/// </summary>
	/// <value>
	/// The tile coordinate in the upper left of the screen.
	/// </value>
	public Vector3 coordUpLeft {
		get {
			return GetCoordAt(new Vector3(0f,Camera.main.pixelHeight,0f));
		}
	}
	/// <summary>
	/// Gets the tile coordinate in the lower right of the screen.
	/// </summary>
	/// <value>
	/// The tile coordinate in the lower right of the screen.
	/// </value>
	public Vector3 coordDnRight {
		get {
			return GetCoordAt(new Vector3(Camera.main.pixelWidth,0f,0f));
		}
	}
	/// <summary>
	/// Gets the tile coordinate in the upper right of the screen.
	/// </summary>
	/// <value>
	/// The tile coordinate in the upper right of the screen.
	/// </value>
	public Vector3 coordUpRight {
		get {
			return GetCoordAt(new Vector3(Camera.main.pixelWidth,Camera.main.pixelHeight,0f));
		}
	}
	/// <summary>
	/// Gets the tile coordinate in the center of the screen.
	/// </summary>
	/// <value>
	/// The tile coordinate in the center of the screen.
	/// </value>
	public Vector3 coordCenter {
		get {
			return GetCoordAt(new Vector3(Camera.main.pixelWidth*0.5f,Camera.main.pixelHeight*0.5f,0f));
		}
	}
	
	/// <summary>
	/// Gets a value indicating whether any part of the terrain is in the view frustum.
	/// </summary>
	/// <value>
	/// <c>true</c> if any part of the terrain is in the view frustum; otherwise, <c>false</c>.
	/// </value>
	public bool isTerrainInViewFrustum {
		get {
			Region r = GetVisibleRegion();
			Vector3 min = TerrainManager.use.terrain.transform.position;
			Vector3 max = min + TerrainManager.terrainData.size;
			bool isInVertical = !(r.top<min.z || r.bottom>max.z);
			bool isInHorizontal = !(r.right<min.x || r.left>max.x);
			return (isInHorizontal && isInVertical);
		}
	}
	
	/// <summary>
	/// Gets a value indicating whether the view frustum is entirely terrain and no padding.
	/// </summary>
	/// <value>
	/// <c>true</c> if the view frustum entirely terrain; otherwise, <c>false</c>.
	/// </value>
	public bool isViewFrustumEntirelyTerrain {
		get {
			bool ret = true;
			foreach (GameObject go in TerrainManager.use.paddingMeshes) {
				if (go.renderer.isVisible) ret = false;
			}
			return ret;
		}
	}
}