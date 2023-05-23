﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using ILCompiler;
using ILCompiler.DependencyAnalysis;

using Internal.IL;
using Internal.Text;
using Internal.TypeSystem;
using Internal.TypeSystem.Ecma;

[assembly: InternalsVisibleTo("ILCompiler.LLVM")]

namespace Internal.JitInterface
{
    internal sealed unsafe partial class CorInfoImpl
    {
        private static readonly void*[] s_jitExports = new void*[(int)JitApiId.Count + 1];

        private void* _pNativeContext; // Per-thread context pointer. Used by the Jit; opaque to the EE.

        [UnmanagedCallersOnly]
        public static void addCodeReloc(IntPtr thisHandle, void* handle)
        {
            var _this = GetThis(thisHandle);
            ISymbolNode node = (ISymbolNode)_this.HandleToObject(handle);

            _this._codeRelocs.Add(new Relocation(RelocType.IMAGE_REL_BASED_REL32, 0, node));
        }

        // So the char* in cpp is terminated.
        private static byte[] AppendNullByte(byte[] inputArray)
        {
            byte[] nullTerminated = new byte[inputArray.Length + 1];
            inputArray.CopyTo(nullTerminated, 0);
            nullTerminated[inputArray.Length] = 0;
            return nullTerminated;
        }

        [UnmanagedCallersOnly]
        public static byte* getMangledMethodName(IntPtr thisHandle, CORINFO_METHOD_STRUCT_* ftn)
        {
            var _this = GetThis(thisHandle);
            MethodDesc method = _this.HandleToObject(ftn);
            Utf8String mangledName = _this._compilation.NameMangler.GetMangledMethodName(method);

            return (byte*)_this.GetPin(AppendNullByte(mangledName.UnderlyingArray));
        }

        [UnmanagedCallersOnly]
        public static byte* getMangledSymbolName(IntPtr thisHandle, void* symbolHandle)
        {
            var _this = GetThis(thisHandle);
            var node = (ISymbolNode)_this.HandleToObject(symbolHandle);

            Utf8StringBuilder sb = new Utf8StringBuilder();
            node.AppendMangledName(_this._compilation.NameMangler, sb);

            sb.Append("\0");
            return (byte*)_this.GetPin(sb.UnderlyingArray);
        }

        [UnmanagedCallersOnly]
        public static int getSignatureForMethodSymbol(IntPtr thisHandle, void* symbolHandle, CORINFO_SIG_INFO* pSig)
        {
            var _this = GetThis(thisHandle);
            var node = (ISymbolNode)_this.HandleToObject(symbolHandle);

            MethodDesc method = null;
            if (node is IMethodNode { Offset: 0 } methodNode)
            {
                method = methodNode.Method;
            }
            else if (node is ReadyToRunHelperNode { Id: ReadyToRunHelperId.VirtualCall } helperNode)
            {
                method = (MethodDesc)helperNode.Target;
            }

            if (method != null)
            {
                _this.Get_CORINFO_SIG_INFO(method, pSig, scope: null);
                if (method.IsUnmanagedCallersOnly || node is RuntimeImportMethodNode)
                {
                    pSig->callConv = CorInfoCallConv.CORINFO_CALLCONV_UNMANAGED;
                }

                return 1;
            }

            return 0;
        }

        [UnmanagedCallersOnly]
        public static uint isRuntimeImport(IntPtr thisHandle, CORINFO_METHOD_STRUCT_* ftn)
        {
            var _this = GetThis(thisHandle);

            MethodDesc method = _this.HandleToObject(ftn);

            return method.HasCustomAttribute("System.Runtime", "RuntimeImportAttribute") ? 1u : 0u; // bool is not blittable in .net5 so use uint, TODO: revert to bool for .net 6 (https://github.com/dotnet/runtime/issues/51170)
        }

        [UnmanagedCallersOnly]
        public static CorInfoType getPrimitiveTypeForTrivialWasmStruct(IntPtr thisHandle, CORINFO_CLASS_STRUCT_* structHnd)
        {
            var _this = GetThis(thisHandle);
            TypeDesc structType = _this.HandleToObject(structHnd);
            if (_this._compilation.GetPrimitiveTypeForTrivialWasmStruct(structType) is TypeDesc primitiveType)
            {
                return _this.asCorInfoType(primitiveType);
            }

            return CorInfoType.CORINFO_TYPE_UNDEF;
        }

        [UnmanagedCallersOnly]
        public static uint padOffset(IntPtr thisHandle, CORINFO_CLASS_STRUCT_* structHnd, uint atOffset)
        {
            var _this = GetThis(thisHandle);
            TypeDesc type = _this.HandleToObject(structHnd);

            return (uint)_this._compilation.PadOffset(type, (int)atOffset);
        }

