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
        public static void CreateNewObject(Type funcType, IEnumerable<Type> attrTypes)
        {
            var myType = MakeDynamicType(funcType, attrTypes);
            var myInstance = Activator.CreateInstance(myType);
        }
        public static Type MakeDynamicType(Type funcType, IEnumerable<Type> attributeTypes)
        {
            TypeBuilder tb = GetTypeBuilder();
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName);

            CreateNinjectedProperty(tb, funcType);

            foreach (var type in attributeTypes)
            {
                var builder = GetAttributeBuilder(type);
                tb.SetCustomAttribute(builder);
            }

            Type objectType = tb.CreateType();
            return objectType;
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

        private static void CreateNinjectedProperty(TypeBuilder tb, Type funcType)
        {
            var fieldName = "_func";
            var propName = "Func";

            FieldBuilder fb = tb.DefineField(fieldName, funcType, FieldAttributes.Private);
            PropertyBuilder pb = tb.DefineProperty(propName, PropertyAttributes.HasDefault, funcType, null);

            // set the [Inject] attribute on the property
            CustomAttributeBuilder ab = GetAttributeBuilder(typeof(Ninject.InjectAttribute));
            pb.SetCustomAttribute(ab);

            // generate the { get; } method
            MethodBuilder getMb = tb.DefineMethod(
                "get_" + propName, 
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, 
                funcType, 
                Type.EmptyTypes);

            ILGenerator getIlg = getMb.GetILGenerator();

            getIlg.Emit(OpCodes.Ldarg_0);
            getIlg.Emit(OpCodes.Ldfld, fb);
            getIlg.Emit(OpCodes.Ret);


            // generate teh { set; } method
            // needed for the [Inject] thing to work
            MethodBuilder setMb = tb.DefineMethod(
                "set_" + propName, 
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, 
                null, 
                new[] { funcType });

            ILGenerator setIlg = setMb.GetILGenerator();
            Label modifyProperty = setIlg.DefineLabel();
            Label exitSet = setIlg.DefineLabel();

            setIlg.MarkLabel(modifyProperty);
            setIlg.Emit(OpCodes.Ldarg_0);
            setIlg.Emit(OpCodes.Ldarg_1);
            setIlg.Emit(OpCodes.Stfld, fb);
            setIlg.Emit(OpCodes.Nop);
            setIlg.MarkLabel(exitSet);
            setIlg.Emit(OpCodes.Ret);

            pb.SetGetMethod(getMb);
            pb.SetSetMethod(setMb);
        }

        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "CreamOnTheInside";
            var moduleName = "CleanOnTheOutside";

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
