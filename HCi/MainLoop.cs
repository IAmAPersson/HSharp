using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class MainLoop
{
    private static void AddData()
    {
        AddNecData();
        Parser.vars.Add("sin", "internalvarx->internalcmmdsineinternalvarx");
        Parser.vars.Add("cos", "internalvarx->internalcmmdcosiinternalvarx");
        Parser.vars.Add("tan", "internalvarx->internalcmmdtanginternalvarx");
        Parser.vars.Add("asin", "internalvarx->internalcmmdasininternalvarx");
        Parser.vars.Add("acos", "internalvarx->internalcmmdacosinternalvarx");
        Parser.vars.Add("atan", "internalvarx->internalcmmdataninternalvarx");
        Parser.vars.Add("print", "internalvarx->internalvary->internalcmmdprntinternalvarx,internalvary");
        Parser.vars.Add("getstr", "internalvarx->internalcmmdistrinternalvarx");
        Parser.vars.Add("getnum", "internalvarx->internalcmmdinuminternalvarx");
        Parser.vars.Add("printstr", "internalvarx->internalvary->internalcmmdpstrinternalvarx,internalvary");
        Parser.vars.Add("printstrln", "internalvarx->internalvary->printstr|(internalvarx++[10])|internalvary");
        Parser.vars.Add("floor", "internalvarx->internalcmmdflorinternalvarx");
        Parser.vars.Add("ceiling", "internalvarx->internalcmmdceilinternalvarx");
        Parser.vars.Add("map", "internalvarf->internalvarl->internalcmmdmapfinternalvarf,internalvarl");
        Parser.vars.Add("filter", "internalvarf->internalvarl->internalcmmdfiltinternalvarf,internalvarl");
        Parser.vars.Add("round", "internalvarx->floor|(internalvarx+.5)");
        Parser.vars.Add("max", "internalvara->internalvarb->_if_internalvara>internalvarb_then_internalvara_else_internalvarb");
        Parser.vars.Add("min", "internalvara->internalvarb->_if_internalvara<internalvarb_then_internalvara_else_internalvarb");
        Parser.vars.Add("abs", "internalvarx->_if_internalvarx<0_then_-(internalvarx)_else_internalvarx");
        Parser.vars.Add("ln", "internalvarx->internalcmmdnloginternalvarx,e");
        Parser.vars.Add("log", "internalvarx->internalcmmdnloginternalvarx,10");
        Parser.vars.Add("logn", "internalvarx->internalvary->internalcmmdnloginternalvarx,internalvary");
        Parser.vars.Add("sign", "internalvarx->_if_internalvarx=0_then_0_else__if_internalvarx>0_then_1_else_-1");
        Parser.vars.Add("sqrt", "internalvarx->internalvarx^0.5");
        Parser.vars.Add("cbrt", "internalvarx->internalvarx^(1/3)");
        Parser.vars.Add("nrt", "internalvarx->internalvarn->internalvarx^(1/internalvarn)");
        Parser.vars.Add("truncate", "internalvarx->internalcmmdtruninternalvarx");
        Parser.vars.Add("reciprocal", "internalvarx->1/internalvarx");
        Parser.vars.Add("length", "internalvarx->internalcmmdlenginternalvarx");
        Parser.vars.Add("ignore", "internalvarx->unit");
        Parser.vars.Add("querystr", "internalvars->internalvarw->internalcmmdqursinternalvars,internalvarw");
        Parser.vars.Add("querynum", "internalvars->internalvarw->internalcmmdqurninternalvars,internalvarw");
        Parser.vars.Add("add", "internalvarx->internalvary->internalvarx+internalvary");
        Parser.vars.Add("subtract", "internalvarx->internalvary->internalvarx-internalvary");
        Parser.vars.Add("multiply", "internalvarx->internalvary->internalvarx*internalvary");
        Parser.vars.Add("divide", "internalvarx->internalvary->internalvarx/internalvary");
        Parser.vars.Add("debug", "internalvarx->internalcmmddebginternalvarx");
    }

    private static void AddNecData()
    {
        Parser.wildcards.Add("Internalvarx!", "internalcmmdgamainternalvarx+1");
        Parser.wildcards.Add("Internalvarx{Internalvarz}", "internalcmmdgetlinternalvarx,internalvarz");
        Parser.wildcards.Add("Internalvara%Internalvarb", "internalcmmdmoduinternalvara,internalvarb");
        Parser.wildcards.Add("Internalvara++Internalvarb", "internalcmmdadltinternalvara,internalvarb");
        Parser.wildcards.Add(".Internalvarx", "0.internalvarx");
        Parser.wildcards.Add("-.Internalvarx", "-0.internalvarx");
        Parser.wildcards.Add("--Internalvarx", "internalvarx");
        Parser.wildcards.Add("Internalvarx*0", "0");
        Parser.wildcards.Add("0*Internalvarx", "0");
        Parser.wildcards.Add("Internalvarx*1", "internalvarx");
        Parser.wildcards.Add("1*Internalvarx", "internalvarx");
        Parser.wildcards.Add("0+Internalvarx", "internalvarx");
        Parser.wildcards.Add("Internalvarx+0", "internalvarx");
        Parser.wildcards.Add("0`Internalvarx", "-internalvarx");
        Parser.wildcards.Add("Internalvarx/1", "internalvarx");
        Parser.wildcards.Add("0/Internalvarx", "0");
        Parser.vars.Add("pi", Convert.ToString(Math.PI));
        Parser.vars.Add("e", Convert.ToString(Math.E));
        Parser.vars.Add("∞", "infinity");
    }

    public static void Exec(string[] args)
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
        {
            if (ParseTools.remspaces(lines[i]) == "##enableIO")
                Parser.IO = true;
            else if (ParseTools.remspaces(lines[i]) == "##disableIO")
                Parser.IO = false;
            else if (!Parser.RunCommands(lines[i], args))
            {
                string res = Parser.ParseMath(lines[i]);
                if (!new[] { "unit", "" }.Contains(res))
                {
                    Console.WriteLine("Error: Unhandled value from line " + (i + 1) + " `" + orig[i] + "`, left excess value `" + res + "`. Try piping result to \"ignore\" function.");
                    break;
                }
            }
        }
    }

    public static void Main(string[] args)
    {
#if DEBUG
        Console.WriteLine("Warning: You are running a debugging build. H# may be significantly slower to parse or be unstable.");
#endif
        World.Stop.Start();
        World.RefreshWorld();
        AddData();
        if (args.Length == 0)
        {
            Console.Title = "HSCi";
            Console.Write("Version 1.4.1, H# Interactive: http://iamapersson.github.io/hsharp/ \nCopyright 2016 Phil Lane Creations\n");
            string inp;
            Parser.IO = true;
            while (true)
            {
                Console.Write("HSCi> ");
                inp = Preprocessor.Preprocess(Console.ReadLine());
                if (!Parser.RunCommands(inp, args))
                    Console.WriteLine(Parser.ParseMath(inp));
            }
        }
        else
        {
            Console.Title = "HSC";
            Exec(args);
            Console.Write("\nProgram completed in " + Math.Floor(Convert.ToDouble(World.Stop.ElapsedMilliseconds / 1000)) + "s, " + World.Stop.ElapsedMilliseconds % 1000 + "ms. Press any key to exit."); Console.ReadKey();
        }
    }
}