// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Generated by Fuzzlyn v2.4 on 2024-09-10 14:15:16
// Run on Arm Linux
// Seed: 9455791714420973255
// Reduced from 119.7 KiB to 12.1 KiB in 00:15:49
// Hits JIT assert in Release:
// Assertion failed 'sourceIntervals[targetReg] != nullptr' in 'Program:M8(byref,S0):long' during 'LSRA allocate' (IL size 7341; hash 0x093e6d2a; FullOpts)
// 
//     File: /__w/1/s/src/coreclr/jit/lsra.cpp Line: 10074
// 
using System;
using Xunit;

namespace _107621
{
    public struct S0
    {
        public bool F0;
        public long F1;
        public long F2;
        public float F3;
        public byte F5;
        public sbyte F6;
        public S0(long f2, float f3, double f4, sbyte f6) : this()
        {
        }
    }

    public class C0
    {
        public long F0;
        public S0 F2;
        public S0 F3;
        public long F5;
        public C0(double f1, S0 f2, S0 f3, double f4, long f5)
        {
        }
    }

    public struct S1
    {
        public bool F1;
        public S0 F2;
        public C0 F3;
        public byte F4;
        public uint F5;
        public S1(S0 f2, C0 f3) : this()
        {
        }

        public short M10()
        {
            return default(short);
        }
    }

