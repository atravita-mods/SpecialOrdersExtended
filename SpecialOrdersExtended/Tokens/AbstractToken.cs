﻿namespace SpecialOrdersExtended.Tokens;

/// <summary>
/// Abstract token that keeps a cache that's a list of strings
/// And can return either the list
/// or if any item exists in the list.
/// </summary>
internal abstract class AbstractToken
{
    /// <summary>
    /// Internal cache for token. Will be null if not ready.
    /// </summary>
    private List<string>? tokenCache = null;

    /// <summary>
    /// Whether or not the token allows input. Default, true.
    /// </summary>
    /// <returns>true - all derived tokens should allow input.</returns>
    [Pure]
    public virtual bool AllowsInput() => true;

    /// <summary>
    /// Whether or not the token will produce multiple outputs, depending on the input to the token.
    /// </summary>
    /// <param name="input"></param>
    /// <returns>Will return one value if given a Special Order, or all Special Orders if not.</returns>
    [Pure]
    public virtual bool CanHaveMultipleValues(string? input = null) => input is null;

    /// <summary>Get whether the token is available for use.</summary>
    /// <returns>True if token ready, false otherwise.</returns>
    [Pure]
    public virtual bool IsReady() => this.tokenCache is not null;

    /// <summary>Validate that the provided input arguments are valid.</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <param name="error">The validation error, if any.</param>
    /// <returns>Returns whether validation succeeded.</returns>
    /// <remarks>Expect zero arguments or single argument |contains=.</remarks>
    public virtual bool TryValidateInput(string input, out string error)
    {
        error = "Expected zero arguments or single argument |contains=";
        string[] vals = input.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (vals.Length >= 2 || (vals.Length == 1 && !vals[0].StartsWith("contains_")))
        {
            return false;
        }
        return true;
    }

    /// <summary>Get the current values.</summary>
    /// <param name="input">The input arguments, if applicable.</param>
    /// <returns>Values for the token, if any.</returns>
    public virtual IEnumerable<string> GetValues(string input)
    {
        if (this.tokenCache is null)
        {
            yield break;
        }
        else if (input is null)
        {
            foreach (string str in this.tokenCache)
            {
                yield return str;
            }
        }
        else
        {
            yield return this.tokenCache.Contains(input["|contains=".Length..]) ? "true" : "false";
        }
    }

    /// <summary>Get whether the token always chooses from a set of known values for the given input. Mutually exclusive with <see cref="HasBoundedRangeValues"/>.</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <param name="allowedValues">The possible values for the input.</param>
    /// <returns>True if the inputs are bounded, false otherwise.</returns>
    public virtual bool HasBoundedValues(string input, out IEnumerable<string> allowedValues)
    {
        allowedValues = new List<string>() { "true", "false" };
        if (input is null)
        {
            return false;
        }
        return true;
    }

    /// <summary>Update the values when the context changes.</summary>
    /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
    public abstract bool UpdateContext();

    /// <summary>
    /// Checks a List of strings against the cache, updates the cache if necessary.
    /// </summary>
    /// <param name="newValues">The new values for the token.</param>
    /// <returns>true if cache updated, false otherwise.</returns>
    protected bool UpdateCache(List<string>? newValues)
    {
        if (newValues == this.tokenCache)
        {
            return false;
        }
        else
        {
            this.tokenCache = newValues;
            return true;
        }
    }
}
