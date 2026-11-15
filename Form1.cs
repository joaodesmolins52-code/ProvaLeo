using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crud
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void MostrarEstoque()
        {
            MySqlDataAdapter tabelaSql = BancoDeDados.Consultar("SELECT * FROM produtos;");
            DataTable tabelaCsharp = new DataTable();
            tabelaSql.Fill(tabelaCsharp);

            dgvProdutos.DataSource = tabelaCsharp;
        }

        private bool ConferirCampos()
        {
            if (string.IsNullOrEmpty(txtNome.Text))
            {
                MessageBox.Show("O campo nome deve ser preenchido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNome.Focus();
                return false;
            }

            if (cmbCategoria.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione uma Categoria!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbCategoria.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtPreco.Text))
            {
                MessageBox.Show("O campo preço deve ser preenchido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPreco.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPreco.Text, out decimal preco) || preco <= 0.00m)
            {
                MessageBox.Show("O campo preço deve ser um número decimal positivo!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPreco.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtQuantidade.Text))
            {
                MessageBox.Show("O campo quantidade deve ser preenchido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtQuantidade.Focus();
                return false;
            }

            if (!int.TryParse(txtQuantidade.Text, out int quantidade) || quantidade <= 0)
            {
                MessageBox.Show("O campo quantidade deve ser um número inteiro positivo!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtQuantidade.Focus();
                return false;
            }


            return true;
        }

        private void LimparCampos()
        {
            txtID.Text = string.Empty;
            txtNome.Text = string.Empty;
            txtPreco.Text = string.Empty;
            txtQuantidade.Text = string.Empty;
            cmbCategoria.SelectedIndex = -1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MostrarEstoque();
        }

        private void btnCadastro_Click(object sender, EventArgs e)
        {
            if (ConferirCampos())
            {
                if (!string.IsNullOrEmpty(txtID.Text))
                {
                    MessageBox.Show("O campo ID deve ser vazio!\nLimpe a tela!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnLimpar.Focus();
                    return;
                }

                Produto produto = new Produto();
                produto.Nome = txtNome.Text;
                produto.Categoria = cmbCategoria.Text;
                produto.Preco = decimal.Parse(txtPreco.Text);
                produto.Quantidade = int.Parse(txtQuantidade.Text);

                string query = "Insert INTO produtos(nome, categoria, preco, estoque) VALUES (@nome, @cat, @preco, @qt)";
                
                var resultado = BancoDeDados.Inserir(query, comando =>
                {
                    comando.Parameters.AddWithValue("@nome", produto.Nome);
                    comando.Parameters.AddWithValue("@cat", produto.Categoria);
                    comando.Parameters.AddWithValue("@preco", produto.Preco);
                    comando.Parameters.AddWithValue("@qt", produto.Quantidade);
                });

                if (resultado.numeroLinhas > 0)
                {
                    MessageBox.Show("Produto inserido com êxito.", "Sucesso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparCampos();
                    MostrarEstoque();
                }
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            if (ConferirCampos())
            {
                if (string.IsNullOrEmpty(txtID.Text))
                {
                    MessageBox.Show("O campo ID não deve ser vazio!\nSelecione algo no estoque.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dgvProdutos.Focus();
                    return;
                }

                Produto produto = new Produto();
                produto.ID = long.Parse(txtID.Text);
                produto.Nome = txtNome.Text;
                produto.Categoria = cmbCategoria.Text;
                produto.Preco = decimal.Parse(txtPreco.Text);
                produto.Quantidade = int.Parse(txtQuantidade.Text);

                string query = "UPDATE produtos SET nome = @nome, categoria = @cat, preco = @preco, estoque = @qt WHERE id = @id";

                long resultado = BancoDeDados.Executar(query, comando =>
                {
                    comando.Parameters.AddWithValue("@id", produto.ID);
                    comando.Parameters.AddWithValue("@nome", produto.Nome);
                    comando.Parameters.AddWithValue("@cat", produto.Categoria);
                    comando.Parameters.AddWithValue("@preco", produto.Preco);
                    comando.Parameters.AddWithValue("@qt", produto.Quantidade);
                });

                if (resultado > 0)
                {
                    MessageBox.Show("Produto Atualizado com êxito.", "Sucesso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparCampos();
                    MostrarEstoque();
                }
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("O campo ID não deve ser vazio!\nSelecione algo no estoque.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvProdutos.Focus();
                return;
            }

            DialogResult resultadoUsuario = MessageBox.Show("Você tem certeza de que vai apagar este produto?", "Cuidado!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resultadoUsuario == DialogResult.Yes)
            {
                long numeroLinhas = BancoDeDados.Executar("DELETE FROM produtos WHERE id = @id", comando =>
                {
                    comando.Parameters.AddWithValue("@id", long.Parse(txtID.Text));
                });
                
                MostrarEstoque();
            }
        }

        private void dgvProdutos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow linha = dgvProdutos.Rows[e.RowIndex];
                txtID.Text = linha.Cells["id"].Value.ToString();
                txtNome.Text = linha.Cells["nome"].Value.ToString();
                cmbCategoria.Text = linha.Cells["categoria"].Value.ToString();
                txtPreco.Text = linha.Cells["preco"].Value.ToString();
                txtQuantidade.Text = linha.Cells["estoque"].Value.ToString();
            }
        }
    }
}
