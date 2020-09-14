using MilasQuest.GameData;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MilasQuest.EditorTools
{
    public class EditorTools
    {
        private static System.Random _randomizer;
        public static int GetRandomInt()
        {
            if (_randomizer == null)
                _randomizer = new System.Random();

            return _randomizer.Next();
        }

        public static void GenerateEnumFromDatas(string name, BaseData[] datas, string outputPath, string targetNamespace = "")
        {
            List<string> fileTextLines = new List<string>();
            WriteEnumHeader(fileTextLines, targetNamespace);
            WriteEnumBodyWithDataIds(fileTextLines, name, datas);
            WriteEnumEnd(fileTextLines, targetNamespace);

            System.IO.File.WriteAllLines(Application.dataPath + outputPath + name + ".cs", fileTextLines.ToArray());
            AssetDatabase.Refresh();
        }

        private static void WriteEnumHeader(List<string> fileTextLines, string Namespace = "")
        {
            fileTextLines.Add("using UnityEngine;");
            if (!string.IsNullOrEmpty(Namespace))
                fileTextLines.Add("namespace " + Namespace + " {");
        }

        private static void WriteEnumBodyWithDataIds(List<string> fileTextLines, string enumName, BaseData[] enumEntries)
        {
            fileTextLines.Add("	public enum " + enumName);
            fileTextLines.Add("	{");

            fileTextLines.Add("        NONE,");

            foreach (BaseData entry in enumEntries)
            {
                fileTextLines.Add("        " + (string.IsNullOrEmpty(entry.definedName) ? entry.name : entry.definedName )+ " = " + entry.id + ",");
            }

            fileTextLines.Add("        COUNT=" + enumEntries.Length.ToString());
            fileTextLines.Add("	}");
        }

        private static void WriteEnumEnd(List<string> fileTextLines, string Namespace = "")
        {
            if (!string.IsNullOrEmpty(Namespace))
                fileTextLines.Add("}");
        }
    }
}
