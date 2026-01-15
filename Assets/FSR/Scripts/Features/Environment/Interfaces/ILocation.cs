using System;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Environment.Interfaces
{
    public interface ILocation 
    {
        public Uri LocationId { get; }
        public string LocationName { get; }
    }
}