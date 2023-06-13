namespace JupebPortal.Repository.AppServices
{
    public interface IUserService
    {
        string GetUserId();
        bool IsAuthenticated();
    }
}