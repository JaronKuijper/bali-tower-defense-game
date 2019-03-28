using UnityEngine;
using Game.Entities.MovingEntities;
using System;
using System.Linq;
using Game.SplineSystem;

namespace Game.Entities
{
	/// <summary>Handles minion pooling.</summary>
	public class MinionPoolController : SceneObjectPool<Minion>
	{
		[SerializeField] private Minion _MinionPrefab = null;
		[SerializeField] private int _PoolObjectCount = 20;
		private SplinePathManager _PathManager;

		public void Start()
		{
			_PathManager = SplinePathManager.Instance;
			for (int i = 0; i < _PoolObjectCount; i++)
			{
				Minion minionGameObject = Instantiate(_MinionPrefab, transform);
				Add(minionGameObject);
			}
			//ActivateObject(x => x != x.IsConducting()); //TODO: Debug code. (spawns in 1 minion from the pool)
		}

		public override Minion ActivateObject(Func<Minion, bool> predicate)
		{
			Minion minion = _objects.FirstOrDefault(predicate);

			if (minion == null || _PathManager.SplineCount == 0)
				return null;

			int splineBranchIndex = UnityEngine.Random.Range(0, _PathManager.SplineCount);
			Vector3 startPoint = _PathManager.GetStartedPoints()[splineBranchIndex];
			minion.transform.position = startPoint;
			minion.SplineBranch = _PathManager.GetSplineDataObject(splineBranchIndex);
			minion.Activate();

			return minion;
		}
	}
}
