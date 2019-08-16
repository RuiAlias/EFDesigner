//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
//
//     Produced by Entity Framework Visual Editor
//     https://github.com/msawczyn/EFDesigner
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sandbox_EFCore
{
   public partial class PressRelease
   {
      partial void Init();

      /// <summary>
      /// Default constructor
      /// </summary>
      public PressRelease()
      {
         PressReleaseDetails = new global::Sandbox_EFCore.NavigationProperty<global::Sandbox_EFCore.PressRelease, global::Sandbox_EFCore.PressReleaseDetail, PressRelease_PressReleaseDetails_x_PressReleaseDetail>(this);
         PressReleaseDetailHistory = new global::Sandbox_EFCore.NavigationProperty<global::Sandbox_EFCore.PressRelease, global::Sandbox_EFCore.PressReleaseDetail, PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases>(this);

         Init();
      }

      /*************************************************************************
       * Persistent properties
       *************************************************************************/

      /// <summary>
      /// Identity, Required, Indexed
      /// </summary>
      [Key]
      [Required]
      public int Id { get; private set; }

      public string Name { get; set; }

      /*************************************************************************
       * Persistent navigation properties
       *************************************************************************/

      public virtual ICollection<PressRelease_PressReleaseDetails_x_PressReleaseDetail> PressRelease_PressReleaseDetails_x_PressReleaseDetail_List { get; } = new HashSet<PressRelease_PressReleaseDetails_x_PressReleaseDetail>();
      [NotMapped]
      public ICollection<global::Sandbox_EFCore.PressReleaseDetail> PressReleaseDetails { get; private set; }

      public virtual ICollection<PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases> PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases_List { get; } = new HashSet<PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases>();
      [NotMapped]
      public ICollection<global::Sandbox_EFCore.PressReleaseDetail> PressReleaseDetailHistory { get; private set; }

   }
}

