namespace MrCMS.DbConfiguration.Caches.Azure
{
    /// <summary>
    /// Defines a serialization interface to allow the implementor to provide a custom serialization implementation that
    /// can be configured to override the default App Fabric XML seriailization which isn't particularly performant in 
    /// many scenarios.
    /// </summary>
    public interface ISerializationProvider
    {
        /// <summary>
        /// Serilizes the object graph.
        /// </summary>
        /// <param name="value">The object graph to serialize.</param>
        /// <returns>The serialized object.</returns>
        byte[] Serialize(object value);

        /// <summary>
        /// Deserializes the raw bytes into the original object graph.
        /// </summary>
        /// <param name="bytes">The data to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        object Deserialize(byte[] bytes);
    }
}