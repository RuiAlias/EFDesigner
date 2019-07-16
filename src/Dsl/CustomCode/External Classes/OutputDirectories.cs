using System.ComponentModel;

namespace Sawczyn.EFDesigner.EFModel
{

   /// <summary>
   ///    Bundles the various output directory choices in the model. Done this way for backward compatability; prior to
   ///    1.3.0.5 these were four separate properties, and we don't want to break old models by remmoving those and
   ///    replacing them with the one.
   /// </summary>
   public class OutputDirectories
   {
      private readonly ModelRoot modelRoot;

      public OutputDirectories(ModelRoot modelRoot)
      {
         this.modelRoot = modelRoot;
      }

      [DisplayName("Enums")]
      [Description("Project directory for enums")]
      [TypeConverter(typeof(ProjectDirectoryTypeConverter))]
      public string EnumOutputDirectory
      {
         get { return modelRoot?.EnumOutputDirectory; }
         set { if (modelRoot != null && modelRoot.EnumOutputDirectory != value) modelRoot.EnumOutputDirectory = value; }
      }

      [DisplayName("DbContext")]
      [Description("Project directory for DbContext-related files")]
      [TypeConverter(typeof(ProjectDirectoryTypeConverter))]
      public string ContextOutputDirectory
      {
         get { return modelRoot?.ContextOutputDirectory; }
         set { if (modelRoot != null && modelRoot.ContextOutputDirectory != value) modelRoot.ContextOutputDirectory = value; }
      }

      [DisplayName("Entities")]
      [Description("Output directory for entities")]
      [TypeConverter(typeof(ProjectDirectoryTypeConverter))]
      public string EntityOutputDirectory
      {
         get { return modelRoot?.EntityOutputDirectory; }
         set { if (modelRoot != null && modelRoot.EntityOutputDirectory != value) modelRoot.EntityOutputDirectory = value; }
      }

      [DisplayName("Structs")]
      [Description("Project directory for generated structures (owned/complex types)")]
      [TypeConverter(typeof(ProjectDirectoryTypeConverter))]
      public string StructOutputDirectory
      {
         get { return modelRoot?.StructOutputDirectory; }
         set { if (modelRoot != null && modelRoot.StructOutputDirectory != value) modelRoot.StructOutputDirectory = value; }
      }
   }

}