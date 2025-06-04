using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utility
{
    public class IniPlus
    {
        private Dictionary<string, List<string>> _hiddenValues = new Dictionary<string, List<string>>();

        public string IniPath { get; set; }

        public Dictionary<string, IniPlusSection> Sections { get; set; }

        public IniPlus(string iniPath) 
        {
            IniPath = iniPath;
            Sections = new Dictionary<string, IniPlusSection>();
        }

        public IniPlusSection NewSection(string section)
        {
            IniPlusSection newSection = null;
            if (!Sections.ContainsKey(section))
            {
                newSection = new IniPlusSection();
                Sections.Add(section, newSection);
            }
            else
            {
                newSection = Sections[section];
            }

            return newSection;
        }

        public IniPlusValue NewOrUpdateValue(string section, string key, string value, bool isHidden = false)
        {
            if (!Sections.ContainsKey(section))
            {
                NewSection(section);
            }
            IniPlusValue newValue = null;
            if (!Sections[section].Values.ContainsKey(key))
            {
                newValue = new IniPlusValue(value, isHidden);
                Sections[section].Values.Add(key, newValue);
            }
            else
            {
                newValue = Sections[section].Values[key];
                newValue.Value = value;
                newValue.IsHidden = isHidden;
            }
            if (isHidden)
            {
                if (!_hiddenValues.ContainsKey(section)) { _hiddenValues[section] = new List<string>(); }
                _hiddenValues[section].Add(key);
            }
            return newValue;
        }

        public IniPlusValue ReadValue(string section, string key)
        {
            if (!Sections.ContainsKey(section) && !_hiddenValues.ContainsKey(section))
            {
                throw new KeyNotFoundException("Section not found.");
            }
            if (!Sections[section].Values.ContainsKey(key))
            {
                if (!_hiddenValues.ContainsKey(section))
                {
                    throw new KeyNotFoundException("Key not found.");
                }
                if (!_hiddenValues[section].Contains(key))
                {
                    throw new KeyNotFoundException("Key not found.");
                }
            }
            return Sections[section].Values[key];
        }

        public void UpdateValue(string section, string key, IniPlusValue value)
        {
            if (!Sections.ContainsKey(section))
            {
                throw new KeyNotFoundException("Section not found.");
            }
            if (!Sections[section].Values.ContainsKey(key))
            {
                throw new KeyNotFoundException("Key not found.");
            }
            Sections[section].Values[key] = value;
        }

        public void ReadFromFile(string iniPath = null)
        {
            iniPath = iniPath ?? IniPath;
            if (!File.Exists(iniPath))
            {
                return;
            }
            string curSection = string.Empty;
            foreach (string line in System.IO.File.ReadLines(iniPath))
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                if (line.Length <= 1)
                {
                    continue;
                }
                if (line[0] == '[' && line[line.Length-1] == ']' && line.Length > 2)
                {
                    curSection = line.Substring(1, line.Length - 2);
                    NewSection(curSection);
                    continue;
                }
                int equal = line.IndexOf('=');
                if ( equal < 0) { continue; }

                string key;
                if (line[0] != ';')
                {
                    key = line.Substring(0, equal);
                    string value = line.Substring(equal + 1);
                    IniPlusValue iniValue = NewOrUpdateValue(curSection, key, value);
                }
                else
                {
                    key = line.Substring(1, equal-1);
                    string valueBench = line.Substring(equal + 1);
                    try
                    {
                        IniPlusValue iniValue = ReadValue(curSection, key);
                        iniValue.AddValueBench(valueBench);
                    }
                    catch { }
                }
            }
        }

        public void WriteToFile(string iniPath = null)
        {
            iniPath = iniPath ?? IniPath;
            using (StreamWriter sw = new StreamWriter(iniPath))
            {
                sw.Write(this.ToString());
            }
        }

        public override string ToString()
        {
            if (Sections == null)
            {
                return string.Empty;
            }
            if (Sections.Count <=0)
            {
                return string.Empty;
            }
            StringBuilder result = new StringBuilder();
            if (Sections.Keys.Contains(string.Empty))
            {
                result.Append(Sections[string.Empty].ToString(string.Empty));
            }            
            foreach (var section in Sections)
            {
                if (section.Key != string.Empty) {
                    result.Append(section.Value.ToString(section.Key));
                }
            }
            return result.ToString();
        }
    }

    public class IniPlusValue
    {
        public string Value { get; set; }
        public List<string> ValueBench { get; set; }
        public List<string> Comment { get; set; }
        public bool IsHidden { get; set; } = false;

        public IniPlusValue(string value, bool isHidden = false)
        {
            Value = value;
            IsHidden = isHidden;
        }

        public void CommentAppendLine(string comment)
        {
            if (Comment == null)
            {
                Comment = new List<string>();
            }
            Comment.Add(comment);
        }
        public void AddValueBench(string value)
        {
            if (ValueBench == null)
            {
                ValueBench = new List<string>();
            }
            if (!ValueBench.Contains(value))
            {
                ValueBench.Add(value);
            }
        }
        public string ToString(string key)
        {
            StringBuilder result = new StringBuilder();
            if(Comment != null)
            {
                result.Append(IniPlusUtility.CommentStringify(Comment));
            }
            if(Value != null)
            {
                result.Append(ValueStringify(key, Value));
            }
            if(ValueBench != null)
            {
                foreach(var value in ValueBench)
                {
                    if (value == Value) { continue; }
                    result.Append(ValueBenchStringify(key, value));
                }
            }
            if (Comment != null)
            {
                result.AppendLine();
            }
            return result.ToString();
        }
        protected string ValueStringify(string key, string value)
        {
            StringBuilder result = new StringBuilder();
            result.Append(key);
            result.Append("=");
            result.AppendLine(value);
            return result.ToString();
        }
        protected string ValueBenchStringify(string key, string value)
        {
            StringBuilder result = new StringBuilder();
            result.Append(';');
            result.Append(ValueStringify(key, value));
            return result.ToString();
        }
    }

    public class IniPlusSection
    {
        public Dictionary<string, IniPlusValue> Values { get; set; }
        public List<string> Comment { get; set; }
        public IniPlusSection()
        {
            Values = new Dictionary<string, IniPlusValue>();
        }
        public string ToString(string section)
        {
            if (Values == null)
            {
                return string.Empty;
            }
            if (Values.Count <= 0)
            {
                return string.Empty;
            }
            StringBuilder result = new StringBuilder();
            if (Comment != null)
            {
                result.Append(IniPlusUtility.CommentStringify(Comment));
            }
            result.Append(SectionStringify(section));
            foreach(var val in Values)
            {
                if (val.Value.IsHidden) { continue; }
                result.Append(val.Value.ToString(val.Key));
            }

            result.AppendLine();
            return result.ToString();
        }
        protected string SectionStringify(string section)
        {
            if (string.IsNullOrEmpty(section))
            {
                return string.Empty;
            }
            StringBuilder result = new StringBuilder();
            result.Append('[');
            result.Append(section);
            result.AppendLine("]");
            return result.ToString();
        }
    }

    public class IniPlusUtility
    {
        public static string CommentStringify(List<string> comment)
        {
            char commentSpliterChar = ';';
            string commentLineHead = new string(commentSpliterChar, 2) + " ";
            int longestNum = comment.OrderByDescending(x => x.Length).First().Length + commentLineHead.Length + 1;
            string commentSpliter = new string(commentSpliterChar, longestNum);            
            StringBuilder result = new StringBuilder();
            result.AppendLine(commentSpliter);
            foreach (string line in comment)
            {
                result.Append(commentLineHead);
                result.AppendLine(line);
            }
            result.AppendLine(commentSpliter);
            return result.ToString();
        }
    }

    public static class IniPlusExtensions
    {
        public static bool IniValueToBoolean(this string iniValue)
        {
            return iniValue.ToLower() == "true" || iniValue == "1" || iniValue.ToLower() == "yes" || iniValue.ToLower() == "on";
        }
    }
}
