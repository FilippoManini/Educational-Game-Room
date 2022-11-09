using UnityEngine;
using UnityEngine.UI;

namespace Assets.DM.Script.Object
{
    public class ButtonDoorRaycast : MonoBehaviour
    {
        [SerializeField] private int rayLength = 5;
        [SerializeField] private LayerMask layerMaskInteract;
        [SerializeField] private string excludeLayerName = null;

        private ButtonDoorController raycastedObj;

        [SerializeField] private KeyCode openDoorKey = KeyCode.Mouse0; //Mouse left click

        [SerializeField] private Image crosshair = null;
        private bool isCrosshairActive;
        private bool doOnce;

        private const string InteractableTag = "DoorButton";

        private void Update()
        {
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            int mask = 1 << LayerMask.NameToLayer(excludeLayerName) | layerMaskInteract.value;

            if(Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
            {
                if(hit.collider.CompareTag(InteractableTag))
                {
                    if (!doOnce)
                    {
                        raycastedObj = hit.collider.gameObject.GetComponent<ButtonDoorController>();
                        CrosshairChange(true);
                    }

                    isCrosshairActive = true;
                    doOnce = true;

                    if(Input.GetKeyDown(openDoorKey))
                    {
                        raycastedObj.PlayAnimation();
                    }
                }       
            }
            else
            {
                if(isCrosshairActive)
                {
                    CrosshairChange(false);
                    doOnce = false;

                }
            }
        }

        void CrosshairChange(bool on)
        {
            if( on && !doOnce)
                crosshair.color = Color.red;
            else
            {
                crosshair.color = Color.white;
                isCrosshairActive = false;
            }
        }

    }
}
