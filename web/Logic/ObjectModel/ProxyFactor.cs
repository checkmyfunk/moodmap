namespace Logic.ObjectModel
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using System.Text;
    using Microsoft.CSharp;

    public static class ProxyFactory
    {
        static Dictionary<Type, Type> proxies = new Dictionary<Type, Type>();
        static string spaces = "";

        public static Type Proxy<T, IdType>()
            where T : BusinessObjectBase<IdType>
            where IdType : IComparable
        {
            lock (proxies)
            {
                Type type = typeof(T);
                return proxies.ContainsKey(type) ? proxies[type] : (proxies[type] = BuildProxy(type));
            }
        }

        public static Type Proxy<T>() where T : BusinessObjectBase<int>
        {
            return Proxy<T, int>();
        }

        public static T CreateInstance<T, IdType>()
            where T : BusinessObjectBase<IdType>
            where IdType : IComparable
        {
            return Activator.CreateInstance(Proxy<T, IdType>()) as T;
        }

        private static void OpenSqueue(StringBuilder code)
        {
            code.AppendLine(spaces + "{");
            spaces = spaces + "    ";
        }

        private static void CloseSqueue(StringBuilder code)
        {
            spaces = spaces.Substring(4);
            code.AppendLine(spaces + "}");
        }

        private static void WriteProp(StringBuilder code, PropertyInfo p, string aName, bool inverse)
        {
            var args = p.PropertyType.GetGenericArguments();
            bool isList = p.PropertyType.IsGenericType && args.Length == 1 && typeof(List<>).MakeGenericType(args).IsAssignableFrom(p.PropertyType);
            code.AppendLine();
            string fieldName = "base." + p.Name;
            string ptype = isList
                ? string.Format("{0}<{1}>", p.PropertyType.Name.Substring(0, p.PropertyType.Name.Length - 2), args[0].Name)
                : p.PropertyType.Name;
            code.AppendLine(string.Format("{0}public override {1} {2}", spaces, ptype, p.Name));
            OpenSqueue(code);
            code.AppendLine(string.Format("{0}get", spaces));
            OpenSqueue(code);
            code.AppendLine(isList
                ? string.Format("{0}return {1} ?? ({1} = {2}.GetAll(o => o.{3}.Equals(Id)));", spaces, fieldName, args[0].Name, aName)
                : inverse
                    ? string.Format("{0}return {1} ?? ({1} = {2}.GetFirst(o => o.{3}.Equals(Id)));", spaces, fieldName, ptype, aName)
                    : string.Format("{0}return {1} ?? ({1} = {2}.GetById({3}));", spaces, fieldName, ptype, aName));
            CloseSqueue(code);
            code.AppendLine(string.Format("{0}set", spaces));
            OpenSqueue(code);
            code.AppendLine(string.Format("{0}{1} = value;", spaces, fieldName));
            if (!isList)
            {
                code.AppendLine(string.Format("{0}if ({1} != null)", spaces, fieldName));
                OpenSqueue(code);
                code.AppendLine(string.Format("{0}{1} = {2}.Id;", spaces, aName, fieldName));
                CloseSqueue(code);
            }
            CloseSqueue(code);
            CloseSqueue(code);
        }

        public static Type BuildProxy(Type t)
        {
            StringBuilder code = new StringBuilder();
            string type = t.Name + "Proxy";
            spaces = "";
            code.AppendLine("using System.Collections.Generic;");
            code.AppendLine("using System.Linq;");
            code.AppendLine(string.Format("namespace {0}", t.Namespace));
            OpenSqueue(code);
            code.AppendLine(string.Format("{0}public class {1}: {2}", spaces, type, t.Name));
            OpenSqueue(code);
            var props = t.GetProperties();
            bool found = false;
            foreach (var p in props)
            {
                if (p.GetMethod.Attributes.HasFlag(MethodAttributes.Virtual))
                {
                    ForeignKeyAttribute a = p.GetCustomAttribute<ForeignKeyAttribute>();
                    if (a != null)
                    {
                        found = true;
                        WriteProp(code, p, a.Name, false);
                    }
                    else
                    {
                        InversePropertyAttribute ia = p.GetCustomAttribute<InversePropertyAttribute>();
                        if (ia != null)
                        {
                            found = true;
                            WriteProp(code, p, ia.Property, true);
                        }
                    }
                }
            }
            CloseSqueue(code);
            CloseSqueue(code);
            if (found)
            {
                using (CSharpCodeProvider compiler = new CSharpCodeProvider())
                {
                    CompilerParameters cparams = new CompilerParameters
                    {
                        GenerateExecutable = false,
                        GenerateInMemory = true,
                    };

                    cparams.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
                    cparams.ReferencedAssemblies.Add(t.Assembly.Location);
                    cparams.ReferencedAssemblies.Add("System.Core.dll");
                    cparams.ReferencedAssemblies.Add("System.dll");
                    var res = compiler.CompileAssemblyFromSource(cparams, code.ToString());
                    Assembly a = res.CompiledAssembly;
                    return a.GetType(t.Namespace + "." + type);
                }
            }
            else return t;
        }

    }
}
