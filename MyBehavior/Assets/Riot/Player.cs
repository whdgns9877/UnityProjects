using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rito.BehaviorTree;

using static Rito.BehaviorTree.NodeHelper;

public class Player : MonoBehaviour, ICore
{
	[SerializeField] float moveSpeed = 5f;
	private INode _rootNode;


    void Awake()
    {
		MakeNode();
	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		_rootNode.Run();
	}

	/// <summary> BT 노드 조립 </summary>
	private void MakeNode()
	{
		_rootNode =

			//If(() => Input.GetKey(KeyCode.Q)).
			Selector
			(
				IfAction(KeyMoveInput, KeyMoveAction),
				IfAction(MouseMoveInput, MouseMoveAction)
			);
	}

	private bool KeyMoveInput()
	{
		bool result =
			Input.GetKey(KeyCode.W) ||
			Input.GetKey(KeyCode.A) ||
			Input.GetKey(KeyCode.S) ||
			Input.GetKey(KeyCode.D);

		//Debug.Log($"Condition : Key Move INPUT ({result})");
		return result;
	}
	private bool MouseMoveInput()
	{
		bool result = Input.GetMouseButton(1);
		//Debug.Log($"Condition : Mouse Move INPUT ({result})");
		return result;
	}
	private void KeyMoveAction()
	{
		//Debug.Log($"Action : Key Move");

		float moveX = 0f, moveZ = 0f;
		if (Input.GetKey(KeyCode.A)) moveX = -1f;
		else if (Input.GetKey(KeyCode.D)) moveX = 1f;
		else moveX = 0f;

		if (Input.GetKey(KeyCode.W)) moveZ = 1f;
		else if (Input.GetKey(KeyCode.S)) moveZ = -1f;
		else moveZ = 0f;


		Vector3 move = Vector3.zero;
		move.x = moveX * Time.deltaTime * moveSpeed;
		move.z = moveZ * Time.deltaTime * moveSpeed;
		transform.Translate(move);


	}
	private void MouseMoveAction()
	{
		//Debug.Log($"Action : Mouse Move");



		
	}
}
