using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class CoroutinesVsRx : MonoBehaviour
{
    public float angularVelocity;
    public Transform withCoroutines;
    public Transform withRx;
    public Transform withCoroutinesUntilClickOrTimeUp;
    public Transform withRxUntilClickOrTimeUp;
    public float totalTime = 10f;

    bool MouseUp { get { return !Input.GetMouseButton(0); } }

    void Awake()
    {
        StartCoroutine(RotateObjectCoroutine());
        RotateObjectRx();

        StartCoroutine(RotateObjectUntilClickOrTimeUpCoroutine());
        RotateObjectUntilClickOrTimeUpRx();
    }

    IEnumerator RotateObjectCoroutine()
    {
        while (true)
        {
            if (MouseUp)
                Rotate(Time.deltaTime, withCoroutines);

            // yield return new WaitForEndOfFrame();
            // yield return new WaitForFixedUpdate();
            yield return 0;
        }
    }

    void RotateObjectRx()
    {
        // Observable.EveryLateUpdate()
        // Observable.EveryFixedUpdate()
        Observable.EveryUpdate()
            .Select(_ => Time.deltaTime)
			.Where(_ => MouseUp)
			.Subscribe(delta => Rotate(delta, withRx));
    }

    IEnumerator RotateObjectUntilClickOrTimeUpCoroutine()
    {
		var time = 0f;

		while(MouseUp && time < totalTime)
		{
			Rotate(Time.deltaTime, withCoroutinesUntilClickOrTimeUp);

			time += Time.deltaTime;

	        yield return 0;
		}
    }

    void RotateObjectUntilClickOrTimeUpRx()
    {
        var mouse = Observable.EveryUpdate().Where(_ => !MouseUp);
        var timeUp = Observable.Timer(TimeSpan.FromSeconds(totalTime));
		var mouseOrTimer = mouse.Merge(timeUp);

		Observable.EveryUpdate()
			.Select(_ => Time.deltaTime)
			.TakeUntil(mouseOrTimer)
			.Subscribe(delta => Rotate(delta, withRxUntilClickOrTimeUp));
    }
    void Rotate(float time, Transform target)
    {
        target.Rotate(0, 0, angularVelocity * time);
    }
}
