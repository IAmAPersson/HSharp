using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace HSCi
{
    static class Parser
    {
        public static Dictionary<string, string> vars = new Dictionary<string, string>();
        public static Dictionary<string, string> wildcards = new Dictionary<string, string>();
        private static bool CAS = true;

        private static int[] range(int a, int b, int step)
        {
            bool backs = false;
            if (a > b && step == 1)
            {
                backs = true;
                int c = ++a;
                a = --b;
                b = c;

            }
            if (step < 0) b -= 2;
            List<int> res = new List<int>();
            for (int i = a; step > 0 ? i < b : i > b; i += step)
                res.Add(i);
            if (backs) res.Reverse();
            return res.ToArray();
        }

        private static Tuple<bool, Dictionary<string, string>> matches(string test, string str)
        {
            str = " " + str + " ";
            test = " " + test + " ";
            Dictionary<string, string> varlist = new Dictionary<string, string>();
            string dicttempkey;
            string dicttempvals;
            for (int i = 0; i < str.Length; i++)
            {
                dicttempkey = "";
                dicttempvals = "";
                try
                {
                    if (str[i] != test[i])
                    {
                        if (str[i] < 'A' || str[i] > 'Z')
                        {
                            return new Tuple<bool, Dictionary<string, string>>(false, varlist);
                        }
                        string strtemp = str.Substring(i);
                        string testtemp = test.Substring(i);
                        while ((strtemp[0] >= 'a' && strtemp[0] <= 'z') || (strtemp[0] >= 'A' && strtemp[0] <= 'Z'))
                        {
                            dicttempkey += strtemp[0];
                            strtemp = strtemp.Substring(1);
                        }
                        while ((testtemp[0] >= '0' && testtemp[0] <= '9') || testtemp[0] == '-' || testtemp[0] == '.' || (testtemp[0] >= 'a' && testtemp[0] <= 'z') || (testtemp[0] == '[' && dicttempvals == ""))
                        {
                            if (testtemp[0] == '[')
                            {
                                int cnt = 0;
                                do
                                {
                                    if (testtemp[0] == ']') cnt--;
                                    if (testtemp[0] == '[') cnt++;
                                    dicttempvals += testtemp[0];
                                    testtemp = testtemp.Substring(1);
                                } while (cnt != 0);
                            }
                            else
                            {
                                dicttempvals += testtemp[0];
                                testtemp = testtemp.Substring(1);
                            }
                        }
                        test = test.Substring(0, i - 1) + testtemp;
                        str = str.Substring(0, i - 1) + strtemp;
                        i -= 2;
                        varlist.Add(dicttempkey, dicttempvals);
                    }
                }
                catch
                {
                    return new Tuple<bool, Dictionary<string, string>>(false, varlist);
                }
            }
            if (str.Length != test.Length)
            {
                return new Tuple<bool, Dictionary<string, string>>(false, varlist);
            }
            if (varlist.ContainsValue("") || varlist.ContainsKey(""))
            {
                return new Tuple<bool, Dictionary<string, string>>(false, varlist);
            }
            return new Tuple<bool, Dictionary<string, string>>(true, varlist);
        }

        private static string[] split(string s, string splitter)
        {
            List<string> ret = new List<string>();
            string temp = "";
            for (int i = 0; i < s.Length; i++)
            {
                try
                {
                    if (s.Substring(i, splitter.Length) == splitter)
                    {
                        ret.Add(temp);
                        temp = "";
                        i += splitter.Length - 1;
                    }
                    else
                    {
                        temp += s.Substring(i, 1);
                    }
                }
                catch
                {
                    temp += s.Substring(i, 1);
                }
            }
            ret.Add(temp);
            return ret.ToArray();
        }

        private static string[] listsplit(string s, string splitter)
        {
            List<string> ret = new List<string>();
            string temp = "";
            int isinarr = 0;
            for (int i = 0; i < s.Length; i++)
            {
                try
                {
                    if (s[i] == '[' || s[i] == '{')
                    {
                        isinarr++;
                    }
                    if (s[i] == ']' || s[i] == '}')
                    {
                        isinarr--;
                    }
                    if (s.Substring(i, splitter.Length) == splitter && isinarr == 0)
                    {
                        ret.Add(temp);
                        temp = "";
                        i += splitter.Length - 1;
                    }
                    else
                    {
                        temp += s.Substring(i, 1);
                    }
                }
                catch
                {
                    temp += s.Substring(i, 1);
                }
            }
            ret.Add(temp);
            return ret.ToArray();
        }

        private static string[] singlesplit(string s, string splitter)
        {
            List<string> ret = new List<string>();
            string temp = "";
            bool alrsplt = false;
            for (int i = 0; i < s.Length; i++)
            {
                try
                {
                    if (s.Substring(i, splitter.Length) == splitter && !alrsplt)
                    {
                        ret.Add(temp);
                        temp = "";
                        i += splitter.Length - 1;
                        alrsplt = true;
                    }
                    else
                    {
                        temp += s.Substring(i, 1);
                    }
                }
                catch
                {
                    temp += s.Substring(i, 1);
                }
            }
            ret.Add(temp);
            return ret.ToArray();
        }

        private static string[] betwparen(string s)
        {
            int inparen = 0;
            string text = "";
            List<string> arr = new List<string>();
            foreach (char i in s)
            {
                if (i == ')' && inparen == 1)
                {
                    arr.Add(text);
                    inparen = 0;
                    text = "";
                }
                else if (i == ')')
                {
                    inparen -= 1;
                }
                if (inparen > 0)
                    text += i;
                if (i == '(')
                    inparen += 1;
            }
            return arr.ToArray();
        }

        private static string[] betwbracks(string s)
        {
            int inparen = 0;
            string text = "";
            List<string> arr = new List<string>();
            foreach (char i in s)
            {
                if (i == ']' && inparen == 1)
                {
                    arr.Add(text);
                    inparen = 0;
                    text = "";
                }
                else if (i == ']')
                {
                    inparen -= 1;
                }
                if (inparen > 0)
                    text += i;
                if (i == '[')
                    inparen += 1;
            }
            return arr.ToArray();
        }

        private static string[] betwbraces(string s)
        {
            int inparen = 0;
            string text = "";
            List<string> arr = new List<string>();
            foreach (char i in s)
            {
                if (i == '}' && inparen == 1)
                {
                    arr.Add(text);
                    inparen = 0;
                    text = "";
                }
                else if (i == '}')
                {
                    inparen -= 1;
                }
                if (inparen > 0)
                    text += i;
                if (i == '{')
                    inparen += 1;
            }
            return arr.ToArray();
        }

        private static string remspaces(string s)
        {
            string res = "";
            foreach (char i in s)
            {
                if (i != ' ' && i != '\t')
                    res += i;
            }
            return res;
        }

        private static string remstartspaces(string s)
        {
            for (int i = 0; i < s.Length; i++)
                if (s[i] != ' ') return s.Substring(i);
            return "";
        }

        private static string fixneg(string s)
        {
            foreach (char i in s)
            {
                if (i == '`')
                {
                    if (s.Length > 0 && s[0] == '`')
                    {
                        s = "-" + s.Substring(1);
                    }
                    return s;
                }
            }
            string res = "";
            foreach (int i in range(0, s.Length, 1))
            {
                try
                {
                    if (s[i] == '-' && (s[i - 1] < '0' || s[i - 1] > '9'))
                        res += "`";
                    else
                    {
                        res += s[i];
                    }
                }
                catch
                {
                    if (s[0] == '-')
                    {
                        res += '`';
                    }
                }
            }
            s = res;
            res = "";
            foreach (int i in range(0, s.Length, 1))
            {
                if (s[i] == '-')
                    res += "`";
                else if (s[i] == '`')
                    res += "-";
                else
                    res += s[i];
            }
            if (res.Length > 0 && res[0] == '`')
                res = "-" + res.Substring(1);
            return res;
        }

        private static bool isarray(string s)
        {
            if (s.Substring(0, 1) == "[" && s.ToCharArray().ToList().Last() == ']')
            {
                foreach (string ret in split(betwbracks(s)[0], ","))
                {
                    if (!(ret == "true" || ret == "false" || ret == "unknown" || ret == "infinity" || Convert.ToString(Convert.ToDouble(ret)) == ret))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private static string sine(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot take sine of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Sin(num);
            return Convert.ToString(num);
        }

        private static string cosi(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot take cosine of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Cos(num);
            return Convert.ToString(num);
        }

        private static string tang(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot take tangent of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Tan(num);
            if (num > -286411217403672 && num < -286411217403670) return "infinity";
            return Convert.ToString(num);
        }

        private static string asin(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot take arcsine of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Asin(num);
            return Convert.ToString(num);
        }

        private static string acos(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot take arccosine of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Acos(num);
            return Convert.ToString(num);
        }

        private static string atan(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot take arctangent of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Atan(num);
            return Convert.ToString(num);
        }

        private static string flor(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot floor a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Floor(num);
            return Convert.ToString(num);
        }

        private static string rond(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot round a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Floor(num + 0.5);
            return Convert.ToString(num);
        }

        private static string trun(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot truncate a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Truncate(num);
            return Convert.ToString(num);
        }

        private static string ceil(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot ceiling a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Ceiling(num);
            return Convert.ToString(num);
        }

        private static string getl(string s, string base_)
        {
            int num1 = 0;
            string[] num;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (vars.ContainsKey(base_))
                base_ = parseMath(vars[base_]);
            if (!int.TryParse(base_, out num1))
            {
                Console.WriteLine("Error: Cannot get list element with noninteger index value (includes booleans, unknown variables, decimals, or infinity). Aborting operation.");
                return "";
            }
            num = listsplit(betwbracks(s)[0], ",");
            return remspaces(num[num1 % num.Length]);
        }

        private static string mapf(string f, string l)
        {
            string res = "[";
            string[] num;
            if (vars.ContainsKey(l))
                l = parseMath(vars[l]);
            if (vars.ContainsKey(f))
                f = parseMath(vars[f]);
            if (!isarray(l))
            {
                Console.WriteLine("Error: Cannot map function to non-list value. Aborting operation.");
                return "";
            }
            num = listsplit(betwbracks(l)[0], ",").Select(x => parseMath(f + "[" + x + "]")).ToArray();
            foreach (string i in num)
                res += i + ",";
            res = res.Substring(0, res.Length - 1);
            res += "]";
            return res;
        }

        private static string filt(string f, string l)
        {
            string res = "[";
            string[] num;
            if (vars.ContainsKey(l))
                l = parseMath(vars[l]);
            if (vars.ContainsKey(f))
                f = parseMath(vars[f]);
            if (!isarray(l))
            {
                Console.WriteLine("Error: Cannot map function to non-list value. Aborting operation.");
                return "";
            }
            num = listsplit(betwbracks(l)[0], ",").Where(x => parseMath(f + "[" + x + "]") == "true" ? true : false).ToArray();
            foreach (string i in num)
                res += i + ",";
            res = res.Substring(0, res.Length - 1);
            res += "]";
            return res;
        }

        private static string modu(string s, string base_)
        {
            double num = 0, num1 = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (vars.ContainsKey(base_))
                base_ = parseMath(vars[base_]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot take modulo with argument of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            if (!double.TryParse(base_, out num1))
            {
                Console.WriteLine("Error: Cannot take modulo with argument of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = num % num1;
            return Convert.ToString(num);
        }

        private static string nlog(string s, string base_)
        {
            double num = 0, num1 = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (vars.ContainsKey(base_))
                base_ = parseMath(vars[base_]);
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot take log of a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            if (!double.TryParse(base_, out num1))
            {
                Console.WriteLine("Error: Cannot take log with base of a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Log(num, num1);
            return Convert.ToString(num);
        }

        private static string prnt(string s)
        {
            s = parseMath(s);
            Console.WriteLine(s);
            return s;
        }

        private static string gama(string s)
        {
            double alpha = 0;
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (!double.TryParse(s, out alpha))
            {
                Console.WriteLine("Error: Cannot take factorial of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            if (alpha <= 0)
            {
                Console.WriteLine("Error: Cannot take factorial of negative value. Aborting operation.");

                return "";
            }
            try
            {
                double gamma = 0;
                if (alpha > 0)
                {
                    if (alpha > 0 && alpha < 1)
                    {
                        gamma = Convert.ToDouble(gama(Convert.ToString(alpha + 1))) / alpha;
                    }
                    else if (alpha >= 1 && alpha <= 2)
                    {
                        gamma = 1 - 0.577191652 * Math.Pow(alpha - 1, 1) + 0.988205891 * Math.Pow(alpha - 1, 2) -
                                0.897056937 * Math.Pow(alpha - 1, 3) + 0.918206857 * Math.Pow(alpha - 1, 4) -
                                0.756704078 * Math.Pow(alpha - 1, 5) + 0.482199394 * Math.Pow(alpha - 1, 6) -
                                0.193527818 * Math.Pow(alpha - 1, 7) + 0.03586843 * Math.Pow(alpha - 1, 8);
                    }
                    else
                    {
                        gamma = (alpha - 1) * Convert.ToDouble(gama(Convert.ToString(alpha - 1)));
                    }
                }
                if (alpha > 171)
                {
                    gamma = Math.Pow(10, 307);
                }
                return Convert.ToString(gamma);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static string testinternal(string s)
        {
            try
            {
                if (s.Length > 16 && s.Substring(0, 12) == "internalcmmd")
                {
                    switch (s.Substring(12, 4))
                    {
                        case "sine":
                            s = sine(s.Substring(16));
                            break;
                        case "cosi":
                            s = cosi(s.Substring(16));
                            break;
                        case "tang":
                            s = tang(s.Substring(16));
                            break;
                        case "asin":
                            s = asin(s.Substring(16));
                            break;
                        case "acos":
                            s = acos(s.Substring(16));
                            break;
                        case "atan":
                            s = atan(s.Substring(16));
                            break;
                        case "prnt":
                            s = prnt(s.Substring(16));
                            break;
                        case "flor":
                            s = flor(s.Substring(16));
                            break;
                        case "rond":
                            s = rond(s.Substring(16));
                            break;
                        case "ceil":
                            s = ceil(s.Substring(16));
                            break;
                        case "trun":
                            s = trun(s.Substring(16));
                            break;
                        case "modu":
                            s = modu(split(s.Substring(16), ",")[0], split(s.Substring(16), ",")[1]);
                            break;
                        case "nlog":
                            s = nlog(split(s.Substring(16), ",")[0], split(s.Substring(16), ",")[1]);
                            break;
                        case "getl":
                            s = getl(listsplit(s.Substring(16), ",")[0], listsplit(s.Substring(16), ",")[1]);
                            break;
                        case "mapf":
                            s = mapf(listsplit(s.Substring(16), ",")[0], listsplit(s.Substring(16), ",")[1]);
                            break;
                        case "filt":
                            s = filt(listsplit(s.Substring(16), ",")[0], listsplit(s.Substring(16), ",")[1]);
                            break;
                        case "gama":
                            string temps = parseMath(s.Substring(16));
                            s = gama(temps);
                            if (split(temps, ".").Length == 1 && split(s, "+").Length == 1)
                                s = split(s, ".")[0];
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            { }
            return s;
        }

        private static string parseMath(string s)
        {
            double trash;
            if (double.TryParse(s, out trash)) return remspaces(s);
            try
            {
                s = remspaces(s);
                s = fixneg(s);
                s = testinternal(s);
                if (vars.ContainsKey(s))
                {
                    s = parseMath(vars[s]);
                }
                foreach (string stemp in wildcards.Keys.ToArray().Reverse())
                {
                    Tuple<bool, Dictionary<string, string>> temp = matches(s, stemp);
                    if (temp.Item1)
                    {
                        s = wildcards[stemp];
                        foreach (string sdict in temp.Item2.Keys)
                            s = mapToVars("_let_" + sdict + "=" + temp.Item2[sdict], s);
                        s = parseMath(s);
                        foreach (string sdict in temp.Item2.Keys)
                            forget("_let_" + sdict + "=" + temp.Item2[sdict]);
                    }
                }
                if (betwparen(s).Length > 0)
                    foreach (string i in betwparen(s))
                        s = split(s, "(")[0] + parseMath(i) + s.Substring(split(s, "(")[0].Length + i.Length + 2);
                if (betwbraces(s).Length > 0)
                    foreach (string i in betwbraces(s))
                        s = split(s, "{")[0] + "{" + parseMath(i) + "}" + s.Substring(split(s, "{")[0].Length + i.Length + 2);
                if (vars.ContainsKey(s))
                    s = parseMath(vars[s]);
                foreach (string stemp in wildcards.Keys.ToArray().Reverse())
                {
                    Tuple<bool, Dictionary<string, string>> temp = matches(s, stemp);
                    if (temp.Item1)
                    {
                        s = wildcards[stemp];
                        foreach (string sdict in temp.Item2.Keys)
                            s = mapToVars("_let_" + sdict + "=" + temp.Item2[sdict], s);
                        s = parseMath(s);
                        foreach (string sdict in temp.Item2.Keys)
                            forget("_let_" + sdict + "=" + temp.Item2[sdict]);
                    }
                }
                if (split(s, "_where_").Length > 1)
                {
                    string temps = "_let_" + singlesplit(s, "_where_")[1];
                    if (split(temps, "=")[0].Last() == '/')
                    {
                        Console.WriteLine("Error: Cannot use /= in \"where\" predicate.");
                        return "";
                    }
                    if (split(temps, "_if_").Length > 1 && split(temps, "_then_").Length == 1)
                    {
                        Console.WriteLine("Error: Cannot use \"if\" predicate off of \"where\" predicate, only \"let\" commands.");
                        return "";
                    }
                    let(temps, true);
                    s = parseMath(split(s, "_where_")[0]);
                    forget(temps);
                }
                if (split(s, "_if_").Length > 1)
                {
                    if (parseMath(split(split(s, "_if_")[1], "_then_")[0]) != "true" && parseMath(split(split(s, "_if_")[1], "_then_")[0]) != "false")
                        Console.WriteLine("Error: Expected boolean expression, received \"" + split(split(s, "_if_")[1], "_then_")[0] + "\"");
                    else if (parseMath(split(split(s, "_if_")[1], "_then_")[0]) == "true")
                    {
                        try
                        {
                            s = split(s, "_if_")[0] + parseMath(split(split(s, "_then_")[1], "_else_")[0]);
                        }
                        catch
                        { }
                    }
                    else if (parseMath(split(split(s, "_if_")[1], "_then_")[0]) == "false")
                    {
                        s = split(s, "_if_")[0] + parseMath(singlesplit(s, "_else_")[1]);
                    }
                }
                if (betwbracks(s).Length > 0)
                {
                    foreach (string i in betwbracks(s))
                    {
                        try
                        {
                            string res = "";
                            bool b = false;
                            List<string> args = new List<string>();
                            if (split(i, "..").Length == 2)
                            {
                                b = true;
                                if (betwbracks(i).Length == 0)
                                {
                                    if (split(split(i, "..")[0], ",").Length == 2)
                                        args = range(Convert.ToInt32(parseMath(split(split(i, "..")[0], ",")[0])), Convert.ToInt32(parseMath(split(i, "..")[1])) + 1, Convert.ToInt32(parseMath(split(split(i, "..")[0], ",")[1])) - Convert.ToInt32(parseMath(split(split(i, "..")[0], ",")[0]))).Select(x => Convert.ToString(x)).ToList();
                                    else
                                        args = range(Convert.ToInt32(parseMath(split(i, "..")[0])), Convert.ToInt32(parseMath(split(i, "..")[1])) + 1, 1).Select(x => Convert.ToString(x)).ToList();
                                }
                                else
                                    b = false;
                            }
                            if (b == false)
                                args = listsplit(i, ",").Cast<string>().Select(x => parseMath(x)).ToList();
                            for (int j = 0; j < args.Count - 1; j++)
                                res += args[j] + ",";
                            res += args.Last();
                            s = split(s, "[")[0] + "«" + res + "»" + s.Substring(split(s, "[")[0].Length + i.Length + 2);
                        }
                        catch
                        {
                            Console.WriteLine("Error: Could not parse list \"[" + i + "]\". Assuming [], resuming operation.");
                            s = split(s, "[")[0] + "«" + "»" + s.Substring(split(s, "[")[0].Length + i.Length + 2);
                        }
                    }
                    s = " " + s;
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] == '«')
                            s = s.Substring(0, i) + '[' + s.Substring(i + 1);
                        if (s[i] == '»')
                            s = s.Substring(0, i) + ']' + s.Substring(i + 1);
                    }
                    s = s.Substring(1);
                }
                if (split(s, "_and_").Length > 1)
                {
                    string[] args = split(s, "_and_");
                    foreach (string i in args)
                        if (parseMath(i) == "false")
                            return "false";
                    return "true";
                }
                else if (split(s, "_or_").Length > 1)
                {
                    string[] args = split(s, "_or_");
                    foreach (string i in args)
                        if (parseMath(i) == "true")
                            return "true";
                    return "false";
                }
                else if (split(s, "/=").Length == 2)
                {
                    string a = parseMath(split(s, "/=")[0]), b = parseMath(split(s, "/=")[1]);
                    if (vars.ContainsKey(split(s, "/=")[0] + "/=" + b))
                        return vars[split(s, "/=")[0] + "/=" + b];
                    if (vars.ContainsKey(a + "/=" + split(s, "/=")[1]))
                        return vars[a + "/=" + split(s, "/=")[1]];
                    if (a == "unknown" || b == "unknown")
                        return "unknown";
                    return a != b ? "true" : "false";
                }
                else if (split(s, ">=").Length == 2)
                {
                    string a = parseMath(split(s, ">=")[0]), b = parseMath(split(s, ">=")[1]);
                    if (a == "unknown" || b == "unknown")
                        return "unknown";
                    return Convert.ToDouble(a) >= Convert.ToDouble(b) ? "true" : "false";
                }
                else if (split(s, "<=").Length == 2)
                {
                    string a = parseMath(split(s, "<=")[0]), b = parseMath(split(s, "<=")[1]);
                    if (a == "unknown" || b == "unknown")
                        return "unknown";
                    return Convert.ToDouble(a) <= Convert.ToDouble(b) ? "true" : "false";
                }
                else if (split(s, ">").Length == 2)
                {
                    string a = parseMath(split(s, ">")[0]), b = parseMath(split(s, ">")[1]);
                    if (a == "unknown" || b == "unknown")
                        return "unknown";
                    return Convert.ToDouble(a) > Convert.ToDouble(b) ? "true" : "false";
                }
                else if (split(s, "<").Length == 2)
                {
                    string a = parseMath(split(s, "<")[0]), b = parseMath(split(s, "<")[1]);
                    if (a == "unknown" || b == "unknown")
                        return "unknown";
                    return Convert.ToDouble(a) < Convert.ToDouble(b) ? "true" : "false";
                }
                else if (split(s, "=").Length == 2)
                {
                    string a = parseMath(split(s, "=")[0]), b = parseMath(split(s, "=")[1]);
                    if (vars.ContainsKey(split(s, "=")[0] + "=" + b))
                        return vars[split(s, "=")[0] + "=" + b];
                    if (vars.ContainsKey(a + "=" + split(s, "=")[1]))
                        return vars[a + "=" + split(s, "=")[1]];
                    if (a == "unknown" || b == "unknown")
                        return "unknown";
                    return a == b ? "true" : "false";
                }
                else if (singlesplit(s, "+").Length == 2 || singlesplit(s, "`").Length == 2)
                {
                    List<string> nums = new List<string>();
                    List<char> ops = new List<char>();
                    string temp = "";
                    foreach (char i in s)
                    {
                        if (i == '+' || i == '`')
                        {
                            nums.Add(temp);
                            ops.Add(i);
                            temp = "";
                        }
                        else
                            temp += i;
                    }
                    nums.Add(temp);
                    foreach (char i in range(0, ops.Count, 1))
                    {
                        if (ops[0] == '+')
                        {
                            try
                            {
                                nums[1] = Convert.ToString(Convert.ToDouble(parseMath(nums[0])) + Convert.ToDouble(parseMath(nums[1])));
                            }
                            catch
                            {
                                if (nums[1] == "" || nums[0] == "") throw new StackOverflowException();
                                nums[1] = parseMath(nums[0]) + "+" + parseMath(nums[1]);
                            }
                            nums.RemoveAt(0);
                            ops.RemoveAt(0);
                        }
                        else if (ops[0] == '`')
                        {
                            try
                            {
                                nums[1] = Convert.ToString(Convert.ToDouble(parseMath(nums[0])) - Convert.ToDouble(parseMath(nums[1])));
                            }
                            catch
                            {
                                if (nums[1] == "" || nums[0] == "") throw new StackOverflowException();
                                nums[1] = parseMath(nums[0]) + "-" + parseMath(nums[1]);
                            }
                            nums.RemoveAt(0);
                            ops.RemoveAt(0);
                        }
                    }
                    s = nums[0];
                }
                else if (singlesplit(s, "*").Length == 2 || singlesplit(s, "/").Length == 2)
                {
                    List<string> nums = new List<string>();
                    List<char> ops = new List<char>();
                    string temp = "";
                    foreach (char i in s)
                    {
                        if (i == '*' || i == '/')
                        {
                            nums.Add(temp);
                            ops.Add(i);
                            temp = "";
                        }
                        else
                            temp += i;
                    }
                    nums.Add(temp);
                    foreach (int i in range(0, ops.Count, 1))
                    {
                        if (ops[0] == '*')
                        {
                            try
                            {
                                nums[1] = Convert.ToString(Convert.ToDouble(parseMath(nums[0])) * Convert.ToDouble(parseMath(nums[1])));
                            }
                            catch
                            {
                                if (nums[1] == "" || nums[0] == "") throw new StackOverflowException();
                                nums[1] = parseMath(nums[0]) + "*" + parseMath(nums[1]);
                            }
                            nums.RemoveAt(0);
                            ops.RemoveAt(0);
                        }
                        else if (ops[0] == '/')
                        {
                            try
                            {
                                nums[1] = Convert.ToString(Convert.ToDouble(parseMath(nums[0])) / Convert.ToDouble(parseMath(nums[1])));
                            }
                            catch
                            {
                                if (nums[1] == "" || nums[0] == "") throw new StackOverflowException();
                                nums[1] = parseMath(nums[0]) + "/" + parseMath(nums[1]);
                            }
                            nums.RemoveAt(0);
                            ops.RemoveAt(0);
                        }
                    }
                    s = nums[0];
                }
                else if (singlesplit(s, "^").Length == 2)
                {
                    List<string> nums = new List<string>();
                    string temp = "";
                    foreach (char i in s)
                    {
                        if (i == '^')
                        {
                            nums.Add(temp);
                            temp = "";
                        }
                        else
                            temp += i;
                    }
                    nums.Add(temp);
                    nums.Reverse();
                    foreach (int i in range(0, nums.Count - 1, 1))
                    {
                        try
                        {
                            nums[0] = Convert.ToString(Math.Pow(Convert.ToDouble(parseMath(nums[1])), Convert.ToDouble(parseMath(nums[0]))));
                        }
                        catch
                        {
                            if (nums[1] == "" || nums[0] == "") throw new StackOverflowException();
                            nums[0] = parseMath(nums[1]) + "^" + parseMath(nums[0]);
                        }
                        nums.RemoveAt(1);
                    }
                    s = nums[0];
                }
                else
                {
                    foreach (string stemp in wildcards.Keys.ToArray().Reverse())
                    {
                        Tuple<bool, Dictionary<string, string>> temp = matches(s, stemp);
                        if (temp.Item1)
                        {
                            s = wildcards[stemp];
                            foreach (string sdict in temp.Item2.Keys)
                                s = mapToVars("_let_" + sdict + "=" + temp.Item2[sdict], s);
                            s = parseMath(s);
                            foreach (string sdict in temp.Item2.Keys)
                                forget("_let_" + sdict + "=" + temp.Item2[sdict]);
                        }
                    }
                }
                s = testinternal(s);
                if (vars.ContainsKey(s))
                    s = parseMath(vars[s]);
                if (singlesplit(s, ".").Length == 2 && singlesplit(s, ".")[1] == "0")
                    s = singlesplit(s, ".")[0];
                return s;
            }
            catch
            {
                Console.WriteLine("Internal Error: Math Parser crashed upon parsing. Aborting operation.");
                return "";
            }
        }

        public static string ParseMath(string s)
        {
            if (split(s, "//").Length > 1) s = split(s, "//")[0];
            string ret = parseMath(s);
            if (CAS)
                return ret;
            else
                try
                {
                    if (ret == "true" || ret == "false" || ret == "unknown" || ret == "infinity" || isarray(ret) || Convert.ToString(Convert.ToDouble(ret)) == ret)
                    {
                        return ret;
                    }
                }
                catch
                {
                    return "Error: Could not parse query: \"" + s + "\"";
                }
            return "";
        }

        private static bool let(string s, bool suppressdup)
        {
            string varname = fixneg(remspaces(split(s.Substring(5), "=")[0]));
            string valname = remstartspaces(singlesplit(s.Substring(5), "=")[1]);
            bool haswildcard = false;
            int i = 0;
            while (varname.ToList().Contains('(') && (!varname.ToList().Contains(')')))
            {
                varname += "=" + remspaces(split(s.Substring(5), "=")[++i]);
                valname = remstartspaces(singlesplit(valname, "=")[1]);
            }
            if (varname.ToList().Contains('(')) varname = betwparen(varname)[0];
            if (varname == valname)
            {
                Console.WriteLine("Error: Cannot bind variable to itself, tried to bind \"" + varname + "\" to \"" + valname + "\"");
                return true;
            }
            if (vars.ContainsKey(varname))
            {
                if (!suppressdup) Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + vars[varname] + "\"");
                return true;
            }
            if (wildcards.ContainsKey(varname))
            {
                if (!suppressdup) Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + wildcards[varname] + "\"");
                return true;
            }
            double trash;
            if (double.TryParse(varname, out trash))
            {
                Console.WriteLine("Error: Cannot bind directly to a numberic value, tried to bind to \"" + varname + "\"");
                return true;
            }
            if (split(s, "_if_").Length == 2 && split(s, "_then_").Length == 1)
            {
                string res = ParseMath(split(s, "_if_")[1]);
                if (res == "false")
                    return true;
                else if (res == "true")
                {
                    valname = remspaces(split(valname, "_if_")[0]);
                    if (varname == valname)
                    {
                        Console.WriteLine("Error: Cannot bind variable to itself, tried to bind \"" + varname + "\" to \"" + valname + "\"");
                        return true;
                    }
                    foreach (char j in varname)
                    {
                        if (j >= 'A' && j <= 'Z')
                        {
                            haswildcard = true;
                            break;
                        }
                    }
                    if (!haswildcard)
                        vars.Add(varname, valname);
                    else
                        wildcards.Add(varname, valname);
                }
                else return true;
            }
            else
            {
                foreach (char j in varname)
                {
                    if (j >= 'A' && j <= 'Z')
                    {
                        haswildcard = true;
                        break;
                    }
                }
                if (!haswildcard)
                    vars.Add(varname, valname);
                else
                    wildcards.Add(varname, valname);
            }
            return true;
        }

        private static string mapToVars(string s, string mapto)
        {
            string varname = remspaces(split(s.Substring(5), "=")[0]);
            string valname = remstartspaces(singlesplit(s.Substring(5), "=")[1]);
            int i = 0;
            while (varname.ToList().Contains('(') && (!varname.ToList().Contains(')')))
            {
                varname += "=" + remspaces(split(s.Substring(5), "=")[++i]);
                valname = remstartspaces(singlesplit(valname, "=")[1]);
            }
            if (varname.ToList().Contains('(')) varname = betwparen(varname)[0];
            if (varname == valname)
            {
                Console.WriteLine("Error: Cannot bind variable to itself, tried to bind \"" + varname + "\" to \"" + valname + "\"");
                return "";
            }
            else
            {
                mapto = " " + mapto + " ";
                for (int j = 0; j < mapto.Length - varname.Length + 1; j++)
                {
                    if (mapto.Substring(j, varname.Length) == varname)
                    {
                        mapto = mapto.Substring(0, j) + valname + mapto.Substring(j + varname.Length);
                    }
                }
            }
            return mapto;
        }

        private static bool notlet(string s)
        {
            string varname = fixneg(remspaces(split(s.Substring(5), "/=")[0]));
            string valname = remstartspaces(singlesplit(s.Substring(5), "/=")[1]);
            bool haswildcard = false;
            int i = 0;
            while (varname.ToList().Contains('(') && (!varname.ToList().Contains(')')))
            {
                varname += "/=" + remspaces(split(s.Substring(5), "/=")[++i]);
                valname = remstartspaces(singlesplit(valname, "/=")[1]);
            }
            if (varname.ToList().Contains('(')) varname = betwparen(varname)[0];
            if (varname == valname)
            {
                Console.WriteLine("Error: Cannot break reflexive law of equality, tried to say \"" + varname + "\" is not equal to \"" + valname + "\"");
                return true;
            }
            if (vars.ContainsKey(varname))
                Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + vars[varname] + "\"");
            else if (split(s, "_if_").Length == 2 && split(s, "_then_").Length == 1)
            {
                string res = ParseMath(split(s, "_if_")[1]);
                if (res == "false")
                    return true;
                else if (res == "true")
                {
                    valname = remspaces(split(valname, "_if_")[0]);
                    if (varname == valname)
                    {
                        Console.WriteLine("Error: Cannot break reflexive law of equality, tried to say \"" + varname + "\" is not equal to \"" + valname + "\"");
                        return true;
                    }
                    foreach (char j in valname)
                    {
                        if (j >= 'A' && j <= 'Z')
                        {
                            haswildcard = true;
                            break;
                        }
                    }
                    if (!haswildcard)
                    {
                        vars.Add(varname, "unknown");
                        vars.Add(varname + "=" + valname, "false");
                        vars.Add(valname + "=" + varname, "false");
                        vars.Add(varname + "/=" + valname, "true");
                        vars.Add(valname + "/=" + varname, "true");
                    }
                    else
                    {
                        wildcards.Add(varname, "unknown");
                        wildcards.Add(varname + "=" + valname, "false");
                        wildcards.Add(valname + "=" + varname, "false");
                        wildcards.Add(varname + "/=" + valname, "true");
                        wildcards.Add(valname + "/=" + varname, "true");
                    }

                }
                else return true;
            }
            else
            {
                foreach (char j in valname)
                {
                    if (j >= 'A' && j <= 'Z')
                    {
                        haswildcard = true;
                        break;
                    }
                }
                if (!haswildcard)
                {
                    vars.Add(varname, "unknown");
                    vars.Add(varname + "=" + valname, "false");
                    vars.Add(valname + "=" + varname, "false");
                    vars.Add(varname + "/=" + valname, "true");
                    vars.Add(valname + "/=" + varname, "true");
                }
                else
                {
                    wildcards.Add(varname, "unknown");
                    wildcards.Add(varname + "=" + valname, "false");
                    wildcards.Add(valname + "=" + varname, "false");
                    wildcards.Add(varname + "/=" + valname, "true");
                    wildcards.Add(valname + "/=" + varname, "true");
                }
            }
            return true;
        }

        private static bool forget(string s)
        {
            string varname = fixneg(remspaces(split(s.Substring(5), "=")[0]));
            bool haswildcard = false;
            int i = 0;
            while (varname.ToList().Contains('(') && (!varname.ToList().Contains(')')))
            {
                varname += "=" + remspaces(split(s.Substring(5), "=")[++i]);
            }
            if (varname.ToList().Contains('(')) varname = betwparen(varname)[0];
            if (split(s, "_if_").Length == 2 && split(s, "_then_").Length == 1)
            {
                string res = ParseMath(split(s, "_if_")[1]);
                if (res == "false")
                    return true;
                else if (res == "true")
                {
                    foreach (char j in varname)
                    {
                        if (j >= 'A' && j <= 'Z')
                        {
                            haswildcard = true;
                            break;
                        }
                    }
                    if (!haswildcard)
                        vars.Remove(varname);
                    else
                        wildcards.Remove(varname);
                }
                else return true;
            }
            else
            {
                foreach (char j in varname)
                {
                    if (j >= 'A' && j <= 'Z')
                    {
                        haswildcard = true;
                        break;
                    }
                }
                if (!haswildcard)
                    vars.Remove(varname);
                else
                    wildcards.Remove(varname);
            }
            return true;
        }

        public static bool RunCommands(string s)
        {
            if (split(s, "//").Length > 1) s = split(s, "//")[0];
            s = remstartspaces(s);
            try
            {
                if (s.Substring(0, 5) == "_let_")
                {
                    if (split(s, "=")[0].Substring((split(s, "=")[0].Length - 1)) == "/")
                    {
                        return notlet(s);
                    }
                    return let(s, false);
                }
                else if (s == "##disableCAS")
                {
                    CAS = false;
                    return true;
                }
                else if (s == "##enableCAS")
                {
                    CAS = true;
                    return true;
                }
            }
            catch
            { }
            return false;
        }

        private static bool IsntSurrounded(string s, int i, int length)
        {
            return (!(s.Substring(i - 1)[0] >= 'A' && s.Substring(i - 1)[0] <= 'Z') && !(s.Substring(i - 1)[0] >= 'a' && s.Substring(i - 1)[0] <= 'z') && !(s.Substring(i + length)[0] >= 'A' && s.Substring(i + length)[0] <= 'Z') && !(s.Substring(i + length)[0] >= 'a' && s.Substring(i + length)[0] <= 'z'));
        }

        public static string Preprocessor(string s)
        {
            s = " " + s;
            for (int i = 0; i < s.Length; i++)
            {
                try
                {
                    if (s.Substring(i, 2) == "if" && IsntSurrounded(s, i, 2))
                        s = s.Substring(0, i) + "_if_" + s.Substring(i + 2);
                    else if (s.Substring(i, 4) == "then" && IsntSurrounded(s, i, 4))
                        s = s.Substring(0, i) + "_then_" + s.Substring(i + 4);
                    else if (s.Substring(i, 4) == "else" && IsntSurrounded(s, i, 4))
                        s = s.Substring(0, i) + "_else_" + s.Substring(i + 4);
                    else if (s.Substring(i, 3) == "let" && IsntSurrounded(s, i, 3))
                        s = s.Substring(0, i) + "_let_" + s.Substring(i + 3);
                    else if (s.Substring(i, 3) == "and" && IsntSurrounded(s, i, 3))
                        s = s.Substring(0, i) + "_and_" + s.Substring(i + 3);
                    else if (s.Substring(i, 2) == "or" && IsntSurrounded(s, i, 2))
                        s = s.Substring(0, i) + "_or_" + s.Substring(i + 2);
                    else if (s.Substring(i, 5) == "where" && IsntSurrounded(s, i, 5))
                        s = s.Substring(0, i) + "_where_" + s.Substring(i + 5);
                    else
                        i--;
                    i++;
                }
                catch
                { }
            }
            return s;
        }

        public static void EffPrep(string s)
        {
            s = " " + s;
            for (int i = 0; i < s.Length; i++)
            {
                try
                {
                    if (s.Substring(i, 3) == "sin" && IsntSurrounded(s, i, 3) && !wildcards.ContainsKey("sin[X]"))
                        wildcards.Add("sin[X]", "internalcmmdsineX");
                    else if (s.Substring(i, 3) == "cos" && IsntSurrounded(s, i, 3) && !wildcards.ContainsKey("cos[X]"))
                        wildcards.Add("cos[X]", "internalcmmdcosiX");
                    else if (s.Substring(i, 3) == "tan" && IsntSurrounded(s, i, 3) && !wildcards.ContainsKey("tan[X]"))
                        wildcards.Add("tan[X]", "internalcmmdtangX");
                    else if (s.Substring(i, 4) == "asin" && IsntSurrounded(s, i, 4) && !wildcards.ContainsKey("asin[X]"))
                        wildcards.Add("asin[X]", "internalcmmdasinX");
                    else if (s.Substring(i, 4) == "acos" && IsntSurrounded(s, i, 4) && !wildcards.ContainsKey("acos[X]"))
                        wildcards.Add("acos[X]", "internalcmmdacosX");
                    else if (s.Substring(i, 4) == "atan" && IsntSurrounded(s, i, 4) && !wildcards.ContainsKey("atan[X]"))
                        wildcards.Add("atan[X]", "internalcmmdatanX");
                    else if (s.Substring(i, 5) == "print" && IsntSurrounded(s, i, 5) && !wildcards.ContainsKey("print[X]"))
                        wildcards.Add("print[X]", "internalcmmdprntX");
                    else if (s.Substring(i, 5) == "floor" && IsntSurrounded(s, i, 5) && !wildcards.ContainsKey("floor[X]"))
                        wildcards.Add("floor[X]", "internalcmmdflorX");
                    else if (s.Substring(i, 7) == "ceiling" && IsntSurrounded(s, i, 7) && !wildcards.ContainsKey("ceiling[X]"))
                        wildcards.Add("ceiling[X]", "internalcmmdceilX");
                    else if (s.Substring(i, 3) == "map" && IsntSurrounded(s, i, 3) && !wildcards.ContainsKey("map[F,L]"))
                        wildcards.Add("map[F,L]", "internalcmmdmapfF,L");
                    else if (s.Substring(i, 5) == "round" && IsntSurrounded(s, i, 5) && !wildcards.ContainsKey("round[X]"))
                        wildcards.Add("round[X]", "internalcmmdrondX");
                    else if (s.Substring(i, 3) == "max" && IsntSurrounded(s, i, 3) && !wildcards.ContainsKey("max[A,B]"))
                        wildcards.Add("max[A,B]", "_if_A>B_then_A_else_B");
                    else if (s.Substring(i, 3) == "min" && IsntSurrounded(s, i, 3) && !wildcards.ContainsKey("min[A,B]"))
                        wildcards.Add("min[A,B]", "_if_A<B_then_A_else_B");
                    else if (s.Substring(i, 3) == "abs" && IsntSurrounded(s, i, 3) && !wildcards.ContainsKey("abs[X]"))
                        wildcards.Add("abs[X]", "_if_X<0_then_-(X)_else_X");
                    else if (s.Substring(i, 2) == "ln" && IsntSurrounded(s, i, 2) && !wildcards.ContainsKey("ln[X]"))
                        wildcards.Add("ln[X]", "internalcmmdnlogX,e");
                    else if (s.Substring(i, 3) == "log" && IsntSurrounded(s, i, 3) && !wildcards.ContainsKey("log[X]"))
                    {
                        wildcards.Add("log[X]", "internalcmmdnlogX,10");
                        wildcards.Add("log[X,Y]", "internalcmmdnlogX,Y");
                    }
                    else if (s.Substring(i, 4) == "sign" && IsntSurrounded(s, i, 4) && !wildcards.ContainsKey("sign[X]"))
                        wildcards.Add("sign[X]", "_if_X=0_then_0_else__if_X>0_then_1_else_-1");
                    else if (s.Substring(i, 4) == "sqrt" && IsntSurrounded(s, i, 4) && !wildcards.ContainsKey("sqrt[X]"))
                        wildcards.Add("sqrt[X]", "X^0.5");
                    else if (s.Substring(i, 4) == "cbrt" && IsntSurrounded(s, i, 4) && !wildcards.ContainsKey("cbrt[X]"))
                        wildcards.Add("cbrt[X]", "X^(1/3)");
                    else if (s.Substring(i, 3) == "nrt" && IsntSurrounded(s, i, 3) && !wildcards.ContainsKey("nrt[X,N]"))
                        wildcards.Add("nrt[X,N]", "X^(1/N)");
                    else if (s.Substring(i, 8) == "truncate" && IsntSurrounded(s, i, 8) && !wildcards.ContainsKey("truncate[X]"))
                        wildcards.Add("truncate[X]", "internalcmmdtrunX");
                    else if (s.Substring(i, 10) == "reciprocal" && IsntSurrounded(s, i, 10) && !wildcards.ContainsKey("reciprocal[X]"))
                        wildcards.Add("reciprocal[X]", "1/X");
                    else if (s.Substring(i, 6) == "filter" && IsntSurrounded(s, i, 6) && !wildcards.ContainsKey("filter[F,L]"))
                        wildcards.Add("filter[F,L]", "internalcmmdfiltF,L");
                }
                catch
                { }
            }
        }
    }
    class MainLoop
    {
        private static void AddData()
        {
            Parser.wildcards.Add("X!", "internalcmmdgamaX+1");
            Parser.wildcards.Add("X{A}", "internalcmmdgetlX,A");
            Parser.wildcards.Add("sin[X]", "internalcmmdsineX");
            Parser.wildcards.Add("cos[X]", "internalcmmdcosiX");
            Parser.wildcards.Add("tan[X]", "internalcmmdtangX");
            Parser.wildcards.Add("asin[X]", "internalcmmdasinX");
            Parser.wildcards.Add("acos[X]", "internalcmmdacosX");
            Parser.wildcards.Add("atan[X]", "internalcmmdatanX");
            Parser.wildcards.Add("print[X]", "internalcmmdprntX");
            Parser.wildcards.Add("floor[X]", "internalcmmdflorX");
            Parser.wildcards.Add("ceiling[X]", "internalcmmdceilX");
            Parser.wildcards.Add("map[F,L]", "internalcmmdmapfF,L");
            Parser.wildcards.Add("filter[F,L]", "internalcmmdfiltF,L");
            Parser.wildcards.Add("round[X]", "floor{X+.5}");
            Parser.wildcards.Add("max[A,B]", "_if_A>B_then_A_else_B");
            Parser.wildcards.Add("min[A,B]", "_if_A<B_then_A_else_B");
            Parser.wildcards.Add("abs[X]", "_if_X<0_then_-(X)_else_X");
            Parser.wildcards.Add("A%B", "internalcmmdmoduA,B");
            Parser.wildcards.Add("ln[X]", "internalcmmdnlogX,e");
            Parser.wildcards.Add("log[X]", "internalcmmdnlogX,10");
            Parser.wildcards.Add("log[X,Y]", "internalcmmdnlogX,Y");
            Parser.wildcards.Add("sign[X]", "_if_X=0_then_0_else__if_X>0_then_1_else_-1");
            Parser.wildcards.Add("sqrt[X]", "X^0.5");
            Parser.wildcards.Add("cbrt[X]", "X^(1/3)");
            Parser.wildcards.Add("nrt[X,N]", "X^(1/N)");
            Parser.wildcards.Add("truncate[X]", "internalcmmdtrunX");
            Parser.wildcards.Add(".X", "0.X");
            Parser.wildcards.Add("-.X", "-0.X");
            Parser.wildcards.Add("--X", "X");
            Parser.wildcards.Add("X*0", "0");
            Parser.wildcards.Add("0*X", "0");
            Parser.wildcards.Add("X*1", "X");
            Parser.wildcards.Add("1*X", "X");
            Parser.wildcards.Add("0+X", "X");
            Parser.wildcards.Add("X+0", "X");
            Parser.wildcards.Add("0`X", "-X");
            Parser.wildcards.Add("X/1", "X");
            Parser.wildcards.Add("0/X", "0");
            Parser.wildcards.Add("reciprocal[X]", "1/X");
            Parser.vars.Add("pi", Convert.ToString(Math.PI));
            Parser.vars.Add("e", Convert.ToString(Math.E));
            Parser.vars.Add("∞", "infinity");
        }

        private static void AddNecData()
        {
            Parser.wildcards.Add("X!", "internalcmmdgamaX+1");
            Parser.wildcards.Add("X{A}", "internalcmmdgetlX,A");
            Parser.wildcards.Add("A%B", "internalcmmdmoduA,B");
            Parser.wildcards.Add(".X", "0.X");
            Parser.wildcards.Add("-.X", "-0.X");
            Parser.wildcards.Add("--X", "X");
            Parser.wildcards.Add("X*0", "0");
            Parser.wildcards.Add("0*X", "0");
            Parser.wildcards.Add("X*1", "X");
            Parser.wildcards.Add("1*X", "X");
            Parser.wildcards.Add("0+X", "X");
            Parser.wildcards.Add("X+0", "X");
            Parser.wildcards.Add("0`X", "-X");
            Parser.wildcards.Add("X/1", "X");
            Parser.wildcards.Add("0/X", "0");
            Parser.vars.Add("pi", Convert.ToString(Math.PI));
            Parser.vars.Add("e", Convert.ToString(Math.E));
            Parser.vars.Add("∞", "infinity");
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                AddData();
                Console.Write("Build 1.2, H# Interactive\nPhil Lane Creations\n");
                string inp;
                while (true)
                {
                    Console.Write("HSCi> ");
                    inp = Parser.Preprocessor(Console.ReadLine());
                    if (!Parser.RunCommands(inp)) Console.WriteLine(Parser.ParseMath(inp));
                }
            }
            else
            {
                AddNecData();
                string inp = File.ReadAllText(args[0]);
                List<string> lines = new List<string>();
                string temp = "";
                for (int i = 0; i < inp.Length; i++)
                {
                    try
                    {
                        if (inp.Substring(i, 2) == Environment.NewLine)
                            i++;
                        else
                        {
                            if (inp[i] == ';')
                            {
                                lines.Add(temp);
                                temp = "";
                            }
                            else
                                temp += inp[i];
                        }
                    }
                    catch
                    {
                    }
                }
                lines.Add(temp);
                for (int i = 0; i < lines.Count; i++)
                {
                    lines[i] = Parser.Preprocessor(lines[i]);
                    Parser.EffPrep(lines[i]);
                }
                for (int i = 0; i < lines.Count; i++)
                    if (!Parser.RunCommands(lines[i])) Parser.ParseMath(lines[i]);
                Console.WriteLine("\nProgram completed, press any key to exit."); Console.ReadKey();
            }
        }
    }
}
