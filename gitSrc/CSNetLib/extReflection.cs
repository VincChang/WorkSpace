using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace System.Reflection;

public static class extReflection
{
    public static object xInvoke(this MethodInfo method, object target, object[] args)
    {
        return method.Invoke(target, args);
    }
}
