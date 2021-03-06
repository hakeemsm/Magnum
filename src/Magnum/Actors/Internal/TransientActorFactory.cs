// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Actors.Internal
{
	using Fibers;
	using Reflection;

	/// <summary>
	///   Creates a new instance of an actor for each call
	/// </summary>
	/// <typeparam name = "TActor"></typeparam>
	public class TransientActorFactory<TActor> :
		ActorFactory<TActor>
		where TActor : class
	{
		private readonly FiberFactory _fiberFactory;

		public TransientActorFactory(FiberFactory fiberFactory)
		{
			_fiberFactory = fiberFactory;
		}

		public TActor GetActor()
		{
			Fiber fiber = _fiberFactory();

			return FastActivator<TActor>.Create(fiber);
		}
	}
}