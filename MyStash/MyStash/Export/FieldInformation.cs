using System;
using System.Reflection;

namespace MyStash.Export
{

    /// <summary>
    /// Information about a source field
    /// </summary>
    public class FieldInformation
    {
        #region fields

        private bool hidden;
        private string exportName;

        #endregion

        #region Properties
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInformation"/> class.
        /// </summary>
        public FieldInformation()
        {
            IsProperty = true;
            DisplayWidth = 30;
            if (IsNumber) DisplayWidth = 10;
        }

        /// <summary>
        /// Gets or sets the field type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this field is hidden.
        /// </summary>
        /// <value><c>true</c> if this field is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden
        {
            get { return hidden || !IsSupportedType; }
            set { hidden = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this field is exported.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this field is exported; otherwise, <c>false</c>.
        /// </value>
        public bool IsExported
        {
            get { return !hidden && IsSupportedType; }
            set { hidden = !value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this field type is supported.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this field type is supported; otherwise, <c>false</c>.
        /// </value>
        public bool IsSupportedType { get; set; }

        /// <summary>
        /// Gets or sets the display width.
        /// </summary>
        /// <value>The display width.</value>
        public int DisplayWidth { get; set; }

        /// <summary>
        /// Gets or sets the exported name 
        /// </summary>
        /// <value>The name of the export.</value>
        public string ExportName
        {
            get
            {
                return (string.IsNullOrEmpty(exportName)) ? Name : exportName;
            }
            set
            {
                if (value == null) return;
                exportName = value.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the field position.
        /// </summary>
        /// <value>The field position.</value>
        public int FieldPosition { get; set; }

        /// <summary>
        /// Gets a value indicating whether this field is integer.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this field is integer; otherwise, <c>false</c>.
        /// </value>
        public bool IsInteger => Type == typeof(short) ||
                                 Type == typeof(int) ||
                                 Type == typeof(long) ||
                                 Type == typeof(byte) ||
                                 Type == typeof(sbyte) ||
                                 Type == typeof(ushort) ||
                                 Type == typeof(uint) ||
                                 Type == typeof(ulong);

        /// <summary>
        /// Gets a value indicating whether this field is real.
        /// </summary>
        /// <value><c>true</c> if this field is real; otherwise, <c>false</c>.</value>
        public bool IsReal => (Type == typeof(decimal) ||
                               Type == typeof(float) ||
                               Type == typeof(double));

        /// <summary>
        /// Gets a value indicating whether this field is number.
        /// </summary>
        /// <value><c>true</c> if this field is number; otherwise, <c>false</c>.</value>
        public bool IsNumber
        {
            get { return IsInteger || IsReal; }
        }

        /// <summary>
        /// Gets a value indicating whether this field is date time.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this field is date time; otherwise, <c>false</c>.
        /// </value>
        public bool IsDateTime
        {
            get { return Type == typeof(DateTime); }
        }

        /// <summary>
        /// Gets or sets the pro info.
        /// </summary>
        /// <value>The pro info.</value>
        public PropertyInfo ProInfo { get; set; }

        /// <summary>
        /// Gets or sets the field info.
        /// </summary>
        /// <value>The field info.</value>
        public FieldInfo FieldInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this field is a property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this field is a property; otherwise, <c>false</c>.
        /// </value>
        public bool IsProperty { get; set; }

        /// <summary>
        /// Gets or sets the "get field value" callback.
        /// </summary>
        /// <value>The "get field" callback.</value>
        public GetFieldTextDelegate GetFieldCallback { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; } 
        #endregion
    }
}
