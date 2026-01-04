using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared.Silicons.Laws;

/// <summary>
/// Builder pattern for <see cref="SiliconLawset"/>s to make lawset changes simple and modular.
/// Accepts either a prototype or an existing instance as the starting lawset and exposes simple modification methods.
/// After making the desired changes invoke <see cref="Build()"/> to get the built lawset.
/// </summary>
public sealed class LawsetBuilder
{
    public List<SiliconLaw> Laws { get; private init; } = new List<SiliconLaw>(0);

    public string ObeysTo { get; private init; } = string.Empty;

    public static LawsetBuilder Empty() => new()
    {
        Laws = [],
        ObeysTo = string.Empty
    };

    /// <summary>
    /// Initialize LawsetBuilder using the provided SiliconLawset Prototype as the base.
    /// </summary>
    public static LawsetBuilder FromPrototype(ProtoId<SiliconLawsetPrototype> protoId, IPrototypeManager prototypeManager)
    {
        var prototype = prototypeManager.Index(protoId);
        return new()
        {
            Laws = Scaffold(prototype.Laws, prototypeManager),
            ObeysTo = prototype.ObeysTo
        };
    }

    /// <summary>
    /// Initialize LawsetBuilder using the provided SiliconLawset instance as the base.
    /// </summary>
    public static LawsetBuilder FromInstance(SiliconLawset lawset) => new()
    {
        Laws = Scaffold(lawset.Laws),
        ObeysTo = lawset.ObeysTo
    };

    /// <summary>
    /// Adds a single new law literal by appending it to the existing lawset.
    /// </summary>
    public LawsetBuilder AddLaw(SiliconLaw law)
    {
        Laws.Add(Scaffold(law));
        return this;
    }

    /// <summary>
    /// Adds a single new law by indexing its prototype and appending it to the existing lawset.
    /// </summary>
    public LawsetBuilder AddLaw(ProtoId<SiliconLawPrototype> prototype, IPrototypeManager prototypeManager, NewLawOptions? options = null)
    {
        Laws.Add(Scaffold(prototype, prototypeManager, options));
        return this;
    }

    /// <summary>
    /// Inserts a single new law literal at a given position of the existing lawset.
    /// Other laws' numbering will change if added before other laws and the new law is not <see cref="SiliconLaw.AutonumberingExempt"/>.
    /// </summary>
    public LawsetBuilder InsertLaw(int index, SiliconLaw law)
    {
        DebugTools.Assert(Laws.Count > index, $"Index of law to insert out of range: {index}.");
        Laws.Insert(index, Scaffold(law));
        return this;
    }

    /// <summary>
    /// Inserts a single new law by indexing its prototype and inserting it at a given position of the existing lawset.
    /// Other laws' numbering will change if added before other laws and the new law is not <see cref="SiliconLaw.AutonumberingExempt"/>.
    /// </summary>
    public LawsetBuilder InsertLaw(int index, ProtoId<SiliconLawPrototype> prototype, IPrototypeManager prototypeManager, NewLawOptions? options = null)
    {
        DebugTools.Assert(Laws.Count > index, $"Index of law to insert out of range: {index}.");
        Laws.Insert(index, Scaffold(prototype, prototypeManager, options));
        return this;
    }

    /// <summary>
    /// Removes a single law at random. No action if there are no laws.
    /// </summary>
    public LawsetBuilder RemoveLaw(IRobustRandom robustRandom)
    {
        if (Laws.Count > 0)
        {
            var indexOfLawToRemove = robustRandom.Next(Laws.Count);
            Laws.RemoveAt(indexOfLawToRemove);
        }
        return this;
    }

    /// <summary>
    /// Removes a specific law indicated by the provided index.
    /// </summary>
    public LawsetBuilder RemoveLaw(int index)
    {
        DebugTools.Assert(Laws.Count > index, $"Index of law to remove out of range: {index}.");
        Laws.RemoveAt(index);
        return this;
    }

    /// <summary>
    /// Replaces a random law with a specified new literal one. No action if there are no laws.
    /// </summary>
    public LawsetBuilder ReplaceLaw(IRobustRandom robustRandom, SiliconLaw replacement)
    {
        if (Laws.Count > 0)
        {
            var indexOfLawToReplace = robustRandom.Next(Laws.Count);
            Laws.RemoveAt(indexOfLawToReplace);
            Laws.Insert(indexOfLawToReplace, Scaffold(replacement));
        }
        return this;
    }

