namespace Ninja.Sharp.OpenMessagingMiddleware.Utilities
{
    public static class RetryPolicy
    {
        public static async Task ExecuteAsync(
            Func<Task> action, 
            int maxRetries = 5, 
            TimeSpan? delay = null)
        {
            delay ??= TimeSpan.FromSeconds(2);

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await action();
                    return;
                }
                catch (Exception)
                {
                    if (i == maxRetries - 1)
                    {
                        throw;
                    }
                    await Task.Delay(delay.Value);
                    delay = TimeSpan.FromMilliseconds(delay.Value.TotalMilliseconds * 2); // Backoff esponenziale
                }
            }
        }
    }
}
