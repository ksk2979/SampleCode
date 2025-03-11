using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// 타겟이 스피드만큼 바라보고 있는 방향으로 가는것
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    public static void TransformTranslate(Transform target, float speed)
    {
        target.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
