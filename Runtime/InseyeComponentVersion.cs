// Module name: com.inseye.unity.sdk
// File name: InseyeComponentVersion.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;

namespace Inseye
{
    /// <summary>
    ///     Represents version of components in Inseye SDK.
    /// </summary>
    // ReSharper disable once RedundantNameQualifier
    public readonly struct InseyeComponentVersion : System.IEquatable<InseyeComponentVersion>
    {
        /// <summary>
        ///     Version major component. Changes in major version are braking from API point of view.
        /// </summary>
        public readonly int Major;

        /// <summary>
        ///     Version minor component. Changes in minor version introduce new features in non breaking way.
        /// </summary>
        public readonly int Minor;

        /// <summary>
        ///     Version patch component. Changes in patch version introduce bug fixes and minor improvements.
        /// </summary>
        public readonly int Patch;

        /// <summary>
        ///     Optional string identifier.
        /// </summary>
        public readonly string Extra;

        /// <summary>
        ///     Creates new version.
        /// </summary>
        /// <param name="major">Version major value. Must not be less than zero.</param>
        /// <param name="minor">Version minor value. Must not be less than zero.</param>
        /// <param name="patch">Version patch value. Must not be less than zero.</param>
        /// <param name="extra">Extra information, any string.</param>
        /// <exception cref="System.ArgumentException">Thrown when any value is breaking constrain.</exception>
        public InseyeComponentVersion(int major, int minor, int patch, string extra)
        {
            if (major < 0)
                throw new ArgumentException("Major version cannot be less than zero");
            if (minor < 0)
                throw new ArgumentException("Minor version cannot be less than zero");
            if (patch < 0)
                throw new ArgumentException("Patch version cannot be less than zero");
            Major = major;
            Minor = minor;
            Patch = patch;
            Extra = extra ?? "";
        }

        /// <summary>
        ///     Creates new version.
        /// </summary>
        /// <param name="major">Version major value.</param>
        /// <param name="minor">Version minor value.</param>
        /// <param name="patch">Version patch value.</param>
        /// <exception cref="System.ArgumentException">Thrown when any numeric value of the version is less than zero</exception>
        public InseyeComponentVersion(int major, int minor, int patch) : this(major, minor, patch, string.Empty)
        {
        }

        /// <summary>
        ///     Parses string in format {major}.{minor}.{patch} or {major}.{minor}.{patch}-{extra}.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Component version.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when input string is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when input string is empty.</exception>
        /// <exception cref="System.FormatException">Thrown when input string is in invalid format.</exception>
        public static InseyeComponentVersion Parse(string str)
        {
            if (str is null)
                throw new ArgumentNullException(nameof(str));
            if (str.Length == 0)
                throw new ArgumentException("String is empty");
            return Parse(str.AsSpan());
        }

        /// <summary>
        ///     Parses span of characters in format {major}.{minor}.{patch} or {major}.{minor}.{patch}-{extra}.
        /// </summary>
        /// <param name="charSpan">Span of characters to parse.</param>
        /// <returns>Component version.</returns>
        /// <exception cref="System.ArgumentException">Thrown when input span is empty.</exception>
        /// <exception cref="System.FormatException">Thrown when input span is in invalid format</exception>
        // ReSharper disable once RedundantNameQualifier
        public static InseyeComponentVersion Parse(System.ReadOnlySpan<char> charSpan)
        {
            if (charSpan.IsEmpty)
                throw new ArgumentException("Span is empty");
            var extra = "";
            var substringStart = 0;
            var index = 0;
            // major
            while (index < charSpan.Length && charSpan[index] != '.') index++;
            if (!int.TryParse(charSpan[..index], out var major))
                throw new FormatException(
                    $"Failed to parse {charSpan.ToString()} as {nameof(InseyeComponentVersion)} at major version step.");

            if (index == charSpan.Length)
                return new InseyeComponentVersion(1, 0, 0, string.Empty);
            substringStart = ++index;
            // minor
            while (index < charSpan.Length && charSpan[index] != '.') index++;
            if (!int.TryParse(charSpan.Slice(substringStart, index - substringStart), out var minor))
                throw new FormatException(
                    $"Failed to parse {charSpan.ToString()} as {nameof(InseyeComponentVersion)} at minor version step.");
            if (index == charSpan.Length)
                return new InseyeComponentVersion(major, minor, 0, string.Empty);
            substringStart = ++index;
            // patch
            while (index < charSpan.Length && charSpan[index] != '-') index++;
            if (!int.TryParse(charSpan.Slice(substringStart, index - substringStart), out var patch))
                throw new FormatException(
                    $"Failed to parse {charSpan.ToString()} as {nameof(InseyeComponentVersion)} at patch version");
            if (index == charSpan.Length)
                return new InseyeComponentVersion(major, minor, patch, string.Empty);
            // extra
            if (charSpan[index] == '-')
            {
                substringStart = ++index;
                extra = charSpan.Slice(substringStart, charSpan.Length - substringStart).ToString();
            }

            return new InseyeComponentVersion(major, minor, patch, extra);
        }

        /// <summary>
        ///     Overload returning well formatted version representation that is parsable.
        /// </summary>
        /// <returns>String in format {major}.{minor}.{patch}[-{extra}]</returns>
        public override string ToString()
        {
            var str = $"{Major}.{Minor}.{Patch}";
            if (!string.IsNullOrEmpty(Extra))
                str += $"-{Extra}";
            return str;
        }

        /// <summary>
        ///     Checks version equality.
        /// </summary>
        /// <param name="other">other component version</param>
        /// <returns>True if versions are equal.</returns>
        public bool Equals(InseyeComponentVersion other)
        {
            return this == other;
        }

        /// <summary>
        ///     Casts object to <see cref="InseyeComponentVersion" /> and then compares it.
        /// </summary>
        /// <param name="obj">Any object.</param>
        /// <returns>True if argument is and <see cref="InseyeComponentVersion" /> and it's values are equal to this.</returns>
        public override bool Equals(object obj)
        {
            return obj is InseyeComponentVersion other && Equals(other);
        }

        /// <summary>
        ///     Computes version hash value.
        /// </summary>
        /// <returns>Combined hash value of Major, Minor, Patch and Extra fields.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, Extra);
        }

