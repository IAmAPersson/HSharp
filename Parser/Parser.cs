using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class Parser
{
    public static Dictionary<string, string> vars = new Dictionary<string, string>();
    public static Dictionary<string, string> wildcards = new Dictionary<string, string>();
    private static bool CAS = true;
    public static bool IO = false;
    private static string currfunc = "";
    private static int lamcount = 0;
    delegate string del(string x);

    private static void Exec(string[] args)
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
        string[] orig = lines.ToArray();
        for (int i = 0; i < lines.Count; i++)
            lines[i] = Preprocessor.Preprocess(lines[i]);
        for (int i = 0; i < lines.Count; i++)
            if (!RunCommands(lines[i], args))
            {
                string res = Parser.ParseMath(lines[i]);
                if (!new[] { "unit", "" }.Contains(res))
                {
                    Console.WriteLine("Error: Unhandled value from line " + (i + 1) + " `" + orig[i] + "`, left excess value `" + res + "`. Try piping result to \"ignore\" function.");
                    break;
                }
            }
    }

    private static Tuple<bool, Dictionary<string, string>> matches(string test, string str)
    {
        str = " " + str + " ";
        test = " " + test + " ";
        Dictionary<string, string> varlist = new Dictionary<string, string>();
        string dicttempkey;
        string dicttempvals;
        bool rest = false;
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
                    while ((strtemp[0] >= 'a' && strtemp[0] <= 'z') || (strtemp[0] >= 'A' && strtemp[0] <= 'Z') || strtemp[0] == 92)
                    {
                        if (strtemp[0] != 92)
                        {
                            dicttempkey += strtemp[0];
                            strtemp = strtemp.Substring(1);
                        }
                        else
                        {
                            rest = true;
                            break;
                        }
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
                        else if (testtemp[0] == '-' && dicttempvals.Length > 0) break;
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
                    if (rest)
                    {
                        test = '\\' + test;
                        rest = false;
                    }
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

    private static string testinternal(string s)
    {
        try
        {
            if (s.Length >= 16 && s.Substring(0, 12) == "internalcmmd")
            {
                switch (s.Substring(12, 4))
                {
                    case "sine":
                        s = InternalCommands.sine(s.Substring(16));
                        break;
                    case "cosi":
                        s = InternalCommands.cosi(s.Substring(16));
                        break;
                    case "tang":
                        s = InternalCommands.tang(s.Substring(16));
                        break;
                    case "asin":
                        s = InternalCommands.asin(s.Substring(16));
                        break;
                    case "acos":
                        s = InternalCommands.acos(s.Substring(16));
                        break;
                    case "atan":
                        s = InternalCommands.atan(s.Substring(16));
                        break;
                    case "leng":
                        s = InternalCommands.leng(s.Substring(16));
                        break;
                    case "prnt":
                        if (IO)
                            s = InternalCommands.prnt(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "pstr":
                        if (IO)
                            s = InternalCommands.pstr(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "istr":
                        if (IO)
                            s = InternalCommands.istr(s.Substring(16));
                        break;
                    case "inum":
                        if (IO)
                            s = InternalCommands.inum(s.Substring(16));
                        break;
                    case "qurs":
                        if (IO)
                            s = InternalCommands.qurs(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "qurn":
                        if (IO)
                            s = InternalCommands.qurn(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "debg":
                        if (IO)
                            s = InternalCommands.debg(s.Substring(16));
                        break;
                    case "flor":
                        s = InternalCommands.flor(s.Substring(16));
                        break;
                    case "rond":
                        s = InternalCommands.rond(s.Substring(16));
                        break;
                    case "ceil":
                        s = InternalCommands.ceil(s.Substring(16));
                        break;
                    case "trun":
                        s = InternalCommands.trun(s.Substring(16));
                        break;
                    case "modu":
                        s = InternalCommands.modu(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "nlog":
                        s = InternalCommands.nlog(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "getl":
                        s = InternalCommands.getl(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "apnd":
                        s = InternalCommands.apnd(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "adlt":
                        s = InternalCommands.adlt(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "mapf":
                        s = InternalCommands.mapf(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "filt":
                        s = InternalCommands.filt(ParseTools.listsplit(s.Substring(16), ",")[0], ParseTools.listsplit(s.Substring(16), ",")[1]);
                        break;
                    case "gama":
                        string temps = parseMath(s.Substring(16));
                        s = InternalCommands.gama(temps);
                        if (ParseTools.split(temps, ".").Length == 1 && ParseTools.split(s, "+").Length == 1)
                            s = ParseTools.split(s, ".")[0];
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
    
    public static string parseMath(string s)
    {
        if (s == "]") return "[]";
        double trash;
        if (double.TryParse(ParseTools.remspaces(s), out trash)) return ParseTools.remspaces(s);
        try
        {
            s = ParseTools.remspaces(s);
            s = ParseTools.fixneg(s);
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            foreach (string stemp in wildcards.Keys.ToArray().Reverse())
            {
                string stemp0 = stemp.Length > 5 && stemp.Substring(0, 5) == "norec" ? stemp.Substring(5) : stemp;
                Tuple<bool, Dictionary<string, string>> temp = matches(s, stemp0);
                if (temp.Item1)
                {
                    if (currfunc == stemp && stemp.Length > 5 && stemp.Substring(0, 5) == "norec") break;
                    s = wildcards[stemp];
                    foreach (string sdict in temp.Item2.Keys)
                        s = mapTovars("_let_" + sdict + "=" + temp.Item2[sdict], s);
                    string tempfunc = currfunc; currfunc = stemp;
                    s = parseMath(s);
                    currfunc = tempfunc;
                    foreach (string sdict in temp.Item2.Keys)
                        forget("_let_" + sdict + "=" + temp.Item2[sdict]);
                    return s;
                }
            }
            s = testinternal(s);
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (ParseTools.listsplit(s, "|>").Length > 1 && ParseTools.listsplit(s, "<|").Length > 1)
            {
                Console.WriteLine("Error: Cannot have |> and <| on same line without grouping via parenthesis. Aborting operation.");
                return "";
            }
            if (ParseTools.listsplit(s, "|>").Length > 1)
            {
                List<string> args = ParseTools.listsplit(s, "|>").ToList();
                while (args.Count > 1)
                {
                    args[0] = parseMath("(" + args[1] + ")(" + args[0] + ")");
                    args.RemoveAt(1);
                }
                s = args[0];
            }
            if (ParseTools.listsplit(s, "<|").Length > 1)
            {
                List<string> args = ParseTools.listsplit(s, "<|").ToList();
                args.Reverse();
                while (args.Count > 1)
                {
                    args[0] = parseMath("(" + args[1] + ")(" + args[0] + ")");
                    args.RemoveAt(1);
                }
                s = args[0];
            }
            if (ParseTools.listsplit(s, "_where_").Length > 1)
            {
                string temps = ParseTools.singlesplit(s, "_where_")[1];
                if (ParseTools.split(temps, "=")[0].Last() == '/')
                {
                    Console.WriteLine("Error: Cannot use /= in \"where\" predicate.");
                    return "";
                }
                if (ParseTools.split(temps, "_if_").Length > 1 && ParseTools.split(temps, "_then_").Length == 1)
                {
                    Console.WriteLine("Error: Cannot use \"if\" predicate off of \"where\" predicate, only \"let\" commands.");
                    return "";
                }
                foreach (string stemp in ParseTools.listsplit(temps, ","))
                    let("_let_" + stemp, true);
                s = parseMath(ParseTools.split(s, "_where_")[0]);
                foreach (string stemp in ParseTools.listsplit(temps, ","))
                    forget("_let_" + stemp);
            }
            if (ParseTools.listsplit(s, "->").Length > 1)
            {
                string stemp = s;
                string lamname = "internallam" + ParseTools.makeletters(lamcount++) + "z";
                s = lamname;
                wildcards.Add(lamname + ParseTools.singlelistsplit(stemp, "->")[0].ToUpper(), ParseTools.singlelistsplit(stemp, "->")[1]);
            }
            if (ParseTools.listsplit(s, "_if_").Length > 1)
            {
                if (parseMath(ParseTools.split(ParseTools.split(s, "_if_")[1], "_then_")[0]) != "true" && parseMath(ParseTools.split(ParseTools.split(s, "_if_")[1], "_then_")[0]) != "false")
                    Console.WriteLine("Error: Expected boolean expression, received \"" + ParseTools.split(ParseTools.split(s, "_if_")[1], "_then_")[0] + "\"");
                else if (parseMath(ParseTools.split(ParseTools.split(s, "_if_")[1], "_then_")[0]) == "true")
                {
                    try
                    {
                        s = ParseTools.split(s, "_if_")[0] + parseMath(ParseTools.split(ParseTools.split(s, "_then_")[1], "_else_")[0]);
                    }
                    catch
                    { }
                }
                else if (parseMath(ParseTools.split(ParseTools.split(s, "_if_")[1], "_then_")[0]) == "false")
                {
                    s = ParseTools.split(s, "_if_")[0] + parseMath(ParseTools.singlesplit(s, "_else_")[1]);
                }
            }
            if (ParseTools.listsplit(s, "_match_").Length == 2)
            {
                char[] tempc = ParseTools.split(ParseTools.split(s, "_match_")[1], "_with_")[0].ToCharArray();
                for (int i = 0; i < tempc.Length; i++) if (tempc[i] == '`') tempc[i] = '-';
                string val = new string(tempc);
                string[] conditions = ParseTools.listsplit(ParseTools.listsplit(s, "_with_")[1], "|").Skip(1).Select((string x) => ParseTools.listsplit(x, "=>")[0]).ToArray();
                string[] results = ParseTools.listsplit(ParseTools.listsplit(s, "_with_")[1], "|").Skip(1).Select((string x) => ParseTools.listsplit(x, "=>")[1]).ToArray();
                for (int i = 0; i < conditions.Length; i++)
                {
                    var parsed = parseMath(val);
                    if (vars.ContainsKey(val)) val = vars[val];
                    string[] cond = ParseTools.split(conditions[i], "_when_");
                    var temp = matches(val, cond[0]);
                    var temp1 = matches(parsed, cond[0]);
                    if (val == cond[0] || parsed == cond[0])
                    {
                        if (cond.Length == 2)
                        {
                            if (parseMath(cond[1]) == "true")
                            {
                                s = results[i];
                                break;
                            }
                        }
                        else
                        {
                            s = results[i];
                            break;
                        }
                    }
                    else if (temp.Item1)
                    {
                        s = results[i];
                        foreach (string sdict in temp.Item2.Keys)
                        {
                            if (cond.Length == 2) cond[1] = mapTovars("_let_" + sdict + "=" + temp.Item2[sdict], cond[1]);
                            s = mapTovars("_let_" + sdict + "=" + temp.Item2[sdict], s);
                        }
                        if (cond.Length == 2)
                        {
                            if (parseMath(cond[1]) == "true")
                            {
                                s = parseMath(s);
                                foreach (string sdict in temp.Item2.Keys)
                                    forget("_let_" + sdict + "=" + temp.Item2[sdict]);
                                break;
                            }
                        }
                        else
                        {
                            s = parseMath(s);
                            foreach (string sdict in temp.Item2.Keys)
                                forget("_let_" + sdict + "=" + temp.Item2[sdict]);
                            break;
                        }
                        foreach (string sdict in temp.Item2.Keys)
                            forget("_let_" + sdict + "=" + temp.Item2[sdict]);
                    }
                    else if (temp1.Item1)
                    {
                        s = results[i];
                        foreach (string sdict in temp.Item2.Keys)
                        {
                            if (cond.Length == 2) cond[1] = mapTovars("_let_" + sdict + "=" + temp.Item2[sdict], cond[1]);
                            s = mapTovars("_let_" + sdict + "=" + temp.Item2[sdict], s);
                        }
                        if (cond.Length == 2)
                        {
                            if (parseMath(cond[1]) == "true")
                            {
                                s = parseMath(s);
                                foreach (string sdict in temp1.Item2.Keys)
                                    forget("_let_" + sdict + "=" + temp1.Item2[sdict]);
                                break;
                            }
                        }
                        else
                        {
                            s = parseMath(s);
                            foreach (string sdict in temp1.Item2.Keys)
                                forget("_let_" + sdict + "=" + temp1.Item2[sdict]);
                            break;
                        }
                        foreach (string sdict in temp1.Item2.Keys)
                            forget("_let_" + sdict + "=" + temp1.Item2[sdict]);
                    }
                    if (i == conditions.Length - 1)
                    {
                        Console.WriteLine("Unable to match \"" + val + "\" with given patterns, aborting operation.");
                        return "";
                    }
                }
            }
            if (ParseTools.betwparen(s).Length > 0)
                foreach (string i in ParseTools.betwparen(s))
                    s = ParseTools.split(s, "(")[0] + parseMath(i) + s.Substring(ParseTools.split(s, "(")[0].Length + i.Length + 2);
            if (ParseTools.betwbracks(s).Length > 0)
            {
                foreach (string i in ParseTools.betwbracks(s))
                {
                    try
                    {
                        string res = "";
                        List<string> args = new List<string>();
                        if (ParseTools.listsplit(i, "..").Length == 2)
                        {
                            if (ParseTools.listsplit(ParseTools.listsplit(i, "..")[0], ",").Length == 2)
                                args = ParseTools.range(Convert.ToInt32(parseMath(ParseTools.listsplit(ParseTools.listsplit(i, "..")[0], ",")[0])), Convert.ToInt32(parseMath(ParseTools.listsplit(i, "..")[1])) + 1, Convert.ToInt32(parseMath(ParseTools.listsplit(ParseTools.listsplit(i, "..")[0], ",")[1])) - Convert.ToInt32(parseMath(ParseTools.listsplit(ParseTools.listsplit(i, "..")[0], ",")[0]))).Select(x => Convert.ToString(x)).ToList();
                            else
                                args = ParseTools.range(Convert.ToInt32(parseMath(ParseTools.listsplit(i, "..")[0])), Convert.ToInt32(parseMath(ParseTools.listsplit(i, "..")[1])) + 1, 1).Select(x => Convert.ToString(x)).ToList();
                        }
                        else
                        {
                            args = ParseTools.listsplit(i, ",").Select(x => parseMath(x)).ToList();
                        }
                        for (int j = 0; j < args.Count - 1; j++)
                            res += args[j] + ",";
                        res += args.Last();
                        s = ParseTools.split(s, "[")[0] + "«" + res + "»" + s.Substring(ParseTools.split(s, "[")[0].Length + i.Length + 2);
                    }
                    catch
                    {
                        Console.WriteLine("Error: Could not parse list \"[" + i + "]\". Assuming [], resuming operation.");
                        s = ParseTools.split(s, "[")[0] + "«»" + s.Substring(ParseTools.split(s, "[")[0].Length + i.Length + 2);
                    }
                }
                char[] temps = s.ToCharArray();
                for (int i = 0; i < temps.Length; i++)
                {
                    if (temps[i] == '«')
                        temps[i] = '[';
                    if (temps[i] == '»')
                        temps[i] = ']';
                }
                s = new string(temps);
            }
            if (ParseTools.betwbraces(s).Length > 0)
            {
                int iterator = 0;
                foreach (string i in ParseTools.betwbraces(s))
                {
                    s = ParseTools.splitnth(s, "{", iterator)[0] + "{" + parseMath(i) + "}" + ParseTools.splitnth(s, "}", iterator)[1];
                    iterator++;
                }
            }
            s = ParseTools.fixneg(s);
            foreach (string stemp in wildcards.Keys.ToArray().Reverse())
            {
                string stemp0 = stemp.Length > 5 && stemp.Substring(0, 5) == "norec" ? stemp.Substring(5) : stemp;
                Tuple<bool, Dictionary<string, string>> temp = matches(s, stemp0);
                if (temp.Item1)
                {
                    if (currfunc == stemp && stemp.Length > 5 && stemp.Substring(0, 5) == "norec") break;
                    s = wildcards[stemp];
                    foreach (string sdict in temp.Item2.Keys)
                        s = mapTovars("_let_" + sdict + "=" + temp.Item2[sdict], s);
                    string tempfunc = currfunc; currfunc = stemp;
                    s = parseMath(s);
                    currfunc = tempfunc;
                    foreach (string sdict in temp.Item2.Keys)
                        forget("_let_" + sdict + "=" + temp.Item2[sdict]);
                    return s;
                }
            }
            if (s.Contains("_and_"))
            {
                string[] args = ParseTools.split(s, "_and_");
                foreach (string i in args)
                    if (parseMath(i) == "false")
                        return "false";
                return "true";
            }
            else if (s.Contains("_or_"))
            {
                string[] args = ParseTools.split(s, "_or_");
                foreach (string i in args)
                    if (parseMath(i) == "true")
                        return "true";
                return "false";
            }
            else if (ParseTools.split(s, "?=").Length == 2)
            {
                string a = parseMath(ParseTools.split(s, "?=")[0]), b = parseMath(ParseTools.split(s, "?=")[1]);
                if (vars.ContainsKey(ParseTools.split(s, "?=")[0] + "?=" + b))
                    return vars[ParseTools.split(s, "?=")[0] + "?=" + b];
                if (vars.ContainsKey(a + "?=" + ParseTools.split(s, "?=")[1]))
                    return vars[a + "?=" + ParseTools.split(s, "?=")[1]];
                if (a == "unknown" || b == "unknown")
                    return "true";
                try
                {
                    if ((a == "true" || a == "false" || a == "unknown" || a == "infinity" || a == "unit" || a == "world" || ParseTools.isarray(a) || Convert.ToString(Convert.ToDouble(a)) == a) && (b == "true" || b == "false" || b == "unknown" || b == "infinity" || b == "unit" || b == "world" || ParseTools.isarray(b) || Convert.ToString(Convert.ToDouble(b)) == b))
                        return a == b ? "true" : "false";
                }
                catch
                {
                    return "true";
                }
            }
            else if (ParseTools.split(s, "/=").Length == 2)
            {
                string a = parseMath(ParseTools.split(s, "/=")[0]), b = parseMath(ParseTools.split(s, "/=")[1]);
                if (vars.ContainsKey(ParseTools.split(s, "/=")[0] + "/=" + b))
                    return vars[ParseTools.split(s, "/=")[0] + "/=" + b];
                if (vars.ContainsKey(a + "/=" + ParseTools.split(s, "/=")[1]))
                    return vars[a + "/=" + ParseTools.split(s, "/=")[1]];
                if (a == "unknown" || b == "unknown")
                    return "unknown";
                return a != b ? "true" : "false";
            }
            else if (ParseTools.split(s, ">=").Length == 2)
            {
                string a = parseMath(ParseTools.split(s, ">=")[0]), b = parseMath(ParseTools.split(s, ">=")[1]);
                if (a == "unknown" || b == "unknown")
                    return "unknown";
                return Convert.ToDouble(a) >= Convert.ToDouble(b) ? "true" : "false";
            }
            else if (ParseTools.split(s, "<=").Length == 2)
            {
                string a = parseMath(ParseTools.split(s, "<=")[0]), b = parseMath(ParseTools.split(s, "<=")[1]);
                if (a == "unknown" || b == "unknown")
                    return "unknown";
                return Convert.ToDouble(a) <= Convert.ToDouble(b) ? "true" : "false";
            }
            else if (ParseTools.split(s, ">").Length == 2)
            {
                string a = parseMath(ParseTools.split(s, ">")[0]), b = parseMath(ParseTools.split(s, ">")[1]);
                if (a == "unknown" || b == "unknown")
                    return "unknown";
                return Convert.ToDouble(a) > Convert.ToDouble(b) ? "true" : "false";
            }
            else if (ParseTools.split(s, "<").Length == 2)
            {
                string a = parseMath(ParseTools.split(s, "<")[0]), b = parseMath(ParseTools.split(s, "<")[1]);
                if (a == "unknown" || b == "unknown")
                    return "unknown";
                return Convert.ToDouble(a) < Convert.ToDouble(b) ? "true" : "false";
            }
            else if (ParseTools.split(s, "=").Length == 2)
            {
                string a = parseMath(ParseTools.split(s, "=")[0]), b = parseMath(ParseTools.split(s, "=")[1]);
                if (vars.ContainsKey(ParseTools.split(s, "=")[0] + "=" + b))
                    return vars[ParseTools.split(s, "=")[0] + "=" + b];
                if (vars.ContainsKey(a + "=" + ParseTools.split(s, "=")[1]))
                    return vars[a + "=" + ParseTools.split(s, "=")[1]];
                if (a == "unknown" || b == "unknown")
                    return "unknown";
                return a == b ? "true" : "false";
            }
            else if (s.Contains('+') || s.Contains('`'))
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
                foreach (char i in ParseTools.range(0, ops.Count, 1))
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
            else if (s.Contains('*') || s.Contains('/'))
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
                foreach (int i in ParseTools.range(0, ops.Count, 1))
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
            else if (s.Contains('^'))
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
                foreach (int i in ParseTools.range(0, nums.Count - 1, 1))
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
            else if (s.Contains(':'))
            {
                List<string> nums = ParseTools.split(s, ":").ToList();
                string res = nums.Last();
                nums.RemoveAt(nums.Count - 1);
                while (nums.Count > 0)
                {
                    res = InternalCommands.apnd(nums[nums.Count - 1], res);
                    nums.RemoveAt(nums.Count - 1);
                }
                if (res[res.Length - 2] == ',')
                    res = res.Substring(0, res.Length - 2) + "]";
                s = res;
            }
            else if (s.Contains('|'))
            {
                List<string> args = ParseTools.split(s, "|").ToList();
                while (args.Count > 1)
                {
                    args[0] = parseMath("(" + args[0] + ")" + args[1]);
                    args.RemoveAt(1);
                }
                s = args[0];
            }
            else
            {
                foreach (string stemp in wildcards.Keys.ToArray().Reverse())
                {
                    string stemp0 = stemp.Length > 5 && stemp.Substring(0, 5) == "norec" ? stemp.Substring(5) : stemp;
                    Tuple<bool, Dictionary<string, string>> temp = matches(s, stemp0);
                    if (temp.Item1)
                    {
                        if (currfunc == stemp && stemp.Length > 5 && stemp.Substring(0, 5) == "norec") break;
                        s = wildcards[stemp];
                        foreach (string sdict in temp.Item2.Keys)
                            s = mapTovars("_let_" + sdict + "=" + temp.Item2[sdict], s);
                        string tempfunc = currfunc; currfunc = stemp;
                        s = parseMath(s);
                        currfunc = tempfunc;
                        foreach (string sdict in temp.Item2.Keys)
                            forget("_let_" + sdict + "=" + temp.Item2[sdict]);
                        return s;
                    }
                }
            }
            s = testinternal(s);
            if (vars.ContainsKey(s))
                s = parseMath(vars[s]);
            if (ParseTools.singlesplit(s, ".").Length == 2 && ParseTools.singlesplit(s, ".")[1] == "0")
                s = ParseTools.singlesplit(s, ".")[0];
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
        if (ParseTools.split(s, "//").Length > 1) s = ParseTools.split(s, "//")[0];
        string ret = parseMath(s);
        
        //Reenable this code to allow lambda garbage collection after each expression (may cause some bugs relating to lines like "let x <- lambda")

        /* if (lamcount != 0)
              foreach (string stemp in wildcards.Keys.ToArray())
                  if (stemp.Length > 11 && stemp.Substring(0, 11) == "internallam")
                      wildcards.Remove(stemp);
        lamcount = 0;*/

        if (CAS)
        {
            if (ret.Length > 8 && ret.Substring(0, 8) == "internal") return ParseTools.remspaces(s);
            if (ret == "]") return "[]";
            return ret;
        }
        else
            try
            {
                if (ret == "true" || ret == "false" || ret == "unknown" || ret == "infinity" || ret == "unit" || ret == "world" || ParseTools.isarray(ret) || Convert.ToString(Convert.ToDouble(ret)) == ret)
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

    private static string mapTovars(string s, string mapto)
    {
        string varname = ParseTools.remspaces(ParseTools.split(s.Substring(5), "=")[0]);
        string valname = ParseTools.remstartspaces(ParseTools.singlesplit(s.Substring(5), "=")[1]);
        int i = 0;
        while (varname.ToList().Contains('(') && (!varname.ToList().Contains(')')))
        {
            varname += "=" + ParseTools.remspaces(ParseTools.split(s.Substring(5), "=")[++i]);
            valname = ParseTools.remstartspaces(ParseTools.singlesplit(valname, "=")[1]);
        }
        if (varname.ToList().Contains('(')) varname = ParseTools.betwparen(varname)[0];
        if (varname == valname)
        {
            Console.WriteLine("Error: Cannot bind variable to itself, tried to bind \"" + varname + "\" to \"" + valname + "\"");
            return "";
        }
        else
        {
            mapto = " " + mapto + " ";
            for (int j = 0; j < mapto.Length - varname.Length + 1; j++)
                if (mapto.Substring(j, varname.Length) == varname.ToLower())
                    mapto = mapto.Substring(0, j) + valname + mapto.Substring(j + varname.Length);
        }
        return mapto;
    }

    private static bool let(string s, bool suppressdup)
    {
        string varname = ParseTools.fixneg(ParseTools.remspaces(ParseTools.listsplit(s.Substring(5), "=")[0]));
        string valname = ParseTools.remstartspaces(ParseTools.singlelistsplit(s.Substring(5), "=")[1]);
        if (ParseTools.betwparen(varname).Length > 0 && ParseTools.betwparen(varname)[0] == varname.Substring(1, varname.Length - 2))
            varname = ParseTools.betwparen(varname)[0];
        bool haswildcard = false;
        if (valname.Contains("world"))
        {
            Console.WriteLine("Error: Cannot use world value from within a function, attempted to call \"world\" from \"" + valname + "\"");
            return true;
        }
        if (varname == valname)
        {
            Console.WriteLine("Error: Cannot bind variable to itself, tried to bind \"" + varname + "\" to \"" + valname + "\"");
            return true;
        }
        if (vars.ContainsKey(varname) && vars[varname] != "unknown")
        {
            if (!suppressdup) Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + vars[varname] + "\"");
            return true;
        }
        if (varname == "world")
        {
            Console.WriteLine("Error: Cannot change world value.");
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
            Console.WriteLine("Error: Cannot bind directly to a numeric value, tried to bind to \"" + varname + "\"");
            return true;
        }
        if (ParseTools.split(s, "_if_").Length == 2 && ParseTools.split(s, "_then_").Length == 1)
        {
            string res = ParseMath(ParseTools.split(s, "_if_")[1]);
            if (res == "false")
                return true;
            else if (res == "true")
            {
                valname = ParseTools.remspaces(ParseTools.split(valname, "_if_")[0]);
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
                    Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
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
                Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
        }
        return true;
    }

    private static bool eagerlet(string s)
    {
        string varname = ParseTools.fixneg(ParseTools.remspaces(ParseTools.listsplit(s.Substring(5), "<-")[0]));
        string valname = ParseTools.remstartspaces(ParseTools.singlelistsplit(s.Substring(5), "<-")[1]);
        if (ParseTools.betwparen(varname).Length > 0 && ParseTools.betwparen(varname)[0] == varname.Substring(1, varname.Length - 2))
            varname = ParseTools.betwparen(varname)[0];
        bool haswildcard = false;
        if (varname == valname)
        {
            Console.WriteLine("Error: Cannot use world value from within a function, attempted to call \"world\" from \"" + valname + "\"");
            return true;
        }
        if (vars.ContainsKey(varname) && vars[varname] != "unknown")
        {
            Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + vars[varname] + "\"");
            return true;
        }
        if (wildcards.ContainsKey(varname))
        {
            Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + wildcards[varname] + "\"");
            return true;
        }
        double trash;
        if (double.TryParse(varname, out trash))
        {
            Console.WriteLine("Error: Cannot bind directly to a numeric value, tried to bind to \"" + varname + "\"");
            return true;
        }
        if (ParseTools.split(s, "_if_").Length == 2 && ParseTools.split(s, "_then_").Length == 1)
        {
            string res = ParseMath(ParseTools.split(s, "_if_")[1]);
            if (res == "false")
                return true;
            else if (res == "true")
            {
                valname = ParseTools.remspaces(ParseTools.split(valname, "_if_")[0]);
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
                    vars.Add(varname, parseMath(valname));
                else
                    Console.WriteLine("Error: Cannot have capital letters in variable name, tried to use name \"" + varname + "\"");
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
                vars.Add(varname, parseMath(valname));
            else
                wildcards.Add(varname, parseMath(valname));
        }
        return true;
    }

    private static bool notlet(string s)
    {
        string varname = ParseTools.fixneg(ParseTools.remspaces(ParseTools.listsplit(s.Substring(5), "/=")[0]));
        string valname = ParseTools.remstartspaces(ParseTools.singlelistsplit(s.Substring(5), "/=")[1]);
        if (ParseTools.betwparen(varname).Length > 0 && ParseTools.betwparen(varname)[0] == varname.Substring(1, varname.Length - 2))
            varname = ParseTools.betwparen(varname)[0];
        bool haswildcard = false;
        if (valname.Contains("world"))
        {
            Console.WriteLine("Error: Cannot use world value from within a function, attempted to call \"world\" from \"" + valname + "\"");
            return true;
        }
        if (varname == valname)
        {
            Console.WriteLine("Error: Cannot break reflexive law of equality, tried to say \"" + varname + "\" is not equal to \"" + valname + "\"");
            return true;
        }
        if (vars.ContainsKey(varname) && vars[varname] != "unknown")
            Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + vars[varname] + "\"");
        else if (ParseTools.split(s, "_if_").Length == 2 && ParseTools.split(s, "_then_").Length == 1)
        {
            string res = ParseMath(ParseTools.split(s, "_if_")[1]);
            if (res == "false")
                return true;
            else if (res == "true")
            {
                valname = ParseTools.remspaces(ParseTools.split(valname, "_if_")[0]);
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
                    Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
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
                Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
        }
        return true;
    }

    private static bool greaterlet(string s)
    {
        string varname = ParseTools.fixneg(ParseTools.remspaces(ParseTools.listsplit(s.Substring(5), ">")[0]));
        string valname = ParseTools.remstartspaces(ParseTools.singlelistsplit(s.Substring(5), ">")[1]);
        if (ParseTools.betwparen(varname).Length > 0 && ParseTools.betwparen(varname)[0] == varname.Substring(1, varname.Length - 2))
            varname = ParseTools.betwparen(varname)[0];
        bool haswildcard = false;
        if (valname.Contains("world"))
        {
            Console.WriteLine("Error: Cannot use world value from within a function, attempted to call \"world\" from \"" + valname + "\"");
            return true;
        }
        if (varname == valname)
        {
            Console.WriteLine("Error: Cannot make value greater than itself, tried to say \"" + varname + "\" is greater than \"" + valname + "\"");
            return true;
        }
        if (vars.ContainsKey(varname) && vars[varname] != "unknown")
            Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + vars[varname] + "\"");
        else if (ParseTools.split(s, "_if_").Length == 2 && ParseTools.split(s, "_then_").Length == 1)
        {
            string res = ParseMath(ParseTools.split(s, "_if_")[1]);
            if (res == "false")
                return true;
            else if (res == "true")
            {
                valname = ParseTools.remspaces(ParseTools.split(valname, "_if_")[0]);
                if (varname == valname)
                {
                    Console.WriteLine("Error: Cannot make value greater than itself, tried to say \"" + varname + "\" is greater than \"" + valname + "\"");
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
                    vars.Add(varname + ">" + valname, "true");
                    vars.Add(varname + ">=" + valname, "true");
                    vars.Add(varname + "<" + valname, "false");
                    vars.Add(varname + "<=" + valname, "false");
                    vars.Add(varname + "/=" + valname, "true");
                    vars.Add(valname + "=" + varname, "false");
                    vars.Add(valname + "<" + varname, "true");
                    vars.Add(valname + "<=" + varname, "true");
                    vars.Add(valname + ">" + varname, "false");
                    vars.Add(valname + ">=" + varname, "false");
                    vars.Add(valname + "/=" + varname, "true");
                    wildcards.Add(varname + "?=Internalvarx", "_if_internalvarx>" + valname + "_then_true_else_false");
                    wildcards.Add("Internalvarx?=" + varname, "_if_internalvarx>" + valname + "_then_true_else_false");
                }
                else
                {
                    Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
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
                vars.Add(varname + ">" + valname, "true");
                vars.Add(varname + ">=" + valname, "true");
                vars.Add(varname + "<" + valname, "false");
                vars.Add(varname + "<=" + valname, "false");
                vars.Add(varname + "/=" + valname, "true");
                vars.Add(valname + "=" + varname, "false");
                vars.Add(valname + "<" + varname, "true");
                vars.Add(valname + "<=" + varname, "true");
                vars.Add(valname + ">" + varname, "false");
                vars.Add(valname + ">=" + varname, "false");
                vars.Add(valname + "/=" + varname, "true");
                wildcards.Add(varname + "?=Internalvarx", "_if_internalvarx>" + valname + "_then_true_else_false");
                wildcards.Add("Internalvarx?=" + varname, "_if_internalvarx>" + valname + "_then_true_else_false");
            }
            else
                Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
        }
        return true;
    }

    private static bool lesslet(string s)
    {
        string varname = ParseTools.fixneg(ParseTools.remspaces(ParseTools.listsplit(s.Substring(5), "<")[0]));
        string valname = ParseTools.remstartspaces(ParseTools.singlelistsplit(s.Substring(5), "<")[1]);
        if (ParseTools.betwparen(varname).Length > 0 && ParseTools.betwparen(varname)[0] == varname.Substring(1, varname.Length - 2))
            varname = ParseTools.betwparen(varname)[0];
        bool haswildcard = false;
        if (valname.Contains("world"))
        {
            Console.WriteLine("Error: Cannot use world value from within a function, attempted to call \"world\" from \"" + valname + "\"");
            return true;
        }
        if (varname == valname)
        {
            Console.WriteLine("Error: Cannot make value less than itself, tried to say \"" + varname + "\" is less than \"" + valname + "\"");
            return true;
        }
        if (vars.ContainsKey(varname) && vars[varname] != "unknown")
            Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + vars[varname] + "\"");
        else if (ParseTools.split(s, "_if_").Length == 2 && ParseTools.split(s, "_then_").Length == 1)
        {
            string res = ParseMath(ParseTools.split(s, "_if_")[1]);
            if (res == "false")
                return true;
            else if (res == "true")
            {
                valname = ParseTools.remspaces(ParseTools.split(valname, "_if_")[0]);
                if (varname == valname)
                {
                    Console.WriteLine("Error: Cannot make value less than itself, tried to say \"" + varname + "\" is less than \"" + valname + "\"");
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
                    vars.Add(varname + "<" + valname, "true");
                    vars.Add(varname + "<=" + valname, "true");
                    vars.Add(varname + ">" + valname, "false");
                    vars.Add(varname + ">=" + valname, "false");
                    vars.Add(varname + "/=" + valname, "true");
                    vars.Add(valname + "=" + varname, "false");
                    vars.Add(valname + ">" + varname, "true");
                    vars.Add(valname + ">=" + varname, "true");
                    vars.Add(valname + "<" + varname, "false");
                    vars.Add(valname + "<=" + varname, "false");
                    vars.Add(valname + "/=" + varname, "true");
                    wildcards.Add(varname + "?=Internalvarx", "_if_internalvarx<" + valname + "_then_true_else_false");
                    wildcards.Add("Internalvarx?=" + varname, "_if_internalvarx<" + valname + "_then_true_else_false");
                }
                else
                {
                    Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
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
                vars.Add(varname + "<" + valname, "true");
                vars.Add(varname + "<=" + valname, "true");
                vars.Add(varname + ">" + valname, "false");
                vars.Add(varname + ">=" + valname, "false");
                vars.Add(varname + "/=" + valname, "true");
                vars.Add(valname + "=" + varname, "false");
                vars.Add(valname + ">" + varname, "true");
                vars.Add(valname + ">=" + varname, "true");
                vars.Add(valname + "<" + varname, "false");
                vars.Add(valname + "<=" + varname, "false");
                vars.Add(valname + "/=" + varname, "true");
                wildcards.Add(varname + "?=Internalvarx", "_if_internalvarx<" + valname + "_then_true_else_false");
                wildcards.Add("Internalvarx?=" + varname, "_if_internalvarx<" + valname + "_then_true_else_false");
            }
            else
                Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
        }
        return true;
    }

    private static bool greatereqlet(string s)
    {
        string varname = ParseTools.fixneg(ParseTools.remspaces(ParseTools.listsplit(s.Substring(5), ">=")[0]));
        string valname = ParseTools.remstartspaces(ParseTools.singlelistsplit(s.Substring(5), ">=")[1]);
        if (ParseTools.betwparen(varname).Length > 0 && ParseTools.betwparen(varname)[0] == varname.Substring(1, varname.Length - 2))
            varname = ParseTools.betwparen(varname)[0];
        bool haswildcard = false;
        if (valname.Contains("world"))
        {
            Console.WriteLine("Error: Cannot use world value from within a function, attempted to call \"world\" from \"" + valname + "\"");
            return true;
        }
        if (varname == valname)
        {
            Console.WriteLine("Error: Cannot make value greater thanor equal to itself, tried to say \"" + varname + "\" is greater thanor equal to \"" + valname + "\"");
            return true;
        }
        if (vars.ContainsKey(varname) && vars[varname] != "unknown")
            Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + vars[varname] + "\"");
        else if (ParseTools.split(s, "_if_").Length == 2 && ParseTools.split(s, "_then_").Length == 1)
        {
            string res = ParseMath(ParseTools.split(s, "_if_")[1]);
            if (res == "false")
                return true;
            else if (res == "true")
            {
                valname = ParseTools.remspaces(ParseTools.split(valname, "_if_")[0]);
                if (varname == valname)
                {
                    Console.WriteLine("Error: Cannot make value greater or equal to than itself, tried to say \"" + varname + "\" is greater than or equal to \"" + valname + "\"");
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
                    vars.Add(varname + ">=" + valname, "true");
                    vars.Add(varname + "<" + valname, "false");
                    vars.Add(valname + "<=" + varname, "true");
                    vars.Add(valname + ">" + varname, "false");
                    wildcards.Add(varname + "?=Internalvarx", "_if_internalvarx>=" + valname + "_then_true_else_false");
                    wildcards.Add("Internalvarx?=" + varname, "_if_internalvarx>=" + valname + "_then_true_else_false");
                }
                else
                {
                    Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
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
                vars.Add(varname + ">=" + valname, "true");
                vars.Add(varname + "<" + valname, "false");
                vars.Add(valname + "<=" + varname, "true");
                vars.Add(valname + ">" + varname, "false");
                wildcards.Add(varname + "?=Internalvarx", "_if_internalvarx>=" + valname + "_then_true_else_false");
                wildcards.Add("Internalvarx?=" + varname, "_if_internalvarx>=" + valname + "_then_true_else_false");
            }
            else
                Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
        }
        return true;
    }

    private static bool lesseqlet(string s)
    {
        string varname = ParseTools.fixneg(ParseTools.remspaces(ParseTools.listsplit(s.Substring(5), "<=")[0]));
        string valname = ParseTools.remstartspaces(ParseTools.singlelistsplit(s.Substring(5), "<=")[1]);
        if (ParseTools.betwparen(varname).Length > 0 && ParseTools.betwparen(varname)[0] == varname.Substring(1, varname.Length - 2))
            varname = ParseTools.betwparen(varname)[0];
        bool haswildcard = false;
        if (valname.Contains("world"))
        {
            Console.WriteLine("Error: Cannot use world value from within a function, attempted to call \"world\" from \"" + valname + "\"");
            return true;
        }
        if (varname == valname)
        {
            Console.WriteLine("Error: Cannot make value less than or equal to itself, tried to say \"" + varname + "\" is less than or equal to \"" + valname + "\"");
            return true;
        }
        if (vars.ContainsKey(varname) && vars[varname] != "unknown")
            Console.WriteLine("Error: Value under name \"" + varname + "\" already exists, bound to value \"" + vars[varname] + "\"");
        else if (ParseTools.split(s, "_if_").Length == 2 && ParseTools.split(s, "_then_").Length == 1)
        {
            string res = ParseMath(ParseTools.split(s, "_if_")[1]);
            if (res == "false")
                return true;
            else if (res == "true")
            {
                valname = ParseTools.remspaces(ParseTools.split(valname, "_if_")[0]);
                if (varname == valname)
                {
                    Console.WriteLine("Error: Cannot make value less than or equal to itself, tried to say \"" + varname + "\" is less than or equal to \"" + valname + "\"");
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
                    vars.Add(varname + "<=" + valname, "true");
                    vars.Add(varname + ">" + valname, "false");
                    vars.Add(valname + "<=" + varname, "true");
                    vars.Add(valname + ">" + varname, "false");
                    wildcards.Add(varname + "?=Internalvarx", "_if_internalvarx<=" + valname + "_then_true_else_false");
                    wildcards.Add("Internalvarx?=" + varname, "_if_internalvarx<=" + valname + "_then_true_else_false");
                }
                else
                {
                    Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
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
                vars.Add(varname + "<=" + valname, "true");
                vars.Add(varname + ">" + valname, "false");
                vars.Add(valname + "<=" + varname, "true");
                vars.Add(valname + ">" + varname, "false");
                wildcards.Add(varname + "?=Internalvarx", "_if_internalvarx<=" + valname + "_then_true_else_false");
                wildcards.Add("Internalvarx?=" + varname, "_if_internalvarx<=" + valname + "_then_true_else_false");
            }
            else
                Console.WriteLine("Error: Cannot have capital letters in function/variable name, tried to use name \"" + varname + "\"");
        }
        return true;
    }

    private static bool forget(string s)
    {
        string varname = ParseTools.fixneg(ParseTools.remspaces(ParseTools.split(s.Substring(5), "=")[0]));
        bool haswildcard = false;
        int i = 0;
        while (varname.ToList().Contains('(') && (!varname.ToList().Contains(')')))
        {
            varname += "=" + ParseTools.remspaces(ParseTools.split(s.Substring(5), "=")[++i]);
        }
        if (varname.ToList().Contains('(')) varname = ParseTools.betwparen(varname)[0];
        if (ParseTools.split(s, "_if_").Length == 2 && ParseTools.split(s, "_then_").Length == 1)
        {
            string res = ParseMath(ParseTools.split(s, "_if_")[1]);
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

    public static bool RunCommands(string s, string[] args)
    {
        if (ParseTools.split(s, "//").Length > 1) s = ParseTools.split(s, "//")[0];
        s = ParseTools.remstartspaces(s);
        try
        {
            if (s.Substring(0, 5) == "_let_")
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == '(')
                    {
                        int count = 0;
                        for (; i < s.Length; i++)
                        {
                            if (s[i] == '(') count++;
                            if (s[i] == ')') count--;
                            if (count == 0) break;
                        }
                    }
                    if (s.Substring(i, 2) == "<-")
                    {
                        foreach (string stemp in ParseTools.listsplit(s.Substring(5), ","))
                            eagerlet("_let_" + stemp);
                        break;
                    }
                    if (s.Substring(i, 2) == "/=")
                    {
                        foreach (string stemp in ParseTools.listsplit(s.Substring(5), ","))
                            notlet("_let_" + stemp);
                        break;
                    }
                    if (s.Substring(i, 2) == "<=")
                    {
                        foreach (string stemp in ParseTools.listsplit(s.Substring(5), ","))
                            lesseqlet("_let_" + stemp);
                        break;
                    }
                    if (s.Substring(i, 2) == ">=")
                    {
                        foreach (string stemp in ParseTools.listsplit(s.Substring(5), ","))
                            greatereqlet("_let_" + stemp);
                        break;
                    }
                    if (s.Substring(i, 1) == "<")
                    {
                        foreach (string stemp in ParseTools.listsplit(s.Substring(5), ","))
                            lesslet("_let_" + stemp);
                        break;
                    }
                    if (s.Substring(i, 1) == ">")
                    {
                        foreach (string stemp in ParseTools.listsplit(s.Substring(5), ","))
                            greaterlet("_let_" + stemp);
                        break;
                    }
                    if (s.Substring(i, 1) == "=")
                    {
                        foreach (string stemp in ParseTools.listsplit(s.Substring(5), ","))
                            let("_let_" + stemp, false);
                        break;
                    }
                }
                return true;
            }
            else if (s.Substring(0, 8) == "_import_")
            {
                if (args.Length == 0)
                    Exec(new string[] { Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + ParseTools.remspaces(s.Substring(8)) + ".hsh" });
                else
                {
                    del mydel = x =>
                    {
                        for (int i = x.Length - 1; i >= 0; i--)
                        {
                            if (x[i] == '\\')
                            {
                                x = x.Substring(0, i);
                                break;
                            }
                        }
                        return x;
                    };
                    Exec(new string[] { mydel(args[0]) + "\\" + ParseTools.remspaces(s.Substring(8)) + ".hsh" });
                }
                return true;
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
}