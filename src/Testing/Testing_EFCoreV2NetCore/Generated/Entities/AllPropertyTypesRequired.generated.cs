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

namespace Testing
{
   public partial class AllPropertyTypesRequired
   {
      partial void Init();

      /// <summary>
      /// Default constructor. Protected due to required properties, but present because EF needs it.
      /// </summary>
      protected AllPropertyTypesRequired()
      {
         Init();
      }

      /// <summary>
      /// Public constructor with required data
      /// </summary>
      /// <param name="binaryattr"></param>
      /// <param name="booleanattr"></param>
      /// <param name="byteattr"></param>
      /// <param name="datetimeattr"></param>
      /// <param name="datetimeoffsetattr"></param>
      /// <param name="decimalattr"></param>
      /// <param name="doubleattr"></param>
      /// <param name="guidattr"></param>
      /// <param name="int16attr"></param>
      /// <param name="int32attr"></param>
      /// <param name="int64attr"></param>
      /// <param name="singleattr"></param>
      /// <param name="stringattr"></param>
      /// <param name="timeattr"></param>
      public AllPropertyTypesRequired(byte[] binaryattr, bool booleanattr, byte byteattr, DateTime datetimeattr, DateTimeOffset datetimeoffsetattr, decimal decimalattr, double doubleattr, Guid guidattr, short int16attr, int int32attr, long int64attr, Single singleattr, string stringattr, TimeSpan timeattr)
      {
         this.BinaryAttr = binaryattr;
         this.BooleanAttr = booleanattr;
         this.ByteAttr = byteattr;
         this.DateTimeAttr = datetimeattr;
         this.DateTimeOffsetAttr = datetimeoffsetattr;
         this.DecimalAttr = decimalattr;
         this.DoubleAttr = doubleattr;
         this.GuidAttr = guidattr;
         this.Int16Attr = int16attr;
         this.Int32Attr = int32attr;
         this.Int64Attr = int64attr;
         this.SingleAttr = singleattr;
         if (string.IsNullOrEmpty(stringattr)) throw new ArgumentNullException(nameof(stringattr));
         this.StringAttr = stringattr;
         this.TimeAttr = timeattr;
         Init();
      }

      /// <summary>
      /// Static create function (for use in LINQ queries, etc.)
      /// </summary>
      /// <param name="binaryattr"></param>
      /// <param name="booleanattr"></param>
      /// <param name="byteattr"></param>
      /// <param name="datetimeattr"></param>
      /// <param name="datetimeoffsetattr"></param>
      /// <param name="decimalattr"></param>
      /// <param name="doubleattr"></param>
      /// <param name="guidattr"></param>
      /// <param name="int16attr"></param>
      /// <param name="int32attr"></param>
      /// <param name="int64attr"></param>
      /// <param name="singleattr"></param>
      /// <param name="stringattr"></param>
      /// <param name="timeattr"></param>
      public static AllPropertyTypesRequired Create(byte[] binaryattr, bool booleanattr, byte byteattr, DateTime datetimeattr, DateTimeOffset datetimeoffsetattr, decimal decimalattr, double doubleattr, Guid guidattr, short int16attr, int int32attr, long int64attr, Single singleattr, string stringattr, TimeSpan timeattr)
      {
         return new AllPropertyTypesRequired(binaryattr, booleanattr, byteattr, datetimeattr, datetimeoffsetattr, decimalattr, doubleattr, guidattr, int16attr, int32attr, int64attr, singleattr, stringattr, timeattr);
      }

      /*************************************************************************
       * Persistent properties
       *************************************************************************/

      /// <summary>
      /// Backing field for Id
      /// </summary>
      protected int _Id;
      /// <summary>
      /// When provided in a partial class, allows value of Id to be changed before setting.
      /// </summary>
      partial void SetId(int oldValue, ref int newValue);
      /// <summary>
      /// When provided in a partial class, allows value of Id to be changed before returning.
      /// </summary>
      partial void GetId(ref int result);

      /// <summary>
      /// Identity, Required, Indexed
      /// </summary>
      [Key]
      [Required]
      public int Id
      {
         get
         {
            int value = _Id;
            GetId(ref value);
            return (_Id = value);
         }
         private set
         {
            int oldValue = _Id;
            SetId(oldValue, ref value);
            if (oldValue != value)
            {
               _Id = value;
            }
         }
      }

