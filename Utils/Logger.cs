using Godot;
using System.Collections.Generic;
using System;

namespace LooksLike.Utils;

public class Logger
{
	private string _name;
	private string _color;

	private static Dictionary<string, Logger> _loggers = new();
	private string _time => DateTime.Now.ToString("HH:mm:ss");

	private Logger(string name, string? color = null)
	{
		_name = name;
		_color = color ?? "#ffffff";
	}

	public static Logger GetLogger(string name, string? color = null)
	{
		if (!_loggers.ContainsKey(name))
			_loggers.Add(name, new Logger(name, color));
		return _loggers[name];
	}

	public void Info(object[] message) => GD.PrintRich($"[{_time}][color={_color}][{_name}][/color] {string.Join(" ", message)}");
	public void Info(object message) => Info(new object[] { message });

	public void Error(object[] message) => GD.PrintRich($"[{_time}][color={_color}][{_name}][/color] [color=#ff0000]ERROR{string.Join(" ", message)}[/color]");
	public void Error(object message) => Error(new object[] { message });

	public void Warn(object[] message) => GD.PrintRich($"[{_time}][color={_color}][{_name}][/color] [color=#ffff00]WARN{string.Join(" ", message)}[/color]");
	public void Warn(object message) => Warn(new object[] { message });
}
