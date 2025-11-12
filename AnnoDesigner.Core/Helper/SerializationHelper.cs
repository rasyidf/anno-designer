using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AnnoDesigner.Core.Helper;

public static class SerializationHelper
{
    private static readonly IFileSystem _fileSystem;
    private static readonly JsonConverter[] _converter;
    static SerializationHelper()
    {
        _converter = [new VersionConverter(), new IsoDateTimeConverter(), new StringEnumConverter()];
        _fileSystem = new FileSystem();
    }
    private static JsonSerializer GetSerializer()
    {
        JsonSerializer serializer = new();
        foreach (JsonConverter curConverter in _converter)
        {
            serializer.Converters.Add(curConverter);
        }

        return serializer;
    }

    /// <summary>
    /// Serializes the given object to JSON and writes it to the given file.
    /// </summary>
    /// <typeparam name="T">type of the object being serialized</typeparam>
    /// <param name="obj">object to serialize</param>
    /// <param name="filename">output JSON filename</param>
    public static void SaveToFile<T>(T obj, string filename)
    {
        _fileSystem.File.WriteAllText(filename, SaveToJsonString(obj));
    }

    /// <summary>
    /// Serializes the given object to JSON and writes it to the given <see cref="Stream"/>.
    /// </summary>
    /// <typeparam name="T">type of the object being serialized</typeparam>
    /// <param name="obj">object to serialize</param>
    /// <param name="stream">output JSON stream</param>
    public static void SaveToStream<T>(T obj, Stream stream)
    {
        JsonSerializer serializer = GetSerializer();
        using StreamWriter sw = new(stream, Encoding.UTF8, 1024, true);//use constructor that does not close base stream
        using JsonTextWriter jsonWriter = new(sw);
        serializer.Serialize(jsonWriter, obj, typeof(T));
        jsonWriter.Flush();
    }


    /// <summary>
    /// Serializes the given object to JSON and returns it as a <see cref="string"/>.
    /// </summary>
    /// <typeparam name="T">type of the object being serialized</typeparam>
    /// <param name="obj">object to serialize</param>
    public static string SaveToJsonString<T>(T obj, Formatting formatting = Formatting.None)
    {
        return JsonConvert.SerializeObject(obj, formatting, _converter);
    }

    /// <summary>
    /// Deserializes the given JSON file to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">type of object being deserialized</typeparam>
    /// <param name="filename">input JSON filename</param>
    /// <returns>deserialized object</returns>
    public static T LoadFromFile<T>(string filename)
    {
        string fileContents = _fileSystem.File.ReadAllText(filename);
        return LoadFromJsonString<T>(fileContents);
    }

    /// <summary>
    /// Deserializes the given JSON stream to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">type of object being deserialized</typeparam>
    /// <param name="stream">input JSON stream</param>
    /// <returns>deserialized object</returns>
    public static T LoadFromStream<T>(Stream stream)
    {
        JsonSerializer serializer = GetSerializer();
        using StreamReader sr = new(stream);
        using JsonTextReader jsonReader = new(sr);
        return serializer.Deserialize<T>(jsonReader);
    }

    /// <summary>
    /// Deserializes the given JSON string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">type of object being deserialized</typeparam>
    /// <param name="jsonString">JSON string to deserialize</param>
    /// <exception cref="Newtonsoft.Json.JsonSerializationException">If <paramref name="jsonString"/> 
    /// is null or empty, or the json is not valid for the given object.</exception>
    /// <returns>deserialized object</returns>
    public static T LoadFromJsonString<T>(string jsonString)
    {
        return string.IsNullOrWhiteSpace(jsonString) ? default : JsonConvert.DeserializeObject<T>(jsonString, _converter);
    }

    /// <summary>
    /// Legacy deserialization method for deserializing layout files <see cref="CoreConstants.LayoutFileVersion"/> 3 or older.
    /// </summary>
    /// <typeparam name="T">type of object being deserialized</typeparam>
    /// <param name="jsonString">JSON string to deserialize</param>
    /// <returns>deserialized object</returns>
    public static T LoadFromJsonStringLegacy<T>(string jsonString)
    {
        using MemoryStream ms = new(Encoding.UTF8.GetBytes(jsonString));
        DataContractJsonSerializer serializer = new(typeof(T));
        return (T)serializer.ReadObject(ms);
    }
}
