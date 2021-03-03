// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Tests;
using Xunit;

[assembly: JsonSerializable(typeof(PropertyVisibilityTests.MyClass_WithNonPublicAccessors))]
[assembly: JsonSerializable(typeof(PropertyVisibilityTests.MyClass_WithNonPublicAccessors_WithPropertyAttributes))]

namespace System.Text.Json.Serialization.Tests
{
    public abstract partial class PropertyVisibilityTests
    {
        [Fact]
        public async Task NonPublic_AccessorsNotSupported_WithoutAttribute()
        {
            string json = @"{
                ""MyInt"":1,
                ""MyString"":""Hello"",
                ""MyFloat"":2,
                ""MyUri"":""https://microsoft.com""
            }";

            var obj = await Deserializer.DeserializeWrapper<MyClass_WithNonPublicAccessors>(json);
            Assert.Equal(0, obj.MyInt);
            Assert.Null(obj.MyString);
            Assert.Equal(2f, obj.GetMyFloat);
            Assert.Equal(new Uri("https://microsoft.com"), obj.MyUri);

            json = await Serializer.SerializeWrapper(obj);
            Assert.Contains(@"""MyInt"":0", json);
            Assert.Contains(@"""MyString"":null", json);
            Assert.DoesNotContain(@"""MyFloat"":", json);
            Assert.DoesNotContain(@"""MyUri"":", json);
        }

        public class MyClass_WithNonPublicAccessors
        {
            public int MyInt { get; private set; }
            public string MyString { get; internal set; }
            public float MyFloat { private get; set; }
            public Uri MyUri { internal get; set; }

            // For test validation.
            internal float GetMyFloat => MyFloat;
        }

        [Fact]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task JsonSerializablePropertyAttribute_OnProperties_WithNonPublicAccessors()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new NotImplementedException();
        }

        public class MyClass_WithNonPublicAccessors_WithPropertyAttributes
        {
            [JsonInclude]
            public int MyInt { get; private set; }
            [JsonInclude]
            public string MyString { get; internal set; }
            [JsonInclude]
            public float MyFloat { private get; set; }
            [JsonInclude]
            public Uri MyUri { internal get; set; }

            // For test validation.
            internal float GetMyFloat => MyFloat;
        }

        private class MyClass_WithNonPublicAccessors_WithPropertyAttributes_And_PropertyIgnore
        {
            [JsonInclude]
            [JsonIgnore]
            public int MyInt { get; private set; }

            [JsonInclude]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string MyString { get; internal set; } = "DefaultString";

            [JsonInclude]
            [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public float MyFloat { private get; set; }

            [JsonInclude]
            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public Uri MyUri { internal get; set; }

            // For test validation.
            internal float GetMyFloat => MyFloat;
        }

        [Fact]
public async Task ExtensionDataCanHaveNonPublicSetter()
        {
            string json = @"{""Key"":""Value""}";

            // Baseline
            var obj1 = await Deserializer.DeserializeWrapper<ClassWithExtensionData_NonPublicSetter>(json);
            Assert.Null(obj1.ExtensionData);
            Assert.Equal("{}", await Serializer.SerializeWrapper(obj1));

            // With attribute
            var obj2 = await Deserializer.DeserializeWrapper<ClassWithExtensionData_NonPublicSetter_WithAttribute>(json);
            Assert.Equal("Value", obj2.ExtensionData["Key"].GetString());
            Assert.Equal(json, await Serializer.SerializeWrapper(obj2));
        }

        private class ClassWithExtensionData_NonPublicSetter
        {
            [JsonExtensionData]
            public Dictionary<string, JsonElement> ExtensionData { get; private set; }
        }

        private class ClassWithExtensionData_NonPublicSetter_WithAttribute
        {
            [JsonExtensionData]
            [JsonInclude]
            public Dictionary<string, JsonElement> ExtensionData { get; private set; }
        }

        private class ClassWithExtensionData_NonPublicGetter
        {
            [JsonExtensionData]
            public Dictionary<string, JsonElement> ExtensionData { internal get; set; }
        }

        [Fact]
public async Task HonorCustomConverter()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());

            string json = @"{""MyEnum"":""AnotherValue"",""MyInt"":2}";

