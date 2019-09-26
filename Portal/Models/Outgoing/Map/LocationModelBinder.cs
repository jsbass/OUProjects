using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Models.Outgoing.Map
{
    public class LocationModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
            Type modelType)
        {
            if (modelType == typeof(Location))
            {
                var location = (Location) bindingContext.Model;
                Type newType;
                switch (location.LocationType)
                {
                    case LocationTypes.Building:
                        newType = typeof(Building);
                        break;
                    case LocationTypes.Parking:
                        newType = typeof(Parking);
                        break;
                    case LocationTypes.None:
                        return location;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var obj = Activator.CreateInstance(newType);
                bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => obj, newType);
                bindingContext.ModelMetadata.Model = obj;
                return obj;
            }
            return base.CreateModel(controllerContext, bindingContext, modelType);
        }
    }
}