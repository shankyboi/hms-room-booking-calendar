public class ApiSettings
{
		public string apiURL { get; set; }
		public string baseURL { get; set; }
		public string clientURL { get; set; }
		public string adminURL { get; set; }
		public string accesskey { get; set; }
}
public class MailSettings
                                        {
	                                        public string fromMail { get; set; }
	                                        public string Password { get; set; }
	                                        public string displayName { get; set; }
	                                        public string adminEmail { get; set; }
	                                        public string hostName { get; set; }
	                                        public int portNumber { get; set; }
	                                        public bool enableSSL { get; set; }
	                                        public bool issecured { get; set; }


                                        }
public class PublicFormSettings
{
    public string DefaultUserId { get; set; }
}

