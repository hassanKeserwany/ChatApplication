namespace API.Extenstions
{
    public static class ApplicationServiceExtensions
        //we just add this extensions for cleaning only ,
        //remove services from program.cs and put them here
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services) {
            //example of using AddControllers() to services //just for training 


            return services;

        }
    }
}
