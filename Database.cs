using PublisherBot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublisherBot
{
    internal class DatabaseProvider
    {
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
