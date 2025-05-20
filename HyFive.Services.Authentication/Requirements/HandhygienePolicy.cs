namespace HyFive.Services.Authentication.Requirements
{
    public static class HandhygienePolicy
    {
        public const string FhiAdmin = "FhiAdmin";
        public const string Coordinator = "Coordinator";
        public const string FhiAdminOrCoordinator = "FhiAdmin,Coordinator";
        public const string Observer = "Observer";
    }
}
