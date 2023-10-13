using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using volcanes_api.Models;

namespace volcanes_api.tests
{
    public class BasePruebas
    {
        protected volcanesDBContext ConstruirContexto(string nombreDB)
        {
            var opciones = new DbContextOptionsBuilder<volcanesDBContext>()
                .UseInMemoryDatabase(nombreDB).Options;

            var dbContext = new volcanesDBContext(opciones);
            return dbContext;
        }


    }
}
