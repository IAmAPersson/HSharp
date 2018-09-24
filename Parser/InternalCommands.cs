using System;
using System.Linq;

public static class InternalCommands
{
    public static string sine(string s)
    {
        double num = 0;
        s = Parser.parseMath(s);
        if (!double.TryParse(s, out num))
        {
            Console.WriteLine("Error: Cannot take sine of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
            return "";
        }
        num = Math.Sin(num);
        return Convert.ToString(num);
    }

    public static string cosi(string s)
    {
        double num = 0;
        s = Parser.parseMath(s);
        if (!double.TryParse(s, out num))
        {
            Console.WriteLine("Error: Cannot take cosine of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
            return "";
        }
        num = Math.Cos(num);
        return Convert.ToString(num);
    }

    public static string tang(string s)
    {
        double num = 0;
        s = Parser.parseMath(s);
        if (!double.TryParse(s, out num))
        {
            Console.WriteLine("Error: Cannot take tangent of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
            return "";
        }
        num = Math.Tan(num);
        if (num > -286411217403672 && num < -286411217403670) return "infinity";
        return Convert.ToString(num);
    }

    public static string asin(string s)
    {
        double num = 0;
        s = Parser.parseMath(s);
        if (!double.TryParse(s, out num))
        {
            Console.WriteLine("Error: Cannot take arcsine of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
            return "";
        }
        num = Math.Asin(num);
        return Convert.ToString(num);
    }

    public static string acos(string s)
    {
        double num = 0;
        s = Parser.parseMath(s);
        if (!double.TryParse(s, out num))
        {
            Console.WriteLine("Error: Cannot take arccosine of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
            return "";
        }
        num = Math.Acos(num);
        return Convert.ToString(num);
    }

    public static string atan(string s)
    {
        double num = 0;
        s = Parser.parseMath(s);
        if (!double.TryParse(s, out num))
        {
            Console.WriteLine("Error: Cannot take arctangent of nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
            return "";
        }
        num = Math.Atan(num);
        return Convert.ToString(num);
    }

    public static string flor(string s)
    {
        double num = 0;
        s = Parser.parseMath(s);
        if (!double.TryParse(s, out num))
        {
            Console.WriteLine("Error: Cannot floor a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
            return "";
        }
        num = Math.Floor(num);
        return Convert.ToString(num);
    }

    public static string rond(string s)
    {
        double num = 0;
        s = Parser.parseMath(s);
        if (!double.TryParse(s, out num))
        {
            Console.WriteLine("Error: Cannot round a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
            return "";
        }
        num = Math.Floor(num + 0.5);
        return Convert.ToString(num);
    }

    public static string trun(string s)
    {
        double num = 0;
        s = Parser.parseMath(s);
        if (!double.TryParse(s, out num))
        {
            Console.WriteLine("Error: Cannot truncate a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
            return "";
        }
        num = Math.Truncate(num);
        return Convert.ToString(num);
    }

    public static string ceil(string s)
    {
        double num = 0;
        s = Parser.parseMath(s);
        if (!double.TryParse(s, out num))
        {
            Console.WriteLine("Error: Cannot ceiling a nonnumeric value (includes booleans, unknown variables, or infinity). Aborting operation.");
            return "";
        }
        num = Math.Ceiling(num);
        return Convert.ToString(num);
    }

    public static string getl(string s, string base_)
    {
        int num1 = 0;
        string[] num;
        s = Parser.parseMath(s);
        base_ = Parser.parseMath(base_);
        if (!int.TryParse(base_, out num1))
        {
            Console.WriteLine("Error: Cannot get list element with noninteger index value (includes booleans, unknown variables, decimals, or infinity). Aborting operation.");
            return "";
        }
        num = ParseTools.listsplit(ParseTools.betwbracks(s)[0], ",");
        return ParseTools.remspaces(num[num1 % num.Length]);
    }

    public static string apnd(string ap, string list)
    {
        ap = Parser.parseMath(ap);
        list = Parser.parseMath(list);
        if (list[0] == '[')
            return "[" + ap + "," + list.Substring(1);
        else
        {
            Console.WriteLine("Error: Right side of colon must contain a list (does not include booleans, unknown variables, numbers, or infinity). Aborting operation.");
            return "";
        }
    }

    public static string adlt(string list1, string list2)
    {
        list1 = Parser.parseMath(list1);
        list2 = Parser.parseMath(list2);
        if (list1[0] == '[' && list1.Last() == ']' && list2[0] == '[' && list2.Last() == ']')
            return list1.Substring(0, list1.Length - 1) + "," + list2.Substring(1);
        else
        {
            Console.WriteLine("Error: Both sides of list addition operator must contain lists (does not include booleans, unknown variables, numbers, or infinity). Aborting operation.");
            return "";
        }
    }

    public static string mapf(string f, string l)
    {
        string res = "[";
        if (l == "[]") return l;
        string[] num;
        l = Parser.parseMath(l);
        f = Parser.parseMath(f);
        if (!ParseTools.isarray(l))
        {
            Console.WriteLine("Error: Cannot map function to non-list value. Aborting operation.");
            return "";
        }
        num = ParseTools.listsplit(ParseTools.betwbracks(l)[0], ",").Select(x => Parser.parseMath("(" + f + ")" + x)).ToArray();
        foreach (string i in num)
            res += i + ",";
        res = res.Substring(0, res.Length - 1);
        res += "]";
        return res;
    }

    public static string filt(string f, string l)
    {
        string res = "[";
        if (l == "[]") return l;
        string[] num;
        l = Parser.parseMath(l);
        f = Parser.parseMath(f);
        if (!ParseTools.isarray(l))
        {
            Console.WriteLine("Error: Cannot filter function on non-list value. Aborting operation.");
            return "";
        }
        num = ParseTools.listsplit(ParseTools.betwbracks(l)[0], ",").Where(x => Parser.parseMath("(" + f + ")" + x) == "true" ? true : false).ToArray();
        foreach (string i in num)
            res += i + ",";
        res = res.Substring(0, res.Length - 1);
        res += "]";
        if (res == "]") res = "[]";
        return res;
    }

    public static string modu(string s, string base_)
    {
        double num = 0, num1 = 0;
        s = Parser.parseMath(s);
        base_ = Parser.parseMath(base_);
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

    public static string nlog(string s, string base_)
    {
        double num = 0, num1 = 0;
        s = Parser.parseMath(s);
        base_ = Parser.parseMath(base_);
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

    public static string prnt(string s, string world)
    {
        if (Parser.parseMath(world) != "world")
        {
            Console.WriteLine("Error: Cannot perform IO without legal \"world\" parameter. Expected \"world\", received " + world);
            return "";
        }
        s = Parser.parseMath(s);
        Console.Write(s);
        World.RefreshWorld();
        return "unit";
    }

    public static string pstr(string s, string world)
    {
        if (Parser.parseMath(world) != "world")
        {
            Console.WriteLine("Error: Cannot perform IO without legal \"world\" parameter. Expected \"world\", received " + world);
            return "";
        }
        s = Parser.parseMath(s);
        if (!ParseTools.isarray(s))
        {
            Console.WriteLine("Error: Illegal string (includes non-list values, empty lists, or unknown variables). Aborting operation.");
            Console.WriteLine("Tried to print \"" + s + "\"");
            return "";
        }
        Console.Write(ParseTools.listtostring(s));
        World.RefreshWorld();
        return "unit";
    }

    public static string debg(string s)
    {
        s = Parser.parseMath(s);
        Console.WriteLine(s);
        return s;
    }

    public static string istr(string world)
    {
        if (Parser.parseMath(world) != "world")
        {
            Console.WriteLine("Error: Cannot perform IO without legal \"world\" parameter. Expected \"world\", received " + world);
            return "";
        }
        World.RefreshWorld();
        return ParseTools.stringtolist(Console.ReadLine());
    }

    public static string inum(string world)
    {
        if (Parser.parseMath(world) != "world")
        {
            Console.WriteLine("Error: Cannot perform IO without legal \"world\" parameter. Expected \"world\", received " + world);
            return "";
        }
        double trash;
        string input = Console.ReadLine();
        World.RefreshWorld();
        if (!double.TryParse(input, out trash))
            return "0";
        return input;
    }

    public static string gama(string s)
    {
        double alpha = 0;
        s = Parser.parseMath(s);
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

    public static string leng(string s)
    {
        s = Parser.parseMath(s);
        if (s[0] == '[')
            return ParseTools.listsplit(ParseTools.betwbracks(s)[0], ",").Length.ToString();
        else
        {
            Console.WriteLine("Error: Must call \"length\" on a list (does not include booleans, unknown variables, numbers, or infinity). Aborting operation.");
            return "";
        }
    }

    public static string qurs(string s, string world)
    {
        s = Parser.parseMath(s + "++[10]");
        pstr(s, world);
        return istr(world);
    }

    public static string qurn(string s, string world)
    {
        s = Parser.parseMath(s + "++[10]");
        pstr(s, world);
        return inum(world);
    }
}