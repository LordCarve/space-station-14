using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.Laws;

/// <summary>
/// Lawset data used internally.
/// </summary>
[DataDefinition, Serializable, NetSerializable]
public sealed partial class SiliconLawset
{
    /// <summary>
    /// List of ordered laws in this lawset.
    /// They will be automatically numbered based on the order they appear in the lawset.
    /// </summary>
    [DataField(required: true)]
    public List<SiliconLaw> Laws = []; //todo with builder - try to make private?

    /// <summary>
    /// What entity the lawset considers as a figure of authority.
    /// </summary>
    [DataField(required: true)]
    public string ObeysTo = string.Empty;

    /// <summary>
    /// Read laws together with their identifiers.
    /// Unless a law has an override, they will be autonumbered in their order.
    /// </summary>
    public List<LawInContext> ReadLawsetLaws()
    {
        var next = 1;
        var result = new List<LawInContext>(Laws.Count);

        for (var i = 0; i < Laws.Count; i++)
        {
            var law = Laws[i];
            var identifier = law.CustomIdentifier ?? $"{next}";
            result.Add(new(i, identifier, law));
            if (law.IncrementsAutonumbering)
                next++;
        }
        return result;
    }

    /// <summary>
    /// A single line used in logging laws.
    /// </summary>
    public string LoggingString()
    {
        var laws = new List<string>(Laws.Count);
        foreach (var law in ReadLawsetLaws())
        {
            laws.Add($"{law.LawIdentifier}: {Loc.GetString(law.Law.LawString)}");
        }

        return string.Join(" / ", laws);
    }

    public bool Equals(SiliconLawset? other)
    {
        if (other == null)
            return false;

        if (Laws.Count != other.Laws.Count)
            return false;

        for (var i = 0; i < Laws.Count; i++)
            if (!Laws[i].Equals(other.Laws[i]))
                return false;

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        return Equals(obj as SiliconLaw);
    }

    public override int GetHashCode()
    {
        var i = 1;
        foreach (var law in Laws)
            i = i * 31 + law.GetHashCode();
        return i;
    }
}

/// <summary>
/// Small helper record used to pass reference to a specific law within a lawset including its co-relations with the lawset.
/// </summary>
public sealed record LawInContext(int OrderInLawset, string LawIdentifier, SiliconLaw Law);

/// <summary>
/// This is a prototype for a <see cref="SiliconLawPrototype"/> list.
/// Used with <see cref="LawsetBuilder.FromPrototype"/>
/// </summary>
[Prototype]
public sealed partial class SiliconLawsetPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// The locstring of the lawset for the guidebook entry, if no name is provided, defaults to the ID
    /// </summary>    
    [DataField]
    public LocId? Name = null;

    /// <summary>
    /// List of core law prototype ids in this lawset.
    /// </summary>
    [DataField(required: true)]
    public List<ProtoId<SiliconLawPrototype>> Laws = [];

    /// <summary>
    /// What entity the lawset considers as a figure of authority.
    /// </summary>
    [DataField(required: true), ViewVariables(VVAccess.ReadWrite)]
    public string ObeysTo = string.Empty;
}