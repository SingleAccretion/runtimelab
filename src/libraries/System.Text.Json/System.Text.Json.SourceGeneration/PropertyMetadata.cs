﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace System.Text.Json.SourceGeneration
{
    [DebuggerDisplay("Name={Name}, Type={TypeMetadata}")]
    internal class PropertyMetadata
    {
        /// <summary>
        /// The CLR name of the property.
        /// </summary>
        public string ClrName { get; init; }

        /// <summary>
        /// What kind of <see cref="MemberInfo"/> is this?
        /// </summary>
        public MemberTypes MemberType { get; init; }

        /// <summary>
        /// The property name specified via JsonPropertyNameAttribute, if available.
        /// </summary>
        public string? JsonPropertyName { get; init; }

        /// <summary>
        /// Whether the property has a public or internal (only usable when JsonIncludeAttribute is specified)
        /// getter that can be referenced in generated source code.
        /// </summary>
        public bool HasGetter { get; init; }

        /// <summary>
        /// Whether the property has a public or internal (only usable when JsonIncludeAttribute is specified)
        /// setter that can be referenced in generated source code.
        /// </summary>
        public bool HasSetter { get; init; }

        public bool GetterIsVirtual { get; init; }

        public bool SetterIsVirtual { get; init; }

        /// <summary>
        /// The <see cref="JsonIgnoreCondition"/> for the property.
        /// </summary>
        public JsonIgnoreCondition? IgnoreCondition { get; init; }

        /// <summary>
        /// The <see cref="JsonNumberHandling"/> for the property.
        /// </summary>
        public JsonNumberHandling? NumberHandling { get; init; }

        /// <summary>
        /// Whether the property has the JsonIncludeAttribute. If so, non-public accessors can be used for (de)serialziation.
        /// </summary>
        public bool HasJsonInclude { get; init; }

        /// <summary>
        /// Metadata for the property's type.
        /// </summary>
        public TypeMetadata TypeMetadata { get; init; }

        /// <summary>
        /// Compilable name of the property's declaring type.
        /// </summary>
        public string DeclaringTypeCompilableName { get; init; }
    }
}
