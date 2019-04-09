//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
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
using System.Data.Entity.Spatial;

namespace Testing
{
   public partial class SpatialProperties
   {
      partial void Init();

      /// <summary>
      /// Default constructor. Protected due to required properties, but present because EF needs it.
      /// </summary>
      protected SpatialProperties()
      {
         Init();
      }

      /// <summary>
      /// Public constructor with required data
      /// </summary>
      /// <param name="attrgeography"></param>
      /// <param name="attrgeographycollection"></param>
      /// <param name="attrgeographylinestring"></param>
      /// <param name="attrgeographymultilinestring"></param>
      /// <param name="attrgeographymultipoint"></param>
      /// <param name="attrgeographymultipolygon"></param>
      /// <param name="attrgeographypoint"></param>
      /// <param name="attrgeographypolygon"></param>
      /// <param name="attrgeometry"></param>
      /// <param name="attrgeometrycollection"></param>
      /// <param name="attrgeometrylinestring"></param>
      /// <param name="attrgeometrymultilinestring"></param>
      /// <param name="attrgeometrymultipoint"></param>
      /// <param name="attrgeometrymultipolygon"></param>
      /// <param name="attrgeometrypoint"></param>
      public SpatialProperties(DbGeography attrgeography, DbGeography attrgeographycollection, DbGeography attrgeographylinestring, DbGeography attrgeographymultilinestring, DbGeography attrgeographymultipoint, DbGeography attrgeographymultipolygon, DbGeography attrgeographypoint, DbGeography attrgeographypolygon, DbGeometry attrgeometry, DbGeometry attrgeometrycollection, DbGeometry attrgeometrylinestring, DbGeometry attrgeometrymultilinestring, DbGeometry attrgeometrymultipoint, DbGeometry attrgeometrymultipolygon, DbGeometry attrgeometrypoint)
      {
         if (attrgeography == null) throw new ArgumentNullException(nameof(attrgeography));
         this.AttrGeography = attrgeography;
         if (attrgeographycollection == null) throw new ArgumentNullException(nameof(attrgeographycollection));
         this.AttrGeographyCollection = attrgeographycollection;
         if (attrgeographylinestring == null) throw new ArgumentNullException(nameof(attrgeographylinestring));
         this.AttrGeographyLineString = attrgeographylinestring;
         if (attrgeographymultilinestring == null) throw new ArgumentNullException(nameof(attrgeographymultilinestring));
         this.AttrGeographyMultiLineString = attrgeographymultilinestring;
         if (attrgeographymultipoint == null) throw new ArgumentNullException(nameof(attrgeographymultipoint));
         this.AttrGeographyMultiPoint = attrgeographymultipoint;
         if (attrgeographymultipolygon == null) throw new ArgumentNullException(nameof(attrgeographymultipolygon));
         this.AttrGeographyMultiPolygon = attrgeographymultipolygon;
         if (attrgeographypoint == null) throw new ArgumentNullException(nameof(attrgeographypoint));
         this.AttrGeographyPoint = attrgeographypoint;
         if (attrgeographypolygon == null) throw new ArgumentNullException(nameof(attrgeographypolygon));
         this.AttrGeographyPolygon = attrgeographypolygon;
         if (attrgeometry == null) throw new ArgumentNullException(nameof(attrgeometry));
         this.AttrGeometry = attrgeometry;
         if (attrgeometrycollection == null) throw new ArgumentNullException(nameof(attrgeometrycollection));
         this.AttrGeometryCollection = attrgeometrycollection;
         if (attrgeometrylinestring == null) throw new ArgumentNullException(nameof(attrgeometrylinestring));
         this.AttrGeometryLineString = attrgeometrylinestring;
         if (attrgeometrymultilinestring == null) throw new ArgumentNullException(nameof(attrgeometrymultilinestring));
         this.AttrGeometryMultiLineString = attrgeometrymultilinestring;
         if (attrgeometrymultipoint == null) throw new ArgumentNullException(nameof(attrgeometrymultipoint));
         this.AttrGeometryMultiPoint = attrgeometrymultipoint;
         if (attrgeometrymultipolygon == null) throw new ArgumentNullException(nameof(attrgeometrymultipolygon));
         this.AttrGeometryMultiPolygon = attrgeometrymultipolygon;
         if (attrgeometrypoint == null) throw new ArgumentNullException(nameof(attrgeometrypoint));
         this.AttrGeometryPoint = attrgeometrypoint;
         Init();
      }

      /// <summary>
      /// Static create function (for use in LINQ queries, etc.)
      /// </summary>
      /// <param name="attrgeography"></param>
      /// <param name="attrgeographycollection"></param>
      /// <param name="attrgeographylinestring"></param>
      /// <param name="attrgeographymultilinestring"></param>
      /// <param name="attrgeographymultipoint"></param>
      /// <param name="attrgeographymultipolygon"></param>
      /// <param name="attrgeographypoint"></param>
      /// <param name="attrgeographypolygon"></param>
      /// <param name="attrgeometry"></param>
      /// <param name="attrgeometrycollection"></param>
      /// <param name="attrgeometrylinestring"></param>
      /// <param name="attrgeometrymultilinestring"></param>
      /// <param name="attrgeometrymultipoint"></param>
      /// <param name="attrgeometrymultipolygon"></param>
      /// <param name="attrgeometrypoint"></param>
      public static SpatialProperties Create(DbGeography attrgeography, DbGeography attrgeographycollection, DbGeography attrgeographylinestring, DbGeography attrgeographymultilinestring, DbGeography attrgeographymultipoint, DbGeography attrgeographymultipolygon, DbGeography attrgeographypoint, DbGeography attrgeographypolygon, DbGeometry attrgeometry, DbGeometry attrgeometrycollection, DbGeometry attrgeometrylinestring, DbGeometry attrgeometrymultilinestring, DbGeometry attrgeometrymultipoint, DbGeometry attrgeometrymultipolygon, DbGeometry attrgeometrypoint)
      {
         return new SpatialProperties(attrgeography, attrgeographycollection, attrgeographylinestring, attrgeographymultilinestring, attrgeographymultipoint, attrgeographymultipolygon, attrgeographypoint, attrgeographypolygon, attrgeometry, attrgeometrycollection, attrgeometrylinestring, attrgeometrymultilinestring, attrgeometrymultipoint, attrgeometrymultipolygon, attrgeometrypoint);
      }

      /*************************************************************************
       * Persistent properties
       *************************************************************************/

      /// <summary>
      /// Identity, Required
      /// </summary>
      [Key]
      [Required]
      public int Id { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeography AttrGeography { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeography AttrGeographyCollection { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeography AttrGeographyLineString { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeography AttrGeographyMultiLineString { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeography AttrGeographyMultiPoint { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeography AttrGeographyMultiPolygon { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeography AttrGeographyPoint { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeography AttrGeographyPolygon { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeometry AttrGeometry { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeometry AttrGeometryCollection { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeometry AttrGeometryLineString { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeometry AttrGeometryMultiLineString { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeometry AttrGeometryMultiPoint { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeometry AttrGeometryMultiPolygon { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DbGeometry AttrGeometryPoint { get; set; }

   }
}

