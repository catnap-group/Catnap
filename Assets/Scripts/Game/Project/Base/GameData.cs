using UnityEngine;
using System.Collections;

public partial class GameData
{

	public class DataBase
	{

		protected int _num = 0;

		public int Num
		{

			get
			{
				return _num;
			}

		}

	}

	public class Coin : DataBase
	{

		public enum CoinType
		{
			Gold = 2,
			Diamond = 5
		}

		public Coin(int id)
		{

			//_data = BaseDataManager.Instance.GetTableDataByID<resourceBase>(id);

		}

//		protected resourceBase _data;
//
//		public resourceBase Data
//		{
//
//			get
//			{
//				return _data;
//			}
//
//		}

		public bool IsGold
		{

			get { return false; }//(CoinType)_data.type == CoinType.Gold; }

		}
//
		public bool IsDiamond
		{

			get { return false; }//(CoinType)_data.type == CoinType.Diamond; }

		}

		public int Use(int value)
		{

			int result = _num - value;

			result = RefreshValue(result);

			return result;

		}

		public int Gain(int value)
		{

			int result = _num + value;

			result = RefreshValue(result);

			return result;
		}

		public int RefreshValue(int value)
		{

			_num = value;

			if (_num < 0)
				_num = 0;
//			else if (_num >= _data.num)
//			{
//
//				_num = _data.num;
//
//				full = true;
//
//			}
//			else if (_num < _data.num)
//				full = false;

			return _num;

		}

		bool full = false;

		public bool Full
		{
			get { return full; }
		}

	}
}

