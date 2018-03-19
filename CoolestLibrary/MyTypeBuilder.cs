using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

// https://msdn.microsoft.com/en-us/library/btz51t4a(v=vs.110).aspx
// https://stackoverflow.com/questions/3862226/how-to-dynamically-create-a-class-in-c

namespace CoolestLibrary
{
    public static class MyTypeBuilder
    {
        public static void CreateNewObject(
            Type funcType,
            IEnumerable<Type> classAttributeTypes,
            ParameterInfo parameterInfo)
        {
            var myType = MakeDynamicType(funcType, classAttributeTypes, parameterInfo);
            var myInstance = Activator.CreateInstance(myType);
        }
        public static Type MakeDynamicType(
            Type funcType,
            IEnumerable<Type> classAttributeTypes,
            ParameterInfo parameterInfo)
        {
            TypeBuilder tb = GetTypeBuilder();

            GenerateCtorAndProperties(tb, funcType, parameterInfo);

            SetClassLevelAttributes(classAttributeTypes, tb);

            Type objectType = tb.CreateType();
            return objectType;
        }

        private static void SetClassLevelAttributes(IEnumerable<Type> classAttributeTypes, TypeBuilder tb)
        {
            foreach (var type in classAttributeTypes)
            {
                var builder = GetAttributeBuilder(type);
                tb.SetCustomAttribute(builder);
            }
        }

        private static CustomAttributeBuilder GetAttributeBuilder(Type attributeType)
        {
            ConstructorInfo ci = attributeType.GetConstructors().First(); // we don't really care which one
            ParameterInfo[] pi = ci.GetParameters();

            var defaultArgs = pi.Select(p => GetDefaultCtorArgs(p)).ToArray();

            return new CustomAttributeBuilder(ci, defaultArgs);
        }

        private static object GetDefaultCtorArgs(ParameterInfo p)
        {
            if (p.ParameterType.IsValueType)
            {
                return Activator.CreateInstance(p.ParameterType);
            }
            else if (p.ParameterType == typeof(string))
            {
                return string.Empty;
            }
            else
            {
                return null;
            }
        }

        private static void GenerateCtorAndProperties(TypeBuilder tb, Type funcType, ParameterInfo parameterInfo)
        {
            var fieldName = "_func";
            var propName = "Func";

            FieldBuilder fb = tb.DefineField(fieldName, funcType, FieldAttributes.Private);

            GenerateCustomConstructor(tb, funcType, parameterInfo, fb);

            GeneratePropertyGetter(tb, funcType, propName, fb);
        }

        private static void GeneratePropertyGetter(TypeBuilder tb, Type funcType, string propName, FieldBuilder fb)
        {
            PropertyBuilder pb = tb.DefineProperty(propName, PropertyAttributes.HasDefault, funcType, null);
            MethodBuilder getMb = tb.DefineMethod(
                "get_" + propName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                funcType,
                Type.EmptyTypes);

            ILGenerator getIlg = getMb.GetILGenerator();

            getIlg.Emit(OpCodes.Ldarg_0);
            getIlg.Emit(OpCodes.Ldfld, fb);
            getIlg.Emit(OpCodes.Ret);

            pb.SetGetMethod(getMb);
        }

        private static void GenerateCustomConstructor(TypeBuilder tb, Type funcType, ParameterInfo parameterInfo, FieldBuilder fb)
        {
            Type objType = typeof(object);
            ConstructorInfo objCtor = objType.GetConstructor(new Type[0]);
            Type[] ctorParams = new Type[] { funcType };
            ConstructorBuilder customCtor = tb.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                ctorParams);

            var paramBuilder = customCtor.DefineParameter(1, parameterInfo.Attributes, "func");
            var paramAttributes = parameterInfo.CustomAttributes;

            foreach (var attr in paramAttributes)
            {
                var targetAttrBuilder = GetAttributeBuilder(attr.AttributeType);
                paramBuilder.SetCustomAttribute(targetAttrBuilder);
            }

            ILGenerator ctorIlg = customCtor.GetILGenerator();

            ctorIlg.Emit(OpCodes.Ldarg_0);
            ctorIlg.Emit(OpCodes.Call, objCtor);
            ctorIlg.Emit(OpCodes.Ldarg_0);
            ctorIlg.Emit(OpCodes.Ldarg_1);

            // store the ctor arg in the _func field
            ctorIlg.Emit(OpCodes.Stfld, fb);

            ctorIlg.Emit(OpCodes.Ret);
        }

        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "MyDynamicType";
            var moduleName = "MyDynamicModule";

            var assName = new AssemblyName(typeSignature);
            AssemblyBuilder assBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assBuilder.DefineDynamicModule(moduleName);
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null);

            return tb;
        }
    }
}
