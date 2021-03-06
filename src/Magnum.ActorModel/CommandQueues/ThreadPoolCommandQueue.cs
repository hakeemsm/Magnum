namespace Magnum.ActorModel.CommandQueues
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Exceptions;

	public class ThreadPoolCommandQueue :
		CommandQueue
	{
		private readonly List<Action> _actions = new List<Action>();
		private readonly object _lock = new object();
		private bool _enabled = true;
		private bool _flushPending;

		public void Enqueue(Action action)
		{
			lock (_lock)
			{
				_actions.Add(action);
				if (!_enabled) return;
				if (_flushPending) return;

				QueueExecutor();
			}
		}

		public void EnqueueAll(params Action[] actions)
		{
			lock (_lock)
			{
				_actions.AddRange(actions);
				if (!_enabled) return;
				if (_flushPending) return;

				QueueExecutor();
			}
		}

		public void Run()
		{
		}

		public void Disable()
		{
			lock (_lock)
			{
				_enabled = false;
			}
		}

		private void ExecuteActions(object state)
		{
			Action[] actions = DequeueAll();
			if (actions == null) return;

			foreach (var action in actions)
			{
				if(_enabled == false)
					break;

				action();
			}

			lock (_lock)
			{
				if (_actions.Count > 0)
				{
					QueueExecutor();
				}
				else
				{
					_flushPending = false;
				}
			}
		}

		private void QueueExecutor()
		{
			if (!ThreadPool.QueueUserWorkItem(ExecuteActions))
				throw new QueueFullException("Unable to queue executor to thread pool");

			_flushPending = true;
		}

		private Action[] DequeueAll()
		{
			lock (_lock)
			{
				if (ActionsAvailable())
				{
					Action[] results = _actions.ToArray();

					_actions.Clear();

					return results;
				}
				return null;
			}
		}

		private bool ActionsAvailable()
		{
			while (_actions.Count == 0 && _enabled)
			{
				Monitor.Wait(_lock);
			}

			return _enabled;
		}
	}
}