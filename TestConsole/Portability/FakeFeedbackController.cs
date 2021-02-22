// Copyright (C) 2017 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement
// under which you licensed this source code.

using System;
using System.Linq;
using Crestron.RAD.Common.Logging;
using Crestron.RAD.Common.Enums;
using System.Collections.Generic;
namespace Crestron.RAD.Common.BasicDriver.FakeFeedback
{
	/// <summary>
	/// Used by the framework to generate fake-feedback for discrete commands that support feedback.
	/// This is used by device-types that use the StateChanged event to describe all possible changes with an enum.
	/// This will use the driver's static clock when feedback is being faked.
	/// </summary>
	public sealed class FeedbackController : IDisposable
	{
		/// <summary>
		/// The duration that feedback will be faked in milliseconds
		/// </summary>
		public ushort DurationOfFakeFeedback { get; private set; }

		/// <summary>
		/// Dictionary keyed by the device-type state object, where display would be DisplayStateObjects
		/// with a value indicating the tick that it started to be faked.
		/// </summary>
		private Dictionary<object, FakeFeedbackData> _currentFakedStates;

		/// <summary>
		/// This can be called on multiple threads, so process one request at a time
		/// </summary>
		private object _globalLock;

		/// <summary>
		/// The last tick this iterated through the state collection
		/// </summary>
		private int _lastExpiredStateRemoval;

		/// <summary>
		/// The minimum amount of ticks before iterating through the state collection to look for expired states
		/// </summary>
		private const int _removeExpiredStatesInterval = 1000;

		/// <summary>
		/// Counter that is increased whenever the clock ticks
		/// </summary>
		private int _clockTicks;

		/// <summary>
		/// Clock is 25ms.
		/// To get 1 second: 1000ms/25ms = 40 ticks
		/// </summary>
		private uint _oneSecondInClockTicks = 1000 / DriverClock.Clock25ms;

		/// <summary>
		/// Used for logging
		/// </summary>
		private Logger _logger;


		/// <summary>
		/// There may only be one feedback generater per driver.
		/// This must be disposed before creating a new one for the same driver.
		/// </summary>
		/// <param name="durationOfFakeFeedback">How long to fake feedback for all commands</param>
		internal FeedbackController(ushort durationOfFakeFeedback, Logger logger)
		{
			DurationOfFakeFeedback = durationOfFakeFeedback;

			_globalLock = new object();
			_currentFakedStates = new Dictionary<object, FakeFeedbackData>();

			_logger = logger;
		}

		/// <summary>
		/// There may only be one feedback generater per driver.
		/// This must be disposed before creating a new one for the same driver.
		/// </summary>
		/// </summary>
		internal FeedbackController(Logger logger)
		{
			DurationOfFakeFeedback = 10000;

			_globalLock = new object();
			_currentFakedStates = new Dictionary<object, FakeFeedbackData>();

			_logger = logger;
		}


		/// <summary>
		/// Cycles through the collection of currently faked states and removes any that have expired.
		/// </summary>
		private void RemoveExpiredStateChanges()
		{
			var keys = _currentFakedStates.Keys.ToList();
			for (int i = 0; i < keys.Count; i++)
			{
				if (_currentFakedStates[keys[i]].ForceRemoval ||
					Math.Abs(Environment.TickCount - _currentFakedStates[keys[i]].StartTick) > DurationOfFakeFeedback)
				{
					_currentFakedStates.Remove(keys[i]);
				}
			}

			if (_currentFakedStates.Count == 0)
			{
				StopListeningToDriverClock();
			}

			_lastExpiredStateRemoval = Environment.TickCount;
		}

		/// <summary>
		/// This will remove any expired state changes if they havn't already occoured recently.
		/// This should be called once per clock cycle
		/// </summary>
		internal void On25msPassed()
		{
			// Do this once a second
			if (_clockTicks > 0 && _clockTicks % _oneSecondInClockTicks == 0)
			{
				// Don't remove anything if we have performed this operation less than a second ago
				if (Math.Abs(Environment.TickCount - _lastExpiredStateRemoval) >= _removeExpiredStatesInterval)
				{
					try
					{
						lock (_globalLock)
							RemoveExpiredStateChanges();
					}
					catch (Exception e)
					{
						if (_logger.CurrentLevel == LoggingLevel.Warning)
							_logger.Warning($"[FeedbackController] On25msPassed Exception: {e}");
					}
				}
				_clockTicks = 0;
			}
			else
				_clockTicks++;
		}