            // Deserialization baseline, without enum converter, we get JsonException.
            await Assert.ThrowsAsync<JsonException>(async () => await Deserializer.DeserializeWrapper<StructWithPropertiesWithConverter>(json));

            var obj = await Deserializer.DeserializeWrapper<StructWithPropertiesWithConverter>(json, options);
            Assert.Equal(MySmallEnum.AnotherValue, obj.GetMyEnum);
            Assert.Equal(25, obj.MyInt);

            // ConverterForInt32 throws this exception.
            await Assert.ThrowsAsync<NotImplementedException>(async () => await Serializer.SerializeWrapper(obj, options));
        }

        private struct StructWithPropertiesWithConverter
        {
            [JsonInclude]
            public MySmallEnum MyEnum { private get; set; }

            [JsonInclude]
            [JsonConverter(typeof(ConverterForInt32))]
            public int MyInt { get; private set; }

            // For test validation.
            internal MySmallEnum GetMyEnum => MyEnum;
        }

        public enum MySmallEnum
        {
            DefaultValue = 0,
            AnotherValue = 1
        }

        [Fact]
public async Task HonorCaseInsensitivity()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            string json = @"{""MYSTRING"":""Hello""}";
            Assert.Null((await Deserializer.DeserializeWrapper<MyStruct_WithNonPublicAccessors_WithTypeAttribute>(json)).MyString);
            Assert.Equal("Hello", (await Deserializer.DeserializeWrapper<MyStruct_WithNonPublicAccessors_WithTypeAttribute>(json, options)).MyString);
        }

        public struct MyStruct_WithNonPublicAccessors_WithTypeAttribute
        {
            [JsonInclude]
            public int MyInt { get; private set; }
            [JsonInclude]
            public string MyString { get; internal set; }
            [JsonInclude]
            public float MyFloat { private get; set; }
            [JsonInclude]
            public Uri MyUri { internal get; set; }

            // For test validation.
            internal float GetMyFloat => MyFloat;
        }

        [Fact]
public async Task HonorNamingPolicy()
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = new SimpleSnakeCasePolicy() };

            string json = @"{""my_string"":""Hello""}";
            Assert.Null((await Deserializer.DeserializeWrapper<MyStruct_WithNonPublicAccessors_WithTypeAttribute>(json)).MyString);
            Assert.Equal("Hello", (await Deserializer.DeserializeWrapper<MyStruct_WithNonPublicAccessors_WithTypeAttribute>(json, options)).MyString);
        }

        [Fact]
#if GENERATE_JSON_METADATA
        [ActiveIssue("https://github.com/dotnet/runtimelab/projects/1#card-48716081")]
        // Need enum support
