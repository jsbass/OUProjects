using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Portal.Models
{
    public interface IGeometry
    {
        GeometryType Type { get; }
    }


    public enum GeometryType
    {
        Point,
        MultiPoint,
        LineString,
        MultiLineString,
        Polygon,
        MultiPolygon,
        GeometryCollection
    }
}