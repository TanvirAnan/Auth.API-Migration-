using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.ValueObjects;
public readonly record struct UserId
{
    public Guid Value { get; init; }

    [JsonConstructor]
    public UserId(Guid value) => Value = value;

    public static UserId New() => new(Guid.NewGuid());
    public static UserId FromGuid(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("UserId cannot be empty", nameof(value));
        return new UserId(value);
    }
    public static UserId FromString(string value)
    {
        if (!Guid.TryParse(value, out var guid)) throw new ArgumentException("Invalid guid format", nameof(value));
        return FromGuid(guid);
    }
    public override string ToString() => Value.ToString();
}
