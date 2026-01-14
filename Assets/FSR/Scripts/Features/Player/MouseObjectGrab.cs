using System;
using System.Collections;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.Player.Controls;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FSR.DigitalTwin.Client.Features.Player
{
    public class MouseObjectGrab : MonoBehaviour
    {
        [SerializeField] private FirstPersonHumanRobotInteractionInputActions inputActions;

        private Rigidbody _rigidbody;

        private void Start()
        {
            inputActions.Grab
                .Where(x => x)
                .Subscribe(_ => OnObjectGrab())
                .AddTo(this);
            inputActions.Grab
                .Where(x => !x && _rigidbody != null)
                .Subscribe(_ => OnObjectRelease())
                .AddTo(this);
            inputActions.GrabMove
                .Where(_ => _rigidbody != null)
                .Subscribe(_ => OnObjectMove())
                .AddTo(this);
        }
        private void OnObjectMove()
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit))
            {
                _rigidbody.transform.position = raycastHit.point;
            }
        }
        private void OnObjectRelease()
        {
            _rigidbody.constraints = RigidbodyConstraints.None;
            _rigidbody.gameObject.layer = LayerMask.NameToLayer(TagNames.DEFAULT_LAYER_NAME);
            _rigidbody = null;
        }
        private void OnObjectGrab()
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
            {
                GameObject grabbed = raycastHit.transform.gameObject;
                if (grabbed.tag == TagNames.WORKPIECE_TAG_NAME && grabbed.TryGetComponent(out Rigidbody rigidbody))
                {
                    _rigidbody = rigidbody;
                    _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY 
                        | RigidbodyConstraints.FreezePositionZ;
                    _rigidbody.gameObject.layer = LayerMask.NameToLayer(TagNames.LOCATION_LAYER_NAME);
                    // Debug.Log($"Grabbed {rigidbody.name}");
                }
            }
        }
    }

}