        [UnmanagedCallersOnly]
        public static byte* getAlternativeFunctionName(IntPtr thisHandle)
        {
            var _this = GetThis(thisHandle);
            IMethodNode methodNode = _this._methodCodeNode;
            RyuJitCompilation compilation = _this._compilation;

            string alternativeName = compilation.GetRuntimeExportManagedEntrypointName(methodNode.Method) ?? compilation.NodeFactory.GetSymbolAlternateName(methodNode);
            if ((alternativeName == null) && methodNode.Method.IsUnmanagedCallersOnly)
            {
                // TODO-LLVM: delete once the IL backend is gone.
                alternativeName = methodNode.Method.Name;
            }

            return (alternativeName != null) ? (byte*)_this.GetPin(StringToUTF8(alternativeName)) : null;
        }

        [UnmanagedCallersOnly]
        public static IntPtr getExternalMethodAccessor(IntPtr thisHandle, CORINFO_METHOD_STRUCT_* methodHandle, TargetAbiType* sig, int sigLength)
        {
            CorInfoImpl _this = GetThis(thisHandle);
            MethodDesc method = _this.HandleToObject(methodHandle);
            ISymbolNode accessorNode = _this._compilation.GetExternalMethodAccessor(method, new ReadOnlySpan<TargetAbiType>(sig, sigLength));

            return _this.ObjectToHandle(accessorNode);
        }

        [UnmanagedCallersOnly]
        private static IntPtr getLlvmHelperFuncEntrypoint(IntPtr thisHandle, CorInfoHelpLlvmFunc helperFunc)
        {
            CorInfoImpl _this = GetThis(thisHandle);
            NodeFactory factory = _this._compilation.NodeFactory;
            ISymbolNode helperFuncNode;
            switch (helperFunc)
            {
                case CorInfoHelpLlvmFunc.CORINFO_HELP_LLVM_EH_UNHANDLED_EXCEPTION:
                    // TODO-LLVM: we are breaking the abstraction here. Compiler is not allowed to access methods from the
                    // managed runtime directly and assume they are compiled into CoreLib. The handler routine should be
                    // made into a RuntimeExport once we solve the issues around calling convention mismatch for them.
                    MetadataType type = _this._compilation.TypeSystemContext.SystemModule.GetKnownType("System.Runtime", "EH");
                    MethodDesc method = type.GetKnownMethod("HandleUnhandledException", null);
                    helperFuncNode = factory.MethodEntrypoint(method);
                    break;
                default:
                    string methodName = helperFunc switch
                    {
                        CorInfoHelpLlvmFunc.CORINFO_HELP_LLVM_GET_OR_INIT_SHADOW_STACK_TOP => "RhpGetOrInitShadowStackTop",
                        CorInfoHelpLlvmFunc.CORINFO_HELP_LLVM_SET_SHADOW_STACK_TOP => "RhpSetShadowStackTop",
                        CorInfoHelpLlvmFunc.CORINFO_HELP_LLVM_EH_DISPATCHER_MUTUALLY_PROTECTING => "RhpDispatchHandleExceptionWasmMutuallyProtectingCatches",
                        CorInfoHelpLlvmFunc.CORINFO_HELP_LLVM_EH_DISPATCHER_CATCH => "RhpDispatchHandleExceptionWasmCatch",
                        CorInfoHelpLlvmFunc.CORINFO_HELP_LLVM_EH_DISPATCHER_FILTER => "RhpDispatchHandleExceptionWasmFilteredCatch",
                        CorInfoHelpLlvmFunc.CORINFO_HELP_LLVM_EH_DISPATCHER_FAULT => "RhpDispatchHandleExceptionWasmFault",
                        CorInfoHelpLlvmFunc.CORINFO_HELP_LLVM_DYNAMIC_STACK_ALLOC => "RhpDynamicStackAlloc",
                        CorInfoHelpLlvmFunc.CORINFO_HELP_LLVM_DYNAMIC_STACK_RELEASE => "RhpDynamicStackRelease",
                        _ => throw new UnreachableException()
                    };
                    helperFuncNode = factory.ExternSymbol(methodName);
                    break;
            }

            return _this.ObjectToHandle(helperFuncNode);
        }

        [UnmanagedCallersOnly]
        private static void* getSingleThreadedCompilationContext(IntPtr thisHandle)
        {
            return GetThis(thisHandle)._pNativeContext;
        }

        public struct TypeDescriptor
        {
            public uint Size;
            public uint FieldCount;
            public CORINFO_FIELD_STRUCT_** Fields; // array of CORINFO_FIELD_STRUCT_*
            public uint HasSignificantPadding; // Change to a uint flags if we need more bools
        }

