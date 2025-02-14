using Game.Entities.Interfaces;
using Game.Entities.Towers;
using UnityEngine;

namespace Game.Entities.Towers
{
	[CreateAssetMenu(fileName = "General Upgrade", menuName = "Tower/Upgrades/General Upgrade", order = 1)]
	public class TowerUpgrade : ScriptableObject, IUpgrade
	{
		[SerializeField] private float _UpgradeValue = 20.0f;

		/// <inheritdoc />
		/// <summary>
		/// Applies an upgrade to the instance 
		/// </summary>
		public void ApplyUpgrade(in Tower instance)
		{
			instance.IncreaseRange(_UpgradeValue);
			instance.IncreaseDamage(_UpgradeValue);
		}

		/// <inheritdoc />
		/// <summary>
		/// Used to request the cost of this upgrade.
		/// </summary>
		/// <returns> An integer representing the cost of this upgrade.</returns>
		public int GetCost() => 200;

		/// <inheritdoc />
		/// <summary>
		/// Used to get a textual representation of this upgrade.
		/// </summary>
		/// <returns> A human readable representation of this upgrade.</returns>
		public string GetDescription() => "Increases the damage of the turret";
	}
}
