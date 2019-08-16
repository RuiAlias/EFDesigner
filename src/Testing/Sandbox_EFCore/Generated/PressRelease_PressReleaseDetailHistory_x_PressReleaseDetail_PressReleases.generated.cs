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

namespace Sandbox_EFCore
{
   /// <summary>
   /// Class linking many-to-many association between global::Sandbox_EFCore.PressRelease.PressReleaseDetailHistory and global::Sandbox_EFCore.PressReleaseDetail.PressReleases
   /// </summary>
   public sealed class PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases : IJoin<global::Sandbox_EFCore.PressRelease>, IJoin<global::Sandbox_EFCore.PressReleaseDetail>, IEquatable<PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases>
   {
      public PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases()
      {
      }

      public global::Sandbox_EFCore.PressRelease LHS { get; set; }
      public Int32 LHS_Id { get; set; }

      public global::Sandbox_EFCore.PressReleaseDetail RHS { get; set; }
      public Int32 RHS_Id { get; set; }

      global::Sandbox_EFCore.PressRelease IJoin<global::Sandbox_EFCore.PressRelease>.Navigation
      {
         get { return LHS; }
         set { LHS = value; }
      }

      global::Sandbox_EFCore.PressReleaseDetail IJoin<global::Sandbox_EFCore.PressReleaseDetail>.Navigation
      {
         get { return RHS; }
         set { RHS = value; }
      }

      #region IEquatable

      /// <summary>Determines whether the specified object is equal to the current object.</summary>
      /// <param name="obj">The object to compare with the current object. </param>
      /// <returns>
      /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, obj)) return false;
         if (ReferenceEquals(this, obj)) return true;
         if (obj.GetType() != this.GetType()) return false;
         return Equals((PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases)obj);
      }

      /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
      /// <param name="other">An object to compare with this object.</param>
      /// <returns>
      /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
      public bool Equals(PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases other)
      {
         if (ReferenceEquals(null, other)) return false;
         if (ReferenceEquals(this, other)) return true;
         return LHS.Equals(other.LHS) && RHS.Equals(other.RHS);
      }

      /// <summary>Serves as the default hash function. </summary>
      /// <returns>A hash code for the current object.</returns>
      public override int GetHashCode()
      {
         unchecked
         {
            return (LHS.GetHashCode() * 397) ^ RHS.GetHashCode();
         }
      }

      /// <summary>Returns a value that indicates whether the values of two <see cref="T:Sandbox_EFCorePressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases" /> objects are equal.</summary>
      /// <param name="left">The first value to compare.</param>
      /// <param name="right">The second value to compare.</param>
      /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
      public static bool operator ==(PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases left, PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases right)
      {
         return Equals(left, right);
      }

      /// <summary>Returns a value that indicates whether two <see cref="T:Sandbox_EFCorePressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases" /> objects have different values.</summary>
      /// <param name="left">The first value to compare.</param>
      /// <param name="right">The second value to compare.</param>
      /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
      public static bool operator !=(PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases left, PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases right)
      {
         return !Equals(left, right);
      }

      #endregion

   }
}

