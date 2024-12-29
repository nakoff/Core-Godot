using Godot;
using System.Collections.Generic;
using System;

namespace LooksLike.Utils;

public class Logger
{
    public bool IsActive { get; set; } = true;

    private enum LogType
    {
        Info,
        Error,
        Warn
    }

    private string _name;
    private string _color;

    private static Dictionary<string, Logger> _loggers = new();
    private string _time => DateTime.Now.ToString("HH:mm:ss");

    private Logger(string name, string? color = null)
    {
        _name = name;
        _color = color ?? "#999999";
    }

    public static Logger GetLogger(string name, string? color = null)
    {
        if (!_loggers.ContainsKey(name))
            _loggers.Add(name, new Logger(name, color));
        return _loggers[name];
    }

    public void Info(object message) => Info(new object[] { message });
    public void Info(object[] message)
    {
        if (!IsActive)
            return;
        GD.PrintRich($"{GetHeaderMessage()} {GetBodyMessage(LogType.Info, message)}");
    }

    public void Error(object message) => Error(new object[] { message });
    public void Error(object[] message)
    {
        if (!IsActive)
            return;
        GD.PrintRich($"{GetHeaderMessage()} {GetBodyMessage(LogType.Error, message)}");
    }

    public void Warn(object message) => Warn(new object[] { message });
    public void Warn(object[] message)
    {
        if (!IsActive)
            return;
        GD.PrintRich($"{GetHeaderMessage()} {GetBodyMessage(LogType.Warn, message)}");
    }


    private string GetHeaderMessage()
    {
        var coloredName = "";
        var names = _name.Split('/');
        var name = "";

        for (var i = 0; i < names.Length; i++)
        {
            name += $"{names[i]}";
            var color = Logger.GetLogger(name)._color;
            coloredName += $"[color={color}][{names[i]}][/color]";

            if (i < names.Length - 1)
                name += "/";
        }

        return $"[{_time}]{coloredName}";
    }

    private string GetBodyMessage(LogType type, object[] message)
    {
        switch (type)
        {
            case LogType.Info:
                return $"{string.Join(" ", message)}";
            case LogType.Error:
                return $"[color=#ff0000]ERROR{string.Join(" ", message)}[/color]";
            case LogType.Warn:
                return $"[color=#ffff00]WARN{string.Join(" ", message)}[/color]";
        }
        return "";
    }
}
