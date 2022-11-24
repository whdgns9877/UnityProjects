using Rito.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rito.BehaviorTree;
using static Rito.BehaviorTree.NodeHelper;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, ICore
{
	[SerializeField] float moveSpeed = 5f;
	[SerializeField] NavMeshAgent aiAgent = null;
	private INode _rootNode;
	int hp = 100;
	Player myTarget = null;

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

	/// <summary> BT ��� ���� </summary>
	private void MakeNode()
	{
		_rootNode =

			//If(() => Input.GetKey(KeyCode.Q)).
			Selector
			(
				// ���� �׾���?
				Condition(isDead),
				// �÷��̾ �þ߿� �������� ������ Idle
				IfNotAction(findTargetInSight, idleAction),

				// �÷��̾ �þ߿� ��� �Դٸ�
				Selector
				(
					// ���� �����Ÿ� �ȿ� �÷��̾ ���ٸ�, ����
					IfNotAction(isTargetInMyAttackRange, moveAction),

					Sequence
					(
						Action(attackAction),
						Action(delayNextAttack)
					)
				)
			);
	}




	// ����
	bool isDead() { return (hp <= 0); }

	// �þ� ���� ���� Ÿ���� �ֳ�?
	bool findTargetInSight()
	{
		myTarget = FindObjectOfType<Player>();
		return (myTarget != null);
	}

	// ���� ���� �ȿ� �� Ÿ���� �����ϴ°�?
	bool isTargetInMyAttackRange()
	{
		float dist = Vector3.Distance(myTarget.transform.position, transform.position);
		return (dist < 1f);
	}

	void idleAction()
	{

	}

	void moveAction()
	{
		//Vector3 moveDir = (myTarget.transform.position - transform.position).normalized;
		//transform.Translate(moveDir * Time.deltaTime * moveSpeed);
		aiAgent.SetDestination(myTarget.transform.position);
	}

	void attackAction()
	{
		Debug.Log("# ����~!!");

	}

	void delayNextAttack()
	{
		Debug.Log("# ���� ���� ��ٸ�~!!");
	}
}
