using System;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces
{
    public interface IDevicePrimitive
    {
        string Name { get; }
        Uri Id { get; }
        PrimitiveResult Execute(string task);
        Task<PrimitiveResult> ExecuteAsync(string task);
    }
}