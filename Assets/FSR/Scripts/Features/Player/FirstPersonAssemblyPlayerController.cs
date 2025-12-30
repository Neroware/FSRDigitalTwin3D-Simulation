using System.Collections;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Features.Player.Controls;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Player
{
    public class FirstPersonAssemblyPlayerController : MonoBehaviour
    {
        [SerializeField] private Camera workerCamera;
        [SerializeField] private CharacterController player;
        [SerializeField] private PlayerControlsInputActions inputActions;
        
        public Camera MainCamera { set; get; }
        public bool IsActive => workerCamera.enabled && workerCamera.gameObject.activeSelf;

        public void Start()
        {
            MainCamera = Camera.main;
            inputActions.Interact
                .Subscribe(_ => { 
                    if (!IsActive) EnterFirstPersonAssemblyMode(); 
                    else LeaveFirstPersonAssemblyMode(); 
                })
                .AddTo(this);
        }

        public void EnterFirstPersonAssemblyMode()
        {
            MainCamera.enabled = false;
            MainCamera.gameObject.SetActive(false);
            workerCamera.enabled = true;
            workerCamera.gameObject.SetActive(true);
            player.gameObject.SetActive(false);
        }

        public void LeaveFirstPersonAssemblyMode()
        {
            MainCamera.enabled = true;
            MainCamera.gameObject.SetActive(true);
            workerCamera.enabled = false;
            workerCamera.gameObject.SetActive(false);
            player.gameObject.SetActive(true);
        }


    }

}
