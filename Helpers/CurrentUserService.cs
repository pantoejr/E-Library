namespace E_Library.Helpers
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor contextAccessor;
        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }
        public string GetCurrentUser()
        { 
            return contextAccessor.HttpContext.User.Identity.Name;
        }
    }
}
