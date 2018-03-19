using CoolestLibrary;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoolestLibraryTests
{
    [TestFixture]
    public class DynamicTypeBuilderTests
    {
        [Test]
        public void MakeDynamicType_PassInAttrTypes_AttachesToClass()
        {
            // assemble
            var k = new MyTestKernel();
            var attrTypes = new List<Type> { typeof(DulcetTonesAttribute) };
            Type funcType = typeof(Func<int, Actor>);
            var dummyParamInfo = typeof(DummyClass).GetConstructors().First().GetParameters().First();

            // act
            Type dynamicType = MyTypeBuilder.MakeDynamicType(funcType, attrTypes, dummyParamInfo);
            k.Bind(dynamicType).ToSelf();
            var wrapperInstance = k.Get(dynamicType);
            var funcInstance = FuncValueGetter.GetFuncValue(wrapperInstance, dynamicType);

            MethodInfo mi = funcType.GetMethod(nameof(Func<int>.Invoke));
            object result = mi.Invoke(funcInstance, new object[] { 2 });

            // assert
            Assert.IsInstanceOf(typeof(Actor), result);
        }


        [Test]
        public void MakeDynamicType_PassInParameterInfo_AttachesAttributesToWrapperParameter()
        {
            // assemble
            var k = new MyTestKernel();
            var attrTypes = new List<Type> { typeof(DulcetTonesAttribute) };
            Type funcType = typeof(Func<int, Actor>);
            var paramInfo = typeof(DummyClass).GetConstructors().First().GetParameters().First(); // has "Thespian" attr on it

            // act
            Type dynamicType = MyTypeBuilder.MakeDynamicType(funcType, attrTypes, paramInfo);
            var ctorParamAttrs = dynamicType.GetConstructors().First().GetParameters().First().GetCustomAttribute<ThespianAttribute>();

            // assert
            Assert.IsNotNull(ctorParamAttrs);
        }
    }

    public class DummyClass
    {
        public DummyClass([Thespian]int dummyParam)
        {}
    }
}