#endif
        public async Task HonorJsonPropertyName()
        {
            string json = @"{""prop1"":1,""prop2"":2}";

            var obj = await Deserializer.DeserializeWrapper<StructWithPropertiesWithJsonPropertyName>(json);
            Assert.Equal(MySmallEnum.AnotherValue, obj.GetMyEnum);
            Assert.Equal(2, obj.MyInt);

            json = await Serializer.SerializeWrapper(obj);
            Assert.Contains(@"""prop1"":1", json);
            Assert.Contains(@"""prop2"":2", json);
        }

        public struct StructWithPropertiesWithJsonPropertyName
        {
            [JsonInclude]
            [JsonPropertyName("prop1")]
            public MySmallEnum MyEnum { private get; set; }

            [JsonInclude]
            [JsonPropertyName("prop2")]
            public int MyInt { get; private set; }

            // For test validation.
            internal MySmallEnum GetMyEnum => MyEnum;
        }

        [Fact]
public async Task Map_JsonSerializableProperties_ToCtorArgs()
        {
            var obj = await Deserializer.DeserializeWrapper<PointWith_JsonSerializableProperties>(@"{""X"":1,""Y"":2}");
            Assert.Equal(1, obj.X);
            Assert.Equal(2, obj.GetY);
        }

        private struct PointWith_JsonSerializableProperties
        {
            [JsonInclude]
            public int X { get; internal set; }
            [JsonInclude]
            public int Y { internal get; set;  }

            internal int GetY => Y;

            [JsonConstructor]
            public PointWith_JsonSerializableProperties(int x, int y) => (X, Y) = (x, y);
        }

        [Fact]
public async Task Public_And_NonPublicPropertyAccessors_PropertyAttributes()
        {
            string json = @"{""W"":1,""X"":2,""Y"":3,""Z"":4}";

            var obj = await Deserializer.DeserializeWrapper<ClassWithMixedPropertyAccessors_PropertyAttributes>(json);
            Assert.Equal(1, obj.W);
            Assert.Equal(2, obj.X);
            Assert.Equal(3, obj.Y);
            Assert.Equal(4, obj.GetZ);

            json = await Serializer.SerializeWrapper(obj);
            Assert.Contains(@"""W"":1", json);
            Assert.Contains(@"""X"":2", json);
            Assert.Contains(@"""Y"":3", json);
            Assert.Contains(@"""Z"":4", json);
        }

        private class ClassWithMixedPropertyAccessors_PropertyAttributes
        {
            [JsonInclude]
            public int W { get; set; }
            [JsonInclude]
            public int X { get; internal set; }
            [JsonInclude]
            public int Y { get; set; }
            [JsonInclude]
            public int Z { private get; set; }

            internal int GetZ => Z;
        }

        [Theory]
        [InlineData(typeof(ClassWithPrivateProperty_WithJsonIncludeProperty))]
        [InlineData(typeof(ClassWithInternalProperty_WithJsonIncludeProperty))]
        [InlineData(typeof(ClassWithProtectedProperty_WithJsonIncludeProperty))]
        [InlineData(typeof(ClassWithPrivateField_WithJsonIncludeProperty))]
        [InlineData(typeof(ClassWithInternalField_WithJsonIncludeProperty))]
        [InlineData(typeof(ClassWithProtectedField_WithJsonIncludeProperty))]
        [InlineData(typeof(ClassWithPrivate_InitOnlyProperty_WithJsonIncludeProperty))]
        [InlineData(typeof(ClassWithInternal_InitOnlyProperty_WithJsonIncludeProperty))]
        [InlineData(typeof(ClassWithProtected_InitOnlyProperty_WithJsonIncludeProperty))]
        public static void NonPublicProperty_WithJsonInclude_Invalid(Type type)
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize("", type));
            string exAsStr = ex.ToString();
            Assert.Contains("MyString", exAsStr);
            Assert.Contains(type.ToString(), exAsStr);
            Assert.Contains("JsonIncludeAttribute", exAsStr);

            ex = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Serialize(Activator.CreateInstance(type), type));
            exAsStr = ex.ToString();
            Assert.Contains("MyString", exAsStr);
            Assert.Contains(type.ToString(), exAsStr);
            Assert.Contains("JsonIncludeAttribute", exAsStr);
        }

        private class ClassWithPrivateProperty_WithJsonIncludeProperty
        {
            [JsonInclude]
            private string MyString { get; set; }
        }

        private class ClassWithInternalProperty_WithJsonIncludeProperty
        {
            [JsonInclude]
            internal string MyString { get; }
        }

        private class ClassWithProtectedProperty_WithJsonIncludeProperty
        {
            [JsonInclude]
            protected string MyString { get; private set; }
        }

        private class ClassWithPrivateField_WithJsonIncludeProperty
        {
            [JsonInclude]
            private string MyString = null;

            public override string ToString() => MyString;
        }

        private class ClassWithInternalField_WithJsonIncludeProperty
        {
            [JsonInclude]
            internal string MyString = null;
        }

        private class ClassWithProtectedField_WithJsonIncludeProperty
        {
            [JsonInclude]
            protected string MyString = null;
        }

        private class ClassWithPrivate_InitOnlyProperty_WithJsonIncludeProperty
        {
            [JsonInclude]
            private string MyString { get; init; }
        }

        private class ClassWithInternal_InitOnlyProperty_WithJsonIncludeProperty
        {
            [JsonInclude]
            internal string MyString { get; init; }
        }

        private class ClassWithProtected_InitOnlyProperty_WithJsonIncludeProperty
        {
            [JsonInclude]
            protected string MyString { get; init; }
        }
    }
}
