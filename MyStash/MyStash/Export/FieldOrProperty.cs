namespace MyStash.Export
{
    /// <summary>
    /// Defines the way the class to export is inspected by reflection to extract fields or properties
    /// </summary>
    public enum FieldOrProperty
    {
        /// <summary>
        /// Only properties are read. If none, there is nothing to export.
        /// </summary>
        OnlyProperties,
        /// <summary>
        /// Only fields are read. If none, there is nothing to export.
        /// </summary>
        OnlyFields,
        /// <summary>
        /// Default mode : check for properties. Only if none found, check for fields.
        /// </summary>
        PropertiesOrFields,
        /// <summary>
        /// Returns properties and fields
        /// </summary>
        PropertyAndFields
    }
}