        /// <summary>
        ///     Lesser than operator.
        /// </summary>
        /// <param name="first">First argument.</param>
        /// <param name="second">Second argument.</param>
        /// <returns>True if first argument version is lesser than second.</returns>
        public static bool operator <(InseyeComponentVersion first, InseyeComponentVersion second)
        {
            if (first.Major < second.Major)
                return true;
            if (first.Major > second.Major)
                return false;
            // major are equal
            if (first.Minor < second.Minor)
                return true;
            if (first.Minor > second.Minor)
                return false;
            // minor are equal
            return first.Patch < second.Patch;
        }

        /// <summary>
        ///     Greater than operator.
        /// </summary>
        /// <param name="first">First argument.</param>
        /// <param name="second">Second argument.</param>
        /// <returns>True if first argument version is greater than second.</returns>
        public static bool operator >(InseyeComponentVersion first, InseyeComponentVersion second)
        {
            if (first.Major > second.Major)
                return true;
            if (first.Major < second.Major)
                return false;
            // major are equal
            if (first.Minor > second.Minor)
                return true;
            if (first.Minor < second.Minor)
                return false;
            // minor are equal
            return first.Patch > second.Patch;
        }

        /// <summary>
        ///     Equality operator.
        ///     Extra are not compared in this operator.
        /// </summary>
        /// <param name="first">First argument.</param>
        /// <param name="second">Second argument.</param>
        /// <returns>True if first and second argument are equal.</returns>
        public static bool operator ==(InseyeComponentVersion first, InseyeComponentVersion second)
        {
            return first.Major == second.Major && first.Minor == second.Minor && first.Patch == second.Patch;
        }

        /// <summary>
        ///     Equality operator.
        ///     Extra are not compared in this operator.
        /// </summary>
        /// <param name="first">First argument.</param>
        /// <param name="second">Second argument.</param>
        /// <returns>True if first and second argument are not equal.</returns>
        public static bool operator !=(InseyeComponentVersion first, InseyeComponentVersion second)
        {
            return !(first == second);
        }
    }
}