using System.Threading;
using System.Threading.Tasks;
using EventStore.Client.PersistentSubscriptions;

#nullable enable
namespace EventStore.Client {
	partial class EventStorePersistentSubscriptionsClient {
		private static DeleteReq.Types.StreamOptions StreamOptionsForDeleteProto(string streamName) {
			return new DeleteReq.Types.StreamOptions {
				StreamIdentifier = streamName,
			};
		}

		private static DeleteReq.Types.AllOptions AllOptionsForDeleteProto() {
			return new DeleteReq.Types.AllOptions();
		}

		/// <summary>
		/// Deletes a persistent subscription.
		/// </summary>
		/// <param name="streamName"></param>
		/// <param name="groupName"></param>
		/// <param name="userCredentials"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task DeleteAsync(string streamName, string groupName, UserCredentials? userCredentials = null,
			CancellationToken cancellationToken = default) {
			await new PersistentSubscriptions.PersistentSubscriptions.PersistentSubscriptionsClient(
				await SelectCallInvoker(cancellationToken).ConfigureAwait(false)).DeleteAsync(new DeleteReq {
				Options = new DeleteReq.Types.Options {
					Stream = streamName != SystemStreams.AllStream ? StreamOptionsForDeleteProto(streamName) : null,
					All = streamName == SystemStreams.AllStream ? AllOptionsForDeleteProto() : null,
					#pragma warning disable 612
					StreamIdentifier = streamName != SystemStreams.AllStream ? streamName : string.Empty, /*for backwards compatibility*/
					#pragma warning restore 612
					GroupName = groupName
				}
			}, EventStoreCallOptions.Create(Settings, Settings.OperationOptions, userCredentials, cancellationToken));
		}
	}
}
