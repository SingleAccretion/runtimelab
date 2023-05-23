// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//

using System;
using Xunit;

public struct ValX0 { }
public struct ValY0 { }
public struct ValX1<T> { }
public struct ValY1<T> { }
public struct ValX2<T, U> { }
public struct ValY2<T, U> { }
public struct ValX3<T, U, V> { }
public struct ValY3<T, U, V> { }
public class RefX0 { }
public class RefY0 { }
public class RefX1<T> { }
public class RefY1<T> { }
public class RefX2<T, U> { }
public class RefY2<T, U> { }
public class RefX3<T, U, V> { }
public class RefY3<T, U, V> { }



public class GenException<T> : Exception { }
public struct Gen<T>
{
    public static bool ExceptionTest(bool throwException)
    {
        string ExceptionClass = typeof(GenException<T>).ToString();
        try
        {
            if (throwException)
            {
                throw new GenException<T>();
            }
            else
            {
                return true;
            }
        }
        catch (Exception E)
        {
            string EText = E.ToString();
            // Ensure the type of the Exception --> this is currently broken
            if ((EText != null) && (EText.IndexOf(ExceptionClass) >= 0))
            {
                //Ensure we can correctly downcast the Exception to it's original type
                try
                {
                    GenException<T> tmp = (GenException<T>)E;
                    return true;
                }
                catch
                {
                    Console.WriteLine("Failed to downcast Exception object for: " + typeof(Gen<T>));
                    return false;
                }
            }
            Console.WriteLine("Failed to detect " + ExceptionClass + " in Exception class string");
            return false;
        }
    }
}

public class Test_general_struct_static01
{
    public static int counter = 0;
    public static bool result = true;
    public static void Eval(bool exp)
    {
        counter++;
        if (!exp)
        {
            result = exp;
            Console.WriteLine("Test Failed at location: " + counter);
        }

    }

    [Fact]
    public static int TestEntryPoint()
    {
        Eval(Gen<int>.ExceptionTest(true));
        Eval(Gen<double>.ExceptionTest(true));
        Eval(Gen<string>.ExceptionTest(true));
        Eval(Gen<object>.ExceptionTest(true));
        Eval(Gen<Guid>.ExceptionTest(true));

        Eval(Gen<int[]>.ExceptionTest(true));
        Eval(Gen<double[,]>.ExceptionTest(true));
        Eval(Gen<string[][][]>.ExceptionTest(true));
        Eval(Gen<object[, , ,]>.ExceptionTest(true));
        Eval(Gen<Guid[][, , ,][]>.ExceptionTest(true));

        Eval(Gen<RefX1<int>[]>.ExceptionTest(true));
        Eval(Gen<RefX1<double>[,]>.ExceptionTest(true));
        Eval(Gen<RefX1<string>[][][]>.ExceptionTest(true));
        Eval(Gen<RefX1<object>[, , ,]>.ExceptionTest(true));
        Eval(Gen<RefX1<Guid>[][, , ,][]>.ExceptionTest(true));
        Eval(Gen<RefX2<int, int>[]>.ExceptionTest(true));
        Eval(Gen<RefX2<double, double>[,]>.ExceptionTest(true));
        Eval(Gen<RefX2<string, string>[][][]>.ExceptionTest(true));
        Eval(Gen<RefX2<object, object>[, , ,]>.ExceptionTest(true));
        Eval(Gen<RefX2<Guid, Guid>[][, , ,][]>.ExceptionTest(true));
        Eval(Gen<ValX1<int>[]>.ExceptionTest(true));
        Eval(Gen<ValX1<double>[,]>.ExceptionTest(true));
        Eval(Gen<ValX1<string>[][][]>.ExceptionTest(true));
        Eval(Gen<ValX1<object>[, , ,]>.ExceptionTest(true));
        Eval(Gen<ValX1<Guid>[][, , ,][]>.ExceptionTest(true));

        Eval(Gen<ValX2<int, int>[]>.ExceptionTest(true));
        Eval(Gen<ValX2<double, double>[,]>.ExceptionTest(true));
        Eval(Gen<ValX2<string, string>[][][]>.ExceptionTest(true));
        Eval(Gen<ValX2<object, object>[, , ,]>.ExceptionTest(true));
        Eval(Gen<ValX2<Guid, Guid>[][, , ,][]>.ExceptionTest(true));

        Eval(Gen<RefX1<int>>.ExceptionTest(true));
        Eval(Gen<RefX1<ValX1<int>>>.ExceptionTest(true));
        Eval(Gen<RefX2<int, string>>.ExceptionTest(true));
        Eval(Gen<RefX3<int, string, Guid>>.ExceptionTest(true));

        Eval(Gen<RefX1<RefX1<int>>>.ExceptionTest(true));
        Eval(Gen<RefX1<RefX1<RefX1<string>>>>.ExceptionTest(true));
        Eval(Gen<RefX1<RefX1<RefX1<RefX1<Guid>>>>>.ExceptionTest(true));

        Eval(Gen<RefX1<RefX2<int, string>>>.ExceptionTest(true));
        Eval(Gen<RefX2<RefX2<RefX1<int>, RefX3<int, string, RefX1<RefX2<int, string>>>>, RefX2<RefX1<int>, RefX3<int, string, RefX1<RefX2<int, string>>>>>>.ExceptionTest(true));
        Eval(Gen<RefX3<RefX1<int[][, , ,]>, RefX2<object[, , ,][][], Guid[][][]>, RefX3<double[, , , , , , , , , ,], Guid[][][][, , , ,][, , , ,][][][], string[][][][][][][][][][][]>>>.ExceptionTest(true));

        Eval(Gen<ValX1<int>>.ExceptionTest(true));
        Eval(Gen<ValX1<RefX1<int>>>.ExceptionTest(true));
        Eval(Gen<ValX2<int, string>>.ExceptionTest(true));
        Eval(Gen<ValX3<int, string, Guid>>.ExceptionTest(true));

        Eval(Gen<ValX1<ValX1<int>>>.ExceptionTest(true));
        Eval(Gen<ValX1<ValX1<ValX1<string>>>>.ExceptionTest(true));
        Eval(Gen<ValX1<ValX1<ValX1<ValX1<Guid>>>>>.ExceptionTest(true));

        Eval(Gen<ValX1<ValX2<int, string>>>.ExceptionTest(true));
        Eval(Gen<ValX2<ValX2<ValX1<int>, ValX3<int, string, ValX1<ValX2<int, string>>>>, ValX2<ValX1<int>, ValX3<int, string, ValX1<ValX2<int, string>>>>>>.ExceptionTest(true));
        Eval(Gen<ValX3<ValX1<int[][, , ,]>, ValX2<object[, , ,][][], Guid[][][]>, ValX3<double[, , , , , , , , , ,], Guid[][][][, , , ,][, , , ,][][][], string[][][][][][][][][][][]>>>.ExceptionTest(true));



        if (result)
        {
            Console.WriteLine("Test Passed");
            return 100;
        }
        else
        {
            Console.WriteLine("Test Failed");
            return 1;
        }
    }

}
