using System;
using System.Threading.Tasks;
using Xunit;

namespace EventStore.Client {
	public class create_on_all_stream
		: IClassFixture<create_on_all_stream.Fixture> {
		public create_on_all_stream(Fixture fixture) {
			_fixture = fixture;
		}

		private readonly Fixture _fixture;

		public class Fixture : EventStoreClientFixture {
			protected override Task Given() => Task.CompletedTask;
			protected override Task When() => Task.CompletedTask;
		}
	}
}
