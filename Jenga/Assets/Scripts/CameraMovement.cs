using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private float rotationSpeed, zoomMultiplier, zoomVelocity, smoothTime;


    private float currentZoom;
 
    public Transform currentFocus;

    Vector3 camTarget;
    Vector3 camSmoothDampV;


    bool isSwitchingTargets = false;

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        currentZoom = cam.fieldOfView;

        camTarget = cam.transform.position;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isSwitchingTargets)
        {
            if (Input.GetMouseButton(0))
                transform.RotateAround(currentFocus.position, transform.up, Input.GetAxis("Mouse X") * rotationSpeed);


            // Smooth zoom
            float scrollValue = Input.GetAxis("Mouse ScrollWheel");
            currentZoom -= scrollValue * zoomMultiplier;
            currentZoom = Mathf.Clamp(currentZoom, 30, 70);
            cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, currentZoom, ref zoomVelocity, smoothTime);
        }

        // Move the camera smoothly to the target position

    }
    public void ChangeFocus(Transform newTarget, float zoom = 20)
    {
        if (!isSwitchingTargets)
        {
            currentFocus = newTarget;
            camTarget = newTarget.position - zoom * cam.transform.forward;
            StartCoroutine(ChangePositionCoroutine());
        }
    }

    public void FocusOnBlock(Transform newTarget)
    {

        currentFocus = newTarget;
        camTarget = newTarget.position - 6 * cam.transform.forward;
        StartCoroutine(ChangePositionCoroutine());
        newTarget.GetComponent<Outline>().enabled = true;
        
    }

    private IEnumerator ChangePositionCoroutine()
    {
        isSwitchingTargets = true;
        int counter = 0;
        while (counter < 100)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, camTarget, ref camSmoothDampV, 0.12f);
            yield return new WaitForSeconds(0.01f);
            counter++;
        }
        cam.transform.position = camTarget;
        cam.transform.LookAt(currentFocus);
        isSwitchingTargets = false;
        yield return null;
    }

}
