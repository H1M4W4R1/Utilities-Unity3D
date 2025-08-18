using System;

namespace Systems.Utilities.Identifiers.Abstract
{
    /// <summary>
    ///     Marker interface representing identifier
    /// </summary>
    public interface IIdentifier<TIdentifier> : IIdentifier, IEquatable<TIdentifier>, IComparable<TIdentifier>
        where TIdentifier : struct, IIdentifier<TIdentifier>
    {
        
    }

    /// <summary>
    ///     Marker interface representing identifier, to be used in where clauses
    /// </summary>
    public interface IIdentifier
    {
        
    }
}