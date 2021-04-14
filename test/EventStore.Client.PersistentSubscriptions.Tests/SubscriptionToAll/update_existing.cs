using System.Threading.Tasks;
using Xunit;

namespace EventStore.Client.SubscriptionToAll {
	public class update_existing
		: IClassFixture<update_existing.Fixture> {
		private const string Stream = SystemStreams.AllStream;
		private const string Group = "existing";
		private readonly Fixture _fixture;

		public update_existing(Fixture fixture) {
			_fixture = fixture;
		}

		[Fact]
		public async Task the_completion_succeeds() {
			await _fixture.Client.UpdateAsync(Stream, Group,
				new PersistentSubscriptionSettings(), TestCredentials.Root);
		}

		public class Fixture : EventStoreClientFixture {
			protected override async Task Given() {
				await Client.CreateAsync(Stream, Group, new PersistentSubscriptionSettings(),
					TestCredentials.Root);
			}

			protected override Task When() => Task.CompletedTask;
		}
	}
}
