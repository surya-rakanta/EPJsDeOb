using System;
using Microsoft.ClearScript.V8;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EPJsDeob
{
 class Program
 {
  static void Main(string[] args)
  {
   Console.WriteLine("JavaScript DeObfuscator by Script Replacement.");
   //Testing2();
   if (args == null || args.Length < 3 )
   {
    Console.WriteLine("Usage: EPJsDeob <func> <in hdr file> <in dtl file>");
    Console.WriteLine("Example: EPJsDeob _0xfec0 header.js detail.js");
    Console.WriteLine("_0xfec0 -> function name to be replace.");
    Console.WriteLine("header.js -> contain string initialize and function declaration.");
    Console.WriteLine("detail.js -> contain script to be replaced.");
    Console.WriteLine("The processed script will be put into <detail file name>-result.js.");
    return;
   }
   String sFunc = args[0];
   if (!File.Exists(args[1]))
   {
    Console.WriteLine("File " + args[1] + " Not Found.");
    return;
   }
   String sHdrFile = args[1];
   if (!File.Exists(args[2]))
   {
    Console.WriteLine("File " + args[2] + " Not Found.");
    return;
   }
   String sDtlFile = args[2];

   ProcessScript(sFunc, sHdrFile, sDtlFile);
  }

  static void ProcessScript(String sFunc, String sHdrFile, String sDtlFile)
  {
   String sFileResult = Path.GetFileNameWithoutExtension(sDtlFile) + "-result.js";
   String sHdrScript = File.ReadAllText(sHdrFile);
   String sRScript = File.ReadAllText(sDtlFile);

   V8ScriptEngine v8 = new V8ScriptEngine();
   V8Script myScript = v8.Compile(sHdrScript);
   v8.Execute(myScript);

   String sRegex = "(" + sFunc + @"\(\'0[xX][0-9A-Fa-f]+\'\))";
   MatchCollection arMatches = Regex.Matches(sRScript, sRegex);
   int nI;
   String sMatchVal, sRes;
   for (nI = 0; nI < arMatches.Count; nI++)
   {
    sMatchVal = arMatches[nI].Value;
    Console.WriteLine("Processing " + sMatchVal + " ...");
    sRes = v8.ExecuteCommand(sMatchVal);
    sRScript = sRScript.Replace(arMatches[nI].Value, "\"" + sRes + "\"");
   }
   v8.Dispose();
   File.WriteAllText(sFileResult, sRScript);
   return;
  }

  static void Testing2()
  {
   String sFile = "detail.js";
   String sFileResult = Path.GetFileNameWithoutExtension(sFile) + "-result.js";

   String sRScript = File.ReadAllText(sFile);
   File.WriteAllText(sFileResult, sRScript);

   //Console.WriteLine(sRScript);
   String sTest = "abc();_0xfec0('0x00');deb;_0xfec0('0x01');QQQ;_0xfec0('0x02')";
   String sRegex = "(" + "_0xfec0" + @"\(\'0[xX][0-9A-Fa-f]+\'\))";
   MatchCollection arMatches = Regex.Matches(sRScript, sRegex);
   int nI, nPos;
   String sReplace = "myReplaces";
   for (nI=0;nI<arMatches.Count;nI++)
   {
    nPos = arMatches[nI].Index;
    sTest = sTest.Replace(arMatches[nI].Value, "\"" + sReplace + "\"");
   }
   //bool lTest = OnlyHexInString(sTest);
   Console.WriteLine(arMatches.Count.ToString());
   Console.WriteLine(sTest);

  }
  static bool OnlyHexInString(string test)
  {
   // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
   return System.Text.RegularExpressions.Regex.IsMatch(test, @"(_0xfec0\(\'0[xX][0-9A-Fa-f]+\'\))");
  }
  static void Testing()
  {
   Console.WriteLine("Testing Here");
   V8ScriptEngine v8 = new V8ScriptEngine();
   String sScript = File.ReadAllText("header.js");
   String sRScript = File.ReadAllText("detail.js");
   String sFunc = "_0xfec0";
   //Console.WriteLine(sScript);
   V8Script myScript = v8.Compile(sScript);
   v8.Execute(myScript);

   //find certain pattern in the 
   String sTest = v8.ExecuteCommand("_0xfec0('0x0')");
   Console.WriteLine("OK " + sTest);
   sTest = v8.ExecuteCommand("_0xfec0('0x1')");
   Console.WriteLine("OK " + sTest);
   sTest = v8.ExecuteCommand("_0xfec0('0x2')");
   Console.WriteLine("OK " + sTest);
   v8.Dispose();
  }
 }
}
