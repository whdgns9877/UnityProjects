using Rito.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.GraphicsBuffer;

public class Enemy2 : MonoBehaviour
{
	[SerializeField] float moveSpeed = 5f;
	int hp = 100;
	Player myTarget = null;

	void Awake()
	{
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	}

	/// <summary> BT ��� ���� </summary>
	private void MakeNode()
	{
		/*_rootNode =

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
			);*/
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
		return (dist < 3f);
	}

	void idleAction()
	{

	}

	void moveAction()
	{
		Vector3 moveDir = (myTarget.transform.position - transform.position).normalized;
		transform.Translate(moveDir * Time.deltaTime * moveSpeed);
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
