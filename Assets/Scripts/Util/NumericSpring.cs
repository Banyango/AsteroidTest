using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class NumericSpring {
	
	public float dampingRatio = 0.23f;
	public float angularFrequency = 25.1672f;
	
	[HideInInspector] public float value {
		get;
		set;
	}

	[HideInInspector] public float velocity;
	[HideInInspector] public float timeStep;
	[HideInInspector] public float target;

	private void Spring() {				

		float f = 1.0f + 2.0f * timeStep * dampingRatio * angularFrequency;
		float oo = angularFrequency * angularFrequency;
		float hoo = timeStep * oo;
		float hhoo = timeStep * hoo;
		float detInv = 1.0f / (f + hhoo);
		float detX = f * value + timeStep * velocity + hhoo * target;
		float detV = velocity + hoo * (target - value);

		value = detX * detInv;
		velocity = detV * detInv;

	}	

	public void Update(float timeStep) {
		this.timeStep = timeStep;

		Spring (); 
	}

	public void SetTarget(float target) {
		this.target = target;
	}
}


