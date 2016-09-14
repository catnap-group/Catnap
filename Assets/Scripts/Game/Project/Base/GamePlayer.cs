using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePlayer 
{
	GameData.Coin _gold;

	public GameData.Coin Gold
	{

		get
		{

			return _gold;

		}

	}

	GameData.Coin _diamond;

	public GameData.Coin Diamond
	{

		get
		{

			return _diamond;

		}

	}

	public Dictionary<int, GameData.Coin> monies = new Dictionary<int, GameData.Coin>();

	public int UseMoney(int id, int value)
	{

		int result = monies[id].Use(value);

		#region socketlogic
		OnMoneyChange(monies[id]);
		#endregion

		return result;

	}

	public int GainMoney(int id, int value)
	{

		int result = monies[id].Gain(value);

		#region socketlogic
		OnMoneyChange(monies[id]);
		#endregion

		return result;

	}

	public int RefereshMoney(int id, int value)
	{

		int result;

		if (monies.ContainsKey(id))
			result = monies[id].RefreshValue(value);
		else
		{

			GameData.Coin money = new GameData.Coin(id);

			result = money.RefreshValue(value);

			monies.Add(id, money);

			if (money.IsGold)
			{

				_gold = money;

			}

			if (money.IsDiamond)
			{

				_diamond = money;

			}

		}

		#region socketlogic
		OnMoneyChange(monies[id]);
		#endregion

		return result;

	}

	public GameData.Coin GetResource(int id)
	{

		if (monies.ContainsKey(id))
			return monies[id];

		return null;

	}

	#region socketlogic
	public virtual void OnMoneyChange(GameData.Coin coin)
	{

	}
	#endregion

	#region socketlogic
	protected void InitPlayerResource()
	{

//		Dictionary<int, object> resources = BaseDataManager.Instance.GetTableDatas<resourceBase>();
//
//		List<int> keys = new List<int>();
//
//		keys.AddRange(resources.Keys);
//
//		keys.Sort();
//
//		keys.Reverse();
//
//		Dictionary<int, object> sortResources = new Dictionary<int, object>();
//
//		foreach (var item in keys)
//		{
//
//			sortResources.Add(item, resources[item]);
//
//		}
//
//		foreach (var item in sortResources)
//		{
//
//			RefereshMoney(item.Key, 1000);
//
//		}

	}
	#endregion
	public class Me : GamePlayer
	{
		public static Me instance;

		Me()
		{

			instance = this;


			//InitPlayerResource
			//InitPlayerParts
			//InitPlayerShip

		}
	}
}

