// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Generated by Fuzzlyn v2.4 on 2024-08-29 16:28:05
// Run on X64 Windows
// Seed: 6081642137933547738-vectort,vector128,vector256,x86aes,x86avx,x86avx2,x86avx512bw,x86avx512bwvl,x86avx512cd,x86avx512cdvl,x86avx512dq,x86avx512dqvl,x86avx512f,x86avx512fvl,x86avx512fx64,x86bmi1,x86bmi1x64,x86bmi2,x86bmi2x64,x86fma,x86lzcnt,x86lzcntx64,x86pclmulqdq,x86popcnt,x86popcntx64,x86sse,x86ssex64,x86sse2,x86sse2x64,x86sse3,x86sse41,x86sse41x64,x86sse42,x86sse42x64,x86ssse3,x86x86base
// Reduced from 192.6 KiB to 0.8 KiB in 00:27:23
// Exits with error:
// Fatal error. System.Runtime.InteropServices.SEHException (0x80004005): External component has thrown an exception.
//    at Program.Main(Fuzzlyn.ExecutionServer.IRuntime)
//    at Fuzzlyn.ExecutionServer.Program.<RunPairAsync>g__RunAndGetResultAsync|1_0(Byte[], <>c__DisplayClass1_0 ByRef)
//    at Fuzzlyn.ExecutionServer.Program.RunPairAsync(System.Runtime.Loader.AssemblyLoadContext, Fuzzlyn.ExecutionServer.ProgramPair)
//    at Fuzzlyn.ExecutionServer.Program+<>c__DisplayClass0_0.<Main>b__0()
//
using System;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Xunit;

public class Runtime_107146
{
    private static float[] s_24 = new float[100];
    private static Vector256<sbyte> s_25;
    private static sbyte[] s_26 = new sbyte[100];
    private static Vector256<byte> s_29;

    [Fact]
    public static void TestEntryPoint()
    {
        if (Avx512BW.VL.IsSupported && Popcnt.IsSupported)
        {
            for (int vr15 = 0; vr15 < 2; vr15++)
            {
                s_26[0] = 0;
                float vr16 = s_24[0]--;
                System.Console.WriteLine(s_25);
                System.Console.WriteLine(System.BitConverter.SingleToUInt32Bits(vr16));
                var vr17 = (byte)Popcnt.PopCount(1);
                var vr18 = Vector256.Create<byte>(vr17);
                var vr19 = Vector256.Create<byte>(1);
                s_29 = Avx512BW.VL.CompareLessThan(vr18, vr19);
            }
        }
    }
}
