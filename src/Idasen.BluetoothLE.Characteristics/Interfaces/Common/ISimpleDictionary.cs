using System.Collections.Generic ;

// ReSharper disable InvalidXmlDocComment

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Common
{
    /// <summary>
    ///     Represents a collection of keys and values.
    ///     The goal is to have a simple thread-safe dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the values in the dictionary.
    /// </typeparam>
    public interface ISimpleDictionary< TKey, TValue>
    {
        /// <summary>
        ///     Gets a cloned version of the dictionary.
        /// </summary>
        IReadOnlyDictionary<TKey, TValue> ReadOnlyDictionary { get ; }

        /// <summary>
        ///     Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key of the value to get or set.
        /// </param>
        /// <returns>
        ///     The value associated with the specified key. If the specified
        ///     key is not found, a get operation throws a KeyNotFoundException,
        ///     and a set operation creates a new element with the specified key.
        /// </returns>
        TValue this[TKey key] { get; set; }

        /// <summary>
        ///     Removes all keys and values from the Dictionary<TKey,TValue>.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Gets the number of key/value pairs contained in the
        ///     Dictionary<TKey,TValue>.
        /// </summary>
        int Count { get ; }

        /// <summary>
        ///     Gets a collection containing the keys in the Dictionary<TKey,TValue>.
        /// </summary>
        IEnumerable< string > Keys  { get ; }
    }
}