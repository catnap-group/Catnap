using UnityEngine;
using System.Collections;

public class Cat : MonoBehaviour {

	enum ActionName{
		Work,
		Run,
		RaoTou,
		MaiMen,
		Eat,
		Sleep,
	};

	// Use this for initialization
	void Start () {
		//PlayAnimation (ActionName.Work);
	}

	void PlayAnimation(ActionName name)
	{
		string triggerName = "";
		if (name == ActionName.Work) {
			triggerName = "Work";
		} else if (name == ActionName.Run) {
			triggerName = "Run";
		} else if (name == ActionName.RaoTou) {
			triggerName = "RaoTou";
		} else if (name == ActionName.MaiMen) {
			triggerName = "MaiMen";
		} else if (name == ActionName.Eat) {
			triggerName = "Eat";
		} else if (name == ActionName.Sleep) {
			triggerName = "Sleep";
		}

		Animator animator = gameObject.GetComponent<Animator> ();
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (0);
		if (stateInfo.tagHash == Animator.StringToHash ("RaoTou")) {
			animator.SetTrigger (triggerName);
		}
	}
}
