using System.Collections;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Features.UnityClient;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FSR.DigitalTwin.Client.Features.UI {
    public class ControlPanel : MonoBehaviour
    {

        [SerializeField] private Button connectButton;
        [SerializeField] private Button disconnectButton;

        [SerializeField] private TMP_InputField serverIpAddrField;
        [SerializeField] private TMP_InputField serverPortField;

        [SerializeField] private TMP_Text statusLabel;

        [SerializeField] private Toggle isListeningToggle;
        [SerializeField] private Toggle noClippingToggle;

        [SerializeField] private TMP_Dropdown operationMode;

        // Start is called before the first frame update
        private void Start()
        {
            DigitalWorkspace.Instance.Connection.IsConnected.Subscribe(OnConnectionChanged).AddTo(this);
            DigitalWorkspace.Instance.OperationMode = (DigitalWorkspace.EOperationMode) operationMode.value;
        }

        // Update is called once per frame
        private void Update()
        {
        
        }

        private void OnConnectionChanged(bool isConnected) {
            if (isConnected) {
                statusLabel.text = "Connected!";
                statusLabel.color = Color.green;
                connectButton.gameObject.SetActive(false);
                disconnectButton.gameObject.SetActive(true);
            }
            else {
                statusLabel.text = "Disconnected!";
                statusLabel.color = Color.red;
                connectButton.gameObject.SetActive(true);
                disconnectButton.gameObject.SetActive(false);
            }
        }

        public async void OnConnectButtonPress() {
            await DigitalWorkspace.Instance.Connection.Connect();   
        }

        public async void OnDisconnectButtonPress() {
            await DigitalWorkspace.Instance.Connection.Disconnect();
        }

        public void OnIsListeningTick(bool value) {
            
        }

        public void OnNoClipTick(bool value) {
            
        }

        public void OnOperationModeChanged(int value) {
            DigitalWorkspace.Instance.OperationMode = (DigitalWorkspace.EOperationMode) value;
        }
    }
}