        [UnmanagedCallersOnly]
        public static void getTypeDescriptor(IntPtr thisHandle, CORINFO_CLASS_STRUCT_* inputType, TypeDescriptor* pTypeDescriptor)
        {
            var _this = GetThis(thisHandle);
            TypeDesc type = _this.HandleToObject(inputType);

            uint fieldCount = 0;
            foreach (var field in type.GetFields())
            {
                if (!field.IsStatic)
                {
                    fieldCount++;
                }
            }

            CORINFO_FIELD_STRUCT_*[] fields = new CORINFO_FIELD_STRUCT_*[fieldCount];

            fieldCount = 0;
            foreach (var field in type.GetFields())
            {
                if (!field.IsStatic)
                {
                    fields[fieldCount] = _this.ObjectToHandle(field);
                    fieldCount++;
                }
            }

            bool hasSignificantPadding = false;
            if (type is EcmaType ecmaType)
            {
                hasSignificantPadding = ecmaType.IsExplicitLayout || ecmaType.GetClassLayout().Size > 0;
            };

            pTypeDescriptor->Size = (uint)type.GetElementSize().AsInt;
            pTypeDescriptor->FieldCount = fieldCount;
            pTypeDescriptor->Fields = (CORINFO_FIELD_STRUCT_**)_this.GetPin(fields);
            pTypeDescriptor->HasSignificantPadding = hasSignificantPadding ? 1u : 0u;
        }

        [UnmanagedCallersOnly]
        private static CORINFO_LLVM_DEBUG_TYPE_HANDLE getDebugTypeForType(IntPtr thisHandle, CORINFO_CLASS_STRUCT_* typeHandle)
        {
            return GetThis(thisHandle).GetDebugTypeForType(typeHandle);
        }

        [UnmanagedCallersOnly]
        private static void getDebugInfoForDebugType(IntPtr thisHandle, CORINFO_LLVM_DEBUG_TYPE_HANDLE debugTypeHandle, CORINFO_LLVM_TYPE_DEBUG_INFO* pInfo)
        {
            GetThis(thisHandle).GetDebugInfoForDebugType(debugTypeHandle, pInfo);
        }

        [UnmanagedCallersOnly]
        private static void getDebugInfoForCurrentMethod(IntPtr thisHandle, CORINFO_LLVM_METHOD_DEBUG_INFO* pInfo)
        {
            GetThis(thisHandle).GetDebugInfoForMethod(pInfo);
        }

        // These enums must be kept in sync with their unmanaged versions in "jit/llvm.cpp".
        //
        private enum EEApiId
        {
            GetMangledMethodName,
            GetMangledSymbolName,
            GetSignatureForMethodSymbol,
            AddCodeReloc,
            IsRuntimeImport,
            GetPrimitiveTypeForTrivialWasmStruct,
            PadOffset,
            GetTypeDescriptor,
            GetAlternativeFunctionName,
            GetExternalMethodAccessor,
            GetLlvmHelperFuncEntrypoint,
            GetDebugTypeForType,
            GetDebugInfoForDebugType,
            GetDebugInfoForCurrentMethod,
            GetSingleThreadedCompilationContext,
            Count
        }

        private enum JitApiId
        {
            StartSingleThreadedCompilation,
            FinishSingleThreadedCompilation,
            Count
        };

        private enum CorInfoHelpLlvmFunc
        {
            CORINFO_HELP_LLVM_UNDEF = CorInfoHelpFunc.CORINFO_HELP_COUNT,
            CORINFO_HELP_LLVM_GET_OR_INIT_SHADOW_STACK_TOP,
            CORINFO_HELP_LLVM_SET_SHADOW_STACK_TOP,
            CORINFO_HELP_LLVM_EH_DISPATCHER_CATCH,
            CORINFO_HELP_LLVM_EH_DISPATCHER_FILTER,
            CORINFO_HELP_LLVM_EH_DISPATCHER_FAULT,
            CORINFO_HELP_LLVM_EH_DISPATCHER_MUTUALLY_PROTECTING,
            CORINFO_HELP_LLVM_EH_UNHANDLED_EXCEPTION,
            CORINFO_HELP_LLVM_DYNAMIC_STACK_ALLOC,
            CORINFO_HELP_LLVM_DYNAMIC_STACK_RELEASE,
            CORINFO_HELP_ANY_COUNT
        }

        [DllImport(JitLibrary)]
        private static extern void registerLlvmCallbacks(void** jitImports, void** jitExports);

