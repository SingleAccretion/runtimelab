﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Text.RegularExpressions.SRM
{
    /// <summary>
    /// Classifies characters into true or false
    /// </summary>
    internal class BooleanClassifier
    {
        //stores first 64 chars of ASCII
        private ulong _lower;
        //stores next 64 chars of ASCII
        private ulong _upper;
        //stores the remaining characters in a BDD
        private BDD _bdd;

        private BooleanClassifier(ulong lower, ulong upper, BDD bdd)
        {
            _lower = lower;
            _upper = upper;
            _bdd = bdd;
        }

        /// <summary>
        /// Create a Boolean classifier.
        /// </summary>
        /// <param name="solver">character algebra (the algebra is not stored in the classifier)</param>
        /// <param name="domain">elements that map to true</param>
        /// <returns></returns>
        internal static BooleanClassifier Create(CharSetSolver solver, BDD domain)
        {
            ulong lower = 0;
            ulong upper = 0;
            for (int i = 0; i < 64; i++)
                lower |= (domain.Contains(i) ? (ulong)1 << i : 0);
            for (int i = 64; i < 128; i++)
                upper |= (domain.Contains(i) ? (ulong)1 << (i - 64) : 0);
            //remove the ASCII characters from the domain if the domain is not everything
            BDD bdd = (domain.IsFull ? domain : solver.MkAnd(solver.nonascii, domain));
            return new BooleanClassifier(lower, upper, bdd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(ushort c) =>
            c < 64 ? ((_lower & ((ulong)1 << c)) != 0) : (c < 128 ? ((_upper & ((ulong)1 << (c - 64))) != 0) : _bdd.Contains(c));

        #region Serialization
        public void Serialize(StringBuilder sb)
        {
            //use comma to separate the elements, comma is not used in _bdd.Serialize
            sb.Append(Base64.Encode(_lower));
            sb.Append(',');
            sb.Append(Base64.Encode(_upper));
            sb.Append(',');
            _bdd.Serialize(sb);
        }

        public static BooleanClassifier Deserialize(string input, BDDAlgebra solver = null)
        {
            string[] parts = input.Split(',');
            if (parts.Length != 3)
                throw new ArgumentException($"{nameof(BooleanClassifier.Deserialize)} invalid '{nameof(input)}' parameter");

            ulong lower = Base64.DecodeUInt64(parts[0]);
            ulong upper = Base64.DecodeUInt64(parts[1]);
            BDD bdd = BDD.Deserialize(parts[2], solver);
            return new BooleanClassifier(lower, upper, bdd);
        }
        #endregion
    }
}
