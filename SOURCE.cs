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

        private static int[] range(int a, int b)
        {
            bool backs = false;
            if (a > b)
            {
                backs = true;
                int c = ++a;
                a = --b;
                b = c;
            }
            List<int> res = new List<int>();
            for (int i = a; i < b; i++)
            {
                res.Add(i);
            }
            if (backs) res.Reverse();
            return res.ToArray();
        }

        public static Tuple<bool, Dictionary<string, string>> matches(string test, string str)
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
                        while ((testtemp[0] >= '0' && testtemp[0] <= '9') || testtemp[0] == '-' || testtemp[0] == '.' || (testtemp[0] >= 'a' && testtemp[0] <= 'z'))
                        {
                            dicttempvals += testtemp[0];
                            testtemp = testtemp.Substring(1);
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
            foreach (int i in range(0, s.Length))
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
            foreach (int i in range(0, s.Length))
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
                    if (!(ret == "true" || ret == "false" || ret == "unknown" || Convert.ToString(Convert.ToDouble(ret)) == ret))
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
                s = vars[s];
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
                s = vars[s];
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
                s = vars[s];
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
                s = vars[s];
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
                s = vars[s];
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
                s = vars[s];
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
                s = vars[s];
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot floor a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Floor(num);
            return Convert.ToString(num);
        }

        private static string trun(string s)
        {
            double num = 0;
            if (vars.ContainsKey(s))
                s = vars[s];
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
                s = vars[s];
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot ceiling a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Ceiling(num);
            return Convert.ToString(num);
        }

        private static string nlog(string s, string base_)
        {
            double num = 0, num1 = 0;
            if (vars.ContainsKey(s))
                s = vars[s];
            if (vars.ContainsKey(base_))
                base_ = vars[base_];
            if (!double.TryParse(s, out num))
            {
                Console.WriteLine("Error: Cannot take natural log of a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            if (!double.TryParse(base_, out num1))
            {
                Console.WriteLine("Error: Cannot take natural log with base of a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            num = Math.Log(num, num1);
            return Convert.ToString(num);
        }

        private static string prnt(string s)
        {
            if (vars.ContainsKey(s))
                s = vars[s];
            Console.WriteLine(s);
            return s;
        }

        public static string gama(string s)
        {
            double alpha = 0;
            if (vars.ContainsKey(s))
                s = vars[s];
            if (!double.TryParse(s, out alpha))
            {
                Console.WriteLine("Error: Cannot take factorial of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
                return "";
            }
            if (Math.Sign(alpha) == -1)
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

        private static string parseMath(string s)
        {
            double trash;
            if (double.TryParse(s, out trash)) return remspaces(s);
            try
            {
                s = remspaces(s);
                s = fixneg(s);
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
                            case "ceil":
                                s = ceil(s.Substring(16));
                                break;
                            case "trun":
                                s = trun(s.Substring(16));
                                break;
                            case "nlog":
                                s = nlog(split(s.Substring(16), ",")[0], split(s.Substring(16), ",")[1]);
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
                {
                    foreach (string i in betwparen(s))
                        s = split(s, "(")[0] + parseMath(i) + s.Substring(split(s, "(")[0].Length + i.Length + 2);
                }
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
                    if (split(temps, "/=").Length > 1)
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
                    {
                        Console.WriteLine("Error: Expected boolean expression, received \"" + split(split(s, "_if_")[1], "_then_")[0] + "\"");
                    }
                    else if (parseMath(split(split(s, "_if_")[1], "_then_")[0]) == "true")
                    {
                        try
                        {
                            s = parseMath(split(split(s, "_then_")[1], "_else_")[0]);
                        }
                        catch
                        { }
                    }
                    else if (parseMath(split(split(s, "_if_")[1], "_then_")[0]) == "false")
                    {
                        s = parseMath(singlesplit(s, "_else_")[1]);
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
                                    args = range(Convert.ToInt32(parseMath(split(i, "..")[0])), Convert.ToInt32(parseMath(split(i, "..")[1])) + 1).ToList().Select(x => Convert.ToString(x)).ToList();
                                }
                                else
                                {
                                    b = false;
                                }
                            }
                            if (b == false)
                            {
                                args = listsplit(i, ",").Cast<string>().Select(x => parseMath(x)).ToList();
                            }
                            for (int j = 0; j < args.Count - 1; j++)
                            {
                                res += args[j] + ",";
                            }
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
                        {
                            s = s.Substring(0, i) + '[' + s.Substring(i + 1);
                        }
                        if (s[i] == '»')
                        {
                            s = s.Substring(0, i) + ']' + s.Substring(i + 1);
                        }
                    }
                    s = s.Substring(1);
                }
                if (betwbraces(s).Length > 0)
                {
                    foreach (string i in betwbraces(s))
                    {
                        try
                        {
                            string res = "";
                            List<string> args = new List<string>();
                            args = listsplit(i, ",").Cast<string>().Select(x => parseMath(x)).ToList();
                            for (int j = 0; j < args.Count - 1; j++)
                            {
                                res += args[j] + ",";
                            }
                            res += args.Last();
                            s = split(s, "{")[0] + "«" + res + "»" + s.Substring(split(s, "{")[0].Length + i.Length + 2);
                        }
                        catch
                        {
                            Console.WriteLine("Error: Could not parse function parameter list \"{" + i + "}\". Assuming {}, resuming operation.");
                            s = split(s, "{")[0] + "«" + "»" + s.Substring(split(s, "{")[0].Length + i.Length + 2);
                        }
                    }
                    s = " " + s;
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] == '«')
                        {
                            s = s.Substring(0, i) + '{' + s.Substring(i + 1);
                        }
                        if (s[i] == '»')
                        {
                            s = s.Substring(0, i) + '}' + s.Substring(i + 1);
                        }
                    }
                    s = s.Substring(1);
                }
                if (split(s, "_and_").Length > 1)
                {
                    string[] args = split(s, "_and_");
                    foreach (string i in args)
                    {
                        if (parseMath(i) == "false")
                            return "false";
                    }
                    return "true";
                }
                else if (split(s, "_or_").Length > 1)
                {
                    string[] args = split(s, "_or_");
                    foreach (string i in args)
                    {
                        if (parseMath(i) == "true")
                            return "true";
                    }
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
                    foreach (char i in range(0, ops.Count))
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
                    foreach (int i in range(0, ops.Count))
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
                    foreach (int i in range(0, nums.Count - 1))
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
                    foreach (char j in valname)
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
                    {
                        wildcards.Add(varname, valname);
                    }
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
                    vars.Remove(varname);
                }
                else return true;
            }
            else
                vars.Remove(varname);
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
    }
    class MainLoop
    {
        private static void AddData()
        {
            Parser.wildcards.Add("X!", "internalcmmdgamaX+1");
            Parser.wildcards.Add("sin{X}", "internalcmmdsineX");
            Parser.wildcards.Add("cos{X}", "internalcmmdcosiX");
            Parser.wildcards.Add("tan{X}", "internalcmmdtangX");
            Parser.wildcards.Add("asin{X}", "internalcmmdasinX");
            Parser.wildcards.Add("acos{X}", "internalcmmdacosX");
            Parser.wildcards.Add("atan{X}", "internalcmmdatanX");
            Parser.wildcards.Add("print{X}", "internalcmmdprntX");
            Parser.wildcards.Add("floor{X}", "internalcmmdflorX");
            Parser.wildcards.Add("ceiling{X}", "internalcmmdceilX");
            Parser.wildcards.Add("round{X}", "floor{X+.5}");
            Parser.wildcards.Add("max{A,B}", "_if_A>B_then_A_else_B");
            Parser.wildcards.Add("min{A,B}", "_if_A<B_then_A_else_B");
            Parser.wildcards.Add("abs{X}", "_if_X<0_then_-(X)_else_X");
            Parser.wildcards.Add("A%B", "A-B*floor{A/B}");
            Parser.wildcards.Add("ln{X}", "internalcmmdnlogX,e");
            Parser.wildcards.Add("log{X}", "internalcmmdnlogX,10");
            Parser.wildcards.Add("log{X,Y}", "internalcmmdnlogX,Y");
            Parser.wildcards.Add("sign{X}", "_if_X=0_then_0_else__if_X>0_then_1_else_-1");
            Parser.wildcards.Add("sqrt{X}", "X^.5");
            Parser.wildcards.Add("cbrt{X}", "X^(1/3)");
            Parser.wildcards.Add("nrt{X,N}", "X^(1/N)");
            Parser.wildcards.Add("truncate{X}", "internalcmmdtrunX");
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
            Parser.wildcards.Add("reciprocal{X}", "1/X");
            Parser.vars.Add("pi", Convert.ToString(Math.PI));
            Parser.vars.Add("e", Convert.ToString(Math.E));
            Parser.vars.Add("∞", "infinity");
        }

        static void Main(string[] args)
        {
            AddData();
            if (args.Length == 0)
            {
                Console.Write("Build 1.1, H# Interactive\nPhil Lane Creations\n");
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
                    lines[i] = Parser.Preprocessor(lines[i]);
                for (int i = 0; i < lines.Count; i++)
                    if (!Parser.RunCommands(lines[i])) Parser.ParseMath(lines[i]);
                Console.WriteLine("\nProgram completed, press any key to exit."); Console.ReadKey();
            }
        }
    }
}