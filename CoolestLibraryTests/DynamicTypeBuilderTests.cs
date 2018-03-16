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

            // act
            Type dynamicType = MyTypeBuilder.MakeDynamicType(funcType, attrTypes);
            k.Bind(dynamicType).ToSelf();
            var wrapperInstance = k.Get(dynamicType);
            var funcInstance = FuncValueGetter.GetFuncValue(wrapperInstance, dynamicType);

            MethodInfo mi = funcType.GetMethod(nameof(Func<int>.Invoke));
            object result = mi.Invoke(funcInstance, new object[] { 2 });

            // assert
            Assert.IsInstanceOf(typeof(Actor), result);
        }


        //[Test]
        //public void CanSetFuncAsGettableProperty()
        //{
        //    // assemble
        //    var k = new MyTestKernel();

        //    var attrTypes = new List<Type> { typeof(DulcetTonesAttribute) };
        //    Type funcType = typeof(Func<string, Actor>);
        //    Type dynamicType = MyTypeBuilder.MakeDynamicType(funcType, attrTypes);

        //    // act
        //    var instance = Activator.CreateInstance(dynamicType);
        //    var value = FuncValueGetter.GetFuncValue(instance, dynamicType);

        //    // assert
        //    Assert.IsNotNull(value);
        //}
    }
}
