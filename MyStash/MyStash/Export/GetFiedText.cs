namespace MyStash.Export
{
    /// <summary>
    /// Defines a callback that can be set by the calling app to return the value of a given field.
    /// Can be useful for embedded or complex objects not having a ToString() for example.
    /// </summary>
    public delegate string GetFieldTextDelegate(FieldInformation sender, object sourceItem);
}
