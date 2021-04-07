using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventStore.Client.SubscriptionToAll {
	public class happy_case_writing_and_subscribing_to_normal_events_auto_ack
		: IClassFixture<happy_case_writing_and_subscribing_to_normal_events_auto_ack.Fixture> {
		private const string Stream = SystemStreams.AllStream;
		private const string Group = nameof(Group);
		private const int BufferCount = 10;
		private const int EventWriteCount = BufferCount * 2;

		private readonly Fixture _fixture;

		public happy_case_writing_and_subscribing_to_normal_events_auto_ack(Fixture fixture) {
			_fixture = fixture;
		}

		[Fact]
		public async Task Test() {
			await _fixture.EventsReceived.WithTimeout();
		}

		public class Fixture : EventStoreClientFixture {
			private readonly EventData[] _events;
			private readonly TaskCompletionSource<bool> _eventsReceived;
			public Task EventsReceived => _eventsReceived.Task;

			private PersistentSubscription _subscription;
			private int _eventReceivedCount;

			public Fixture() {
				_events = CreateTestEvents(EventWriteCount).ToArray();
				_eventsReceived = new TaskCompletionSource<bool>();
			}

			protected override async Task Given() {
				await Client.CreateAsync(Stream, Group,
					new PersistentSubscriptionSettings(startFrom: Position.End, resolveLinkTos: true),
					TestCredentials.Root);
				_subscription = await Client.SubscribeAsync(Stream, Group,
					(subscription, e, retryCount, ct) => {
						if (e.OriginalStreamId.StartsWith("test-")
							&& Interlocked.Increment(ref _eventReceivedCount) == _events.Length) {
							_eventsReceived.TrySetResult(true);
						}

						return Task.CompletedTask;
					}, (s, r, e) => {
						if (e != null) {
							_eventsReceived.TrySetException(e);
						}
					}, autoAck: true,
					bufferSize: BufferCount,
					userCredentials: TestCredentials.Root);
			}

			protected override async Task When() {
				foreach (var e in _events) {
					await StreamsClient.AppendToStreamAsync("test-" + Guid.NewGuid(), StreamState.Any, new[] {e});
				}
			}

			public override Task DisposeAsync() {
				_subscription?.Dispose();
				return base.DisposeAsync();
			}
		}
	}
}
