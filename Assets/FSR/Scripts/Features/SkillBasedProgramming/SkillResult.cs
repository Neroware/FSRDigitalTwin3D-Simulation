using System;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming
{
    public record SkillResult
    {
        public bool Succeeded { init; get; }
        public bool Failed => !Succeeded;
        public TimeSpan TimeExpired { init; get; }
        public object[] Outputs { init; get; }
        public static SkillResult Failure(TimeSpan timeExpired)
        {
            return new()
            {
                Succeeded = false,
                TimeExpired = timeExpired,
                Outputs = new object[0]
            };
        }
        public static SkillResult Success(object[] outputs, TimeSpan timeExpired)
        {
            return new()
            {
                Succeeded = true,
                TimeExpired = timeExpired,
                Outputs = outputs
            };
        }
    }

    public record PrimitiveResult
    {
        public bool Succeeded { init; get; }
        public bool Failed => !Succeeded;
        public string Message { get; init; }
        public object[] Outputs { get; init; }
        public static PrimitiveResult Failure(string message = "A device primitive failed!")
        {
            return new()
            {
                Succeeded = false,
                Message = message,
                Outputs = new object[0]
            };
        }
        public static PrimitiveResult Success(object[] outputs, string message = "")
        {
            return new ()
            {
                Succeeded = true,
                Message = "",
                Outputs = outputs
            };
        }
    }
}

