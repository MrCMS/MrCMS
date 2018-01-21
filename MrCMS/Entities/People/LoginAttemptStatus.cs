namespace MrCMS.Entities.People
{
    public enum LoginAttemptStatus
    {
        Failure,
        TwoFactorPending,
        LockedOut,
        Success
    }
}