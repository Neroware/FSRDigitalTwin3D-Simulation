using System;
using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.Client.Features.Environment.Interfaces;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using FSR.DigitalTwin.Client.Features.Robotics.KinematicRobot;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.Moveit;
using FSR.DigitalTwin.Client.Features.UnityClient;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Assembly.ScrewBehavior
{
    public class Hole : MonoBehaviour, IDigitalTwinEntity, ILocation
    {
        [Header("Digital Twin Component")]
        [SerializeField] private string _id;
        [SerializeField] private string _shortId;
        [SerializeField] private List<DigitalTwinComponentBase> _components;
        [Header("Screw Behavior")]
        [SerializeField] private ScrewerBase _screwerEE;
        [SerializeField] private NativeScrewEEController _screwerController;

        public Uri Id { get => new(_id); init => _id = value.ToString(); }
        public bool HasConnection => false;
        public IEnumerable<IDigitalTwinEntityComponent> Components { get => _components; set => _components = value.Cast<DigitalTwinComponentBase>().ToList(); }
        public Uri LocationId => Id;
        public string LocationName => _shortId;

        public Screw ScrewPart { set; get; }
        public float ScrewPathPercent { set => UpdateScrewPathPercent(value); }

        private void Start()
        {
            
        }
        private void OnTriggerEnter(Collider other)
        {
            
        }

        // private void OnCollisionEnter(Collision collision)
        // {
        //     Debug.Log($"> {collision.gameObject.name}");
        // }
        // private void OnCollisionExit(Collision collision)
        // {

        // }
        private void UpdateScrewPathPercent(float value)
        {
            throw new NotImplementedException();
        }
    }
}