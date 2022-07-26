using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MagicString
{
    private static readonly Regex VariablePattern = new Regex(@"\@(.*?)\@");

    public static Dictionary<string, string> TextAssetToStringDatabase(TextAsset textAsset)
    {
        Dictionary<string, string> stringDatabase = new Dictionary<string, string>();

        var lines = textAsset.text.Split("\n");

        foreach (var line in lines)
        {
            LineToVariable(stringDatabase, line);
        }

        return stringDatabase;
    }

    private static void LineToVariable(Dictionary<string, string> stringDatabase, string line)
    {
        var splitString = line.Split("=");
        var key = splitString[0].Split("@")[1];
        var value = splitString[1];

        stringDatabase.Add(key, value);
    }

    private readonly Dictionary<string, string> _stringDatabase;

    public MagicString(Dictionary<string, string> stringDatabase)
    {
        _stringDatabase = stringDatabase;
    }

    public bool AddString(string key, string value) => _stringDatabase.TryAdd(key, value);
    
    public bool RemoveString(string key) => _stringDatabase.Remove(key);


    public bool EditString(string key, string value)
    {
        if (_stringDatabase.ContainsKey(key))
        {
            _stringDatabase[key] = value;
            return true;
        }

        return false;
    }
    
    public string GetString(string key, List<string> ignoreKeys = null)
    {
        ignoreKeys = ignoreKeys == null ? new List<string>() : new List<string>(ignoreKeys);
        if (ignoreKeys.Contains(key)) throw new Exception("Include infinite magic string!");

        ignoreKeys.Add(key);

        if (!_stringDatabase.TryGetValue(key, out var text)) return "";

        text = VariablePattern.Replace(text,
            (matchKey) => GetString(matchKey.Value.Substring(1, matchKey.Value.Length - 2), ignoreKeys));

        return text;
    }
}