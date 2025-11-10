using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// 애매하게 기능이 들어가지만 어디에도 속하지 못하는 함수들이 모이는 곳
// 공부하면서 필요한 함수들 사용법
public static class StaticScript
{
    /// <summary>
    /// num의 수에 divide -> 몇번 분할 할것인가?
    /// </summary>
    /// <param name="num"></param>
    /// <param name="divide"></param>
    /// <returns></returns>
    public static int[] IntArrDivide(int num, int divide)
    {
        int[] temp = new int[divide];
        List<int> list = new List<int>();
        int value = 0;
        while (num > 0)
        {
            value = num % 10;
            list.Add(value);
            num /= 10;
        }
        for (int i = 0; i < divide; ++i)
        {
            temp[i] = list[list.Count - (i + 1)];
        }

        return temp;
    }

    /// <summary>
    /// 숫자 string값을 가져와서 split을 특정 단어로 나누어서 int값으로 내보내기
    /// </summary>
    /// <returns></returns>
    public static int[] IntSplit(string str, char split)
    {
        string[] arrTemp = str.Split(split);
        int[] arrI = new int[arrTemp.Length];

        for (int i = 0; i < arrI.Length; ++i)
        {
            arrI[i] = int.Parse(arrTemp[i]);
        }

        return arrI;
    }

    /// <summary>
    /// 양수와 음수를 바꾸는 함수
    /// </summary>
    /// <returns></returns>
    public static int PositiveNumberAndNegativeNumberChange(int number)
    {
        return number * -1;
    }
    public static float PositiveNumberAndNegativeNumberChange(float number)
    {
        return number * -1f;
    }

    /// <summary>
    /// 목표지점으로 일정한 속도로 이동
    /// </summary>
    /// <param name="nowPosition"></param>
    /// <param name="targetPosition"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static Vector3 VectorMoveTowards(Vector3 nowPosition, Vector3 targetPosition, float speed)
    {
        nowPosition = Vector3.MoveTowards(nowPosition, targetPosition, speed);
        return nowPosition;
    }

    /// <summary>
    /// 목표지점 근처에서 감속하며 이동
    /// </summary>
    /// <param name="nowPosition"></param>
    /// <param name="targetPosition"></param>
    /// <param name="currntVelocity"></param>
    /// <param name="smoothTime"></param>
    /// <returns></returns>
    public static Vector3 VectorSmoothDamp(Vector3 nowPosition, Vector3 targetPosition, Vector3 currntVelocity, float smoothTime)
    {
        // currntVelocity는 보통 Vector3.zero를 넣어준다 참조 속도에 대한 것은 조금 더 공부해봐야 한다
        nowPosition = Vector3.SmoothDamp(nowPosition, targetPosition, ref currntVelocity, smoothTime);
        return nowPosition;
    }

    /// <summary>
    /// 선형 보간을 이용한 목표지점 이동
    /// </summary>
    /// <param name="nowPosition"></param>
    /// <param name="targetPosition"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static Vector3 VectorLerp(Vector3 nowPosition, Vector3 targetPosition, float time)
    {
        // 위에 스무스함수와 비슷한데 함수는 간결하다
        // time은 0~1 사이의 숫자를 사용해야하며 목표 위치의 가중치를 나타낸 것이다
        // 0이 들어오면 움직이지 않고 1을 넣으면 빛의 속도로 목표지점까지 도달한다
        nowPosition = Vector3.Lerp(nowPosition, targetPosition, time);
        return nowPosition;
    }

    /// <summary>
    ///  구형 보간을 이용한 이동
    /// </summary>
    /// <param name="nowPosition"></param>
    /// <param name="targetPosition"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static Vector3 VectorSlerp(Vector3 nowPosition, Vector3 targetPosition, float time)
    {
        // 이 함수는 위 Lerp의 함수와 비슷하기 때문에 설명은 생략
        // 크게 호를 그리면서 이동하는데 목표지점에 가까울수록 속력이 감소된다
        nowPosition = Vector3.Lerp(nowPosition, targetPosition, time);
        return nowPosition;
    }

    /// <summary>
    /// 시작점에서 끝점으로 height만큼 포물선 이동
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="height"></param>ㅠ
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
    /*
    보통 사용할때 함수 활용
    time += Time.deltaTime * _speed;
    Vector3 tempPos = StaticScript.Parabola(_startPos, _targetPos, 5, time);
    _trans.position = tempPos;
     */
    /*
     _trans.Translate(Vector3.forward * Time.deltaTime * _speed);
     */
    
    /// <summary>
    /// 3D 환경에서 앞으로 지속적으로 이동하는 것
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="speed"></param>
    public static void MoveUp(Transform trans, float speed)
    {
        trans.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    public static void ListUseFunction()
    {
        // 리스트<자료형> 변수명 = 동적 할당
        List<int> list = new List<int>();

        // 함수 추가
        list.Add(0);
        list.Add(0);
        // 해당 인덱스의 자료형 제거
        list.Remove(0);
        // 해당 인덱스의 자리 제거
        list.RemoveAt(0);
        // 모든 리스트 제거
        list.Clear();
        // 중간에 삽입
        list.Insert(0, 1); // 0의 자리에 1을 삽입
        // 현재 리스트에 자료형이 몇개가 있는지
        int count = list.Count;

        // 복사
        int[] arr = new int[3];
        arr[0] = 2;
        arr[1] = 3;
        arr[2] = 5;

        // 위에 arr를 List에 복사
        List<int> copyList = new List<int>(arr);
        /*
        Join String List Example
        String.Join을 이용해서 단어 사이에 ','가 찍히는 문자열을 만드는 예제입니다.
        여러 개의 문자열을 구성할 때 유용하게 사용할 수 있습니다. List에서 문자열을 추출할 땐, ToArray를 이용합니다.
         */
        List<string> cities = new List<string>();
        cities.Add("New York");
        cities.Add("Mumbai");
        cities.Add("Berlin");
        cities.Add("Istanbul");

        // Join strings into one CSV line.
        string line = string.Join(",", cities.ToArray());

        /*
        Get range of Element
        GetRange 메소드를 이용해서 범위 안의 요소를 추출할 수 있습니다.
        LINQ의 Skip과 비슷한 면도 있습니다.
        */
        List<string> rivers = new List<string>(new string[]
        {
            "nile",
            "amazon", // River 2
            "yangtze", // River 3
            "mississippi",
            "yellow"
        }); // Get rivers 2 through 3

        List<string> range = rivers.GetRange(1, 2);
        // range를 출력하면 amazon, yangtze 가 나온다
    }

    /*  
        using System.Linq;

        int[] index = { 0, 1, 0, 0, 5, 0, 0 }; // 테스트
        bool[] boolen = { true, false, false, false, true };

        string weaponCheckStr = string.Join(",", index);
        string checkBool = string.Join(",", boolen);
        Debug.Log(weaponCheckStr);
        Debug.Log(checkBool);

        int[] newIndexArray = weaponCheckStr.Split(',').Select(int.Parse).ToArray();
        bool[] newboolenArray = checkBool.Split(',').Select(bool.Parse).ToArray();

        Debug.Log(string.Format("{0}, {1}, {2}", newIndexArray[0] , newIndexArray[1] , newIndexArray[4]));
        Debug.Log(string.Format("{0}, {1}, {2}", newboolenArray[0], newboolenArray[1], newboolenArray[4]));*/
}
