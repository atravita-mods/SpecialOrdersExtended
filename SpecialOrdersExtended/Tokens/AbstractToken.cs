﻿using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace SpecialOrdersExtended.Tokens
{
    internal abstract class AbstractToken
    {
        public List<string> SpecialOrdersCache = null;

        /// <summary>
        /// Whether or not the token allows input. Default, true
        /// </summary>
        /// <returns></returns>
        public virtual bool AllowsInput() { return true; }

        /// <summary>
        /// Whether or not the token will produce multiple outputs, depending on the input to the token
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Will return one value if given a Special Order, or all Special Orders if not</returns>
        public virtual bool CanHaveMultipleValues(string input = null)
        {
            return (input == null);
        }

        /// <summary>Get whether the token is available for use.</summary>
        public virtual bool IsReady()
        {
            return SpecialOrdersCache != null;
        }

        /// <summary>Validate that the provided input arguments are valid.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <param name="error">The validation error, if any.</param>
        /// <returns>Returns whether validation succeeded.</returns>
        /// <remarks>Default true.</remarks>
        public virtual bool TryValidateInput(string input, out string error)
        {
            error = "Expected zero arguments or single argument |contains=";
            string[] vals = input.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (vals.Length >=2 || (vals.Length == 1 && !vals[0].StartsWith("contains_"))){ return false; }
            return true;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public virtual IEnumerable<string> GetValues(string input)
        {

            ModEntry.ModMonitor.Log($"{input}, {string.Join(", ", SpecialOrdersCache)}", LogLevel.Debug);
            if (SpecialOrdersCache == null) { yield break; }
            else if  (input == null)
            {
                foreach (string specialorder in SpecialOrdersCache)
                {
                    yield return specialorder;
                }
            }
            else
            {
                yield return SpecialOrdersCache.Contains(input["|contains=".Length..]) ? "true" : "false" ;
            }
        }

        /// <summary>Get whether the token always chooses from a set of known values for the given input. Mutually exclusive with <see cref="HasBoundedRangeValues"/>.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <param name="allowedValues">The possible values for the input.</param>
        /// <remarks>Default unrestricted.</remarks>
        public virtual bool HasBoundedValues(string input, out IEnumerable<string> allowedValues)
        {
            allowedValues = new List<string>() { "true", "false" };
            if (input == null) {return false; }
            return true;
        }
    }
}