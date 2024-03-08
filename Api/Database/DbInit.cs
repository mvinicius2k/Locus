using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Database
{
    public class DbInit
    {
        private readonly ILogger<DbInit> _logger;
        private readonly Context _context;

        public DbInit(ILogger<DbInit> logger, Context context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Prepare(bool restart = false)
        {
#if !DEBUG
            _logger.LogWarning("Não é possível criar nem deletar o banco de dados fora do perfil de depuração.");
            return;
#endif
            try
            {
                if(restart)
                    await _context.Database.EnsureDeletedAsync();
                await _context.Database.EnsureCreatedAsync();
                
            }
            catch (System.Exception e)
            {
                
                throw;
            }
        }

        public async Task TrySeed()
        {

        }
    }

    
}
