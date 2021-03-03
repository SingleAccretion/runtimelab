﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace System.Reflection
{
    class FieldInfoWrapper : FieldInfo
    {
        private readonly IFieldSymbol _field;
        private readonly MetadataLoadContext _metadataLoadContext;
        public FieldInfoWrapper(IFieldSymbol parameter, MetadataLoadContext metadataLoadContext)
        {
            _field = parameter;
            _metadataLoadContext = metadataLoadContext;
        }

        private FieldAttributes? _attributes;

        public override FieldAttributes Attributes
        {
            get
            {
                if (!_attributes.HasValue)
                {
                    _attributes = default(FieldAttributes);

                    if (_field.IsStatic)
                    {
                        _attributes |= FieldAttributes.Static;
                    }

                    switch (_field.DeclaredAccessibility)
                    {
                        case Accessibility.Public:
                            _attributes |= FieldAttributes.Public;
                            break;
                        case Accessibility.Private:
                            _attributes |= FieldAttributes.Private;
                            break;
                    }
                }

                return _attributes.Value;
            }
        }

        public override RuntimeFieldHandle FieldHandle => throw new NotImplementedException();

        public override Type FieldType => _field.Type.AsType(_metadataLoadContext);

        public override Type DeclaringType => _field.ContainingType.AsType(_metadataLoadContext);

        public override string Name => _field.Name;

        public override Type ReflectedType => throw new NotImplementedException();

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj)
        {
            throw new NotImplementedException();
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            var attributes = new List<CustomAttributeData>();
            foreach (AttributeData a in _field.GetAttributes())
            {
                attributes.Add(new CustomAttributeDataWrapper(a, _metadataLoadContext));
            }
            return attributes;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
