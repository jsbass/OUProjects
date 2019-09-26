using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace Portal.Models
{
    [JsonConverter(typeof(PositionConverter))]
    public class Position : IEquatable<Position>, IEqualityComparer<Position>
    {
        public const int CoordCompareDecimalRounding = 10;
        
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double? Altitude { get; set; }

        public Position(double[] coords)
        {
            if (coords.Length < 2)
            {
                throw new ArgumentException("input coords array must contain at least 2 items");
            }
            Longitude = coords[0];
            Latitude = coords[1];
            Altitude = coords.Length >= 3 ? coords[3] : (double?) null;
            if(Math.Abs(Latitude) > 90) throw new ArgumentException("Latitude must be between 90 and -90");
            if(Math.Abs(Longitude) > 180) throw new ArgumentException("Longitude must be between 180 and -180");
        }

        public Position() {
        }
        #region Equality

        public override bool Equals(object obj)
        {
            return Equals(obj as Position);
        }

        public override int GetHashCode()
        {
            var hash = 1;
            hash = (hash*401) ^ Longitude.GetHashCode();
            hash = (hash*401) ^ Latitude.GetHashCode();
            if (Altitude.HasValue) hash = (hash*401) ^ Altitude.GetHashCode();
            return hash;
        }

        public bool Equals(Position other)
        {
            return Equals(this, other);
        }

        public bool Equals(Position x, Position y)
        {
            return x == y;
        }

        public int GetHashCode(Position obj)
        {
            return obj.GetHashCode();
        }

        #endregion

        #region operators
        public static bool operator ==(Position left, Position right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(null, right)) return false;
            if (ReferenceEquals(null, left)) return false;
            return Convert.ToInt64(left.Longitude*Math.Pow(10, CoordCompareDecimalRounding)) ==
                   Convert.ToInt64(right.Longitude*Math.Pow(10, CoordCompareDecimalRounding))
                   &&
                   Convert.ToInt64(left.Longitude*Math.Pow(10, CoordCompareDecimalRounding)) ==
                   Convert.ToInt64(right.Longitude*Math.Pow(10, CoordCompareDecimalRounding))
                   &&
                   Convert.ToInt64(left.Longitude*Math.Pow(10, CoordCompareDecimalRounding)) ==
                   Convert.ToInt64(right.Longitude*Math.Pow(10, CoordCompareDecimalRounding));
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        #endregion
    }

    public class PositionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var position = value as Position;
            if (position != null)
            {
                writer.WriteStartArray();
                writer.WriteValue(position.Longitude);
                writer.WriteValue(position.Latitude);

                if (position.Altitude.HasValue)
                {
                    writer.WriteValue(position.Altitude);
                }

                writer.WriteEndArray();
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var coords = serializer.Deserialize<double[]>(reader);
                if (coords == null)
                {
                    throw new JsonReaderException("coordinates cannot be null");
                }
                if (coords.Length != 2 && coords.Length != 3)
                {
                    throw new JsonReaderException(
                        $"Coordinates array must be of size 2 or 3, recieved array of size {coords.Length}");
                }

                return new Position(coords);
            }
            catch (Exception e)
            {
                throw new JsonReaderException("error parsing coordinates", e);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Position).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }
    }
}