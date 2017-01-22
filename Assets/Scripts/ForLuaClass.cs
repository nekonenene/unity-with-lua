using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ForLuaClass {
	public int Add(int a, int b) {
		return a + b;
	}

	public static double Divide(int a, int b) {
		return (double)a / b;
	}
}
