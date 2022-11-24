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

	/// <summary> BT 노드 조립 </summary>
	private void MakeNode()
	{
		_rootNode =

			//If(() => Input.GetKey(KeyCode.Q)).
			Selector
			(
				// 내가 죽었나?
				Condition(isDead),
				// 플레이어가 시야에 존재하지 않으면 Idle
				IfNotAction(findTargetInSight, idleAction),

				// 플레이어가 시야에 들어 왔다면
				Selector
				(
					// 공격 사정거리 안에 플레이어가 없다면, 추적
					IfNotAction(isTargetInMyAttackRange, moveAction),

					Sequence
					(
						Action(attackAction),
						Action(delayNextAttack)
					)
				)
			);
	}




	// 조건
	bool isDead() { return (hp <= 0); }

	// 시야 범위 내에 타겟이 있나?
	bool findTargetInSight()
	{
		myTarget = FindObjectOfType<Player>();
		return (myTarget != null);
	}

	// 공격 범위 안에 내 타겟이 존재하는가?
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
		Debug.Log("# 공격~!!");

	}

	void delayNextAttack()
	{
		Debug.Log("# 다음 공격 기다림~!!");
	}
}