    /// <summary>
    /// Replaces a specific law indicated by the provided index with a specified new literal one.
    /// </summary>
    public LawsetBuilder ReplaceLaw(int indexOfLawToReplace, SiliconLaw replacement)
    {
        DebugTools.Assert(Laws.Count > indexOfLawToReplace, $"Index of law to replace out of range: {indexOfLawToReplace}.");
        Laws.RemoveAt(indexOfLawToReplace);
        Laws.Insert(indexOfLawToReplace, Scaffold(replacement));
        return this;
    }

    /// <summary>
    /// Replaces a random law with a new indexed from the provided prototype. No action if there are no laws.
    /// </summary>
    public LawsetBuilder ReplaceLaw(IRobustRandom robustRandom, ProtoId<SiliconLawPrototype> replacement, IPrototypeManager prototypeManager, NewLawOptions? options = null)
    {
        if (Laws.Count > 0)
        {
            var indexOfLawToReplace = robustRandom.Next(Laws.Count);
            Laws.RemoveAt(indexOfLawToReplace);
            Laws.Insert(indexOfLawToReplace, Scaffold(replacement, prototypeManager, options));
        }
        return this;
    }

    /// <summary>
    /// Replaces a specific law indicated by the provided index with a new indexed from the provided prototype.
    /// </summary>
    public LawsetBuilder ReplaceLaw(int indexOfLawToReplace, ProtoId<SiliconLawPrototype> replacement, IPrototypeManager prototypeManager, NewLawOptions? options = null)
    {
        DebugTools.Assert(Laws.Count > indexOfLawToReplace, $"Index of law to replace out of range: {indexOfLawToReplace}.");
        Laws.RemoveAt(indexOfLawToReplace);
        Laws.Insert(indexOfLawToReplace, Scaffold(replacement, prototypeManager, options));
        return this;
    }

    /// <summary>
    /// Shuffles the order of laws in the lawset.
    /// This will affect the laws' identifiers if they are autonumbered.
    /// </summary>
    public LawsetBuilder Shuffle(IRobustRandom robustRandom)
    {
        robustRandom.Shuffle(Laws);
        return this;
    }

    /// <summary>
    /// Builds a resulting <see cref="SiliconLawset"/> instance from the instructions provided to the builder.
    /// </summary>
    public SiliconLawset Build() => new()
    {
        Laws = Laws,
        ObeysTo = ObeysTo
    };

    [return: NotNullIfNotNull(nameof(law))]
    private static SiliconLaw? Scaffold(SiliconLaw? law)
    {
        if (law is null)
            return null;
        return law.ShallowClone();
    }

    private static List<SiliconLaw> Scaffold(List<SiliconLaw> laws)
    {
        var scaffolding = new List<SiliconLaw>(laws.Count);
        foreach (var law in laws)
        {
            scaffolding.Add(law.ShallowClone());
        }
        return scaffolding;
    }

    [return: NotNullIfNotNull(nameof(protoId))]
    private static SiliconLaw? Scaffold(ProtoId<SiliconLawPrototype>? protoId, IPrototypeManager prototypeManager, NewLawOptions? options = null)
    {
        if (protoId is null)
            return null;
        var prototype = prototypeManager.Index(protoId);
        var scaffolding = prototype.ShallowClone();

        if (options?.ApplyLocstringParamsToLawString is not null)
        {
            scaffolding.LawString = Loc.GetString(scaffolding.LawString, options.ApplyLocstringParamsToLawString.ToArray());
        }

        return scaffolding;
    }

    private static List<SiliconLaw> Scaffold(List<ProtoId<SiliconLawPrototype>> protoIds, IPrototypeManager prototypeManager, NewLawOptions? options = null)
    {
        var scaffolding = new List<SiliconLaw>(protoIds.Count);
        foreach (var protoId in protoIds)
        {
            var prototype = prototypeManager.Index(protoId);
            var scaffoldingItem = prototype.ShallowClone();

            if (options?.ApplyLocstringParamsToLawString is not null)
            {
                scaffoldingItem.LawString = Loc.GetString(scaffoldingItem.LawString, options.ApplyLocstringParamsToLawString.ToArray());
            }

            scaffolding.Add(scaffoldingItem);
        }
        return scaffolding;
    }

    // Zero and negative integers are allowed.
    private bool StringIsPositiveInteger(string toValidate)
    {
        return int.TryParse(toValidate, out var parsed) && parsed > 0;
    }

    /// <summary>
    /// Additional options that can be optionally passed to builder methods adding laws from prototypes.
    /// </summary>
    public sealed record NewLawOptions
    {
        /// <summary>
        /// Parameters to apply to locstring upon scaffolding the new law.
        /// </summary>
        public List<(string, object)>? ApplyLocstringParamsToLawString { get; init; } = null;
    }
}
