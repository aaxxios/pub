using PublisherBot.Data;


namespace PublisherBot
{
    internal class DatabaseProvider
    {
#pragma warning disable CS8618
        private static ApplicationDbContext _context;
        private static object mutext = new object();
        public static ApplicationDbContext Instance
        {
            get {
                //_context ?? (_context = new ApplicationDbContext());
                if(_context == null)
                {
                    lock(mutext)
                    {
                        if(_context == null)
                        {

                        _context = new ApplicationDbContext();
                        }

                    }
                }
                return _context;
            }
        }
    }
}