		/// <summary>
		/// Invoked when feedback should start being faked.
		/// </summary>
		/// <param name="stateObject">Device-type specific object representing the changed state</param>
		internal void SetFeedback(object stateObject)
		{
			try
			{
				lock (_globalLock)
				{
					// If we started from an empty state then start listening for the driver clock events
					// since we should have stopping listening when the collection became empty
					// We would only ever start here since this class shouldn't be adding fake feedback
					// on its own.
					if (_currentFakedStates.Count == 0)
						StartListeningToDriverClock();

					_currentFakedStates[stateObject] = new FakeFeedbackData();
				}
			}
			catch (Exception e)
			{
				if (_logger.CurrentLevel == LoggingLevel.Warning)
					_logger.Warning($"[FeedbackController] SetFeedback Exception: {e}");
			}
		}

		/// <summary>
		/// Determines if the given state object can invoke a state change event to applications.
		/// </summary>
		/// <param name="stateObject">The change that should be reported to an application</param>
		/// <returns>True if there is no fake feedback being shown currently</returns>
		public bool CanSendStateChange(object stateObject)
		{
			var canSendStateChange = true;
			try
			{
				lock (_globalLock)
				{
					if (Math.Abs(Environment.TickCount - _lastExpiredStateRemoval) >= _removeExpiredStatesInterval)
						RemoveExpiredStateChanges();

					canSendStateChange = !_currentFakedStates.ContainsKey(stateObject);
				}
			}
			catch (Exception e)
			{
				if (_logger.CurrentLevel == LoggingLevel.Warning)
					_logger.Warning($"[FeedbackController] CanSendStateChange Exception: {e}");
			}

			if (_logger.CurrentLevel == LoggingLevel.Debug)
				_logger.Debug($"[FeedbackController] Requested StateChange: {stateObject} - Allowed: {canSendStateChange}");

			return canSendStateChange;
		}

		/// <summary>
		/// Invoked when the event from the protocol class resulted in the feedback matching the faked state.
		/// </summary>
		/// <param name="stateObject">Device-type specific object representing the changed state</param>
		public void FeedbackMatchedWithExpectedState(object stateObject)
		{
			try
			{
				lock (_globalLock)
				{
					FakeFeedbackData data = null;
					_currentFakedStates.TryGetValue(stateObject, out data);

					if (data != null)
					{
						// Mark it as something that has expired since the reported state
						// matches what we faked it to be
						data.ForceRemoval = true;

						if (_logger.CurrentLevel == LoggingLevel.Debug)
							_logger.Debug($"[FeedbackController] Matched to expected state: {stateObject} - now allowing true feedback");

						// Can add delaying logic here to skip a few more events before accepting it
						// in case the device osscilates states?
					}
				}
			}
			catch (Exception e)
			{
				if (_logger.CurrentLevel == LoggingLevel.Warning)
					_logger.Warning($"[FeedbackController] FeedbackMatchedWithExpectedState Exception: {e}");
			}
		}

		/// <summary>
		/// Starts listening to the driver's static clock.
		/// This object should only listen to events when there is fake feedback being shown.
		/// </summary>
		private void StartListeningToDriverClock()
		{
			DriverClock.Driver25msClockEventHandler -= On25msPassed;
			DriverClock.Driver25msClockEventHandler += On25msPassed;
			if (_logger.CurrentLevel == LoggingLevel.Debug)
			{
				_logger.Debug("[FeedbackController] Now listening to clock events");
			}
		}

		/// <summary>
		/// Stops listening to the driver's static clock.
		/// This object should stop listening when there is no feedback being faked.
		/// </summary>
		private void StopListeningToDriverClock()
		{
			DriverClock.Driver25msClockEventHandler -= On25msPassed;
			if (_logger.CurrentLevel == LoggingLevel.Debug)
			{
				_logger.Debug("[FeedbackController] No longer listening to clock events");
			}
		}

		/// <summary>
		/// Unsubscribe from driver clock
		/// </summary>
		public void Dispose() => DriverClock.Driver25msClockEventHandler -= On25msPassed;
	}
}