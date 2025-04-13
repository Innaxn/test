namespace OrderMonitoring.Model
{
    public class OrderDto
    {
        public Guid ObjID { get; set; }
        public int? InternalBlockOrderNumber { get; set; }
        public int? InternalOrderNumber { get; set; }
        public required DateTime LifeCycleStatusDate { get; set; }
        public int LifeCycleStatusID { get; set; }
        //public required string StatusCaption { get; set; }
    }
}
