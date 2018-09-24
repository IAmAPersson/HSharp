using System;

public static class Preprocessor
{
    private static bool IsntSurrounded(string s, int i, int length)
    {
        return (!(s.Substring(i - 1)[0] >= 'A' && s.Substring(i - 1)[0] <= 'Z') && !(s.Substring(i - 1)[0] >= 'a' && s.Substring(i - 1)[0] <= 'z') && !(s.Substring(i + length)[0] >= 'A' && s.Substring(i + length)[0] <= 'Z') && !(s.Substring(i + length)[0] >= 'a' && s.Substring(i + length)[0] <= 'z'));
    }

    private static string parsestring(string s)
    {
        s = s.Substring(1);
        string ret = "[";
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '"')
                return ret.Substring(0, ret.Length - 1) + "]" + s.Substring(i + 1);
            ret += Convert.ToString((int)s[i]) + ",";
        }
        return ret.Substring(0, ret.Length - 1) + "]";
    }

    public static string Preprocess(string s)
    {
        s = " " + s;
        for (int i = 0; i < s.Length; i++)
        {
            try
            {
                if (s.Substring(i, 1)=="\"")
                    s = s.Substring(0, i) + parsestring(s.Substring(i));
                else if (s.Substring(i, 2) == "if" && IsntSurrounded(s, i, 2))
                    s = s.Substring(0, i) + "_if_" + s.Substring(i + 2);
                else if (s.Substring(i, 2) == "or" && IsntSurrounded(s, i, 2))
                    s = s.Substring(0, i) + "_or_" + s.Substring(i + 2);
                else if (s.Substring(i, 3) == "let" && IsntSurrounded(s, i, 3))
                    s = s.Substring(0, i) + "_let_" + s.Substring(i + 3);
                else if (s.Substring(i, 3) == "and" && IsntSurrounded(s, i, 3))
                    s = s.Substring(0, i) + "_and_" + s.Substring(i + 3);
                else if (s.Substring(i, 4) == "then" && IsntSurrounded(s, i, 4))
                    s = s.Substring(0, i) + "_then_" + s.Substring(i + 4);
                else if (s.Substring(i, 4) == "else" && IsntSurrounded(s, i, 4))
                    s = s.Substring(0, i) + "_else_" + s.Substring(i + 4);
                else if (s.Substring(i, 4) == "with" && IsntSurrounded(s, i, 4))
                    s = s.Substring(0, i) + "_with_" + s.Substring(i + 4);
                else if (s.Substring(i, 4) == "when" && IsntSurrounded(s, i, 4))
                    s = s.Substring(0, i) + "_when_" + s.Substring(i + 4);
                else if (s.Substring(i, 5) == "where" && IsntSurrounded(s, i, 5))
                    s = s.Substring(0, i) + "_where_" + s.Substring(i + 5);
                else if (s.Substring(i, 5) == "match" && IsntSurrounded(s, i, 5))
                    s = s.Substring(0, i) + "_match_" + s.Substring(i + 5);
                else if (s.Substring(i, 6) == "import" && IsntSurrounded(s, i, 6))
                    s = s.Substring(0, i) + "_import_" + s.Substring(i + 6);
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