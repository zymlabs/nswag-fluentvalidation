using Xunit;

namespace ZymLabs.NSwag.FluentValidation.Tests
{
    public class ReflectionExtensionsTest
    {
        [Fact]
        public void IsSubClassOfGenericTest()
        {
            Assert.True(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(BaseGeneric<>)), " 1");
            Assert.False(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(WrongBaseGeneric<>)), " 2");
            Assert.True(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), " 3");
            Assert.False(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(IWrongBaseGeneric<>)), " 4");
            Assert.True(typeof(IChildGeneric).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), " 5");
            Assert.False(typeof(IWrongBaseGeneric<>).IsSubClassOfGeneric(typeof(ChildGeneric2<>)), " 6");
            Assert.True(typeof(ChildGeneric2<>).IsSubClassOfGeneric(typeof(BaseGeneric<>)), " 7");
            Assert.True(typeof(ChildGeneric2<Class1>).IsSubClassOfGeneric(typeof(BaseGeneric<>)), " 8");
            Assert.True(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(BaseGeneric<Class1>)), " 9");
            Assert.False(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(WrongBaseGeneric<Class1>)), "10");
            Assert.True(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "11");
            Assert.False(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(IWrongBaseGeneric<Class1>)), "12");
            Assert.True(typeof(IChildGeneric).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "13");
            Assert.False(typeof(BaseGeneric<Class1>).IsSubClassOfGeneric(typeof(ChildGeneric2<Class1>)), "14");
            Assert.True(typeof(ChildGeneric2<Class1>).IsSubClassOfGeneric(typeof(BaseGeneric<Class1>)), "15");
            Assert.False(typeof(ChildGeneric).IsSubClassOfGeneric(typeof(ChildGeneric)), "16");
            Assert.False(typeof(IChildGeneric).IsSubClassOfGeneric(typeof(IChildGeneric)), "17");
            Assert.False(typeof(IBaseGeneric<>).IsSubClassOfGeneric(typeof(IChildGeneric2<>)), "18");
            Assert.True(typeof(IChildGeneric2<>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "19");
            Assert.True(typeof(IChildGeneric2<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "20");
            Assert.False(typeof(IBaseGeneric<Class1>).IsSubClassOfGeneric(typeof(IChildGeneric2<Class1>)), "21");
            Assert.True(typeof(IChildGeneric2<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "22");
            Assert.False(typeof(IBaseGeneric<Class1>).IsSubClassOfGeneric(typeof(BaseGeneric<Class1>)), "23");
            Assert.True(typeof(BaseGeneric<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "24");
            Assert.False(typeof(IBaseGeneric<>).IsSubClassOfGeneric(typeof(BaseGeneric<>)), "25");
            Assert.True(typeof(BaseGeneric<>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "26");
            Assert.True(typeof(BaseGeneric<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "27");
            Assert.False(typeof(IBaseGeneric<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "28");
            Assert.True(typeof(BaseGeneric2<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<Class1>)), "29");
            Assert.False(typeof(IBaseGeneric<>).IsSubClassOfGeneric(typeof(BaseGeneric2<>)), "30");
            Assert.True(typeof(BaseGeneric2<>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "31");
            Assert.True(typeof(BaseGeneric2<Class1>).IsSubClassOfGeneric(typeof(IBaseGeneric<>)), "32");
            Assert.True(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(BaseGenericA<,>)), "33");
            Assert.False(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(WrongBaseGenericA<,>)), "34");
            Assert.True(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "35");
            Assert.False(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(IWrongBaseGenericA<,>)), "36");
            Assert.True(typeof(IChildGenericA).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "37");
            Assert.False(typeof(IWrongBaseGenericA<,>).IsSubClassOfGeneric(typeof(ChildGenericA2<,>)), "38");
            Assert.True(typeof(ChildGenericA2<,>).IsSubClassOfGeneric(typeof(BaseGenericA<,>)), "39");
            Assert.True(typeof(ChildGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(BaseGenericA<,>)), "40");
            Assert.True(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(BaseGenericA<ClassA, ClassB>)), "41");
            Assert.False(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(WrongBaseGenericA<ClassA, ClassB>)), "42");
            Assert.True(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "43");
            Assert.False(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(IWrongBaseGenericA<ClassA, ClassB>)), "44");
            Assert.True(typeof(IChildGenericA).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "45");

            Assert.False(
                typeof(BaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(ChildGenericA2<ClassA, ClassB>)), "46"
            );

            Assert.True(
                typeof(ChildGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(BaseGenericA<ClassA, ClassB>)), "47"
            );

            Assert.False(typeof(ChildGenericA).IsSubClassOfGeneric(typeof(ChildGenericA)), "48");
            Assert.False(typeof(IChildGenericA).IsSubClassOfGeneric(typeof(IChildGenericA)), "49");
            Assert.False(typeof(IBaseGenericA<,>).IsSubClassOfGeneric(typeof(IChildGenericA2<,>)), "50");
            Assert.True(typeof(IChildGenericA2<,>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "51");
            Assert.True(typeof(IChildGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "52");

            Assert.False(
                typeof(IBaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IChildGenericA2<ClassA, ClassB>)), "53"
            );

            Assert.True(
                typeof(IChildGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "54"
            );

            Assert.False(
                typeof(IBaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(BaseGenericA<ClassA, ClassB>)), "55"
            );

            Assert.True(
                typeof(BaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "56"
            );

            Assert.False(typeof(IBaseGenericA<,>).IsSubClassOfGeneric(typeof(BaseGenericA<,>)), "57");
            Assert.True(typeof(BaseGenericA<,>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "58");
            Assert.True(typeof(BaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "59");

            Assert.False(
                typeof(IBaseGenericA<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "60"
            );

            Assert.True(
                typeof(BaseGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "61"
            );

            Assert.False(typeof(IBaseGenericA<,>).IsSubClassOfGeneric(typeof(BaseGenericA2<,>)), "62");
            Assert.True(typeof(BaseGenericA2<,>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "63");
            Assert.True(typeof(BaseGenericA2<ClassA, ClassB>).IsSubClassOfGeneric(typeof(IBaseGenericA<,>)), "64");

            Assert.False(
                typeof(BaseGenericA2<ClassB, ClassA>).IsSubClassOfGeneric(typeof(IBaseGenericA<ClassA, ClassB>)), "65"
            );

            Assert.False(
                typeof(BaseGenericA<ClassB, ClassA>).IsSubClassOfGeneric(typeof(ChildGenericA2<ClassA, ClassB>)), "66"
            );

            Assert.False(
                typeof(BaseGenericA2<ClassB, ClassA>).IsSubClassOfGeneric(typeof(BaseGenericA<ClassA, ClassB>)), "67"
            );

            Assert.True(
                typeof(ChildGenericA3<ClassA, ClassB>).IsSubClassOfGeneric(
                    typeof(BaseGenericB<ClassA, ClassB, ClassC>)
                ), "68"
            );

            Assert.True(
                typeof(ChildGenericA4<ClassA, ClassB>).IsSubClassOfGeneric(
                    typeof(IBaseGenericB<ClassA, ClassB, ClassC>)
                ), "69"
            );

            Assert.False(
                typeof(ChildGenericA3<ClassB, ClassA>).IsSubClassOfGeneric(
                    typeof(BaseGenericB<ClassA, ClassB, ClassC>)
                ), "68-2"
            );

            Assert.True(
                typeof(ChildGenericA3<ClassA, ClassB2>).IsSubClassOfGeneric(
                    typeof(BaseGenericB<ClassA, ClassB, ClassC>)
                ), "68-3"
            );

            Assert.False(
                typeof(ChildGenericA3<ClassB2, ClassA>).IsSubClassOfGeneric(
                    typeof(BaseGenericB<ClassA, ClassB, ClassC>)
                ), "68-4"
            );

            Assert.False(
                typeof(ChildGenericA4<ClassB, ClassA>).IsSubClassOfGeneric(
                    typeof(IBaseGenericB<ClassA, ClassB, ClassC>)
                ), "69-2"
            );

            Assert.True(
                typeof(ChildGenericA4<ClassA, ClassB2>).IsSubClassOfGeneric(
                    typeof(IBaseGenericB<ClassA, ClassB, ClassC>)
                ), "69-3"
            );

            Assert.False(
                typeof(ChildGenericA4<ClassB2, ClassA>).IsSubClassOfGeneric(
                    typeof(IBaseGenericB<ClassA, ClassB, ClassC>)
                ), "69-4"
            );

            Assert.False(typeof(bool).IsSubClassOfGeneric(typeof(IBaseGenericB<ClassA, ClassB, ClassC>)), "70");
        }
    }

    public class Class1
    {
    }

    public class BaseGeneric<T> : IBaseGeneric<T>
    {
    }

    public class BaseGeneric2<T> : IBaseGeneric<T>, IInterfaceBidon
    {
    }

    public interface IBaseGeneric<T>
    {
    }

    public class ChildGeneric : BaseGeneric<Class1>
    {
    }

    public interface IChildGeneric : IBaseGeneric<Class1>
    {
    }

    public class ChildGeneric2<Class1> : BaseGeneric<Class1>
    {
    }

    public interface IChildGeneric2<Class1> : IBaseGeneric<Class1>
    {
    }

    public class WrongBaseGeneric<T>
    {
    }

    public interface IWrongBaseGeneric<T>
    {
    }

    public interface IInterfaceBidon
    {
    }

    public class ClassA
    {
    }

    public class ClassB
    {
    }

    public class ClassC
    {
    }

    public class ClassB2 : ClassB
    {
    }

    public class BaseGenericA<T, U> : IBaseGenericA<T, U>
    {
    }

    public class BaseGenericB<T, U, V>
    {
    }

    public interface IBaseGenericB<ClassA, ClassB, ClassC>
    {
    }

    public class BaseGenericA2<T, U> : IBaseGenericA<T, U>, IInterfaceBidonA
    {
    }

    public interface IBaseGenericA<T, U>
    {
    }

    public class ChildGenericA : BaseGenericA<ClassA, ClassB>
    {
    }

    public interface IChildGenericA : IBaseGenericA<ClassA, ClassB>
    {
    }

    public class ChildGenericA2<ClassA, ClassB> : BaseGenericA<ClassA, ClassB>
    {
    }

    public class ChildGenericA3<ClassA, ClassB> : BaseGenericB<ClassA, ClassB, ClassC>
    {
    }

    public class ChildGenericA4<ClassA, ClassB> : IBaseGenericB<ClassA, ClassB, ClassC>
    {
    }

    public interface IChildGenericA2<ClassA, ClassB> : IBaseGenericA<ClassA, ClassB>
    {
    }

    public class WrongBaseGenericA<T, U>
    {
    }

    public interface IWrongBaseGenericA<T, U>
    {
    }

    public interface IInterfaceBidonA
    {
    }
}