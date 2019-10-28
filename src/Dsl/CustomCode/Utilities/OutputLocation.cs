using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sawczyn.EFDesigner.EFModel
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class OutputLocation
    {
       private readonly ModelRoot modelRoot;

       public OutputLocation(ModelRoot modelRoot)
       {
          this.modelRoot = modelRoot;
       }

       public string Context
       {
          get { return modelRoot.ContextOutputDirectory; }
          set { modelRoot.ContextOutputDirectory = value; }
       }

       public string Entity
       {
          get { return modelRoot.EntityOutputDirectory; }
          set { modelRoot.EntityOutputDirectory = value; }
       }

       public string Enum
       {
          get { return modelRoot.EnumOutputDirectory; }
          set { modelRoot.EnumOutputDirectory = value; }
       }

       public string Struct
       {
          get { return modelRoot.StructOutputDirectory; }
          set { modelRoot.StructOutputDirectory = value; }
       }
    }

    // not yet

    //[TypeConverter(typeof(ExpandableObjectConverter))]
    //public class OutputLocationDetail
    //{
    //   public string Project { get; set; }
    //   public string Folder { get; set; }
    //}
}
