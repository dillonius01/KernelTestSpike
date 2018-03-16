using System;
using System.Linq;

namespace CoolestLibrary
{
    public static class FuncValueGetter
    {
        public static object GetFuncValue(object wrapperInstance, Type wrapperType)
        {
            var funcProp = wrapperType.GetProperties()
                          .First() // we assume we only have one, per our implementation of GenerateDynamicType
                          .GetAccessors()
                          .First();
            
            return funcProp.Invoke(wrapperInstance, new object[0]);
        }
    }
}
