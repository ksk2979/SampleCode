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

        // ��� ID, ����
        public int GetPlayerValue(ECoalescenceType type) => _playerInfoArr[(int)type];
        public void SetPlayerValue(ECoalescenceType type, int value) => _playerInfoArr[(int)type] = value;
        public int[] GetPlayerInfoArr => _playerInfoArr;

        // ��� ���
        public float GetEquipValue(EItemList type) => _equipValueArr[(int)type];
        public void SetEquipValue(EItemList type, float value) => _equipValueArr[(int)type] = value;
        public float GetEquipBoatValue() => _boatValueArr;
        public void SetEquipBoatValue(float value) => _boatValueArr = value; // ��Ʈ�� �߰� ������ ����
        public float[] GetEquipValueArr => _equipValueArr;

        // ���ټ�
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
        /// �ʱ�ȭ
        /// </summary>
        /// <param name="arr"></param>
        public PlayerInfo(int[] arr)
        {
            _playerInfoArr = new int[Enum.GetValues(typeof(ECoalescenceType)).Length];
            _playerInfoArr = arr;

            _equipValueArr = new float[(int)EItemList.MATERIAL];
        }

        /// <summary>
        /// Info ������Ʈ
        /// </summary>
        /// <param name="arr"></param>
        public void PlayerInfoUpdate(int[] arr) => _playerInfoArr = arr;

        /// <summary>
        /// ���ڿ� Ÿ������ PlayerInfo ��ȯ
        /// </summary>
        /// <returns></returns>
        public string PlayerInfoStr() => string.Join(",", _playerInfoArr);

        /// <summary>
        /// ��� ���� ��
        /// </summary>
        /// <param name="itemList">Ÿ��</param>
        public void LevelUp(EItemList itemList)
        {
            if (itemList > EItemList.ENGINE) return;

            int idx = ((int)itemList * 2) + 1;
            _playerInfoArr[idx]++;
        }

        #region Potential
        /// <summary>
        /// ���ټ� �ʱ�ȭ
        /// </summary>
        public void InitPotential(string captain, string sailor, string engine)
        {
            _captainPotential = captain.Split(',').Select(int.Parse).ToList();
            _sailorPotential = sailor.Split(',').Select(int.Parse).ToList();
            _enginePotential = engine.Split(',').Select(int.Parse).ToList();
        }

        /// <summary>
        /// ���ټ� �ʱ�ȭ(����Ʈ ����)
        /// </summary>
        public void InitPotential(List<int> captain, List<int> sailor, List<int> engine)
        {
            _captainPotential = captain;
            _sailorPotential = sailor;
            _enginePotential = engine;
        }

        /// <summary>
        /// ���ټ� ����
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
        /// ���ټ� ����
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
