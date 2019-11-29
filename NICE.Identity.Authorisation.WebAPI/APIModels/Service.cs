namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class Service
    {
        public Service()
        {
        }
        
        public Service(int serviceId, string name)
        {
            ServiceId = serviceId;
            Name = name;
        }
        
        public Service(DataModels.Service service)
        {
            ServiceId = service.ServiceId;
            Name = service.Name;
        }

        public int? ServiceId { get; set; }
        public string Name { get; set; }
    }
}