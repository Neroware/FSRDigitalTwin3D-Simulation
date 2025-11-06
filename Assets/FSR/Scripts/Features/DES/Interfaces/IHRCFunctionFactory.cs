namespace FSR.DigitalTwin.Client.Features.DES.Interfaces
{
    public interface IHRCFunctionFactory
    {
        HRCFunction Create(string taskId, HRCFunctionDescription description);
    }
}