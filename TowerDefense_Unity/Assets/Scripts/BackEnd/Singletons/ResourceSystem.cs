using System.Collections;
using System.Collections.Generic;
using System.Data;
using Game;
using Game.Entities;
using Game.Entities.EventContainers;
using Game.WaveSystem;
using UnityEngine;


namespace Game
{
	public class ResourceSystem : MonoBehaviourSingleton<ResourceSystem>
	{
		[SerializeField] private int _StartResource, _TickAmount;
		[SerializeField] private float _TickInterval;
		private float _TickTime;
		private WaveManager _WaveManager;
		private Coroutine _UpdateOfferingsRoutine;

		public int ResourceCount { get; private set; }

		/// <summary>
		/// Event fired when a transaction is attempted.
		/// see <see cref="RunTransaction"/> to start a transaction.
		/// Will always be fired even when a transaction has failed.
		/// Gives data payload about the transaction status, and numbers it affected.
		/// </summary>
		public event TypedEventHandler<ResourceSystem, TransactionResult> OnTransaction;

		protected override void Awake()
		{
			base.Awake();
			_TickTime = _TickInterval;
		}

		private void Start()
		{
			if (Instance.RunTransaction(_StartResource).HasTransactionFailed)
				throw new DataException($"Failed to create start resource with count of {_StartResource}");
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_WaveManager = WaveManager.Instance;
			_WaveManager.WaveStarted += _WaveManager_WaveStarted;
			_WaveManager.WaveEnded += _WaveManager_WaveEnded;
		}

		private void _WaveManager_WaveStarted(in WaveManager sender, in WaveEventArgs payload)
		{
			if (_UpdateOfferingsRoutine == null)
				_UpdateOfferingsRoutine = StartCoroutine(UpdateOfferingsRoutine());
		}

		private void _WaveManager_WaveEnded(in WaveManager sender, in WaveEventArgs payload)
		{
			if (_UpdateOfferingsRoutine != null)
			{
				StopCoroutine(_UpdateOfferingsRoutine);
				_UpdateOfferingsRoutine = null;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (_WaveManager != null)
			{
				_WaveManager.WaveStarted -= _WaveManager_WaveStarted;
				_WaveManager.WaveEnded -= _WaveManager_WaveEnded;
			}
		}

		/// <summary>
		/// Call to update the offerings of the player each amount of seconds.
		/// Handles the timer inside of the method.
		/// </summary>
		private IEnumerator UpdateOfferingsRoutine()
		{
			while (true)
			{
				while (_TickTime > 0)
				{
					_TickTime -= Time.deltaTime;
					yield return null;
				}

				_TickTime = _TickInterval;
				Instance.RunTransaction(_TickAmount);
			}
		}

		/// <summary>
		/// Call to start a transaction of resources.
		/// Should always provide a positive number for increasing resources.
		/// Should always provide a negative number for removing resources.
		/// Returns a transaction result at <see cref="OnTransaction"/>
		/// </summary>
		/// <param name="amount"> Amount to give or take to the player.</param>
		public TransactionResult RunTransaction(int amount)
		{
			int newCount = ResourceCount + amount;
			bool isOverdraw = newCount < 0;
			
			TransactionResult result = new TransactionResult(isOverdraw, newCount, ResourceCount);

			if (!isOverdraw)
				ResourceCount = newCount;

			OnTransaction?.Invoke(this, result);
			return result;
		}
	}
}
