using UnityEngine;
using System.Collections;

public class CameraBoundsCollision : MonoBehaviour {

	private Camera _camera;

	private BoxCollider2D _north;
	private BoxCollider2D _south;
	private BoxCollider2D _west;
	private BoxCollider2D _east;

	public float BoxSize = 5f;

	void Start () {
		_camera = GetComponent<Camera> ();

		_north = gameObject.AddComponent<BoxCollider2D> ();
		_south = gameObject.AddComponent<BoxCollider2D> ();
		_west = gameObject.AddComponent<BoxCollider2D> ();
		_east = gameObject.AddComponent<BoxCollider2D> ();

		UpdateCameraBounds ();
	}

	public void UpdateCameraBounds () {
		var bottomLeft = _camera.ScreenToWorldPoint (new Vector3 (0, 0, 0));		
		var topRight = _camera.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0));

		Debug.Log (bottomLeft);

		_north.offset = _camera.ViewportToWorldPoint(new Vector2(0.5f, 1.0f));
		_north.size = new Vector2 ((topRight.x - bottomLeft.x), BoxSize);
		_north.offset = new Vector2 (_north.offset.x, _north.offset.y + BoxSize/2);

		_south.offset = _camera.ViewportToWorldPoint(new Vector2(0.5f, 0.0f));
		_south.size = new Vector2 ((topRight.x - bottomLeft.x), BoxSize);
		_south.offset = new Vector2 (_south.offset.x, _south.offset.y - BoxSize/2);

		_west.offset = _camera.ViewportToWorldPoint(new Vector2(0f, 0.5f));
		_west.size = new Vector2 (BoxSize, (topRight.y - bottomLeft.y));
		_west.offset = new Vector2 (_west.offset.x - BoxSize/2, _west.offset.y);

		_east.offset = _camera.ViewportToWorldPoint(new Vector2(1f, .5f));
		_east.size = new Vector2 (BoxSize, (topRight.y - bottomLeft.y));
		_east.offset = new Vector2 (_east.offset.x + BoxSize/2, _east.offset.y);

	}
}
