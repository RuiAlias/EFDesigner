using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sawczyn.EFDesigner.EFModel
{
   /// <summary>
   ///    Bundles the various namespace choices in the model.
   /// </summary>
   public class TypeNamespaces 
   {
      public string ContextNamespace { get; set; }
      public string EntityNamespace { get; set; }
      public string EnumNamespace { get; set; }
      public string StructNamespace { get; set; }

      public static TypeNamespaces Clone(TypeNamespaces other)
      {
         return new TypeNamespaces
                {
                   ContextNamespace = other.ContextNamespace
                 , EntityNamespace = other.EntityNamespace
                 , EnumNamespace = other.EnumNamespace
                 , StructNamespace = other.StructNamespace
                };
      }
   }
}
