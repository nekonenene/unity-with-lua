using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

/*
 * 参考 : http://www.moonsharp.org/getting_started.html
 */

public class MessageManager : MonoBehaviour {

	// 最初のテスト
	double FirstMoonSharp() {
		string luaScript = @"
			-- ここから Lua
			function fact(n)
				if (n == 0) then
					return 1
				else
					return n * fact(n - 1)
				end
			end

			return fact(5)";

		// lua の function を呼び出す
		Script script = new Script();
		script.DoString(luaScript);
		DynValue luaFactFunction = script.Globals.Get("fact"); // DynValue は MoonSharp のクラス。Lua の function が入るクラスっぽい
		Debug.Log("call func test: " + script.Call(luaFactFunction, 4));

		DynValue res = Script.RunString(luaScript);
		return res.Number;
	}

	// ForLuaClass のメソッドを Lua から呼び出し
	double CallCSharpClass() {
		string luaScript = @"
			local add = csClass.Add(3, 4)
			local divide = csClass.Divide(3, 4)
			return (add - divide)
		";

		UserData.RegisterAssembly(); // [MoonSharpUserData] と付いてるクラスを呼び出してくる

		Script script = new Script();
		script.Globals["csClass"] = new ForLuaClass();

		DynValue res = script.DoString(luaScript);
		return res.Number;
	}

	// 外部Luaファイルを読み込み実行する
	double LoadExLuaScript() {
		Script script = new Script();
		script.Options.ScriptLoader = new FileSystemScriptLoader();
		DynValue res = script.DoFile("External/test.lua");
		return res.Number;
	}

	void CoroutineTest() {
		string luaCode = @"
			return function()
				local x = 0
				while true do
					x = x + 1
					coroutine.yield(x)
				end
			end
		";

		// Lua コードを読み込み、無名関数を luaFunction に代入
		Script script = new Script();
		DynValue luaFunction = script.DoString(luaCode);

		// C# のコルーチン作成
		DynValue coroutine = script.CreateCoroutine(luaFunction);

		// コルーチン処理（Luaコードの coroutine.yield(x) のところまで）を繰り返す
		while (true) {
			DynValue x = coroutine.Coroutine.Resume(); // Lua の coroutine.yield(x) の x を受け取りつつ、Lua再実行
			Debug.Log(x);
			if ((int)x.Number > 5) {
				break;
			}
		}
	}

	void Start() {
		// UpdateText() のコルーチン開始
		StartCoroutine(UpdateText());

		Debug.Log("5! = " + FirstMoonSharp());
		/*
		ForLuaClass cl = new ForLuaClass();
		Debug.Log("3 + 4 = " + cl.Add(3, 4));
		*/
		Debug.Log("3 + 4 - 3 / 4 = " + CallCSharpClass());
		Debug.Log("10 + 40 = " + LoadExLuaScript());

		CoroutineTest();
	}

	void Update() {
	}

	// MainText を Lua により更新させる
	private IEnumerator UpdateText() {
		while (true) {
			MainText.s_text = LoadExLuaScript().ToString();
			yield return new WaitForSecondsRealtime(1.0f);
		}
	}
}
