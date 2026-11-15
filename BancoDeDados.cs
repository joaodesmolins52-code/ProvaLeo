using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Misc;

namespace Crud
{
    public static class BancoDeDados
    {
        private static string strConexao = "Persist Security Info = false; " +
                                           "server = localhost; " +
                                           "database = loja_informatica; " +
                                           "uid = root; " +
                                           "pwd =;";

        internal static MySqlConnection Conectar()
        {
            try
            {
                MySqlConnection conexao = new MySqlConnection(strConexao);
                conexao.Open();
                return conexao;
            }
            catch
            {
                return null;
            }
        }

        public static MySqlDataAdapter Consultar(string query, Action<MySqlCommand> comando = null) //Select
        {
            try
            {
                MySqlConnection conexao = BancoDeDados.Conectar();
                
                MySqlCommand comandador = new MySqlCommand(query, conexao);
                comando?.Invoke(comandador);
                
                MySqlDataAdapter dados = new MySqlDataAdapter(comandador);
                conexao.Close();

                return dados;
            }
            catch
            {
                return null;
            }

        }

        public static (long numeroLinhas, long id) Inserir(string query, Action<MySqlCommand> comando = null) //Insert
        {
            try
            {
                MySqlConnection conexao = BancoDeDados.Conectar();
                MySqlCommand cmdInserir = new MySqlCommand(query, conexao);
                comando?.Invoke(cmdInserir);

                long numeroLinhas = cmdInserir.ExecuteNonQuery();
                long ultimoId = cmdInserir.LastInsertedId;
                conexao.Close();

                return (numeroLinhas, ultimoId);

            }
            catch
            {
                return (0, 0);
            }
        }

        public static long Executar(string query, Action<MySqlCommand> comando = null) //Execute, Alter Update
        {
            try
            {
                MySqlConnection conexao = BancoDeDados.Conectar();
                MySqlCommand cmdInserir = new MySqlCommand(query, conexao);
                comando?.Invoke(cmdInserir);

                long numeroLinhas = cmdInserir.ExecuteNonQuery();
                conexao.Close();

                return numeroLinhas;

            }
            catch
            {
                return 0;
            }
        }
    }
}
