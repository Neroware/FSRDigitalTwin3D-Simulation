using System;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3;

namespace FSR.DigitalTwin.Client.Features.UnityClient.GRPC.AAS.Utils
{
    public static class OperationVariableFactory {

        public static OperationVariableDTO From(SubmodelElementType type, params object[] args) {
            switch(type) {
                case SubmodelElementType.Property: return FromProperty(args[0]);
                default: throw new ArgumentException("The specified operation variable type is currently not supported by the client!");
            }
        }

        public static T GetRawValue<T>(this OperationVariableDTO va) {
            switch(va.Value.SubmodelElementType) {
                case SubmodelElementType.Property: return DataTypeDefXsdConverter.Convert<T>(va.Value.Property.ValueType, va.Value.Property.Value);
                default: throw new ArgumentException("The specified operation variable type is currently not supported by the client!");
            }
        }

        private static OperationVariableDTO FromProperty<T>(T value) {
            var propValue = DataTypeDefXsdConverter.Convert(value);
            var property = SubmodelElementFactory.Create(SubmodelElementType.Property, null, propValue.Item2, propValue.Item1);
            return new OperationVariableDTO() { Value = property };
        }

    }

}