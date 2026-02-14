using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterControllerAnimator))]
public class KoboldLooker : MonoBehaviour
{
    [SerializeField]
    private Transform defaultTarget;

    private Transform target;
    private Collider currentCollider;
    private Vector3 dir;

    private CharacterControllerAnimator characterControllerAnimator;

    private void Awake()
    {
        characterControllerAnimator = GetComponent<CharacterControllerAnimator>();
        dir = characterControllerAnimator.HeadTransform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Kobold kobold))
        {
            SetTarget(kobold.GetAttachPointTransform(Equipment.AttachPoint.Head));
            currentCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(currentCollider == other)
        {
            SetTarget(null);
            currentCollider = null;
        }
    }

    private void Update()
    {
        Transform currentTarget = target != null ? target : defaultTarget;

        Vector3 targetDir = currentTarget.position - characterControllerAnimator.HeadTransform.position;
        dir = Vector3.RotateTowards(dir, targetDir, Time.deltaTime * 3f, 0f);
        Quaternion rotB = Quaternion.LookRotation(dir, Vector3.up);
        var rotEulerB = rotB.eulerAngles;
        characterControllerAnimator.SetEyeRot(new Vector2(rotEulerB.y, -rotEulerB.x));
    }

    private void SetTarget(Transform target)
    {
        this.target = target;
    }
}
