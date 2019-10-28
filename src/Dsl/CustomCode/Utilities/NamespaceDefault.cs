using System.ComponentModel;

namespace Sawczyn.EFDesigner.EFModel
{
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public class NamespaceDefault
   {
      private readonly ModelRoot modelRoot;

      public NamespaceDefault(ModelRoot modelRoot)
      {
         this.modelRoot = modelRoot;

         if (string.IsNullOrEmpty(Entity))
            Entity = Context;

         if (string.IsNullOrEmpty(Enum))
            Enum = Context;

         if (string.IsNullOrEmpty(Struct))
            Struct = Context;
      }

      public string Context
      {
         get { return modelRoot.Namespace; }
         set { modelRoot.Namespace = value; }
      }

      public string Entity
      {
         get { return modelRoot.EntityNamespaceDefault; }
         set { modelRoot.EntityNamespaceDefault = value; }
      }

      public string Enum
      {
         get { return modelRoot.EnumNamespaceDefault; }
         set { modelRoot.EnumNamespaceDefault = value; }
      }

      public string Struct
      {
         get { return modelRoot.StructNamespaceDefault; }
         set { modelRoot.StructNamespaceDefault = value; }
      }
   }
}