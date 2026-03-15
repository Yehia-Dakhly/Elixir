using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using ServiceAbstraction.Abstractions;
using Shared.DataTransferObjects;

namespace Service.Services
{
    public class FirebaseNotificationService : IFirebaseNotificationService
    {
        private readonly ILogger<FirebaseNotificationService> _logger;

        public FirebaseNotificationService(ILogger<FirebaseNotificationService> logger)
        {
            if (FirebaseApp.DefaultInstance is null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(Path.Combine(AppContext.BaseDirectory, "elixir-firebase-adminsdk.json"))
                });
            }

            _logger = logger;
        }

        public async Task<bool> SendToUserAsync(string UserToken, NotificationMessageDTo NotificationMessage)
        {
            if (string.IsNullOrEmpty(UserToken))
                return false;
            var Message = new Message()
            {
                Token = UserToken,
                Notification = new Notification()
                {
                    Title = NotificationMessage.Title,
                    Body = NotificationMessage.Body
                },
                Data = NotificationMessage.Data ?? new Dictionary<string, string>()
            };
            try
            {
                var Response = await FirebaseMessaging.DefaultInstance.SendAsync(Message); // if Failed - > throw Exception
                return true;
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogWarning(ex, $"Failed to send notification to user with token: {UserToken}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while sending notification to user with token: {UserToken}");
                return false;
            }
        }

        public async Task<bool> SendToUsersAsync(IEnumerable<string> UserTokens, NotificationMessageDTo NotificationMessage)
        {
            if (!UserTokens.Any())
            {
                _logger.LogWarning("No user tokens provided for sending notifications.");
                return false;
            }

            var Batches = UserTokens.Chunk(500);
            bool LeastOneBatchSent = false;
            foreach (var Batch in Batches)
            {
                var MultiCastMessage = new MulticastMessage()
                {
                    Tokens = Batch,
                    Notification = new Notification()
                    {
                        Title = NotificationMessage.Title,
                        Body = NotificationMessage.Body,
                    },
                    Data = NotificationMessage.Data ?? new Dictionary<string, string>()
                };
                try
                {
                    var Response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(MultiCastMessage);

                    if(Response.FailureCount > 0)
                    {
                        _logger.LogWarning("Failed to send notification to {FailureCount} devices.", Response.FailureCount);
                        // Log
                        //Console.WriteLine($"Failed to send to {Response.FailureCount} devices.");
                    }
                    if (Response.SuccessCount > 0)
                    {
                        _logger.LogInformation("Successfully sent notification to {SuccessCount} devices.", Response.SuccessCount);
                        LeastOneBatchSent = true; // Log
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while sending notifications to a batch of users.");
                    continue;
                }
            }
                return LeastOneBatchSent;
        }
    }
}
