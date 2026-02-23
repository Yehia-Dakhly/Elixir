using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using ServiceAbstraction;
using Shared.DataTransferObjects;

namespace Service
{
    public class FirebaseNotificationService : IFirebaseNotificationService
    {
        public FirebaseNotificationService()
        {
            if (FirebaseApp.DefaultInstance is null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(Path.Combine(AppContext.BaseDirectory, "elixir-firebase-adminsdk.json"))
                });
            }
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
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendToUsersAsync(IEnumerable<string> UserTokens, NotificationMessageDTo NotificationMessage)
        {
            if (!UserTokens.Any())
                return false;

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
                        // Log
                         //Console.WriteLine($"Failed to send to {Response.FailureCount} devices.");
                    }
                    if (Response.SuccessCount > 0)
                    {
                        LeastOneBatchSent = true; // Log
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
                return LeastOneBatchSent;
        }
    }
}