        public static void JitStartCompilation()
        {
            void** jitImports = stackalloc void*[(int)EEApiId.Count + 1];
            jitImports[(int)EEApiId.GetMangledMethodName] = (delegate* unmanaged<IntPtr, CORINFO_METHOD_STRUCT_*, byte*>)&getMangledMethodName;
            jitImports[(int)EEApiId.GetMangledSymbolName] = (delegate* unmanaged<IntPtr, void*, byte*>)&getMangledSymbolName;
            jitImports[(int)EEApiId.GetSignatureForMethodSymbol] = (delegate* unmanaged<IntPtr, void*, CORINFO_SIG_INFO*, int>)&getSignatureForMethodSymbol;
            jitImports[(int)EEApiId.AddCodeReloc] = (delegate* unmanaged<IntPtr, void*, void>)&addCodeReloc;
            jitImports[(int)EEApiId.IsRuntimeImport] = (delegate* unmanaged<IntPtr, CORINFO_METHOD_STRUCT_*, uint>)&isRuntimeImport;
            jitImports[(int)EEApiId.GetPrimitiveTypeForTrivialWasmStruct] = (delegate* unmanaged<IntPtr, CORINFO_CLASS_STRUCT_*, CorInfoType>)&getPrimitiveTypeForTrivialWasmStruct;
            jitImports[(int)EEApiId.PadOffset] = (delegate* unmanaged<IntPtr, CORINFO_CLASS_STRUCT_*, uint, uint>)&padOffset;
            jitImports[(int)EEApiId.GetTypeDescriptor] = (delegate* unmanaged<IntPtr, CORINFO_CLASS_STRUCT_*, TypeDescriptor*, void>)&getTypeDescriptor;
            jitImports[(int)EEApiId.GetAlternativeFunctionName] = (delegate* unmanaged<IntPtr, byte*>)&getAlternativeFunctionName;
            jitImports[(int)EEApiId.GetExternalMethodAccessor] = (delegate* unmanaged<IntPtr, CORINFO_METHOD_STRUCT_*, TargetAbiType*, int, IntPtr>)&getExternalMethodAccessor;
            jitImports[(int)EEApiId.GetLlvmHelperFuncEntrypoint] = (delegate* unmanaged<IntPtr, CorInfoHelpLlvmFunc, IntPtr>)&getLlvmHelperFuncEntrypoint;
            jitImports[(int)EEApiId.GetDebugTypeForType] = (delegate* unmanaged<IntPtr, CORINFO_CLASS_STRUCT_*, CORINFO_LLVM_DEBUG_TYPE_HANDLE>)&getDebugTypeForType;
            jitImports[(int)EEApiId.GetDebugInfoForDebugType] = (delegate* unmanaged<IntPtr, CORINFO_LLVM_DEBUG_TYPE_HANDLE, CORINFO_LLVM_TYPE_DEBUG_INFO*, void>)&getDebugInfoForDebugType;
            jitImports[(int)EEApiId.GetDebugInfoForCurrentMethod] = (delegate* unmanaged<IntPtr, CORINFO_LLVM_METHOD_DEBUG_INFO*, void>)&getDebugInfoForCurrentMethod;
            jitImports[(int)EEApiId.GetSingleThreadedCompilationContext] = (delegate* unmanaged<IntPtr, void*>)&getSingleThreadedCompilationContext;
            jitImports[(int)EEApiId.Count] = (void*)0x1234;

#if DEBUG
            for (int i = 0; i < (int)EEApiId.Count; i++)
            {
                Debug.Assert(jitImports[i] != null);
            }
#endif

            fixed (void** jitExports = s_jitExports)
            {
                registerLlvmCallbacks(jitImports, jitExports);
                Debug.Assert(jitExports[(int)JitApiId.Count] == (void*)0x1234);
            }
        }

        public void JitStartSingleThreadedCompilation(string outputFileName, string triple, string dataLayout)
        {
            fixed (byte* pOutputFileName = StringToUTF8(outputFileName), pTriple = StringToUTF8(triple), pDataLayout = StringToUTF8(dataLayout))
            {
                var pExport = (delegate* unmanaged<byte*, byte*, byte*, void*>)s_jitExports[(int)JitApiId.StartSingleThreadedCompilation];
                _pNativeContext = pExport(pOutputFileName, pTriple, pDataLayout);
            }
        }

        public void JitFinishSingleThreadedCompilation()
        {
            ((delegate* unmanaged<void*, void>)s_jitExports[(int)JitApiId.FinishSingleThreadedCompilation])(_pNativeContext);
        }
    }

    public enum TargetAbiType : byte
    {
        Void,
        Int32,
        Int64,
        Float,
        Double
    }
}
