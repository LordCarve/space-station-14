using Robust.Shared.Serialization;
using Content.Shared.Silicons.Laws.LawFormats;
using Robust.Shared.Prototypes;

namespace Content.Shared.Silicons.Laws;

[Virtual, DataDefinition]
[Serializable, NetSerializable]
public partial class SiliconLaw : IEquatable<SiliconLaw>
{
    /// <summary>
    /// A locale string which is the source-of-truth for the verbatim text of this law.
    /// Its format can be modified. See <see cref="LawFormat"/>.
    /// </summary>
    [DataField(required: true)]
    public string LawString = string.Empty;

    /// <summary>
    /// How the printed law is presented to the player.
    /// Must never affect the verbatim meaning of the law.
    /// </summary>
    [DataField]
    public ProtoId<LawFormatPrototype> LawFormat = "DefaultLawFormat";

    /// <summary>
    /// Whether this law is ignored for the purposes of auto-numbering laws in a lawset.
    /// Specifically, the number of the law after this one will be that of the law before it +1, rather than +2.
    /// Used optionally together with <see cref="CustomIdentifier"/>.
    /// Ignored if <see cref="CustomIdentifier"/> is not set. This is to disallow two laws with same ID in one lawset.
    /// </summary>
    [DataField]
    public bool AutonumberingExempt = false;

    /// <summary>
    /// Optional custom identifier that overrides the by-default number automatically assigned this law in a lawset.
    /// Useful for corrupted (or otherwise irregular) lawsets.
    /// Can be used together with <see cref="AutonumberingExempt"/> to leave original ordering unaffected.
    /// </summary>
    [DataField]
    public string? CustomIdentifier = null;

    /// <summary>
    /// Does this law contribute to incrementing the autonumbering of law identifiers on its lawset.
    /// </summary>
    public bool IncrementsAutonumbering => CustomIdentifier is null || !AutonumberingExempt;

    public bool Equals(SiliconLaw? other)
    {
        if (other == null)
            return false;
        return LawString == other.LawString
               && AutonumberingExempt == other.AutonumberingExempt
               && CustomIdentifier == other.CustomIdentifier
               && LawFormat == other.LawFormat;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        return Equals(obj as SiliconLaw);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LawString, LawFormat, AutonumberingExempt, CustomIdentifier);
    }

    /// <summary>
    /// Return a shallow clone of this law.
    /// </summary>
    public SiliconLaw ShallowClone()
    {
        return new SiliconLaw()
        {
            LawString = LawString,
            LawFormat = LawFormat,
            AutonumberingExempt = AutonumberingExempt,
            CustomIdentifier = CustomIdentifier
        };
    }
}

/// <summary>
/// This is a prototype for a law governing the behavior of silicons.
/// </summary>
[Prototype]
public sealed partial class SiliconLawPrototype : SiliconLaw, IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;
}
