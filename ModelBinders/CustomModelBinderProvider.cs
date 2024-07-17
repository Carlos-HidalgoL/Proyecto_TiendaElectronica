using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Proyecto_TiendaElectronica.Models;
using System;

namespace Proyecto_TiendaElectronica.ModelBinder
{
    public class CustomModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(Usuario)) return new BinderTypeModelBinder(typeof(UsuarioModelBinder));

            return null;
        }
    }
}