      /// <summary>
      /// Backing field for BinaryAttr
      /// </summary>
      protected byte[] _BinaryAttr;
      /// <summary>
      /// When provided in a partial class, allows value of BinaryAttr to be changed before setting.
      /// </summary>
      partial void SetBinaryAttr(byte[] oldValue, ref byte[] newValue);
      /// <summary>
      /// When provided in a partial class, allows value of BinaryAttr to be changed before returning.
      /// </summary>
      partial void GetBinaryAttr(ref byte[] result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public byte[] BinaryAttr
      {
         get
         {
            byte[] value = _BinaryAttr;
            GetBinaryAttr(ref value);
            return (_BinaryAttr = value);
         }
         set
         {
            byte[] oldValue = _BinaryAttr;
            SetBinaryAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _BinaryAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for BooleanAttr
      /// </summary>
      protected bool _BooleanAttr;
      /// <summary>
      /// When provided in a partial class, allows value of BooleanAttr to be changed before setting.
      /// </summary>
      partial void SetBooleanAttr(bool oldValue, ref bool newValue);
      /// <summary>
      /// When provided in a partial class, allows value of BooleanAttr to be changed before returning.
      /// </summary>
      partial void GetBooleanAttr(ref bool result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public bool BooleanAttr
      {
         get
         {
            bool value = _BooleanAttr;
            GetBooleanAttr(ref value);
            return (_BooleanAttr = value);
         }
         set
         {
            bool oldValue = _BooleanAttr;
            SetBooleanAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _BooleanAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for ByteAttr
      /// </summary>
      protected byte _ByteAttr;
      /// <summary>
      /// When provided in a partial class, allows value of ByteAttr to be changed before setting.
      /// </summary>
      partial void SetByteAttr(byte oldValue, ref byte newValue);
      /// <summary>
      /// When provided in a partial class, allows value of ByteAttr to be changed before returning.
      /// </summary>
      partial void GetByteAttr(ref byte result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public byte ByteAttr
      {
         get
         {
            byte value = _ByteAttr;
            GetByteAttr(ref value);
            return (_ByteAttr = value);
         }
         set
         {
            byte oldValue = _ByteAttr;
            SetByteAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _ByteAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for DateTimeAttr
      /// </summary>
      protected DateTime _DateTimeAttr;
      /// <summary>
      /// When provided in a partial class, allows value of DateTimeAttr to be changed before setting.
      /// </summary>
      partial void SetDateTimeAttr(DateTime oldValue, ref DateTime newValue);
      /// <summary>
      /// When provided in a partial class, allows value of DateTimeAttr to be changed before returning.
      /// </summary>
      partial void GetDateTimeAttr(ref DateTime result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DateTime DateTimeAttr
      {
         get
         {
            DateTime value = _DateTimeAttr;
            GetDateTimeAttr(ref value);
            return (_DateTimeAttr = value);
         }
         set
         {
            DateTime oldValue = _DateTimeAttr;
            SetDateTimeAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _DateTimeAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for DateTimeOffsetAttr
      /// </summary>
      protected DateTimeOffset _DateTimeOffsetAttr;
      /// <summary>
      /// When provided in a partial class, allows value of DateTimeOffsetAttr to be changed before setting.
      /// </summary>
      partial void SetDateTimeOffsetAttr(DateTimeOffset oldValue, ref DateTimeOffset newValue);
      /// <summary>
      /// When provided in a partial class, allows value of DateTimeOffsetAttr to be changed before returning.
      /// </summary>
      partial void GetDateTimeOffsetAttr(ref DateTimeOffset result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public DateTimeOffset DateTimeOffsetAttr
      {
         get
         {
            DateTimeOffset value = _DateTimeOffsetAttr;
            GetDateTimeOffsetAttr(ref value);
            return (_DateTimeOffsetAttr = value);
         }
         set
         {
            DateTimeOffset oldValue = _DateTimeOffsetAttr;
            SetDateTimeOffsetAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _DateTimeOffsetAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for DecimalAttr
      /// </summary>
      protected decimal _DecimalAttr;
      /// <summary>
      /// When provided in a partial class, allows value of DecimalAttr to be changed before setting.
      /// </summary>
      partial void SetDecimalAttr(decimal oldValue, ref decimal newValue);
      /// <summary>
      /// When provided in a partial class, allows value of DecimalAttr to be changed before returning.
      /// </summary>
      partial void GetDecimalAttr(ref decimal result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public decimal DecimalAttr
      {
         get
         {
            decimal value = _DecimalAttr;
            GetDecimalAttr(ref value);
            return (_DecimalAttr = value);
         }
         set
         {
            decimal oldValue = _DecimalAttr;
            SetDecimalAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _DecimalAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for DoubleAttr
      /// </summary>
      protected double _DoubleAttr;
      /// <summary>
      /// When provided in a partial class, allows value of DoubleAttr to be changed before setting.
      /// </summary>
      partial void SetDoubleAttr(double oldValue, ref double newValue);
      /// <summary>
      /// When provided in a partial class, allows value of DoubleAttr to be changed before returning.
      /// </summary>
      partial void GetDoubleAttr(ref double result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public double DoubleAttr
      {
         get
         {
            double value = _DoubleAttr;
            GetDoubleAttr(ref value);
            return (_DoubleAttr = value);
         }
         set
         {
            double oldValue = _DoubleAttr;
            SetDoubleAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _DoubleAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for GuidAttr
      /// </summary>
      protected Guid _GuidAttr;
      /// <summary>
      /// When provided in a partial class, allows value of GuidAttr to be changed before setting.
      /// </summary>
      partial void SetGuidAttr(Guid oldValue, ref Guid newValue);
      /// <summary>
      /// When provided in a partial class, allows value of GuidAttr to be changed before returning.
      /// </summary>
      partial void GetGuidAttr(ref Guid result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public Guid GuidAttr
      {
         get
         {
            Guid value = _GuidAttr;
            GetGuidAttr(ref value);
            return (_GuidAttr = value);
         }
         set
         {
            Guid oldValue = _GuidAttr;
            SetGuidAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _GuidAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for Int16Attr
      /// </summary>
      protected short _Int16Attr;
      /// <summary>
      /// When provided in a partial class, allows value of Int16Attr to be changed before setting.
      /// </summary>
      partial void SetInt16Attr(short oldValue, ref short newValue);
      /// <summary>
      /// When provided in a partial class, allows value of Int16Attr to be changed before returning.
      /// </summary>
      partial void GetInt16Attr(ref short result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public short Int16Attr
      {
         get
         {
            short value = _Int16Attr;
            GetInt16Attr(ref value);
            return (_Int16Attr = value);
         }
         set
         {
            short oldValue = _Int16Attr;
            SetInt16Attr(oldValue, ref value);
            if (oldValue != value)
            {
               _Int16Attr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for Int32Attr
      /// </summary>
      protected int _Int32Attr;
      /// <summary>
      /// When provided in a partial class, allows value of Int32Attr to be changed before setting.
      /// </summary>
      partial void SetInt32Attr(int oldValue, ref int newValue);
      /// <summary>
      /// When provided in a partial class, allows value of Int32Attr to be changed before returning.
      /// </summary>
      partial void GetInt32Attr(ref int result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public int Int32Attr
      {
         get
         {
            int value = _Int32Attr;
            GetInt32Attr(ref value);
            return (_Int32Attr = value);
         }
         set
         {
            int oldValue = _Int32Attr;
            SetInt32Attr(oldValue, ref value);
            if (oldValue != value)
            {
               _Int32Attr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for Int64Attr
      /// </summary>
      protected long _Int64Attr;
      /// <summary>
      /// When provided in a partial class, allows value of Int64Attr to be changed before setting.
      /// </summary>
      partial void SetInt64Attr(long oldValue, ref long newValue);
      /// <summary>
      /// When provided in a partial class, allows value of Int64Attr to be changed before returning.
      /// </summary>
      partial void GetInt64Attr(ref long result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public long Int64Attr
      {
         get
         {
            long value = _Int64Attr;
            GetInt64Attr(ref value);
            return (_Int64Attr = value);
         }
         set
         {
            long oldValue = _Int64Attr;
            SetInt64Attr(oldValue, ref value);
            if (oldValue != value)
            {
               _Int64Attr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for SingleAttr
      /// </summary>
      protected Single _SingleAttr;
      /// <summary>
      /// When provided in a partial class, allows value of SingleAttr to be changed before setting.
      /// </summary>
      partial void SetSingleAttr(Single oldValue, ref Single newValue);
      /// <summary>
      /// When provided in a partial class, allows value of SingleAttr to be changed before returning.
      /// </summary>
      partial void GetSingleAttr(ref Single result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public Single SingleAttr
      {
         get
         {
            Single value = _SingleAttr;
            GetSingleAttr(ref value);
            return (_SingleAttr = value);
         }
         set
         {
            Single oldValue = _SingleAttr;
            SetSingleAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _SingleAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for StringAttr
      /// </summary>
      protected string _StringAttr;
      /// <summary>
      /// When provided in a partial class, allows value of StringAttr to be changed before setting.
      /// </summary>
      partial void SetStringAttr(string oldValue, ref string newValue);
      /// <summary>
      /// When provided in a partial class, allows value of StringAttr to be changed before returning.
      /// </summary>
      partial void GetStringAttr(ref string result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public string StringAttr
      {
         get
         {
            string value = _StringAttr;
            GetStringAttr(ref value);
            return (_StringAttr = value);
         }
         set
         {
            string oldValue = _StringAttr;
            SetStringAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _StringAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for TimeAttr
      /// </summary>
      protected TimeSpan _TimeAttr;
      /// <summary>
      /// When provided in a partial class, allows value of TimeAttr to be changed before setting.
      /// </summary>
      partial void SetTimeAttr(TimeSpan oldValue, ref TimeSpan newValue);
      /// <summary>
      /// When provided in a partial class, allows value of TimeAttr to be changed before returning.
      /// </summary>
      partial void GetTimeAttr(ref TimeSpan result);

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public TimeSpan TimeAttr
      {
         get
         {
            TimeSpan value = _TimeAttr;
            GetTimeAttr(ref value);
            return (_TimeAttr = value);
         }
         set
         {
            TimeSpan oldValue = _TimeAttr;
            SetTimeAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _TimeAttr = value;
            }
         }
      }

   }
}

