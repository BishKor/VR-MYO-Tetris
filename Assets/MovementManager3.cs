
using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class MovementManager3 : MonoBehaviour {
	public GameObject leftMyo = null;
	public GameObject rightMyo = null;
	public ControlWall controlWall = null;
	private Pose _leftlastPose = Pose.Unknown;
	private Pose _rightlastPose = Pose.Unknown;

	// The pose from the last update. This is used to determine if the pose has changed
	// so that actions are only performed upon making them rather than every frame during
	// which they are active.

	//    private float startingHeight;
	// Use this for initialization
	void Start () {
		//        startingHeight = 1;
	}
	void Update() {

	}
	// Update is called once per frame
	void LateUpdate () {

		// Access the ThalmicMyo component attached to the Myo game object.
		ThalmicMyo rightThalmicMyo = rightMyo.GetComponent<ThalmicMyo> ();
		ThalmicMyo leftThalmicMyo = leftMyo.GetComponent<ThalmicMyo> ();

		if ((rightThalmicMyo.pose != _rightlastPose) | (leftThalmicMyo.pose != _leftlastPose)) {
			if ((rightThalmicMyo.pose == Pose.Fist) & (leftThalmicMyo.pose == Pose.Fist)) {
				Debug.Log ("PULL DOWN!\n");
				Debug.Log ("Right X Coord = " + rightThalmicMyo.transform.position.x);
				controlWall.Slam();
			}
			else if (rightThalmicMyo.pose != _rightlastPose) {
				if (rightThalmicMyo.pose == Pose.DoubleTap) {
					Debug.Log ("ROTATE RIGHT!\n");
					controlWall.RotatePlayer(1);
				} else if (rightThalmicMyo.pose == Pose.WaveOut) {
					Debug.Log ("MOVE RIGHT!\n");
					controlWall.UpdatePlayerHorizontally (-1);
				}
			}
			else {
				if (leftThalmicMyo.pose == Pose.DoubleTap) {
					Debug.Log ("ROTATE LEFT!\n");
					controlWall.RotatePlayer(-1);
				} else if (leftThalmicMyo.pose == Pose.WaveOut) {
					Debug.Log ("MOVE LEFT!\n");
					controlWall.UpdatePlayerHorizontally (1);
				}
			}
		}
		_leftlastPose = leftThalmicMyo.pose;
		_rightlastPose = rightThalmicMyo.pose;
	}
}