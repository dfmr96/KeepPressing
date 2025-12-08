using System;
using System.Collections;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{

    public Coroutine StartRotateY(Transform target, float? fromDeg, float toDeg, float speedDegPerSec, Action onComplete = null)
    {
        return StartCoroutine(RotateYCoroutine(target, fromDeg, toDeg, speedDegPerSec, onComplete));
    }

    private IEnumerator RotateYCoroutine(Transform target, float? fromDeg, float toDeg, float speedDegPerSec, Action onComplete)
    {
        if (target == null)
            yield break;
        
        if (fromDeg.HasValue)
        {
            Vector3 e = target.eulerAngles;
            e.y = fromDeg.Value;
            target.eulerAngles = e;
            yield return null;
        }

        
        float currentY = target.eulerAngles.y;
        const float tolerance = 0.01f;

        while (Mathf.Abs(Mathf.DeltaAngle(currentY, toDeg)) > tolerance)
        {
            currentY = Mathf.MoveTowardsAngle(currentY, toDeg, speedDegPerSec * Time.deltaTime);
            Vector3 e = target.eulerAngles;
            e.y = currentY;
            target.eulerAngles = e;
            yield return null;
        }
        
        Vector3 finalE = target.eulerAngles;
        finalE.y = toDeg;
        target.eulerAngles = finalE;
    }
}
