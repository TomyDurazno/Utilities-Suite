using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Utility.Tools
{
    /*
        Bunch of tools. Most of them are not even used in this project, they were copy-pasted from another 
    */

    public static class MyExtensions
    {
        #region Powerful one-liners

        /// <summary>
        /// Projects a value into a function invocation
        /// </summary>
        public static K Project<T, K>(this T obj, Func<T, K> func) => func.Invoke(obj);

        public static T[] AsArray<T>(this T obj) => new T[] { obj };

        public static dynamic[] AsArrayDynamic<T>(this T obj) => new dynamic[]{ obj };

        public static string ToNullString<T>(this T obj) => obj?.ToString() ?? "null";

        /// <summary>
        /// Calls an action over a variable and the returns that variable
        /// </summary>
        public static T Call<T>(this T obj, Action<T> act) { act.Invoke(obj); return obj; }

        #endregion

        public static string GetCurrentMethod() => new StackTrace().GetFrame(1).GetMethod().Name;

        //Gracias Jon Skeet!	
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
            }
        }

        public static IOrderedEnumerable<IGrouping<char, string>> FieldsByName<T>(this T obj)
        {
            return obj.GetType()
                      .GetFields()
                      .Select(f => f.Name)
                      .GroupBy(f => f[0])
                      .OrderBy(f => f.Key);
        }


        #region XML

        public static string ToXml<T>(this T myObj)
        {
            var xsSubmit = new XmlSerializer(typeof(T));

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww, settings))
                {
                    xsSubmit.Serialize(writer, myObj);
                    xml = sww.ToString(); // Your XML
                }
            }

            return xml;
        }

        #endregion
    }

    public static class MyUtils
    {
        #region Paths	

        //Internal use
        private static readonly string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public static string DesktopPath { get { return desktopPath; } }

        public static string CDiskPath { get { return Path.GetPathRoot(Environment.SystemDirectory); } }

        public static string MyQueriesPath { get { return @"C:\Users\tcordara\Documents\LINQPad Queries"; } }

        public static string MyExtensionsPath
        {
            get
            {
                return @"C:\Users\tcordara\Documents\LINQPad Plugins\Framework 4.6\MyExtensions.FW46.linq";
            }
        }

        #endregion

        #region MakeFunc Implementations

        public static Func<T> MakeFunc<T>(Func<T> func) => func;

        public static Func<T, K> MakeFunc<T, K>(Func<T, K> func) => func;

        public static Func<T, K, S> MakeFunc<T, K, S>(Func<T, K, S> func) => func;

        public static Func<T, K, S, R> MakeFunc<T, K, S, R>(Func<T, K, S, R> func) => func;

        public static Func<T, K, S, R, M> MakeFunc<T, K, S, R, M>(Func<T, K, S, R, M> func) => func;

        public static Func<T, K, S, R, M, N> MakeFunc<T, K, S, R, M, N>(Func<T, K, S, R, M, N> func) => func;

        public static Func<T, K, S, R, M, N, O> MakeFunc<T, K, S, R, M, N, O>(Func<T, K, S, R, M, N, O> func) => func;

        public static Func<T, K, S, R, M, N, O, P> MakeFunc<T, K, S, R, M, N, O, P>(Func<T, K, S, R, M, N, O, P> func) => func;

        #endregion

        #region MakeAction

        public static Action MakeAction(Action action) => action;

        public static Action<T> MakeAction<T>(Action<T> action) => action;

        public static Action<T, K> MakeAction<T, K>(Action<T, K> action) => action;

        #endregion

        #region MakeArray

        public static T[] MakeArray<T>(params T[] arr)
        {
            return arr;
        }

        #endregion

        #region Enums

        public static IEnumerable<T> GetEnums<T>() where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("The type is not and enum type");
            }

            return Enum.GetValues(type).Cast<T>();
        }

        public static T ToEnum<T>(this string s) where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("The type is not and enum type");
            }

            return (T)Enum.Parse(typeof(T), s);
        }

        #endregion

        #region IEnumerable

        public static IEnumerable<T> Enumerates<T>(this IEnumerable<T> items, Action<T> act)
        {
            items.ToList().ForEach(act);
            return items;
        }

        public static IEnumerable<K> Enumerates<T, K>(this IEnumerable<T> items, Func<T, K> func)
        {
            return items.Select(i => func(i)).ToList();
        }

        public static IEnumerable<T> ToEnumerable<T>(this int num, Func<T> func)
        {
            for (int i = 0; i < num; i++)
            {
                yield return func();
            }
        }

        public static List<List<T>> Chunk<T>(this IEnumerable<T> items, Func<T, bool> chunker)
        {
            var result = new List<List<T>>();
            var actuals = new List<T>();

            foreach (var item in items)
            {
                if (chunker(item))//is Chunker
                {
                    if (actuals.Any())
                    {
                        result.Add(actuals);
                        actuals = new List<T>();
                    }
                }
                else
                {
                    actuals.Add(item);
                }
            }

            result.Add(actuals);
            return result;
        }

        public static List<T> Enlaze <T>(this IEnumerable<T> items, IEnumerable<T> items2)
        {

            var result = new List<T>();
            if(items.Any() & items2.Any())
            {
                var arr1 = items.ToArray();
                var arr2 = items2.ToArray();

                var index = 0;
                var flag = 2;

                while (flag > 0)
                {
                    if (index < arr1.Length)
                        result.Add(arr1[index]);
                    else
                        flag--;

                    if (index < arr2.Length)
                        result.Add(arr2[index]);
                    else
                        flag--;

                    index++;
                }
            }

            return result;
        }

        #endregion

        #region Txt Read/Write

        public static IEnumerable<string> ReadTxtFromDesktop(string fileName)
        {
            return _ReadTxt(fileName, true);
        }

        public static IEnumerable<string> ReadTxt(string fileName)
        {
            return _ReadTxt(fileName, false);
        }

        private static IEnumerable<string> _ReadTxt(string fileName, bool fromDesktop)
        {
            string line;
            var path = fromDesktop ? Path.Combine(desktopPath, fileName) : fileName;
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                while ((line = streamReader.ReadLine()) != null)
                    yield return line;
            }
        }

        public static void WriteTxtToDesktop(IEnumerable<string> lines, string filename = null, bool append = true)
        {
            filename = filename ?? "archivo.txt";

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(desktopPath, filename), append))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }

        public static void WriteTxt(IEnumerable<string> lines, string filename = null, bool append = true)
        {
            using (StreamWriter outputFile = new StreamWriter(filename, append))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }

        #endregion

        #region XML Read

        public static XmlDocument ExecuteXmlPostRequest(string sXmlRequest, string url)
        {
            var requestXml = new XmlDocument();
            requestXml.LoadXml(sXmlRequest);

            // build XML request 
            var httpRequest = System.Net.HttpWebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "text/xml";

            // set appropriate headers
            using (var requestStream = httpRequest.GetRequestStream())
            {
                requestXml.Save(requestStream);
            }

            using (var response = (System.Net.HttpWebResponse)httpRequest.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    // may want to check response.StatusCode to
                    // see if the request was successful
                    var responseXml = new XmlDocument();
                    responseXml.Load(responseStream);
                    return responseXml;
                }
            }
        }

        #endregion

        #region File Opener

        public static void OpenFile(string filePath)
        {
            System.Diagnostics.Process.Start(filePath);
        }

        public static void OpenFileFromDesktop(string fileName)
        {
            System.Diagnostics.Process.Start(Path.Combine(desktopPath, fileName));
        }

        #endregion

        #region SetTimeOut/Interval

        public static async Task SetIntervalAsync(int milliseconds, TimeSpan timeStop, Action act)
        {
            while (DateTime.Now.TimeOfDay < timeStop)
            {
                act.Invoke();
                await System.Threading.Tasks.Task.Delay(milliseconds);
            }
        }

        public static async Task<T> SetIntervalAsync<T>(int milliseconds, TimeSpan timeStop, Func<T> func)
        {
            while (DateTime.Now.TimeOfDay < timeStop)
            {
                await Task.Delay(milliseconds);
                return await Task.Run(func);
            }

            return await Task.Run(func);
        }

        public static async Task SetTimeOutAsync(int millisecondsInterval, TimeSpan timeStop, Action act)
        {
            while (DateTime.Now.TimeOfDay < timeStop)
            {
                await Task.Delay(millisecondsInterval);
            }

            await Task.Run(act);
        }

        public static async Task<T> SetTimeOutAsync<T>(int millisecondsInterval, TimeSpan timeStop, Func<T> func)
        {
            while (DateTime.Now.TimeOfDay < timeStop)
            {
                await Task.Delay(millisecondsInterval);
            }

            return await Task.Run(func);
        }

        public static async Task SetTimeOutAsync(int millisecondsInterval, TimeSpan timeStop, System.Threading.Tasks.Task act)
        {
            while (DateTime.Now.TimeOfDay < timeStop)
            {
                await System.Threading.Tasks.Task.Delay(millisecondsInterval);
            }

            await act;
        }

        public static Task WaitOneAsync(this WaitHandle waitHandle)
        {
            if (waitHandle == null)
                throw new ArgumentNullException("waitHandle");

            var tcs = new TaskCompletionSource<bool>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                delegate { tcs.TrySetResult(true); }, null, -1, true);
            var t = tcs.Task;
            t.ContinueWith(antecedent => rwh.Unregister(null));
            return t;
        }

        #endregion

        #region String Manipulation

        public static IEnumerable<string> SplitBy(this string auxs, params char[] separators)
        {
            return auxs.Split(separators);
        }

        public static IEnumerable<string> SplitBy(this string auxs, params string[] separators)
        {
            return auxs.Split(separators, StringSplitOptions.None);
        }

        public static IEnumerable<IEnumerable<string>> SplitByTab(this IEnumerable<string> rows)
        {
            return rows.Select(s => s.SplitBy(new char[] { '\t' }));
        }

        public static string CleanFileName(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        public static string RemoveBetween(string s, char begin, char end)
        {
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(s, string.Empty);
        }

        public static string FieldWiseToString<T>(this T obj)
        {
                try
                {
                    var sb = obj.GetType()
                                .GetProperties()
                                .Select(p => {

                                    string sRepresent;

                                    if (p.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(p.PropertyType))
                                    {
                                        var arr = p.GetValue(obj);

                                        sRepresent = string.Format("[{0}]", string.Join(", ", ((IEnumerable<object>)arr)));
                                    }
                                    else
                                    {
                                        sRepresent = p.GetValue(obj).ToString();
                                    }

                                        return new
                                    {
                                        p.Name,
                                        Value = p.GetValue(obj) != null ? sRepresent : null
                                    };
                                })
                                .Aggregate(new StringBuilder(), (builder, item) =>
                                {
                                    builder.Append(string.Format("{0}=", item.Name));
                                    builder.Append(item.Value);
                                    builder.Append("#");
                                    return builder;
                                });

                            return sb.ToString();
                }
                catch (Exception ex)
                {
                    throw;
                }
 
        }

        public static string CleanString(this string sInput, string start, string end)
        {
            var sfirst = sInput.StartsWith(start) ? sInput.Substring(1) : sInput;

            var sRet = sInput.EndsWith(end) ? sfirst.Remove(sfirst.Length - 1) : sfirst;

            return sRet;
        }

        #endregion

        #region DB/SQL Related

        public static class DB
        {
            public static string RowsAsInsert<T>(List<T> myObjs, string insertName, bool excludePK = false)
            {
                Func<int, string> stringFormat = (index) => index == 0 ? "'{0}'" : ", '{0}'";

                Func<object, int, string> stringFormatForNulls = (value, index) =>
                {

                    if (value == null)
                        return index == 0 ? "NULL" : ", NULL";
                    else
                        return index == 0 ? "{0}" : ", '{0}'";
                };

                Func<int, string> stringFormatFields = (index) => index == 0 ? "{0}" : ", {0}";
                Func<int, string> valuesFormat = (index) => index == 0 ? "({0})" : ", ({0})";

                var first = myObjs.First();
                var fields = first.GetType().GetFields();

                Func<string, int, bool> PKFunc = (s, i) => true;
                Func<FieldInfo, int, bool> PKFuncFieldInfo = (f, i) => true;

                if (excludePK)
                {
                    PKFunc = (s, i) => i > 0;
                    PKFuncFieldInfo = (s, i) => i > 0;
                }

                var names = fields.Select(f => f.Name.ToLower())
                                   .Where(PKFunc) //PRIMARY KEY EXCLUDING
                                   .Select((v, i) => string.Format(stringFormatFields(i), v))
                                   .Aggregate(new StringBuilder(), (acum, s) => acum.AppendLine(s))
                                   .ToString();

                var lines = myObjs.Select(obj => obj.GetType().GetFields()
                                        .Where(PKFuncFieldInfo) //PRIMARY KEY EXCLUDING
                                        .Select(f => f.GetValue(obj))
                                        .Select((v, i) => string.Format(stringFormatForNulls(v, i), v))
                                        .Aggregate(new StringBuilder(), (acum, s) => acum.Append(s))
                                        .ToString())
                                 .Select((v, i) => string.Format(valuesFormat(i), v))
                                 .Aggregate(new StringBuilder(), (acum, s) => acum.AppendLine(s))
                                 .ToString();

                return string.Format("Insert into {0} ({1}) Values {2}", insertName, names, lines);
            }
        }

        #endregion

        #region Date Related

        public static string DateAsFileName()
        {
            return string.Concat("_", DateTime.Now.ToShortDateString().SplitBy('/').Project(s => string.Join("-", s)));
        }

        #endregion
    }

    #region DateTimeSpan Class

    //DateTimeSpan
    //href: https://stackoverflow.com/questions/4638993/difference-in-months-between-two-dates

    public struct DateTimeSpan
    {
        private readonly int years;
        private readonly int months;
        private readonly int days;
        private readonly int hours;
        private readonly int minutes;
        private readonly int seconds;
        private readonly int milliseconds;

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            this.years = years;
            this.months = months;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.milliseconds = milliseconds;
        }

        public int Years { get { return years; } }
        public int Months { get { return months; } }
        public int Days { get { return days; } }
        public int Hours { get { return hours; } }
        public int Minutes { get { return minutes; } }
        public int Seconds { get { return seconds; } }
        public int Milliseconds { get { return milliseconds; } }

        enum Phase { Years, Months, Days, Done }

        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            if (date2 < date1)
            {
                var sub = date1;
                date1 = date2;
                date2 = sub;
            }

            DateTime current = date1;
            int years = 0;
            int months = 0;
            int days = 0;

            Phase phase = Phase.Years;
            DateTimeSpan span = new DateTimeSpan();
            int officialDay = current.Day;

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.AddYears(years + 1) > date2)
                        {
                            phase = Phase.Months;
                            current = current.AddYears(years);
                        }
                        else
                        {
                            years++;
                        }
                        break;
                    case Phase.Months:
                        if (current.AddMonths(months + 1) > date2)
                        {
                            phase = Phase.Days;
                            current = current.AddMonths(months);
                            if (current.Day < officialDay && officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                                current = current.AddDays(officialDay - current.Day);
                        }
                        else
                        {
                            months++;
                        }
                        break;
                    case Phase.Days:
                        if (current.AddDays(days + 1) > date2)
                        {
                            current = current.AddDays(days);
                            var timespan = date2 - current;
                            span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                            phase = Phase.Done;
                        }
                        else
                        {
                            days++;
                        }
                        break;
                }
            }

            return span;
        }
    }

    #endregion

    #region ClassBuilder 

    public class ClassBuilder
    {
        AssemblyName asemblyName;

        public ClassBuilder(string ClassName)
        {
            this.asemblyName = new AssemblyName(ClassName);
        }

        public object CreateObject(string[] PropertyNames, Type[] Types = null)
        {
            if (Types != null && PropertyNames.Length != Types.Length)
            {
                Console.WriteLine("The number of property names should match their corresponding types number");
            }

            TypeBuilder DynamicClass = this.CreateClass();
            this.CreateConstructor(DynamicClass);
            for (int ind = 0; ind < PropertyNames.Count(); ind++)
            {
                CreateProperty(DynamicClass, PropertyNames[ind], Types != null ? Types[ind] : typeof(string));
            }

            Type type = DynamicClass.CreateType();

            return Activator.CreateInstance(type);
        }

        private TypeBuilder CreateClass()
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(this.asemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(this.asemblyName.FullName
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);
            return typeBuilder;
        }

        private void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }

        private void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }

    public static class ClassBuilderExtensions
    {
        public static void SetStringProps<T>(this T obj, IEnumerable<string> propsName)
        {
            obj.GetType().GetProperties().Zip(propsName, (prop, result) =>
            {
                prop.SetValue(obj, result);
                return string.Format("prop: {0}, value: {1}", prop.Name, result);
            }).ToList();
        }
    }
    #endregion
}
