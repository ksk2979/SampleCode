using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// BigInteger의 단위를 표현할 수 있는 클래스
/// </summary>
public static class BigIntegerManager
{
    //private static readonly BigInteger _unitSize = 1000;
    //private static Dictionary<string, BigInteger> _unitsMap = new Dictionary<string, BigInteger>();
    //private static Dictionary<string, int> _idxMap = new Dictionary<string, int>();
    //private static readonly List<string> _units = new List<string>();
    //private static int _unitCapacity = 5;
    //private static readonly int _asciiA = 65;
    //private static readonly int _asciiZ = 90;
    //private static bool isInitialize = false;
    //private static void UnitInitialize(int capacity)
    //{
    //    _unitCapacity += capacity;
    //
    //    //Initialize 0~999
    //    _units.Clear();
    //    _unitsMap.Clear();
    //    _idxMap.Clear();
    //    _units.Add("");
    //    _unitsMap.Add("", 0);
    //    _idxMap.Add("", 0);
    //
    //
    //    //capacity만큼 사전생성, capacity가 1인경우 A~Z
    //    //capacity가 2인경우 AA~AZ
    //    //capacity 1마다 ascii 알파벳 26개 생성되는 원리
    //    for (int n = 0; n <= _unitCapacity; n++)
    //    {
    //        for (int i = _asciiA; i <= _asciiZ; i++)
    //        {
    //            string unit = null;
    //            if (n == 0)
    //                unit = ((char)i).ToString();
    //            else
    //            {
    //                var nCount = (float)n / 26;
    //                var nextChar = _asciiA + n - 1;
    //                var fAscii = (char)nextChar;
    //                var tAscii = (char)i;
    //                unit = $"{fAscii}{tAscii}";
    //            }
    //            _units.Add(unit);
    //            _unitsMap.Add(unit, BigInteger.Pow(_unitSize, _units.Count - 1));
    //            _idxMap.Add(unit, _units.Count - 1);
    //        }
    //    }
    //    isInitialize = true;
    //}
    //
    //
    //private static int GetPoint(int value)
    //{
    //    return (value % 1000) / 100;
    //}
    //
    //private static (int value, int idx, int point) GetSize(BigInteger value)
    //{
    //    //단위를 구하기 위한 값으로 복사
    //    var currentValue = value;
    //    var current = (value / _unitSize) % _unitSize;
    //    var idx = 0;
    //    var lastValue = 0;
    //    // 현재 값이 999(unitSize) 이상인경우 나눠야함.
    //    while (currentValue > _unitSize - 1)
    //    {
    //        var predCurrentValue = currentValue / _unitSize;
    //        if (predCurrentValue <= _unitSize - 1)
    //            lastValue = (int)currentValue;
    //        currentValue = predCurrentValue;
    //        idx += 1;
    //    }
    //
    //    var point = GetPoint(lastValue);
    //    var originalValue = currentValue * 1000;
    //    while (_units.Count <= idx)
    //        UnitInitialize(5);
    //    return ((int)currentValue, idx, point);
    //}
    //
    ///// <summary>
    ///// 숫자를 단위로 리턴
    ///// </summary>
    ///// <param name="value">값</param>
    ///// <returns></returns>
    //public static string GetUnit(BigInteger value)
    //{
    //    if (isInitialize == false)
    //        UnitInitialize(5);
    //
    //    var sizeStruct = GetSize(value);
    //    return $"{sizeStruct.value}.{sizeStruct.point}{_units[sizeStruct.idx]}";
    //}
    //
    ///// <summary>
    ///// 단위를 숫자로 변경
    ///// 10A = 10000으로 리턴
    ///// 1.2A = 1200으로 리턴
    ///// 소수점 1자리만 지원함
    ///// </summary>
    ///// <param name="unit">단위</param>
    ///// <returns></returns>
    //public static BigInteger UnitToValue(string unit)
    //{
    //    if (isInitialize == false)
    //        UnitInitialize(5);
    //
    //    var split = unit.Split('.');
    //    //소수점에 관한 연산 들어감
    //    if (split.Length >= 2)
    //    {
    //        var value = BigInteger.Parse(split[0]);
    //        var point = BigInteger.Parse((Regex.Replace(split[1], "[^0-9]", "")));
    //        var unitStr = Regex.Replace(split[1], "[^A-Z]", "");
    //
    //        if (point == 0) return (_unitsMap[unitStr] * value);
    //        else
    //        {
    //            var unitValue = _unitsMap[unitStr];
    //            return (unitValue * value) + (unitValue / 10) * point;
    //        }
    //
    //    }
    //    //비소수 연산 들어감
    //    else
    //    {
    //        var value = BigInteger.Parse((Regex.Replace(unit, "[^0-9]", "")));
    //        var unitStr = Regex.Replace(unit, "[^A-Z]", "");
    //        while (_unitsMap.ContainsKey(unitStr) == false)
    //            UnitInitialize(5);
    //        var result = _unitsMap[unitStr] * value;
    //
    //        if (result == 0)
    //            return int.Parse((unit));
    //        else
    //            return result;
    //    }
    //}

    static readonly string[] CurrencyUnits = new string[] { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", "ca", "cb", "cc", "cd", "ce", "cf", "cg", "ch", "ci", "cj", "ck", "cl", "cm", "cn", "co", "cp", "cq", "cr", "cs", "ct", "cu", "cv", "cw", "cx", };

    /// <summary>
    /// double 형 데이터를 클리커 게임의 화폐 단위로 표현
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string ToCurrencyString(this double number)
    {
        string zero = "0";

        if (-1d < number && number < 1d)
        {
            return zero;
        }

        if (double.IsInfinity(number))
        {
            return "Infinity";
        }

        //  부호 출력 문자열
        string significant = (number < 0) ? "-" : string.Empty;

        //  보여줄 숫자
        string showNumber = string.Empty;

        //  단위 문자열
        string unityString = string.Empty;

        //  패턴을 단순화 시키기 위해 무조건 지수 표현식으로 변경한 후 처리
        string[] partsSplit = number.ToString("E").Split('+');

        //  예외
        if (partsSplit.Length < 2)
        {
            return zero;
        }

        //  지수 (자릿수 표현)
        if (!int.TryParse(partsSplit[1], out int exponent))
        {
            Debug.LogWarningFormat("Failed - ToCurrentString({0}) : partSplit[1] = {1}", number, partsSplit[1]);
            return zero;
        }

        //  몫은 문자열 인덱스
        int quotient = exponent / 3;

        //  나머지는 정수부 자릿수 계산에 사용(10의 거듭제곱을 사용)
        int remainder = exponent % 3;

        //  1A 미만은 그냥 표현
        if (exponent < 3)
        {
            showNumber = System.Math.Truncate(number).ToString();
        }
        else
        {
            //  10의 거듭제곱을 구해서 자릿수 표현값을 만들어 준다.
            var temp = double.Parse(partsSplit[0].Replace("E", "")) * System.Math.Pow(10, remainder);

            //  소수 둘째자리까지만 출력한다.
            showNumber = temp.ToString("F").Replace(".00", "");
        }

        unityString = CurrencyUnits[quotient];

        return string.Format("{0}{1}{2}", significant, showNumber, unityString);
    }
}
