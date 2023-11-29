using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Crematorium.Persistense.Repository.Postgre
{
    public class UsersRepository
    {
        private readonly NpgsqlDataSource _source;
        public UsersRepository(IConfiguration config) 
        {
            string connectionString = config.GetSection("PostgreConnection").Value;
            _source = NpgsqlDataSource.Create(connectionString);
        }

        public async Task<bool> IsExist(string name, string numPassport)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT * FROM IsExistUser('{name}', '{numPassport}');");
            var res = command.ExecuteScalar();
            return (bool)res;
        }
    }
}