    public class Program
    {
        public static IRuntime s_rt;
        public static ulong s_1;
        public static C0 s_5 = new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 1), 0, 0);
        public static ushort[] s_7;
        public static C0 s_11 = new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0);
        
        [Fact]
        public static void TestEntryPoint()
        {
            try
            {
                var vr12 = s_11.F2;
                M8(ref s_5.F3.F3, vr12);
            } catch {}
        }

        private static long M8(ref float arg0, S0 arg1)
        {
            double var1 = default(double);
            bool var8 = default(bool);
            if (arg1.F0)
            {
                if (M9())
                {
                    sbyte[,,] var0 = new sbyte[,,]
                    {
                    {
                        {
                            1
                        }
                    }
                    };
                    arg0 = (arg1.F1 * new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)).M10());
                    new S1(new S0(0, 0, 0, 1), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, -1), 0, 0)).M10();
                    new S1(new S0(0, 0, 0, 0), new C0(-1.7976931348623157E+308d, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)).M10();
                    s_rt.WriteLine(var0[0, 0, 0]);
                    s_rt.WriteLine(System.BitConverter.DoubleToUInt64Bits(var1));
                    byte var3 = arg1.F5++;
                    var vr7 = new S1[]
                    {
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(-1922739055833068276L, 0, 0, 1), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 1), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(-9223372036854775808L, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 1), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(9223372036854775807L, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(-9223372036854775808L, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(3366051754928502355L, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0))
                    };
                    ref float vr16 = ref vr7[0].F3.F3.F3;
                    vr16 = (s_1 >> s_7[0]--);
                    var vr8 = new S1[]
                    {
                    new S1(new S0(-7780288970767849613L, 0, 0, 127), new C0(0, new S0(-7247354589903316995L, 0, 0, 1), new S0(3332629451991572720L, 0, 0, 0), 0, 0)),
                    new S1(new S0(9223372036854775806L, 0, 0, 0), new C0(0, new S0(56844987229889904L, 0, 0, 0), new S0(9223372036854775806L, 3.4028235E+38f, 0, -49), 0, -2954444398035812513L)),
                    new S1(new S0(9223372036854775807L, 0, 0, -127), new C0(0, new S0(9223372036854775807L, 0, 0, -127), new S0(-8108349655554254125L, 0, 0, -128), 0, 0))
                    };
                    float vr19 = vr8[0].F3.F3.F3;
                    C0 var4 = new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0);
                    s_rt.WriteLine(var3);
                    s_rt.WriteLine(var4.F0);
                    s_rt.WriteLine(var4.F2.F0);
                    s_rt.WriteLine(var4.F2.F5);
                    s_rt.WriteLine(var4.F3.F0);
                    s_rt.WriteLine(var4.F3.F1);
                    s_rt.WriteLine(var4.F3.F2);
                    s_rt.WriteLine(var4.F3.F5);
                    s_rt.WriteLine(var4.F5);
                }

                for (int var5 = 0; var5 < 2; var5++)
                {
                    var vr5 = new S1[]
                    {
                    new S1(new S0(0, 1, 1, 0), new C0(0, new S0(0, 1, -1, 0), new S0(0, 1, -2261.3113161007714d, 1), 0, 0)),
                    new S1(new S0(0, 1, 0, 0), new C0(0, new S0(0, 3.4028235E+38f, -1.7976931348623157E+308d, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0))
                    };
                }

                S1 var6 = new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, -1), 0, 0));
                s_rt.WriteLine(var6.F1);
                s_rt.WriteLine(var6.F2.F0);
                s_rt.WriteLine(var6.F2.F2);
                s_rt.WriteLine(var6.F2.F5);
                s_rt.WriteLine(var6.F2.F6);
                s_rt.WriteLine(var6.F3.F0);
                s_rt.WriteLine(var6.F3.F2.F0);
                s_rt.WriteLine(var6.F3.F2.F2);
                s_rt.WriteLine(var6.F3.F2.F5);
                s_rt.WriteLine(var6.F3.F2.F6);
                s_rt.WriteLine(var6.F3.F3.F0);
                s_rt.WriteLine(var6.F3.F3.F5);
                s_rt.WriteLine(var6.F3.F3.F6);
                s_rt.WriteLine(var6.F3.F5);
                s_rt.WriteLine(var6.F4);
                s_rt.WriteLine(var6.F5);
            }
            else
            {
                var vr6 = new S1[]
                {
                new S1(new S0(0, -1, -1.7976931348623157E+308d, 0), new C0(0, new S0(0, 0, 0, 1), new S0(0, -1, 0, 0), 1, 0)),
                new S1(new S0(0, 0, 0, 1), new C0(0, new S0(0, 0, 0, 0), new S0(0, 3.4028235E+38f, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 1), new S0(0, 0, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 0, 1), new C0(0, new S0(0, 0, 0, 1), new S0(0, 0, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0))
                };
                float vr22 = vr6[0].F3.F3.F3;
                arg1.F5 = (byte)vr22;
                C0 var7 = new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0);
                var7.F3.F3--;
                s_7 = new ushort[]
                {
                1
                };
                sbyte[] var9 = new sbyte[]
                {
                1
                };
                if (s_5.F2.F0)
                {
                    uint[] var10 = new uint[]
                    {
                    0,
                    1,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                    };
                    var vr1 = new S1[]
                    {
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 1), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 1), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, -1), 0, 0))
                    };
                    sbyte var11 = (sbyte)(var10[0] - M11(vr1));
                    s_rt.WriteLine(var10[0]);
                    s_rt.WriteLine(var11);
                }
                else
                {
                    ulong[] var12 = new ulong[]
                    {
                    1
                    };
                    s_rt.WriteLine(var12[0]);
                }

                if (s_5.F2.F0)
                {
                    var vr3 = new S1[]
                    {
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, -1, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, -1), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 1), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 1), 0, 0)),
                    new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                    new S1(new S0(0, 0, 0, 1), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0))
                    };
                    C0[] var13 = new C0[]
                    {
                    new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0),
                    new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)
                    };
                    s_rt.WriteLine(var13[0].F0);
                    s_rt.WriteLine(var13[0].F2.F1);
                    s_rt.WriteLine(var13[0].F2.F2);
                    s_rt.WriteLine(var13[0].F2.F5);
                    s_rt.WriteLine(var13[0].F2.F6);
                    s_rt.WriteLine(var13[0].F3.F0);
                    s_rt.WriteLine(var13[0].F3.F1);
                    s_rt.WriteLine(var13[0].F3.F2);
                    s_rt.WriteLine(var13[0].F3.F5);
                    s_rt.WriteLine(var13[0].F3.F6);
                    s_rt.WriteLine(var13[0].F5);
                }
                else
                {
                    return arg1.F2++;
                }

                var7.F3.F3 = var7.F2.F3--;
                float var14 = arg1.F3;
                s_rt.WriteLine(System.BitConverter.SingleToUInt32Bits(var14));
                s_rt.WriteLine(var8);
                var vr9 = new S1[]
                {
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 0, 1), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 1), new S0(0, 0, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 1), new S0(0, 0, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0))
                };
                var vr10 = new S1[]
                {
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 1, 1), new C0(0, new S0(0, 0, 0, 0), new S0(0, 0, 0, 0), 0, 0))
                };
                s_rt.WriteLine(var7.F0);
                s_rt.WriteLine(var7.F2.F0);
                s_rt.WriteLine(var7.F2.F1);
                s_rt.WriteLine(var7.F2.F2);
                s_rt.WriteLine(var7.F2.F5);
                s_rt.WriteLine(var7.F2.F6);
                s_rt.WriteLine(var7.F3.F0);
                s_rt.WriteLine(var7.F3.F1);
                s_rt.WriteLine(var7.F3.F5);
                s_rt.WriteLine(var7.F3.F6);
                s_rt.WriteLine(var7.F5);
            }

            for (int var15 = 0; var15 < 1; var15++)
            {
                var vr2 = new S1[]
                {
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 0, -1.7976931348623157E+308d, 0), new S0(0, 0, 0, 0), 0, 0)),
                new S1(new S0(0, 0, 0, 0), new C0(0, new S0(0, 3.4028235E+38f, 0, 0), new S0(0, 0, 0, 0), 0, 0))
                };
            }

            return 0;
        }

        private static bool M9()
        {
            return default(bool);
        }

        private static ref float M11(S1[] arg0)
        {
            return ref arg0[0].F3.F3.F3;
        }
    }

    public interface IRuntime
    {
        void WriteLine<T>(T value);
    }

    public class Runtime : IRuntime
    {
        public void WriteLine<T>(T value) => System.Console.WriteLine(value);
    }
}