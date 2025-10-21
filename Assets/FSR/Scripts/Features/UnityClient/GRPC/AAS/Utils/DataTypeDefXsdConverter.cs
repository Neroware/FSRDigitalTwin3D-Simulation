using System;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3;

namespace FSR.DigitalTwin.Client.Features.UnityClient.GRPC.AAS.Utils {

    public static class DataTypeDefXsdConverter {

        public static Tuple<DataTypeDefXsd, string> Convert<T>(T value) {
            string propValue = "";
            DataTypeDefXsd propType = DataTypeDefXsd.String;

            if (value is string s) { propType = DataTypeDefXsd.String; propValue = s; }
            else if (value is uint ui) { propType = DataTypeDefXsd.UnsignedInt; propValue = ui.ToString(); }
            else if (value is int i) { propType = DataTypeDefXsd.Int; propValue = i.ToString(); }
            else if (value is ulong ul) { propType = DataTypeDefXsd.UnsignedLong; propValue = ul.ToString(); }
            else if (value is long l) { propType = DataTypeDefXsd.Int; propValue = l.ToString(); }
            else if (value is ushort ush) { propType = DataTypeDefXsd.UnsignedShort; propValue = ush.ToString(); }
            else if (value is short sh) { propType = DataTypeDefXsd.Short; propValue = sh.ToString(); }
            else if (value is byte by) { propType = DataTypeDefXsd.Byte; propValue = by.ToString(); }
            else if (value is char c) { propType = DataTypeDefXsd.UnsignedByte; propValue = ((byte) c).ToString(); }
            else if (value is double d) { propType = DataTypeDefXsd.Double; propValue = d.ToString(); }
            else if (value is float f) { propType = DataTypeDefXsd.Float; propValue = f.ToString(); }
            else if (value is bool b) { propType = DataTypeDefXsd.Boolean; propValue = b.ToString(); }
            // Add more types if needed...
            else { throw new ArgumentException("The specified property type is currently not supported by the client!"); }

            return new Tuple<DataTypeDefXsd, string>(propType, propValue);
        }

        public static T Convert<T>(DataTypeDefXsd valueType, string value) {
            object result = null;
            result = valueType switch
            {
                DataTypeDefXsd.UnsignedInt => uint.Parse(value),
                DataTypeDefXsd.Int => int.Parse(value),
                DataTypeDefXsd.UnsignedLong => ulong.Parse(value),
                DataTypeDefXsd.Long => long.Parse(value),
                DataTypeDefXsd.UnsignedShort => ushort.Parse(value),
                DataTypeDefXsd.Short => short.Parse(value),
                DataTypeDefXsd.Byte => byte.Parse(value),
                DataTypeDefXsd.UnsignedByte => (char)byte.Parse(value),
                DataTypeDefXsd.Double => double.Parse(value),
                DataTypeDefXsd.Float => float.Parse(value),
                DataTypeDefXsd.Boolean => bool.Parse(value),
                // Add more types if needed...
                _ => throw new ArgumentException("The specified property type is currently not supported by the client!"),
            };
            return (T) result;
        }

    }

}