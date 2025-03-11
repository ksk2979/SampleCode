using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Info
{
    public class PlayerInfo
    {
        int[] _playerInfoArr;
        float[] _equipValueArr;
        float _boatValueArr;

        List<int> _captainPotential = new List<int>();
        List<int> _sailorPotential = new List<int>();
        List<int> _enginePotential = new List<int>();

        // 장비 ID, 레벨
        public int GetPlayerValue(ECoalescenceType type) => _playerInfoArr[(int)type];
        public void SetPlayerValue(ECoalescenceType type, int value) => _playerInfoArr[(int)type] = value;
        public int[] GetPlayerInfoArr => _playerInfoArr;

        // 장비 계수
        public float GetEquipValue(EItemList type) => _equipValueArr[(int)type];
        public void SetEquipValue(EItemList type, float value) => _equipValueArr[(int)type] = value;
        public float GetEquipBoatValue() => _boatValueArr;
        public void SetEquipBoatValue(float value) => _boatValueArr = value; // 보트에 추가 데미지 적용
        public float[] GetEquipValueArr => _equipValueArr;

        // 포텐셜
        public List<int> GetPotential(EItemList itemType)
        {
            switch (itemType)
            {
                case EItemList.CAPTAIN:
                    return _captainPotential;
                case EItemList.SAILOR:
                    return _sailorPotential;
                case EItemList.ENGINE:
                    return _enginePotential;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        /// <param name="arr"></param>
        public PlayerInfo(int[] arr)
        {
            _playerInfoArr = new int[Enum.GetValues(typeof(ECoalescenceType)).Length];
            _playerInfoArr = arr;

            _equipValueArr = new float[(int)EItemList.MATERIAL];
        }

        /// <summary>
        /// Info 업데이트
        /// </summary>
        /// <param name="arr"></param>
        public void PlayerInfoUpdate(int[] arr) => _playerInfoArr = arr;

        /// <summary>
        /// 문자열 타입으로 PlayerInfo 반환
        /// </summary>
        /// <returns></returns>
        public string PlayerInfoStr() => string.Join(",", _playerInfoArr);

        /// <summary>
        /// 장비 레벨 업
        /// </summary>
        /// <param name="itemList">타입</param>
        public void LevelUp(EItemList itemList)
        {
            if (itemList > EItemList.ENGINE) return;

            int idx = ((int)itemList * 2) + 1;
            _playerInfoArr[idx]++;
        }

        #region Potential
        /// <summary>
        /// 포텐셜 초기화
        /// </summary>
        public void InitPotential(string captain, string sailor, string engine)
        {
            _captainPotential = captain.Split(',').Select(int.Parse).ToList();
            _sailorPotential = sailor.Split(',').Select(int.Parse).ToList();
            _enginePotential = engine.Split(',').Select(int.Parse).ToList();
        }

        /// <summary>
        /// 포텐셜 초기화(리스트 버전)
        /// </summary>
        public void InitPotential(List<int> captain, List<int> sailor, List<int> engine)
        {
            _captainPotential = captain;
            _sailorPotential = sailor;
            _enginePotential = engine;
        }

        /// <summary>
        /// 포텐셜 변경
        /// </summary>
        public void ChangePotential(EItemList type, string potentials)
        {
            switch (type)
            {
                case EItemList.CAPTAIN:
                    _captainPotential = potentials.Split(',').Select(int.Parse).ToList();
                    break;
                case EItemList.SAILOR:
                    _sailorPotential = potentials.Split(',').Select(int.Parse).ToList();
                    break;
                case EItemList.ENGINE:
                    _enginePotential = potentials.Split(',').Select(int.Parse).ToList();
                    break;
            }
        }

        /// <summary>
        /// 포텐셜 변경
        /// </summary>
        public void ChangePotential(EItemList type, List<int> potentialList)
        {
            switch (type)
            {
                case EItemList.CAPTAIN:
                    _captainPotential = potentialList;
                    break;
                case EItemList.SAILOR:
                    _sailorPotential = potentialList;
                    break;
                case EItemList.ENGINE:
                    _enginePotential = potentialList;
                    break;
            }
        }
        #endregion potential
    }
}
