namespace Grc.ui.App.Http.Responses {
    public class ActivityModel {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public long? EntityId { get; set; }
        public string EntityName { get; set; }
        public long TypeId { get; set; }
        public string TypeDescription { get; set; }
        public string IpAddress { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get;set; }
        public string Period { get; set; }
        public DateTime CreatedOn { get; set; }
    }

}
