using System;
using System.Linq;
using System.Collections.Generic;

public static class ParseTools
{
    public static string[] singlelistsplit(string s, string splitter)
    {
        List<string> ret = new List<string>();
        string temp = "";
        int isinarr = 0;
        bool alrsplit = false;
        for (int i = 0; i < s.Length; i++)
        {
            try
            {
                if (s[i] == '[' || s[i] == '{' || s[i] == '(')
                {
                    isinarr++;
                }
                if (s[i] == ']' || s[i] == '}' || s[i] == ')')
                {
                    isinarr--;
                }
                if (s.Substring(i, splitter.Length) == splitter && isinarr == 0 && !alrsplit)
                {
                    ret.Add(temp);
                    temp = "";
                    i += splitter.Length - 1;
                    alrsplit = true;
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

    public static string listtostring(string s)
    {
        string ret = "";
        try
        {
            foreach (string i in ParseTools.listsplit(ParseTools.betwbracks(s)[0], ","))
                ret += (char)Convert.ToInt32(i);
        }
        catch
        {
            Console.WriteLine("Error: Illegal string (includes booleans, unknown values, or infinity). Aborting operation.");
            return "";
        }
        return ret;
    }

    public static string stringtolist(string s)
    {
        int[] list = s.ToCharArray().Select(x => (int)x).ToArray();
        string res = "[";
        foreach (int i in list)
            res += Convert.ToString(i) + ",";
        return res.Substring(0, res.Length - 1) + "]";
    }

    public static string makeletters(int a)
    {
        char[] temp = Convert.ToString(a).ToCharArray();
        for (int i = 0; i < temp.Length; i++)
            temp[i] = (char)(temp[i] + 49);
        return new string(temp);
    }

    public static int[] range(int a, int b, int step)
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

    public static string[] split(string s, string splitter)
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

    public static string[] splitnth(string s, string splitter, int whichone)
    {
        List<string> ret = new List<string>();
        string temp = "";
        int iterator = 0;
        for (int i = 0; i < s.Length; i++)
        {
            try
            {
                if (s.Substring(i, splitter.Length) == splitter && iterator == whichone)
                {
                    ret.Add(temp);
                    temp = "";
                    i += splitter.Length - 1;
                    iterator++;
                }
                else if (s.Substring(i, splitter.Length) == splitter)
                {
                    iterator++;
                    temp += s.Substring(i, 1);
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

    public static string[] listsplit(string s, string splitter)
    {
        List<string> ret = new List<string>();
        string temp = "";
        int isinarr = 0;
        for (int i = 0; i < s.Length; i++)
        {
            try
            {
                if (s[i] == '[' || s[i] == '{' || s[i] == '(')
                {
                    isinarr++;
                }
                if (s[i] == ']' || s[i] == '}' || s[i] == ')')
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

    public static string[] singlesplit(string s, string splitter)
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

    public static string[] betwparen(string s)
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

    public static string[] betwbracks(string s)
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

    public static string[] betwbraces(string s)
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

    public static string remspaces(string s)
    {
        string res = "";
        foreach (char i in s)
        {
            if (i != ' ' && i != '\t')
                res += i;
        }
        return res;
    }

    public static string remstartspaces(string s)
    {
        for (int i = 0; i < s.Length; i++)
            if (s[i] != ' ') return s.Substring(i);
        return "";
    }

    public static string fixneg(string s)
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
        s = " " + s;
        foreach (int i in range(0, s.Length, 1))
        {
            try
            {
                if ((s[i] == '-' && (s[i - 1] < '0' || s[i - 1] > '9') && (s[i - 1] < 'a' || s[i - 1] > 'z')) && s[i - 1] != ')')
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
        res = res.Replace("`>", "->");
        return remspaces(res);
    }

    public static bool isarray(string s)
    {
        if (s.Substring(0, 1) == "[" && s.ToCharArray().ToList().Last() == ']')
        {
            foreach (string ret in split(betwbracks(s)[0], ","))
            {
                double trash = 0;
                if (!(ret == "true" || ret == "false" || ret == "unknown" || ret == "infinity" || double.TryParse(ret, out trash)))
                    return false;
            }
            return true;
        }
        return false;
    }
}